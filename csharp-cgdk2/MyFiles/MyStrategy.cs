using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed partial class MyStrategy : IStrategy
    {
        private int _index;
        private int _busy;

        private List<AI> _groups = new List<AI>
        {
            new AIFighters { Frequency = 6 },
            new AIHelicopters { Frequency = 6 },
            new AITanks { Frequency = 2 },
            new AIIfvs { Frequency = 2 },
            new AIArrvs { Frequency = 1 }
        };

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

            SelectRandomAI();
        }

        private void SelectRandomAI()
        {
            for (var i = 0; i < 10; i++) // ��������� ��������� ��� �� ������, ���� ��������� AI ����� ��������������
            {
                var frequencySum = _groups.Sum(j => j.Frequency);
                var random = CRandom.GetRandom(frequencySum);
                var offset = 0;

                foreach (var group in _groups)
                {
                    if (random < group.Frequency + offset)
                    {
                        var actions = group.PerformActions();

                        if (actions > 0)
                        {
                            _busy += actions;
                            return;
                        }

                        break;
                    }

                    offset += group.Frequency;
                }
            }
        }
    }
}