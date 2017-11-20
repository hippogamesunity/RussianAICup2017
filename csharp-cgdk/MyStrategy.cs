using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.MyClasses;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk {
    public sealed class MyStrategy : IStrategy
    {
        private readonly Dictionary<long, VehicleWrapper> _units = new Dictionary<long, VehicleWrapper>();

        public void Move(Player me, World world, Game game, Move move)
        {
            UpdateUnits(world);

            if (world.TickIndex == 0) // Stupid tick system. 1 tick = 1 action
            {
                move.Action = ActionType.ClearAndSelect;
                move.Right = world.Width;
                move.Bottom = world.Height;
                return;
            }

            if (world.TickIndex == 1)
            {
                move.Action = ActionType.Move;
                move.X = world.Width / 2;
                move.Y = world.Height / 2;
            }

            if (world.TickIndex == 3)
            {
                SelectFighters(world, move);
            }

            if (world.TickIndex >= 4)
            {
                MoveFigters(me, move);
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

        private static void SelectFighters(World world, Move move)
        {
            move.Action = ActionType.ClearAndSelect;
            move.Right = world.Width;
            move.Bottom = world.Height;
            move.VehicleType = VehicleType.Fighter;
        }

        private void MoveFigters(Player me, Move move)
        {
            var target = _units.Values.FirstOrDefault(i => i.Type == VehicleType.Helicopter && i.PlayerId != me.Id);

            if (target != null) // Fighters can't attack only helicopters
            {
                var myFighter = _units.Values.FirstOrDefault(i => i.Type == VehicleType.Fighter && i.PlayerId == me.Id);

                if (myFighter != null)
                {
                    move.Action = ActionType.Move;
                    move.X = target.X - myFighter.X;
                    move.Y = target.Y - myFighter.Y;
                }
            }
        }
    }
}