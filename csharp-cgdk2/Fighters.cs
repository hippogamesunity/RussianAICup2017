using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class Fighters : Node
    {
        public override List<int> QueueIndex()
        {
            return new List<int> { 0, 3, 6 };
        }

        public override int Update()
        {
            var myUnits = Global.MyUnits.Where(i => i.Type == VehicleType.Fighter).ToList();

            if (myUnits.Count == 0) return 0;

            var enemyUnits = new List<VehicleWrapper>();

            foreach (var type in new[] { VehicleType.Helicopter, VehicleType.Fighter })
            {
                enemyUnits = Global.EnemyUnits.Where(i => i.Type == type).ToList();

                if (enemyUnits.Any()) break;
            }

            if (enemyUnits.Count == 0) return 0;

            var target = Helpers.FindTarget(enemyUnits);
            var offset = target - Helpers.GetCenter(myUnits);

            Actions.SelectAll(VehicleType.Fighter);
            Actions.Move(offset);

            return 2;
        }
    }
}