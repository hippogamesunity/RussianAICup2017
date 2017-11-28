using System.Collections.Generic;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public abstract class Node
    {
        public abstract List<int> QueueIndex();

        public abstract int Update();
    }
}