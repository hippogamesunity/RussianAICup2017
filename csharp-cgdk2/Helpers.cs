using System.Collections.Generic;
using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public static class Helpers
    {
        public static Position FindTarget(List<VehicleWrapper> units)
        {
            var middle = new Position(units.Average(i => i.X), units.Average(i => i.Y));

            return units.OrderBy(i => i.Distance(middle)).First().Position;
        }

        public static Position GetCenter(List<VehicleWrapper> units)
        {
            return new Position(units.Average(i => i.X), units.Average(i => i.Y));
        }
    }
}