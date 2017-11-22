using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed partial class MyStrategy : IStrategy
    {
        private static readonly Dictionary<long, VehicleWrapper> Units = new Dictionary<long, VehicleWrapper>();
        private static readonly List<Move> ActionQueue = new List<Move>();

        public void Move(Player me, World world, Game game, Move move)
        {
            UpdateUnits(world);
            //Hurricane(world, me);
            Rush(world, me);
            NuclearStrike(world, me, game);
            ProcessQueue(me, move);
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

        private static void ProcessQueue(Player me, Move move)
        {
            if (ActionQueue.Any() && me.RemainingActionCooldownTicks == 0)
            {
                move.Action = ActionQueue[0].Action;
                move.X = ActionQueue[0].X;
                move.Y = ActionQueue[0].Y;
                move.Angle = ActionQueue[0].Angle;
                move.Right = ActionQueue[0].Right;
                move.Bottom = ActionQueue[0].Bottom;
                move.VehicleId = move.VehicleId;
                move.VehicleType = ActionQueue[0].VehicleType;
                move.MaxSpeed = ActionQueue[0].MaxSpeed;
                move.Factor = ActionQueue[0].Factor;
                move.Group = ActionQueue[0].Group;
                move.FacilityId = ActionQueue[0].FacilityId;
                ActionQueue.RemoveAt(0);
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
                    var visors = myUnits.Where(i => GetDistance(i, target) < 0.75 * i.Vehicle.VisionRange).ToList();

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
                    var visor = myUnits.Where(i => GetDistance(i, target) < 0.75 * i.Vehicle.VisionRange).OrderBy(i => i.X + i.Y).First();

                    ActionQueue.Add(new Move
                    {
                        Action = ActionType.TacticalNuclearStrike,
                        X = target.X,
                        Y = target.Y,
                        VehicleId = visor.Id
                    });
                }
            }
        }

        #region Helpers

        private static void SelectAll(VehicleType type, World world)
        {
            ActionQueue.Add(new Move { Action = ActionType.ClearAndSelect, Right = world.Width, Bottom = world.Height, VehicleType = type });
        }

        private static void SelectAll(World world)
        {
            ActionQueue.Add(new Move { Action = ActionType.ClearAndSelect, Right = world.Width, Bottom = world.Height });
        }

        private static void Group(int group)
        {
            ActionQueue.Add(new Move { Action = ActionType.Assign, Group = group });
        }

        private static void Scale(double scale, VehicleType type, Player me)
        {
            var units = Units.Values.Where(i => i.Type == type && i.PlayerId == me.Id).ToList();
            var move = new Move { Action = ActionType.Scale, Factor = scale, X = units.Average(i => i.X), Y = units.Average(i => i.Y) };

            ActionQueue.Add(move);
        }

        private static void Scale(double scale, double x, double y)
        {
            ActionQueue.Add(new Move { Action = ActionType.Scale, Factor = scale, X = x, Y = y });
        }

        private static void Scale(double scale, Player me)
        {
            var units = Units.Values.Where(i => i.PlayerId == me.Id).ToList();
            var move = new Move { Action = ActionType.Scale, Factor = scale, X = units.Average(i => i.X), Y = units.Average(i => i.Y) };

            ActionQueue.Add(move);
        }

        private static void Move(double x, double y, double maxSpeed = 0)
        {
            ActionQueue.Add(new Move { Action = ActionType.Move, X = x, Y = y, MaxSpeed = maxSpeed });
        }

        private static void Rotate(double angle, double x, double y)
        {
            ActionQueue.Add(new Move { Action = ActionType.Rotate, Angle = angle, X = x, Y = y, MaxSpeed = 1, MaxAngularSpeed = 1 });
        }

        private void MoveAll(VehicleType type, List<VehicleType> targetTypes, Player me, bool self = false, double maxSpeed = 0)
        {
            var move = new Move();
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

        private double GetDistance(VehicleWrapper from, VehicleWrapper to)
        {
            var x = to.X - from.X;
            var y = to.Y - from.Y;

            return Math.Sqrt(x * x + y * y);
        }

        #endregion
    }
}