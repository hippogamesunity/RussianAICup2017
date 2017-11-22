using System;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed partial class MyStrategy
    {
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

            if (world.TickIndex > world.TickCount / 2)
            {
                Rush(world, me);
                return;
            }

            if (world.TickIndex == 300)
            {
                SelectAll(world);
                Scale(0.1, world.Width / 5, world.Height / 5);
            }

            if (world.TickIndex > 360 && world.TickIndex % 120 == 0)
            {
                var units = Units.Values.Where(j => j.PlayerId == me.Id).ToList();

                Rotate(Math.PI, units.Average(j => j.X), units.Average(j => j.Y));
            }
        }
    }
}
