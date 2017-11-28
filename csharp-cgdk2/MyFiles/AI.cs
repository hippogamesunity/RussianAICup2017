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
        /// <returns>Возвращает false, если AI пропускает действие</returns>
        public abstract bool PerformActions();

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

        private int _index;

        /// <summary>
        /// Чередование сжатия, вращения и бездействия обеспечивает скучивание групп даже из урагана (но это не точно).
        /// Если делать одно только сжатие, то возможно зацикливание, если юниты растянуты в линию.
        /// </summary>
        protected bool Compress(List<VehicleWrapper> units)
        {
            SelectGroup();

            switch (_index++ % 5)
            {
                case 0:
                    SelectGroup();
                    Actions.Scale(0.5, Helpers.GetCenter(units));
                    return true;
                case 1:
                    SelectGroup();
                    Actions.Rotate(Math.PI, Helpers.GetCenter(units));
                    return true;
                case 2:
                    SelectGroup();
                    Actions.Scale(0.5, Helpers.GetCenter(units));
                    return true;
                default:
                    return false;
            }
        }
    }
}