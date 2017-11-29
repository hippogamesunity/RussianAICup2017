using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public static class Helpers
    {
        public static Position FindTarget(List<VehicleWrapper> units)
        {
            var middle = new Position(units.Average(i => i.X), units.Average(i => i.Y));

            return units.OrderBy(i => i.Distance(middle)).First().Position;
        }

        public static Position FindNearest(List<VehicleWrapper> units, Position position)
        {
            return units.OrderBy(i => i.Distance(position)).First().Position;
        }

        public static Position GetCenter(List<VehicleWrapper> units)
        {
            return new Position(units.Average(i => i.X), units.Average(i => i.Y));
        }

        public static Position GetCenter(VehicleType vehicleType)
        {
            return GetCenter(Global.MyUnits.Where(i => i.Type == vehicleType).ToList());
        }

        public static double GetLag(List<VehicleWrapper> units)
        {
            var center = GetCenter(units);

            return units.OrderBy(i => i.Distance(center)).Last().Position.Distance(center);
        }
    }
}