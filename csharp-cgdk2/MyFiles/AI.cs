using System;
using System.Collections.Generic;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public abstract class AI
    {
        public int Frequency = 1;
        public int GroupId = -1;
        public VehicleType VehicleType;

        /// <summary>
        /// Выполняет произвольные действия для своей группы
        /// </summary>
        /// <returns>Количество выполненных действий</returns>
        public abstract int PerformActions();

        protected void SelectGroup()
        {
            if (GroupId != -1)
            {
                Actions.SelectByGroup(GroupId);
            }
            else
            {
                Actions.SelectByType(VehicleType);
            }
        }

        private int _index; // Чтобы не зацикливаться не выполняем сжатие 2 раза подряд

        /// <summary>
        /// Чередование сжатия, вращения и бездействия обеспечивает скучивание групп даже из урагана (но это не точно).
        /// Если делать одно только сжатие, то возможно зацикливание, если юниты растянуты в линию.
        /// </summary>
        protected int Compress(List<VehicleWrapper> units)
        {
            SelectGroup();

            switch (_index++ % 5)
            {
                case 0:
                    SelectGroup();
                    Actions.Scale(0.5, Helpers.GetCenter(units));
                    return 2;
                case 1:
                    SelectGroup();
                    Actions.Rotate(Math.PI * (CRandom.Chance(50) ? 1 : -1), Helpers.GetCenter(units));
                    return 2;
                case 2:
                    SelectGroup();
                    Actions.Scale(0.5, Helpers.GetCenter(units));
                    return 2;
                case 3:
                case 4:
                    return 0;
            }

            return 0;
        }
    }
}