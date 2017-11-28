using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class AIArrvs : AI
    {
        public AIArrvs()
        {
            VehicleType = VehicleType.Arrv;
        }

        public override int PerformActions()
        {
            var myArrvs = Global.MyUnits.Where(i => i.Type == VehicleType.Arrv).ToList();

            if (myArrvs.Count == 0) return 0;

            if (Helpers.GetLag(myArrvs) > 60) // Если застряли или растянулись, то сжимаемся
            {
                var actions = Compress(myArrvs);

                if (actions > 0) return actions;
            }

            var units = new List<VehicleWrapper>();

            foreach (var type in new[] { VehicleType.Tank, VehicleType.Ifv, VehicleType.Helicopter, VehicleType.Fighter })
            {
                units = Global.MyUnits.Where(i => i.Type == type).ToList();

                if (units.Any()) break;
            }

            if (units.Count == 0) return 0;

            var target = Helpers.FindTarget(units);
            var offset = target - Helpers.GetCenter(myArrvs);

            SelectGroup();
            Actions.Move(offset);

            return 2;
        }
    }
}