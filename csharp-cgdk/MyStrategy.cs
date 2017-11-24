using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed partial class MyStrategy : IStrategy
    {
        private static readonly Dictionary<long, VehicleWrapper> Units = new Dictionary<long, VehicleWrapper>();
        private static readonly List<Action> ActionQueue = new List<Action>();
        private bool _advantage;

        public void Move(Player me, World world, Game game, Move move)
        {
            Global.Update(me, world, game, move);
            UpdateUnits(world);

            if (!_advantage && world.TickIndex % 60 == 0)
            {
                _advantage = Units.Values.Count(i => i.PlayerId == me.Id) > 3 * Units.Values.Count(i => i.PlayerId != me.Id);
            }

            if (world.TickIndex < 0.75 * world.TickCount && !_advantage)
            {
                Hurricane(world, me);
            }
            else
            {
                Rush(expand: true);
            }
            
            EvadeNuclearStrike(world, me);
            NuclearStrike(world, me, game);
            ProcessQueue(world, me, move);
        }

        private void UpdateUnits(World world)
        {
            foreach (var newVehicle in world.NewVehicles)
            {
                Units.Add(newVehicle.Id, new VehicleWrapper(newVehicle));
            }

            foreach (var vehicleUpdate in world.VehicleUpdates)
            {
                if (vehicleUpdate.Durability > 0)
                {
                    Units[vehicleUpdate.Id].Update(vehicleUpdate);
                }
                else
                {
                    Units.Remove(vehicleUpdate.Id);
                }
            }
        }

        private static void ProcessQueue(World world, Player me, Move move)
        {
            if (world.TickIndex < _wait)
            {
                if (ActionQueue.Any() && me.RemainingActionCooldownTicks == 0)
                {
                    Execute(ActionQueue.FirstOrDefault(i => i.Urgent && i.Ready), move);
                }

                return;
            }

            if (ActionQueue.Any() && me.RemainingActionCooldownTicks == 0)
            {
                for (var i = 0; i < ActionQueue.Count; i++)
                {
                    if (ActionQueue[i].Urgent && ActionQueue[i].Ready && i > 0 && ActionQueue[0].Action == ActionType.ClearAndSelect)
                    {
                        Execute(ActionQueue[i], move);
                        return;
                    }
                }

                Execute(ActionQueue.FirstOrDefault(i => i.Ready), move);
            }
        }

        private static int _wait = -1;

        private static void Execute(Action action, Move move)
        {
            if (action == null) return;

            if (action.WaitForTick > 0)
            {
                _wait = action.WaitForTick;
            }
            else
            {
                move.Action = action.Action;
                move.X = action.X;
                move.Y = action.Y;
                move.Angle = action.Angle;
                move.Right = action.Right;
                move.Bottom = action.Bottom;
                move.VehicleId = action.VehicleId;
                move.VehicleType = action.VehicleType;
                move.MaxSpeed = action.MaxSpeed;
                move.Factor = action.Factor;
                move.Group = action.Group;
                move.FacilityId = action.FacilityId;
            }
            
            if (action.Callback != null) action.Callback();

            ActionQueue.Remove(action);
        }

        private static int WaitTicks(World world, int ticks)
        {
            var action = new Action { WaitForTick = world.TickIndex + ticks };

            ActionQueue.Add(action);

            return action.WaitForTick;
        }

        private static int WaitSeconds(World world, double seconds)
        {
            var ticks = (int) (seconds * 60);

            return WaitTicks(world, ticks);
        }

        #region Common strategies

        private void EvadeNuclearStrike(World world, Player me)
        {
            var enemy = world.GetOpponentPlayer();

            if (enemy.NextNuclearStrikeTickIndex - world.TickIndex == 29)
            {
                SelectAll(world);
                Scale(2, enemy.NextNuclearStrikeX, enemy.NextNuclearStrikeY);
                WaitTicks(world, enemy.NextNuclearStrikeTickIndex - world.TickIndex + 10);
                Scale(0.1, me);
                ActionQueue[ActionQueue.Count - 1].Urgent = true;
                ActionQueue[ActionQueue.Count - 2].Urgent = true;
                ActionQueue[ActionQueue.Count - 3].Urgent = true;
            }
        }

        private void NuclearStrike(World world, Player me, Game game)
        {
            if (me.NextNuclearStrikeTickIndex == -1 && world.TickIndex % 60 == 0)
            {
                var myUnits = Units.Values.Where(i => i.PlayerId == me.Id).ToList();
                var enemyUnits = Units.Values.Where(i => i.PlayerId != me.Id).ToList();
                var evaluations = new Dictionary<VehicleWrapper, double>();

                foreach (var target in enemyUnits)
                {
                    var visors = myUnits.Where(i => GetDistance(i, target) < 0.85 * i.Vehicle.VisionRange).ToList();

                    if (visors.Count == 0) continue;
                    
                    var affected = Units.Values.Where(i => GetDistance(i, target) < game.TacticalNuclearStrikeRadius).ToList();
                    
                    if (affected.Count <= 50) continue;

                    var damageToMe = affected.Where(i => i.PlayerId == me.Id)
                        .Sum(i => Math.Min(i.Durability, game.MaxTacticalNuclearStrikeDamage * (1 - GetDistance(i, target) / game.TacticalNuclearStrikeRadius)));
                    var damageToEnemy = affected.Where(i => i.PlayerId != me.Id)
                        .Sum(i => Math.Min(i.Durability, game.MaxTacticalNuclearStrikeDamage * (1 - GetDistance(i, target) / game.TacticalNuclearStrikeRadius)));
                    var efficiency = damageToEnemy - damageToMe;

                    if (efficiency > 0)
                    {
                        evaluations.Add(target, efficiency);
                    }
                }

                if (evaluations.Count == 0) return;

                var best = evaluations.OrderBy(i => i.Value).Last();
                var threshold = Math.Min(50 * 100, 0.20 * enemyUnits.Sum(i => i.Durability));

                if (best.Value > threshold)
                {
                    var target = best.Key;
                    var visor = myUnits.Where(i => GetDistance(i, target) < 0.85 * i.Vehicle.VisionRange).OrderBy(i => i.X + i.Y).First();
                    var affectedEnemies = Units.Values.Where(i => i.PlayerId != me.Id && GetDistance(i, target) < game.TacticalNuclearStrikeRadius).ToList();
                    var direction = new Position(affectedEnemies.Average(i => i.Direction.X), affectedEnemies.Average(i => i.Direction.Y));
                    var prediction = new Position(30 * direction.X, 30 * direction.Y);

                    ActionQueue.Add(new Action
                    {
                        Action = ActionType.TacticalNuclearStrike,
                        X = target.X + prediction.X,
                        Y = target.Y + prediction.Y,
                        VehicleId = visor.Id,
                        Urgent = true
                    });
                }
            }
        }

        private int _rushStartedTick = -1;
        private int _iteration;

        private void Rush(bool expand)
        {
            if (_rushStartedTick == -1) _rushStartedTick = Global.World.TickIndex;

            var tick = Global.World.TickIndex - _rushStartedTick;

            if (tick == 0 && expand)
            {
                foreach (VehicleType vehicleType in Enum.GetValues(typeof(VehicleType)))
                {
                    SelectAll(vehicleType, Global.World);
                    Scale(2, vehicleType, Global.Me);
                }
            }

            if (tick > 60 && Global.World.TickIndex % 50 == 0) // 8 actions = 40 tics
            {
                if (_iteration % 9 > 6)
                {
                    foreach (VehicleType vehicleType in Enum.GetValues(typeof(VehicleType)))
                    {
                        if (vehicleType == VehicleType.Arrv) continue;

                        SelectAll(vehicleType, Global.World);
                        Scale(0.5, vehicleType, Global.Me);
                    }
                }
                else
                {
                    SelectAll(VehicleType.Tank, Global.World);
                    MoveAll(VehicleType.Tank, new List<VehicleType> { VehicleType.Tank, VehicleType.Ifv, VehicleType.Arrv, VehicleType.Fighter, VehicleType.Helicopter }, Global.Me);

                    SelectAll(VehicleType.Ifv, Global.World);
                    MoveAll(VehicleType.Ifv, new List<VehicleType> { VehicleType.Helicopter, VehicleType.Fighter, VehicleType.Arrv, VehicleType.Ifv, VehicleType.Tank }, Global.Me);

                    SelectAll(VehicleType.Helicopter, Global.World);
                    MoveAll(VehicleType.Helicopter, new List<VehicleType> { VehicleType.Tank, VehicleType.Arrv, VehicleType.Helicopter, VehicleType.Ifv, VehicleType.Fighter }, Global.Me);

                    SelectAll(VehicleType.Fighter, Global.World);

                    if (Units.Values.Any(i => i.PlayerId != Global.Me.Id && (i.Type == VehicleType.Helicopter || i.Type == VehicleType.Fighter)))
                    {
                        MoveAll(VehicleType.Fighter, new List<VehicleType> { VehicleType.Helicopter, VehicleType.Fighter }, Global.Me);
                    }
                    else
                    {
                        MoveAll(VehicleType.Fighter, new List<VehicleType> { VehicleType.Tank, VehicleType.Ifv, VehicleType.Arrv }, Global.Me, self: true);
                    }

                    var arrvs = Units.Values.Where(i => i.PlayerId == Global.Me.Id && i.Type == VehicleType.Arrv).ToList();

                    if (arrvs.Any())
                    {
                        var pos = new Position(arrvs.Average(i => i.X), arrvs.Average(i => i.Y));

                        SelectAll(VehicleType.Arrv, Global.World);
                        Move(0.2 * Global.World.Width - pos.X, 0.8 * Global.World.Height - pos.Y);
                    }
                }

                _iteration++;
            }
        }

        #endregion

        #region Helpers

        private static void SelectAll(VehicleType type, World world)
        {
            ActionQueue.Add(new Action { Action = ActionType.ClearAndSelect, Right = world.Width, Bottom = world.Height, VehicleType = type });
        }

        private static void SelectAll(World world)
        {
            ActionQueue.Add(new Action { Action = ActionType.ClearAndSelect, Right = world.Width, Bottom = world.Height });
        }

        private static void Group(int group)
        {
            ActionQueue.Add(new Action { Action = ActionType.Assign, Group = group });
        }

        private static void Scale(double scale, VehicleType type, Player me)
        {
            var units = Units.Values.Where(i => i.Type == type && i.PlayerId == me.Id).ToList();

            if (units.Count == 0) return;

            var move = new Action { Action = ActionType.Scale, Factor = scale, X = units.Average(i => i.X), Y = units.Average(i => i.Y) };

            ActionQueue.Add(move);
        }

        private static void Scale(double scale, double x, double y)
        {
            ActionQueue.Add(new Action { Action = ActionType.Scale, Factor = scale, X = x, Y = y });
        }

        private static void Scale(double scale, Position position)
        {
            ActionQueue.Add(new Action { Action = ActionType.Scale, Factor = scale, X = position.X, Y = position.Y });
        }

        private static void Scale(double scale, Player me)
        {
            var units = Units.Values.Where(i => i.PlayerId == me.Id).ToList();
            var move = new Action { Action = ActionType.Scale, Factor = scale, X = units.Average(i => i.X), Y = units.Average(i => i.Y) };

            ActionQueue.Add(move);
        }

        private static void Move(double x, double y, double maxSpeed = 0)
        {
            ActionQueue.Add(new Action { Action = ActionType.Move, X = x, Y = y, MaxSpeed = maxSpeed });
        }

        private static void Rotate(double angle, double x, double y)
        {
            ActionQueue.Add(new Action { Action = ActionType.Rotate, Angle = angle, X = x, Y = y, MaxSpeed = 1, MaxAngularSpeed = 1 });
        }

        private static void Rotate(double angle, Position position)
        {
            ActionQueue.Add(new Action { Action = ActionType.Rotate, Angle = angle, X = position.X, Y = position.Y, MaxSpeed = 1, MaxAngularSpeed = 1 });
        }

        private void MoveAll(VehicleType type, List<VehicleType> targetTypes, Player me, bool self = false, double maxSpeed = 0)
        {
            var move = new Action();
            var myUnits = Units.Values.Where(i => i.Type == type && i.PlayerId == me.Id).ToList();

            if (myUnits.Count == 0) return;

            foreach (var targetType in targetTypes)
            {
                var targets = Units.Values.Where(i => i.Type == targetType && (self ? i.PlayerId == me.Id : i.PlayerId != me.Id)).ToList();

                if (targets.Any())
                {
                    var myCenter = new Position(myUnits.Average(i => i.X), myUnits.Average(i => i.Y));
                    var targetsCenter = new Position(targets.Average(i => i.X), targets.Average(i => i.Y));
                    var target = targets.OrderBy(i => targetsCenter.Distance(i)).First();

                    move.Action = ActionType.Move;
                    move.X = target.X - myCenter.X;
                    move.Y = target.Y - myCenter.Y;

                    if (maxSpeed > 0)
                    {
                        move.MaxSpeed = maxSpeed;
                    }

                    ActionQueue.Add(move);

                    return;
                }
            }
        }

        private static Position Middle(Player player)
        {
            var units = Units.Values.Where(i => i.PlayerId == player.Id).ToList();

            return new Position(units.Average(i => i.X), units.Average(i => i.Y));
        }

        private double GetDistance(VehicleWrapper from, VehicleWrapper to)
        {
            var x = to.X - from.X;
            var y = to.Y - from.Y;

            return Math.Sqrt(x * x + y * y);
        }

        private double GetRandom(double min, double max)
        {
            return CRandom.GetRandom((int) (min * 1000), (int) (max * 1000)) / 1000d;
        }

        #endregion
    }
}