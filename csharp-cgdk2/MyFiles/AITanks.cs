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

        public override int PerformActions()
        {
            var myTanks = Global.MyUnits.Where(i => i.Type == VehicleType.Tank).ToList();

            if (myTanks.Count == 0) return 0;

            if (Helpers.GetLag(myTanks) > 60) // Если застряли или растянулись, то сжимаемся
            {
                var actions = Compress(myTanks);

                if (actions > 0) return actions;
            }

            var enemyUnits = new List<VehicleWrapper>();

            foreach (var type in new[] { VehicleType.Ifv, VehicleType.Tank, VehicleType.Arrv, VehicleType.Fighter, VehicleType.Helicopter })
            {
                enemyUnits = Global.EnemyUnits.Where(i => i.Type == type).ToList();

                if (enemyUnits.Any()) break;
            }

            if (enemyUnits.Count == 0) return 0;

            var target = Helpers.FindTarget(enemyUnits);
            var offset = target - Helpers.GetCenter(myTanks);

            SelectGroup();
            Actions.Move(offset);

            return 2;
        }
    }
}