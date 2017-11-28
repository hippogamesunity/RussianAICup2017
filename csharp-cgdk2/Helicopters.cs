using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class Helicopters : Node
    {
        public override List<int> QueueIndex()
        {
            return new List<int> { 1, 4, 7 };
        }

        public override int Update()
        {
            var myHelicopters = Global.MyUnits.Where(i => i.Type == VehicleType.Helicopter).ToList();

            if (myHelicopters.Count == 0) return 0;

            if (Helpers.GetLag(myHelicopters) > 50) // Если застряли или растянулись, то сжимаемся
            {
                var actions = Compress(myHelicopters);

                if (actions > 0) return actions;
            }

            var attackActions = Attack(myHelicopters);

            return attackActions > 0 ? attackActions : DefaultBehaviour(myHelicopters);
        }

        private static int DefaultBehaviour(List<VehicleWrapper> myHelicopters) // Прикрываем своих
        {
            var myIfv = Global.MyUnits.Where(i => i.Type == VehicleType.Ifv).ToList();

            Actions.SelectAll(myHelicopters[0].Type);
            Actions.Move(Helpers.GetCenter(myIfv) - Helpers.GetCenter(myHelicopters));

            return 2;
        }

        private static int Attack(List<VehicleWrapper> myHelicopters)
        {
            // Далее наша задача - уничтожить танки, когда они не прикрываются самолетами или БТР
            // Иначе - поведение по уполчанию
            // Важное допущение - предполагаем, что соперник не поделил начальные формации на отряды. Иначе нужно определить эти отряды и анализировать их!

            var enemyTanks = Global.EnemyUnits.Where(i => i.Type == VehicleType.Tank).ToList();

            if (enemyTanks.Count == 0) return 0;

            var enemyFighters = Global.EnemyUnits.Where(i => i.Type == VehicleType.Fighter).ToList();
            var enemyIfvs = Global.EnemyUnits.Where(i => i.Type == VehicleType.Ifv).ToList();
            var dangers = new List<List<VehicleWrapper>>();

            if (enemyFighters.Any()) dangers.Add(enemyFighters);
            if (enemyIfvs.Any()) dangers.Add(enemyIfvs);

            // Атаковать можно, если мы прилетим к цели быстрее + у нас будет время на уничтожение целей
            // Пока не учитываем тип местрости и погоду

            var myCenter = Helpers.GetCenter(myHelicopters);
            var targetPosition = Helpers.FindNearest(enemyTanks, myCenter); // Пока ориентируемся по ближайшему юниту
            var myTime = targetPosition.Distance(myCenter) / myHelicopters[0].Vehicle.MaxSpeed;
            var safety = true;
            var destroyTime = 4; // Нужно оценить, исходя из количества нашей и вражеской техники

            foreach (var danger in dangers)
            {
                var position = Helpers.FindNearest(danger, targetPosition); // Пока ориентируемся по ближайшему юниту
                var time = targetPosition.Distance(position) / danger[0].Vehicle.MaxSpeed;

                if (myTime > time + destroyTime)
                {
                    safety = false;
                    break;
                }
            }

            if (safety) // Можем атаковать
            {
                Actions.SelectAll(myHelicopters[0].Type);
                Actions.Move(targetPosition - myCenter);

                return 2;
            }

            return 0;
        }
    }
}