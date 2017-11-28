using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class Arrvs : Node
    {
        public override List<int> QueueIndex()
        {
            return new List<int> { 8 };
        }

        public override int Update()
        {
            var myArrvs = Global.MyUnits.Where(i => i.Type == VehicleType.Arrv).ToList();

            if (myArrvs.Count == 0) return 0;

            if (Helpers.GetLag(myArrvs) > 50) // Если застряли или растянулись, то сжимаемся
            {
                var actions = Compress(myArrvs);

                if (actions > 0) return actions;
            }

            var units = new List<VehicleWrapper>();

            foreach (var type in new[] { VehicleType.Tank, VehicleType.Ifv, VehicleType.Helicopter, VehicleType.Fighter })
            {
                units = Global.EnemyUnits.Where(i => i.Type == type).ToList();

                if (units.Any()) break;
            }

            if (units.Count == 0) return 0;

            var target = Helpers.FindTarget(units);
            var offset = target - Helpers.GetCenter(myArrvs);

            Actions.SelectAll(myArrvs[0].Type);
            Actions.Move(offset);

            return 2;
        }
    }
}