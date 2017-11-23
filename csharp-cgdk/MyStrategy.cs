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

        public void Move(Player me, World world, Game game, Move move)
        {
            UpdateUnits(world);
            Hurricane(world, me);
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
                    var urgent = ActionQueue.FirstOrDefault(i => i.Urgent);

                    if (urgent != null) Execute(urgent, move);
                }

                return;
            }

            if (ActionQueue.Any() && me.RemainingActionCooldownTicks == 0)
            {
                for (var i = 0; i < ActionQueue.Count; i++)
                {
                    if (ActionQueue[i].Urgent && i > 0 && ActionQueue[0].Action == ActionType.ClearAndSelect)
                    {
                        Execute(ActionQueue[i], move);
                        return;
                    }
                }

                Execute(ActionQueue[0], move);
            }
        }

        private static int _wait = -1;

        private static void Execute(Action action, Move move)
        {
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

        private void EvadeNuclearStrike(World world, Player me)
        {
            var enemy = world.GetOpponentPlayer();

            if (enemy.NextNuclearStrikeTickIndex != -1)
            {
                SelectAll(world);
                Scale(10, enemy.NextNuclearStrikeX, enemy.NextNuclearStrikeY);
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
                        .Sum(i => Math.Min(i.Durability, game.MaxTacticalNuclearStrikeDamage * Math.Max(0, game.TacticalNuclearStrikeRadius - GetDistance(i, target))));
                    var damageToEnemy = affected.Where(i => i.PlayerId != me.Id)
                        .Sum(i => Math.Min(i.Durability, game.MaxTacticalNuclearStrikeDamage * Math.Max(0, game.TacticalNuclearStrikeRadius - GetDistance(i, target))));
                    var efficiency = damageToEnemy - damageToMe;

                    if (efficiency > 0)
                    {
                        evaluations.Add(target, efficiency);
                    }
                }

                if (evaluations.Count == 0) return;

                var best = evaluations.OrderBy(i => i.Value).Last();

                if (best.Value > 5000) // Each unit has 100 durability
                {
                    var target = best.Key;
                    var visor = myUnits.Where(i => GetDistance(i, target) < 0.85 * i.Vehicle.VisionRange).OrderBy(i => i.X + i.Y).First();

                    ActionQueue.Add(new Action
                    {
                        Action = ActionType.TacticalNuclearStrike,
                        X = target.X,
                        Y = target.Y,
                        VehicleId = visor.Id,
                        Urgent = true
                    });
                }
            }
        }

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
                    move.Action = ActionType.Move;
                    move.X = targets.Average(i => i.X) - myUnits.Average(i => i.X);
                    move.Y = targets.Average(i => i.Y) - myUnits.Average(i => i.Y);

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