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

        public override int PerformActions()
        {
            var myIfvs = Global.MyUnits.Where(i => i.Type == VehicleType.Ifv).ToList();

            if (myIfvs.Count == 0) return 0;

            if (Helpers.GetLag(myIfvs) > 60) // Если застряли или растянулись, то сжимаемся
            {
                var actions = Compress(myIfvs);

                if (actions > 0) return actions;
            }

            var enemyUnits = new List<VehicleWrapper>();

            foreach (var type in new[] { VehicleType.Helicopter, VehicleType.Fighter, VehicleType.Ifv, VehicleType.Tank, VehicleType.Arrv })
            {
                enemyUnits = Global.EnemyUnits.Where(i => i.Type == type).ToList();

                if (enemyUnits.Any()) break;
            }

            if (enemyUnits.Count == 0) return 0;

            var target = Helpers.FindTarget(enemyUnits);
            var offset = target - Helpers.GetCenter(myIfvs);

            SelectGroup();
            Actions.Move(offset);

            return 2;
        }
    }
}