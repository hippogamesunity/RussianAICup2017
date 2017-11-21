using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk {
    public sealed class MyStrategy : IStrategy
    {
        private static readonly Dictionary<long, VehicleWrapper> Units = new Dictionary<long, VehicleWrapper>();
        private static readonly List<Move> ActionQueue = new List<Move>();

        //  омментарии
        // "»значально количество возможных действий стратегии ограничено 12-ю ходами за 60 тиков" (ѕункт 2.6).
        // ѕо этой причине управление единичными юнитами не представл€етс€ возможным.
        // «а 60 тиков можно сделать всего 5 операций веделени€ и передвижени€.
        // ≈сли группа юнитов раздел€етс€ (из-за преп€тствий), это становитс€ проблемой дл€ ее управлени€, ведь координаты дл€ команды move - это смещени€, а не абсолютные значени€.
        public void Move(Player me, World world, Game game, Move move)
        {
            UpdateUnits(world);
            Hurricane(world, me);
            NuclearStrike(world, me, game);
            ProcessQueue(me, move);
        }

        #region Service

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
                move.VehicleType = ActionQueue[0].VehicleType;
                move.MaxSpeed = ActionQueue[0].MaxSpeed;
                move.Factor = ActionQueue[0].Factor;
                move.FacilityId = -1;
                move.VehicleId = -1;
                ActionQueue.RemoveAt(0);
            }
        }

        #endregion

        #region Strategies

        private void Hurricane(World world, Player me)
        {
            if (world.TickIndex == 0)
            {
                for (var i = 0; i < 5; i++)
                {
                    var type = (VehicleType)i;
                    var units = Units.Values.Where(j => j.Type == type && j.PlayerId == me.Id).ToList();

                    SelectAll(type, world);
                    Move(world.Width / 5 - units.Average(j => j.X), world.Height / 5 - units.Average(j => j.Y));
                }
            }

            if (world.TickIndex == 300)
            {
                SelectAll(world);
            }

            if (world.TickIndex > 300 && world.TickIndex % 120 == 0)
            {
                var units = Units.Values.Where(j => j.PlayerId == me.Id).ToList();

                Rotate(Math.PI, units.Average(j => j.X), units.Average(j => j.Y));
            }
        }

        private void Rush(World world, Player me)
        {
            switch (world.TickIndex % 60)
            {
                case 0:
                    SelectAll(VehicleType.Tank, world);
                    MoveAll(VehicleType.Tank, new List<VehicleType> { VehicleType.Ifv, VehicleType.Arrv, VehicleType.Tank, VehicleType.Fighter, VehicleType.Helicopter }, me);
                    break;
                case 10:
                    SelectAll(VehicleType.Ifv, world);
                    MoveAll(VehicleType.Ifv, new List<VehicleType> { VehicleType.Helicopter, VehicleType.Fighter, VehicleType.Arrv, VehicleType.Ifv, VehicleType.Tank }, me);
                    break;
                case 20:
                    SelectAll(VehicleType.Arrv, world);
                    MoveAll(VehicleType.Arrv, new List<VehicleType> { VehicleType.Tank, VehicleType.Ifv, VehicleType.Helicopter, VehicleType.Fighter }, me, self: true);
                    break;
                case 30:
                    SelectAll(VehicleType.Helicopter, world);
                    MoveAll(VehicleType.Helicopter, new List<VehicleType> { VehicleType.Ifv, VehicleType.Tank, VehicleType.Ifv, VehicleType.Helicopter, VehicleType.Fighter }, me, self: true);
                    break;
                case 40:
                    SelectAll(VehicleType.Fighter, world);
                    MoveAll(VehicleType.Fighter, new List<VehicleType> { VehicleType.Tank, VehicleType.Ifv, VehicleType.Ifv, VehicleType.Helicopter, VehicleType.Fighter }, me, self: true);
                    break;
                case 50:
                    break;
            }
        }

        private void NuclearStrike(World world, Player me, Game game)
        {
            if (me.NextNuclearStrikeTickIndex == -1 && world.TickIndex % 60 == 0)
            {
                var myUnits = Units.Values.Where(i => i.PlayerId == me.Id).ToList();
                var enemyUnits = Units.Values.Where(i => i.PlayerId != me.Id).ToList();

                foreach (var enemyUnit in enemyUnits)
                {
                    var visors = myUnits.Where(i => GetDistance(i, enemyUnit) < 0.9 * i.Vehicle.VisionRange).OrderByDescending(i => i.Durability).ToList();

                    if (visors.Count == 0) continue;
                    
                    var affected = Units.Values.Where(i => GetDistance(i, enemyUnit) < game.TacticalNuclearStrikeRadius).ToList();

                    if (affected.Count <= 30) continue;
                        
                    var my = affected.Count(i => i.PlayerId == me.Id);

                    if (my < 0.3 * affected.Count)
                    {
                        ActionQueue.Add(new Move
                        {
                            Action = ActionType.TacticalNuclearStrike,
                            X = enemyUnit.X,
                            Y = enemyUnit.Y,
                            VehicleId = visors[0].Id
                        });
                    }
                }
            }
        }

        #endregion

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

        private void MoveAll(VehicleType type, List<VehicleType> targetTypes, Player me, bool self = false, float maxSpeed = 0)
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

    /// <summary>
    /// Vehicle wrapper
    /// </summary>
    public class VehicleWrapper
    {
        public long Id;
        public VehicleType Type;
        public double X;
        public double Y;
        public int Durability;
        public long PlayerId;
        public Vehicle Vehicle;

        public VehicleWrapper(Vehicle vehicle)
        {
            Id = vehicle.Id;
            Type = vehicle.Type;
            X = vehicle.X;
            Y = vehicle.Y;
            Durability = vehicle.Durability;
            PlayerId = vehicle.PlayerId;
            Vehicle = vehicle;
        }

        public void Update(VehicleUpdate vehicleUpdate)
        {
            X = vehicleUpdate.X;
            Y = vehicleUpdate.Y;
            Durability = vehicleUpdate.Durability;
        }
    }
}