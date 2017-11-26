using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed partial class MyStrategy : IStrategy
    {
		

        /// <summary>
        /// Основной метод стратегии
        /// </summary>
        /// <param name="me"></param>
        /// <param name="world"></param>
        /// <param name="game"></param>
        /// <param name="move"></param>
        public void Move(Player me, World world, Game game, Move move)
        {
            Global.Update( me, world, game, move );

			Tactic.GroupRush();

            //if (world.TickIndex < 0.75 * world.TickCount && !Tactic.EnemyIsWeaker )
           // {
                //Hurricane(world, me);
            //}
            //else
            //{
            //    Rush(expand: true);
            //}
            
            //EvadeNuclearStrike(world, me);
            NuclearStrike();
            Global.ActionQueue.Process();
        }
        
        
    }
}