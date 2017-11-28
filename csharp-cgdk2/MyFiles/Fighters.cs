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
            var myFighters = Global.MyUnits.Where(i => i.Type == VehicleType.Fighter).ToList();

            if (myFighters.Count == 0) return 0;

            if (Helpers.GetLag(myFighters) > 50) // Если застряли или растянулись, то сжимаемся
            {
                var actions = Compress(myFighters);

                if (actions > 0) return actions;
            }

            var attackActions = Attack(myFighters);

            return attackActions > 0 ? attackActions : DefaultBehaviour(myFighters);
        }

        private static int DefaultBehaviour(List<VehicleWrapper> myFighters) // Прикрываем своих
        {
            var myTanks = Global.MyUnits.Where(i => i.Type == VehicleType.Tank).ToList();

            Actions.SelectAll(myFighters[0].Type);
            Actions.Move(Helpers.GetCenter(myTanks) - Helpers.GetCenter(myFighters));

            return 2;
        }

        private static int Attack(List<VehicleWrapper> myFighters)
        {
            // Далее наша задача - уничтожить вертолеты, когда они не прикрываются самолетами или БТР
            // Иначе - поведение по уполчанию
            // Важное допущение - предполагаем, что соперник не поделил начальные формации на отряды. Иначе нужно определить эти отряды и анализировать их!

            var enemyHelicopters = Global.EnemyUnits.Where(i => i.Type == VehicleType.Helicopter).ToList();

            if (enemyHelicopters.Count == 0) return 0;

            var enemyFighters = Global.EnemyUnits.Where(i => i.Type == VehicleType.Fighter).ToList();
            var enemyIfvs = Global.EnemyUnits.Where(i => i.Type == VehicleType.Ifv).ToList();
            var dangers = new List<List<VehicleWrapper>>();

            if (enemyFighters.Any()) dangers.Add(enemyFighters);
            if (enemyIfvs.Any()) dangers.Add(enemyIfvs);

            // Атаковать можно, если мы прилетим к цели быстрее + у нас будет время на уничтожение целей
            // Пока не учитываем тип местрости и погоду

            var myCenter = Helpers.GetCenter(myFighters);
            var targetPosition = Helpers.FindNearest(enemyHelicopters, myCenter); // Пока ориентируемся по ближайшему юниту
            var myTime = targetPosition.Distance(myCenter) / myFighters[0].Vehicle.MaxSpeed;
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
                Actions.SelectAll(myFighters[0].Type);
                Actions.Move(targetPosition - myCenter);

                return 2;
            }

            return 0;
        }
    }
}