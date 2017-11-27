using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    /*TODO:
     * Поменять расчет слабости противника с количественного на качественный (по хп и атаке)
     * */


    /// <summary>
    /// Тактический помощник
    /// </summary>
    class Tactic
    {
		public static Formation AllMyUnits = new Formation();
		public static Formation Fighters = new Formation(VehicleType.Fighter);
		public static Formation Helicopters = new Formation(VehicleType.Helicopter);
		public static Formation Arrvs = new Formation(VehicleType.Arrv);
		public static Formation Ifvs = new Formation(VehicleType.Ifv);
		public static Formation Tanks = new Formation(VehicleType.Tank);

        /// <summary>
        /// 
        /// </summary>
        private static bool enemyIsWeaker = false;

        /// <summary>
        /// Враг стал слабее втрое (считается по количеству юнитов)
        /// </summary>
        public static bool EnemyIsWeaker
        {
            get
            {
                if ( !enemyIsWeaker )
                    enemyIsWeaker = Global.Units.Values.Count(i => i.PlayerId == Global.Me.Id) > 3 * Global.Units.Values.Count(i => i.PlayerId != Global.Me.Id);
                return enemyIsWeaker;
            }
        }

		public static void UpdateFormations()
		{
			AllMyUnits.Update();
			Fighters.Update();
			Helicopters.Update();
			Arrvs.Update();
			Ifvs.Update();
			Tanks.Update();
		}

		/// <summary>
		/// Тактичекий приказ: Двигать все свои юниты на заданный тип юнитов, выбирая ближайший 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="targetTypes"></param>
		/// <param name="me"></param>
		/// <param name="self"></param>
		/// <param name="maxSpeed"></param>
		public static void CommonAttack( Formation formation, List<VehicleType> targetTypes, bool self = false, double maxSpeed = 0)
		{
			var move = new Action();

			if (formation.Units.Count == 0) return;

			foreach (var targetType in targetTypes)
			{
				var targets = Global.Units.Values.Where(i => i.Type == targetType && (self ? i.PlayerId == Global.Me.Id : i.PlayerId != Global.Me.Id)).ToList();

				if (targets.Any())
				{
					var target = targets.OrderBy(i => formation.MassCenter.Distance(i)).First();

					formation.MoveTo( target.X, target.Y, Formation.TACTICAL_ORDER, maxSpeed );
					return;
				}
			}
		}
		
		#region Common strategies

		#region RUSH

		private static int _rushStartedTick = -1;
		private static int _iteration;

		/// <summary>
		/// Тактика добивания
		/// </summary>
		/// <param name="expand"></param>
		public static void Rush(bool expand)
		{
			if (_rushStartedTick == -1) _rushStartedTick = Global.World.TickIndex;

			var tick = Global.World.TickIndex - _rushStartedTick;

			if (tick == 0 && expand)
			{
				Fighters.Scale(4);
				Helicopters.Scale(4);
				Arrvs.Scale(4);
				Ifvs.Scale(4);
				Tanks.Scale(4);
			}
			else
			{
				if (!Tanks.HasOrders())
				{
					CommonAttack(Tanks, new List<VehicleType> { VehicleType.Tank, VehicleType.Ifv, VehicleType.Arrv, VehicleType.Fighter, VehicleType.Helicopter });
					Tanks.Scale(0.1);
				}
				if (!Ifvs.HasOrders())
				{
					CommonAttack(Ifvs, new List<VehicleType> { VehicleType.Helicopter, VehicleType.Fighter, VehicleType.Arrv, VehicleType.Ifv, VehicleType.Tank });
					Ifvs.Scale(0.1);
				}
				if (!Helicopters.HasOrders())
				{
					CommonAttack(Helicopters, new List<VehicleType> { VehicleType.Tank, VehicleType.Arrv, VehicleType.Helicopter, VehicleType.Ifv, VehicleType.Fighter });
					Helicopters.Scale(0.1);
				}
				if (!Fighters.HasOrders())
				{
					if (Global.Units.Values.Any(i => i.PlayerId != Global.Me.Id && (i.Type == VehicleType.Helicopter || i.Type == VehicleType.Fighter)))
					{
						CommonAttack(Fighters, new List<VehicleType> { VehicleType.Helicopter, VehicleType.Fighter });
					}
					else
					{
						CommonAttack(Fighters, new List<VehicleType> { VehicleType.Tank, VehicleType.Ifv, VehicleType.Arrv }, self: true);
					}
					Fighters.Scale(0.1);
				}

				if (!Arrvs.HasOrders())
				{
					Arrvs.MoveTo(0.2 * Global.World.Width, 0.8 * Global.World.Height);
					Arrvs.Scale(0.1);
				}
			}
		}

		/// <summary>
		/// Добивание всей кучей
		/// </summary>
		public static void GroupRush()
		{
			if (!AllMyUnits.HasOrders())
			{
				CommonAttack(AllMyUnits, new List<VehicleType> { VehicleType.Tank, VehicleType.Ifv, VehicleType.Helicopter, VehicleType.Fighter, VehicleType.Arrv });
				AllMyUnits.Scale(0.1);
			}
		}

		#endregion

		#region Hurricane

		/*int formationCompression = 0;

		int formationMoveFreeze = 0;

		public Point LastHurricaneCenter = new Point();

		Point GetHurricaneCenter(Player me, World world)
		{
			var units = Units.Values.Where(j => j.PlayerId == me.Id).ToList();
			return new Point(units.Average(j => j.X), units.Average(j => j.Y));
		}

		void CompressFormation(Point center)
		{
			if (formationCompression > -1)
			{
				formationCompression--;
				Scale(0.2, center.X, center.Y);
			}
		}

		void DecompressFormation(Point center)
		{
			if (formationCompression < 1)
			{
				formationCompression++;
				Scale(2, center.X, center.Y);
			}
		}

		VehicleWrapper GetNearestEnemy(Point hurricanecenter, Player me)
		{
			VehicleWrapper result = null;
			double d = Double.MaxValue;
			var units = Units.Values.Where(i => i.PlayerId != me.Id).ToList();
			if (units.Count > 0)
			{
				foreach (var unit in units)
				{
					var nd = GetDistance(hurricanecenter, unit);
					if (nd < d)
					{
						d = nd;
						result = unit;
					}
				}
			}
			return result;
		}

		private void Hurricane(World world, Player me)
		{
			Point hurricaneCenter = AllMyUnits.MassCenter;
			bool formationMoved = false;
			if ( hurricaneCenter.Distance(LastHurricaneCenter) > 40 )
				formationMoved = true;
			if (formationMoveFreeze > 0)
				formationMoveFreeze--;
			LastHurricaneCenter = hurricaneCenter;

			if (world.TickIndex > FormationsCreatedTick && world.TickIndex % 180 == 0)
			{
				AllMyUnits.R
				Rotate(Math.PI, hurricaneCenter.X, hurricaneCenter.Y);
			}

			if (world.TickIndex > FormationsCreatedTick && world.TickIndex % 360 == 100)
			{
				Scale(0.2, hurricaneCenter.X, hurricaneCenter.Y);
			}

			if (world.TickIndex > FormationsCreatedTick + 300 && world.TickIndex < 10000)
			{
				if (!formationMoved && formationMoveFreeze == 0)
				{
					var unit = GetNearestEnemy(hurricaneCenter, me);
					if (unit != null)
					{
						var unitDistance = GetDistance(hurricaneCenter, unit);
						if (unitDistance < 40)
						{
							CompressFormation(hurricaneCenter);
						}
						else
						{
							formationMoveFreeze = 100;
							DecompressFormation(hurricaneCenter);
							Move(unit.X - hurricaneCenter.X, unit.Y - hurricaneCenter.Y, 0.35);
						}
					}
				}
			}
		}*/

		#endregion

		#region ANTI NUKE

		public static AntiNukeInfo AntiNuke = null;

		public class AntiNukeInfo
		{
			public Point NukePoint;

			public int Tick;
		}

		public static void AntiNuclearStrike()
		{
			Player opponent = Global.World.GetOpponentPlayer();
			if (opponent.NextNuclearStrikeVehicleId != -1 && AntiNuke == null)
			{
				AllMyUnits.UrgentScale(10, opponent.NextNuclearStrikeX, opponent.NextNuclearStrikeY);
				AntiNuke = new AntiNukeInfo();
				AntiNuke.NukePoint = new Point(opponent.NextNuclearStrikeX, opponent.NextNuclearStrikeY);
				AntiNuke.Tick = opponent.NextNuclearStrikeTickIndex;
			}
			else if (AntiNuke != null)
			{
				if (AntiNuke.Tick < Global.World.TickIndex - 50)
				{
					AntiNuke = null;
				}
				else if (AntiNuke.Tick + 20 == Global.World.TickIndex)
				{
					AllMyUnits.UrgentScale(0.2, AntiNuke.NukePoint.X, AntiNuke.NukePoint.Y);
				}
			}
		}

		#endregion

		#endregion
	}
}
