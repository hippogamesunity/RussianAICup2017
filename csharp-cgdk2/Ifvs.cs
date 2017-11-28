using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class Ifvs : Node
    {
        public override List<int> QueueIndex()
        {
            return new List<int> { 5 };
        }

        public override int Update()
        {
            var myUnits = Global.MyUnits.Where(i => i.Type == VehicleType.Ifv).ToList();

            if (myUnits.Count == 0) return 0;

            var enemyUnits = new List<VehicleWrapper>();

            foreach (var type in new[] { VehicleType.Helicopter, VehicleType.Fighter, VehicleType.Ifv, VehicleType.Tank, VehicleType.Arrv })
            {
                enemyUnits = Global.EnemyUnits.Where(i => i.Type == type).ToList();

                if (enemyUnits.Any()) break;
            }

            if (enemyUnits.Count == 0) return 0;

            var target = Helpers.FindTarget(enemyUnits);
            var offset = target - Helpers.GetCenter(myUnits);

            Actions.SelectAll(VehicleType.Ifv);
            Actions.Move(offset);

            return 2;
        }
    }
}