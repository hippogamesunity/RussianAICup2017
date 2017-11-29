using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed partial class MyStrategy : IStrategy
    {
        private readonly List<AI> _groups = new List<AI>
        {
            new AIFighters { Frequency = 8 },
            new AIHelicopters { Frequency = 8 },
            new AITanks { Frequency = 2 },
            new AIIfvs { Frequency = 2 },
            new AIArrvs { Frequency = 1 }
        };

        private List<Position> _aviaSpots = new List<Position> { new Position(45, 193), new Position(193, 45) }; 

        public void Move(Player me, World world, Game game, Move move)
        {
            Global.Update(me, world, game, move);

            if (Global.World.TickIndex < 240)
            {
                if (Global.World.TickIndex == 0)
                {
                    var centerF = Helpers.GetCenter(VehicleType.Fighter);
                    var centerH = Helpers.GetCenter(VehicleType.Helicopter);
                    var targetF = _aviaSpots.OrderBy(i => i.Distance(centerF)).First();
                    var targetH = _aviaSpots.Single(i => i != targetF);

                    Actions.SelectByType(VehicleType.Fighter);
                    Actions.Move(targetF - centerF);
                    Actions.SelectByType(VehicleType.Helicopter);
                    Actions.Move(targetH - centerH);
                }
            }
            else
            {
                Logic();
            }

            Global.ActionQueue.Process();
        }

        private void MoveToLine(Position center, VehicleType vehicleType)
        {
            var offset = center - new Position(119, 119);

            if (offset.X == 0 && offset.Y == 0) return;

            Actions.SelectByType(vehicleType);
            Actions.Move(offset);
        }
        
        private void Logic()
        {
            if (EvadeNuclearStrike() || NuclearStrike() || Global.ActionQueue.Any()) return;

            SelectRandomAI();
        }

        private void SelectRandomAI()
        {
            for (var i = 0; i < 10; i++) // Повторяем несколько раз на случай, если выбранный AI будет бездействовать
            {
                var frequencySum = _groups.Sum(j => j.Frequency);
                var random = CRandom.GetRandom(frequencySum);
                var offset = 0;

                foreach (var group in _groups)
                {
                    if (random < group.Frequency + offset)
                    {
                        if (group.PerformActions()) return;
                        
                        break;
                    }

                    offset += group.Frequency;
                }
            }
        }
    }
}