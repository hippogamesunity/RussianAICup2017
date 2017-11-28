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

        public override bool PerformActions()
        {
            var myArrvs = Global.MyUnits.Where(i => i.Type == VehicleType.Arrv).ToList();

            if (myArrvs.Count == 0) return false;

            if (Helpers.GetLag(myArrvs) > 60 && Compress(myArrvs)) // Если застряли или растянулись, то сжимаемся
            {
                return true;
            }

            var targets = new List<VehicleWrapper>();

            foreach (var type in new[] { VehicleType.Tank, VehicleType.Ifv, VehicleType.Helicopter, VehicleType.Fighter })
            {
                targets = Global.MyUnits.Where(i => i.Type == type).ToList();

                if (targets.Any()) break;
            }

            if (targets.Count == 0) return false;

            var target = Helpers.FindTarget(targets);
            var offset = target - Helpers.GetCenter(myArrvs);

            SelectGroup();
            Actions.Move(offset);

            return true;
        }
    }
}