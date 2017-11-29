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

		public static Formation[] TanksGroup = new Formation[5];
		public static Formation[] IfvsGroup = new Formation[5];
		public static Formation[] ArrvsGroup = new Formation[5];

		public static int FormationsReady = 0;
		public static double FormationsAngle = 0 - Math.PI * 0.5;

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

		public static void CreateFormations()
		{
			AllMyUnits.Update();
			Fighters.Update();
			Helicopters.Update();
			Arrvs.Update();
			Ifvs.Update();
			Tanks.Update();

			int groupIndex = 1;
			var selectRect = new Rect();

			selectRect.LeftTop.X = Tanks.Rectangle.LeftTop.X;
			selectRect.RightBottom.X = Tanks.Rectangle.RightBottom.X;
			for (int i = 0; i < 5; i++)
			{
				selectRect.LeftTop.Y = Tanks.Rectangle.LeftTop.Y + i * 11;
				selectRect.RightBottom.Y = selectRect.LeftTop.Y + 11;
				Tactic.TanksGroup[i] = new Formation(selectRect, groupIndex++);
			}

			selectRect.LeftTop.X = Ifvs.Rectangle.LeftTop.X;
			selectRect.RightBottom.X = Ifvs.Rectangle.RightBottom.X;
			for (int i = 0; i < 5; i++)
			{
				selectRect.LeftTop.Y = Ifvs.Rectangle.LeftTop.Y + i * 11;
				selectRect.RightBottom.Y = selectRect.LeftTop.Y + 11;
				Tactic.IfvsGroup[i] = new Formation(selectRect, groupIndex++);
			}

			selectRect.LeftTop.X = Arrvs.Rectangle.LeftTop.X;
			selectRect.RightBottom.X = Arrvs.Rectangle.RightBottom.X;
			for (int i = 0; i < 5; i++)
			{
				selectRect.LeftTop.Y = Arrvs.Rectangle.LeftTop.Y + i * 11;
				selectRect.RightBottom.Y = selectRect.LeftTop.Y + 11;
				Tactic.ArrvsGroup[i] = new Formation(selectRect, groupIndex++);
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

			for (int i = 0; i < 5; i++)
			{
				TanksGroup[i].Update();
				IfvsGroup[i].Update();
				ArrvsGroup[i].Update();
			}
		}

		/// <summary>
		/// Тактичекий приказ: Двигать все свои юниты на заданный тип юнитов, выбирая ближайший 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="targetTypes"></param>
		/// <param name="me"></param>
		/// <param name="self"></param>
		/// <param name="maxSpeed"></param>
		public static void CommonAttack(Formation formation, List<VehicleType> targetTypes, bool self = false, double maxSpeed = 0)
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

		static int formationCompression = 0;

		static int formationMoveFreeze = 0;

		public static Point LastHurricaneCenter = new Point(0, 0);

		static VehicleWrapper GetNearestEnemy(Point hurricanecenter)
		{
			VehicleWrapper result = null;
			double d = Double.MaxValue;
			var units = Global.Units.Values.Where(i => i.PlayerId != Global.Me.Id).ToList();
			if (units.Count > 0)
			{
				foreach (var unit in units)
				{
					var nd = hurricanecenter.Distance(unit);
					if (nd < d)
					{
						d = nd;
						result = unit;
					}
				}
			}
			return result;
		}

		public static void Hurricane()
		{
			//FormationHurricane(Helicopters);
			//FormationHurricane(Fighters);
			//FormationHurricane(Tanks);
			//FormationHurricane(Arrvs);
			//FormationHurricane(Ifvs);			
		}

		protected static void FormationHurricane(Formation formation)
		{
			Point hurricaneCenter = formation.MassCenter;
			bool formationMoved = false;
			if (hurricaneCenter.Distance(LastHurricaneCenter) > 40)
				formationMoved = true;
			if (formationMoveFreeze > 0)
				formationMoveFreeze--;
			LastHurricaneCenter = hurricaneCenter;

			if (Global.World.TickIndex > FormationsReady && !formation.Busy)
			{
				formation.Scale(0.2);
			}

			if (Global.World.TickIndex > FormationsReady + 300 && Global.World.TickIndex < 10000 && !formation.Busy)
			{
				if (!formationMoved && formationMoveFreeze == 0)
				{
					var unit = GetNearestEnemy(hurricaneCenter);
					if (unit != null)
					{
						var unitDistance = hurricaneCenter.Distance(unit);
						if (unitDistance < 40)
						{
							formation.Scale(0.1);
						}
						else
						{
							formationMoveFreeze = 100;
							formation.MoveTo(unit.X, unit.Y, 180, 0.35);
						}
					}
				}
			}
		}

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

		#region LINE STRIKE

		public static void LineStrike()
		{
			if (!AllMyUnits.HasOrders() && !AllMyUnits.Busy)
			{
				var enemy = GetNearestEnemy(AllMyUnits.MassCenter);

				if (enemy != null)
				{
					var unitDistance = AllMyUnits.MassCenter.Distance(enemy);
					var unitAngle = Math.Asin((AllMyUnits.MassCenter.Y - enemy.Y) / unitDistance );
					if (AllMyUnits.MassCenter.X > enemy.X)
						unitAngle = Math.PI - unitAngle;
					var unitDistKoeff = (unitDistance<90 ? unitDistance-30 : 50);
					var unitDist = new Point(Math.Cos(unitAngle) * unitDistKoeff, Math.Sin(unitAngle) * unitDistKoeff);

					if (unitAngle != FormationsAngle)
					{
						AllMyUnits.Rotate(unitAngle - FormationsAngle);
						FormationsAngle = unitAngle;
					}
					if (unitDistance < 40)
					{
						
					}
					else
					{						
						AllMyUnits.MoveTo(AllMyUnits.MassCenter.X + unitDist.X, AllMyUnits.MassCenter.Y - unitDist.Y); 
					}
				}
			}
		}

		#endregion

		#endregion
	}
}
