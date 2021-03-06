﻿using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class AIFighters : AI
    {
        public AIFighters()
        {
            VehicleType = VehicleType.Fighter;
        }

        public override bool PerformActions()
        {
            var myFighters = Global.MyUnits.Where(i => i.Type == VehicleType.Fighter).ToList();

            if (myFighters.Count == 0) return false;

            if (Helpers.GetLag(myFighters) > 60 && Compress(myFighters)) // Если застряли или растянулись, то сжимаемся
            {
                return true;
            }

            return Attack(myFighters) || DefaultBehaviour(myFighters);
        }

        private bool DefaultBehaviour(List<VehicleWrapper> myFighters) // Прикрываем своих
        {
            var targets = Global.MyUnits.Where(i => i.Type == VehicleType.Tank).ToList();

            if (targets.Count == 0) return false;

            SelectGroup();
            Actions.Move(Helpers.GetCenter(targets) - Helpers.GetCenter(myFighters));

            return true;
        }

        private bool Attack(List<VehicleWrapper> myFighters)
        {
            // Главная задача - уничтожить самолеты и вертолеты, когда они не прикрываются БТР
            // Иначе - поведение по уполчанию
            // Важное допущение - предполагаем, что соперник не поделил начальные формации на отряды. Иначе нужно определить эти отряды и анализировать их!

            var targets = Global.EnemyUnits.Where(i => i.Type == VehicleType.Fighter || i.Type == VehicleType.Helicopter).ToList();
            
            if (targets.Count == 0) return false;

            //var enemyFighters = Global.EnemyUnits.Where(i => i.Type == VehicleType.Fighter).ToList();
            var enemyIfvs = Global.EnemyUnits.Where(i => i.Type == VehicleType.Ifv).ToList();
            var dangers = new List<List<VehicleWrapper>>();

            //if (enemyFighters.Any() && targets[0].Type != enemyFighters[0].Type) dangers.Add(enemyFighters);
            if (enemyIfvs.Any()) dangers.Add(enemyIfvs);

            // Атаковать можно, если мы прилетим к цели быстрее + у нас будет время на уничтожение целей
            // Пока не учитываем тип местрости и погоду

            var myCenter = Helpers.GetCenter(myFighters);
            var targetPosition = Helpers.FindNearest(targets, myCenter); // Пока ориентируемся по ближайшему юниту
            var myTime = targetPosition.Distance(myCenter) / myFighters[0].Vehicle.MaxSpeed;
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

                offset -= offset.Normalized * myFighters[0].Vehicle.AerialAttackRange;

                SelectGroup();
                Actions.Move(offset);

                return true;
            }

            return false;
        }
    }
}