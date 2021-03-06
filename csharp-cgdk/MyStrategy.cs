using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed partial class MyStrategy : IStrategy
    {
		

        /// <summary>
        /// �������� ����� ���������
        /// </summary>
        /// <param name="me"></param>
        /// <param name="world"></param>
        /// <param name="game"></param>
        /// <param name="move"></param>
        public void Move(Player me, World world, Game game, Move move)
        {
            Global.Update( me, world, game, move );		

			if ( world.TickIndex == 0 )
				CreateFormations();
			else if ( world.TickIndex > 800 && Tactic.FormationsReady == 0 )
				MergeFormations2();
			if (Tactic.FormationsReady > 0 && world.TickIndex == Tactic.FormationsReady)
			{
			}
			if (Tactic.FormationsReady > 0 && world.TickIndex > Tactic.FormationsReady )
			{
				if ( Tactic.AntiNuke == null )
					Tactic.LineStrike();				
				Tactic.AntiNuclearStrike();
			}			
            NuclearStrike();
			Global.ActionQueue.Process();
        }
    }
}