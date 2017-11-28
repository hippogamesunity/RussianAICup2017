using System.Collections.Generic;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed partial class MyStrategy : IStrategy
    {
        private int _index;
        private int _busy;

        private readonly Fighters _fighters = new Fighters();
        private readonly Helicopters _helicopters = new Helicopters();
        private readonly Tanks _tanks = new Tanks();
        private readonly Ifvs _ifvs = new Ifvs();
        private readonly Arrvs _arrvs = new Arrvs();

        public void Move(Player me, World world, Game game, Move move)
        {
            Global.Update(me, world, game, move);
            Logic();
            Global.ActionQueue.Process();
        }
        
        private void Logic()
        {
            if (EvadeNuclearStrike() || NuclearStrike()) return;
            if (Global.World.TickIndex % 12 > 0 || _busy-- > 0) return;

            var queueIndex = _index++ % 9; // F H T F H I F H A

            foreach (var node in new List<Node> { _fighters, _helicopters, _tanks, _ifvs, _arrvs })
            {
                if (node.QueueIndex().Contains(queueIndex))
                {
                    _busy += node.Update();

                    if (_busy > 0) return;
                }
            }
        }
    }
}