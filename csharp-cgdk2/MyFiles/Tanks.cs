using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class Tanks : Node
    {
        public override List<int> QueueIndex()
        {
            return new List<int> { 2 };
        }

        public override int Update()
        {
            var myTanks = Global.MyUnits.Where(i => i.Type == VehicleType.Tank).ToList();

            if (myTanks.Count == 0) return 0;

            if (Helpers.GetLag(myTanks) > 50) // Если застряли или растянулись, то сжимаемся
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

            Actions.SelectAll(myTanks[0].Type);
            Actions.Move(offset);

            return 2;
        }
    }
}