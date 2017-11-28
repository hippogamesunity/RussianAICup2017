using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class AIIfvs : AI
    {
        public AIIfvs()
        {
            VehicleType = VehicleType.Ifv;
        }

        public override bool PerformActions()
        {
            var myIfvs = Global.MyUnits.Where(i => i.Type == VehicleType.Ifv).ToList();

            if (myIfvs.Count == 0) return false;

            if (Helpers.GetLag(myIfvs) > 60 && Compress(myIfvs)) // Если застряли или растянулись, то сжимаемся
            {
                return true;
            }

            var targets = new List<VehicleWrapper>();

            foreach (var type in new[] { VehicleType.Helicopter, VehicleType.Fighter, VehicleType.Ifv, VehicleType.Tank, VehicleType.Arrv })
            {
                targets = Global.EnemyUnits.Where(i => i.Type == type).ToList();

                if (targets.Any()) break;
            }

            if (targets.Count == 0) return false;

            var target = Helpers.FindTarget(targets);
            var offset = target - Helpers.GetCenter(myIfvs);

            SelectGroup();
            Actions.Move(offset);

            return true;
        }
    }
}