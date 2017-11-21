using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.MyClasses;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk {
    public sealed class MyStrategy : IStrategy
    {
        private readonly Dictionary<long, VehicleWrapper> _units = new Dictionary<long, VehicleWrapper>();
        private static readonly List<Move> ActionQueue = new List<Move>();

        private long _id;

        //  омментарии
        // "»значально количество возможных действий стратегии ограничено 12-ю ходами за 60 тиков" (ѕункт 2.6).
        // ѕо этой причине управление единичными юнитами не представл€етс€ возможным.
        // «а 60 тиков можно сделать всего 5 операций веделени€ и передвижени€.
        // ≈сли группа юнитов раздел€етс€ (из-за преп€тствий), это становитс€ проблемой дл€ ее управлени€, ведь координаты дл€ команды move - это смещени€, а не абсолютные значени€.
        public void Move(Player me, World world, Game game, Move move)
        {
            UpdateUnits(world);

            if (world.TickIndex == 0)
            {
                var unit = _units.Values.Where(i => i.PlayerId == me.Id).OrderBy(i => i.X + i.Y).Last();

                _id = unit.Id;
            }

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
                    MoveAll(VehicleType.Arrv, new List<VehicleType> { VehicleType.Tank, VehicleType.Ifv, VehicleType.Helicopter, VehicleType.Fighter }, me, self: true, maxSpeed: 0.3f);
                    break;
                case 30:
                    SelectAll(VehicleType.Helicopter, world);
                    MoveAll(VehicleType.Helicopter, new List<VehicleType> { VehicleType.Tank, VehicleType.Arrv, VehicleType.Ifv, VehicleType.Helicopter, VehicleType.Fighter }, me);
                    break;
                case 40:
                    SelectAll(VehicleType.Fighter, world);
                    MoveAll(VehicleType.Fighter, new List<VehicleType> { VehicleType.Helicopter, VehicleType.Fighter }, me);
                    break;
                case 50:
                    break;
            }

            if (ActionQueue.Any())
            {
                move.Action = ActionQueue[0].Action;
                move.X = ActionQueue[0].X;
                move.Y = ActionQueue[0].Y;
                move.Right = ActionQueue[0].Right;
                move.Bottom = ActionQueue[0].Bottom;
                move.VehicleType = ActionQueue[0].VehicleType;
                move.MaxSpeed = ActionQueue[0].MaxSpeed;
                ActionQueue.RemoveAt(0);
            }
        }

        private void UpdateUnits(World world)
        {
            foreach (var newVehicle in world.NewVehicles)
            {
                _units.Add(newVehicle.Id, new VehicleWrapper(newVehicle));
            }

            foreach (var vehicleUpdate in world.VehicleUpdates)
            {
                if (vehicleUpdate.Durability > 0)
                {
                    _units[vehicleUpdate.Id].Update(vehicleUpdate);
                }
                else
                {
                    _units.Remove(vehicleUpdate.Id);
                }
            }
        }

        private static void SelectAll(VehicleType type, World world)
        {
            var move = new Move
            {
                Action = ActionType.ClearAndSelect,
                Right = world.Width,
                Bottom = world.Height,
                VehicleType = type
            };

            ActionQueue.Add(move);
        }

        private void MoveAll(VehicleType type, List<VehicleType> targetTypes, Player me, bool self = false, float maxSpeed = 0)
        {
            var move = new Move();
            var myUnits = _units.Values.Where(i => i.Type == type && i.PlayerId == me.Id).ToList();

            if (myUnits.Count == 0) return;

            foreach (var targetType in targetTypes)
            {
                var targets = _units.Values.Where(i => i.Type == targetType && (self ? i.PlayerId == me.Id : i.PlayerId != me.Id)).ToList();

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
    }
}