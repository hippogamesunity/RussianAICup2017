using System.Collections.Generic;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public abstract class Node
    {
        public abstract List<int> QueueIndex();

        public abstract int Update();

        private bool _compressed; // Чтобы не зацикливаться

        protected int Compress(List<VehicleWrapper> units)
        {
            if (_compressed)
            {
                _compressed = false;
                return 0;
            }

            Actions.SelectAll(units[0].Type);
            Actions.Scale(0.5, Helpers.GetCenter(units));

            _compressed = true;
            return 2;
        }
    }
}