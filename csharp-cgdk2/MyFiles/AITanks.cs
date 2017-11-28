using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class AITanks : AI
    {
        public AITanks()
        {
            VehicleType = VehicleType.Tank;
        }

        public override bool PerformActions()
        {
            var myTanks = Global.MyUnits.Where(i => i.Type == VehicleType.Tank).ToList();

            if (myTanks.Count == 0) return false;

            if (Helpers.GetLag(myTanks) > 60 && Compress(myTanks)) // Если застряли или растянулись, то сжимаемся
            {
                return true;
            }

            var targets = new List<VehicleWrapper>();

            foreach (var type in new[] { VehicleType.Ifv, VehicleType.Tank, VehicleType.Arrv, VehicleType.Fighter, VehicleType.Helicopter })
            {
                targets = Global.EnemyUnits.Where(i => i.Type == type).ToList();

                if (targets.Any()) break;
            }

            if (targets.Count == 0) return false;

            var target = Helpers.FindTarget(targets);
            var offset = target - Helpers.GetCenter(myTanks);

            SelectGroup();
            Actions.Move(offset);

            return true;
        }
    }
}