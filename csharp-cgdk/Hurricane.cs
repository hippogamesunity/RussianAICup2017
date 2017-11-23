using System;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed partial class MyStrategy
    {
        private readonly int[] _intervals = { 800, -1, -1, -1, -1, -1 };

        private void Hurricane(World world, Player me)
        {
            Compress(world, me);

            if (world.TickIndex == _intervals[0])
            {
                var middle = Middle(me);

                SelectAll(VehicleType.Tank, world);
                Scale(GetRandom(1, 1.2) / 300f, middle);

                SelectAll(VehicleType.Ifv, world);
                Scale(GetRandom(1, 1.25), middle);

                SelectAll(VehicleType.Arrv, world);
                Scale(GetRandom(1, 1.25), middle);

                SelectAll(VehicleType.Helicopter, world);
                Scale(GetRandom(1, 1.30), middle);

                SelectAll(VehicleType.Fighter, world);
                Scale(GetRandom(1, 1.40), middle);

                _intervals[1] = WaitSeconds(world, GetRandom(1, 1.5f));
            }

            if (world.TickIndex == _intervals[1])
            {
                Rotate(world, Middle(me));

                _intervals[2] = WaitSeconds(world, GetRandom(1, 2));
            }

            if (world.TickIndex == _intervals[2])
            {
                SelectAll(world);
                Scale(0.1, Middle(me));

                _intervals[3] = WaitSeconds(world, GetRandom(1, 2));
            }

            if (world.TickIndex == _intervals[3])
            {
                Rotate(world, Middle(me));

                _intervals[4] = WaitSeconds(world, GetRandom(1, 2));
            }

            if (world.TickIndex == _intervals[4])
            {
                var middle = Middle(me);

                SelectAll(VehicleType.Helicopter, world);
                Scale(GetRandom(1, 1.3), middle);
                SelectAll(VehicleType.Fighter, world);
                Scale(GetRandom(1, 1.4), middle);

                _intervals[5] = WaitSeconds(world, GetRandom(0.5, 1));
            }

            if (world.TickIndex == _intervals[5])
            {
                var enemy = Units.Values.Where(i => i.PlayerId != me.Id).ToList();

                if (enemy.Count == 0) return;

                var center = new Position(enemy.Average(i => i.X), enemy.Average(i => i.Y));
                var target = enemy.Select(i => new Position(i.X, i.Y)).OrderBy(i => i.Distance(center)).First();
                var direction = target - Middle(me);

                SelectAll(world);
                Move(direction.X, direction.Y, 0.3);

                _intervals[0] = WaitSeconds(world, GetRandom(1, 2));
            }
        }

        private void Rotate(World world, Position middle)
        {
            SelectAll(VehicleType.Tank, world);
            Rotate(Math.PI - GetRandom(0, 1), middle);

            SelectAll(VehicleType.Ifv, world);
            Rotate(Math.PI - GetRandom(0, 1), middle);

            SelectAll(VehicleType.Arrv, world);
            Rotate(Math.PI - GetRandom(0, 1), middle);

            SelectAll(VehicleType.Helicopter, world);
            Rotate(Math.PI - GetRandom(0, 1), middle);

            SelectAll(VehicleType.Fighter, world);
            Rotate(Math.PI - GetRandom(0, 1), middle);
        }

        private void Compress(World world, Player me)
        {
            if (world.TickIndex == 0)
            {
                SelectAll(world);
                Scale(0.5f, me);
            }

            foreach (var tick in new[] { 60, 120, 180, 240, 300, 360, 420, 480, 540, 600 })
            if (world.TickIndex == tick)
            {
                Rotate(Math.PI, Middle(me));
            }

            foreach (var tick in new[] { 660, 720 })
            if (world.TickIndex == tick)
            {
                Scale(0.1f, me);
            }
        }
    }
}