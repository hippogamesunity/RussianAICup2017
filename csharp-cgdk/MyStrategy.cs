using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed partial class MyStrategy : IStrategy
    {
		public static Formation AllMyUnits = new Formation();
		public static Formation Fighters = new Formation(VehicleType.Fighter);
		public static Formation Helicopters = new Formation(VehicleType.Helicopter);
		public static Formation Arrvs = new Formation(VehicleType.Arrv);
		public static Formation Ifvs = new Formation(VehicleType.Ifv);
		public static Formation Tanks = new Formation(VehicleType.Tank);

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
			AllMyUnits.Update();
			Fighters.Update();
			Helicopters.Update();
			Arrvs.Update();
			Ifvs.Update();
			Tanks.Update();

			Rush(expand: true);

            //if (world.TickIndex < 0.75 * world.TickCount && !Tactic.EnemyIsWeaker )
           // {
                //Hurricane(world, me);
            //}
            //else
            //{
            //    Rush(expand: true);
            //}
            
            //EvadeNuclearStrike(world, me);
            //NuclearStrike(world, me, game);
            Global.ActionQueue.Process();
        }
        
        #region Common strategies

		#region RUSH

		private int _rushStartedTick = -1;
		private int _iteration;
        
		/// <summary>
		/// Тактика добивания
		/// </summary>
		/// <param name="expand"></param>
		private void Rush( bool expand )
		{
			if (_rushStartedTick == -1) _rushStartedTick = Global.World.TickIndex;

			var tick = Global.World.TickIndex - _rushStartedTick;

			if (tick == 0 && expand)
			{
				Fighters.Scale(2);
				Helicopters.Scale(2);
				Arrvs.Scale(2);
				Ifvs.Scale(2);
				Tanks.Scale(2);
			}
			else{
				if ( !Tanks.Busy )
					Tactic.CommonAttack( Tanks, new List<VehicleType> { VehicleType.Tank, VehicleType.Ifv, VehicleType.Arrv, VehicleType.Fighter, VehicleType.Helicopter });

				if (!Ifvs.Busy)
					Tactic.CommonAttack( Ifvs, new List<VehicleType> { VehicleType.Helicopter, VehicleType.Fighter, VehicleType.Arrv, VehicleType.Ifv, VehicleType.Tank });

				if (!Helicopters.Busy)
					Tactic.CommonAttack( Helicopters, new List<VehicleType> { VehicleType.Tank, VehicleType.Arrv, VehicleType.Helicopter, VehicleType.Ifv, VehicleType.Fighter } );

				if (!Fighters.Busy)
				{
					if (Global.Units.Values.Any(i => i.PlayerId != Global.Me.Id && (i.Type == VehicleType.Helicopter || i.Type == VehicleType.Fighter)))
					{
						Tactic.CommonAttack(Fighters, new List<VehicleType> { VehicleType.Helicopter, VehicleType.Fighter });
					}
					else
					{
						Tactic.CommonAttack(Fighters, new List<VehicleType> { VehicleType.Tank, VehicleType.Ifv, VehicleType.Arrv }, self: true);
					}
				}

				if (Arrvs.Units.Count > 0 && !Arrvs.Busy)
				{
					Arrvs.MoveTo(0.2 * Global.World.Width, 0.8 * Global.World.Height);
				}

				if (Tanks.Density < 0.02)
					Tanks.Scale(0.1);
				if (Ifvs.Density < 0.02)
					Ifvs.Scale(0.1);
				if (Helicopters.Density < 0.02)
					Helicopters.Scale(0.1);
				if (Fighters.Density < 0.02)
					Fighters.Scale(0.1);
				if (Arrvs.Density < 0.02)
					Arrvs.Scale(0.1);
			}
		}

		#endregion

		
        
        #endregion
    }
}