using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class AIHelicopters : AI
    {
        public AIHelicopters()
        {
            VehicleType = VehicleType.Helicopter;
        }

        public override bool PerformActions()
        {
            var myHelicopters = Global.MyUnits.Where(i => i.Type == VehicleType.Helicopter).ToList();

            if (myHelicopters.Count == 0) return false;

            if (Helpers.GetLag(myHelicopters) > 60 && Compress(myHelicopters)) // Если застряли или растянулись, то сжимаемся
            {
                return true;
            }

            return Attack(myHelicopters) || DefaultBehaviour(myHelicopters);
        }

        private bool DefaultBehaviour(List<VehicleWrapper> myHelicopters) // Прикрываем своих
        {
            var targets = Global.MyUnits.Where(i => i.Type == VehicleType.Ifv).ToList();

            if (targets.Count == 0) return false;

            SelectGroup();
            Actions.Move(Helpers.GetCenter(targets) - Helpers.GetCenter(myHelicopters));

            return true;
        }

        private bool Attack(List<VehicleWrapper> myHelicopters)
        {
            // Главная задача - уничтожить танки, когда они не прикрываются самолетами, вертолетами или БТР
            // Второстепенная задача - уничтожить ремонтников, когда они не прикрываются самолетами, вертолетами или БТР
            // Иначе - поведение по уполчанию
            // Важное допущение - предполагаем, что соперник не поделил начальные формации на отряды. Иначе нужно определить эти отряды и анализировать их!

            var targets = Global.EnemyUnits.Where(i => i.Type == VehicleType.Tank).ToList();

            if (targets.Count == 0)
            {
                targets = Global.EnemyUnits.Where(i => i.Type == VehicleType.Arrv).ToList();

                if (targets.Count == 0) return false;
            }

            var enemyFighters = Global.EnemyUnits.Where(i => i.Type == VehicleType.Fighter).ToList();
            var enemyIfvs = Global.EnemyUnits.Where(i => i.Type == VehicleType.Ifv).ToList();
            var dangers = new List<List<VehicleWrapper>>();

            if (enemyFighters.Any() && targets[0].Type != enemyFighters[0].Type) dangers.Add(enemyFighters);
            if (enemyIfvs.Any() && targets[0].Type != enemyIfvs[0].Type && enemyIfvs.Sum(i => i.Durability) > myHelicopters.Sum(i => i.Durability)) dangers.Add(enemyIfvs);

            // Атаковать можно, если мы прилетим к цели быстрее + у нас будет время на уничтожение целей
            // Пока не учитываем тип местрости и погоду

            var myCenter = Helpers.GetCenter(myHelicopters);
            var targetPosition = Helpers.FindNearest(targets, myCenter); // Пока ориентируемся по ближайшему юниту
            var myTime = targetPosition.Distance(myCenter) / myHelicopters[0].Vehicle.MaxSpeed;
            var safety = true;
            var destroyTime = 4; // Нужно оценить, исходя из количества нашей и вражеской техники

            foreach (var danger in dangers)
            {
                var position = Helpers.FindNearest(danger, targetPosition); // Пока ориентируемся по ближайшему юниту
                var time = targetPosition.Distance(position) / danger[0].Vehicle.MaxSpeed;

                if (position.Distance(myCenter) < 250 && myTime > time + destroyTime)
                {
                    safety = false;
                    break;
                }
            }

            if (safety) // Можем атаковать
            {
                var offset = targetPosition - myCenter;

                offset -= 0.75 * offset.Normalized * myHelicopters[0].Vehicle.GroundAttackRange;

                SelectGroup();
                Actions.Move(offset);

                return true;
            }

            return false;
        }
    }
}