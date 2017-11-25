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

		/// <summary>
		/// Двигать все свои юниты на заданный тип юнитов, выбирая ближайший 
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
					var target = targets.OrderBy(i => formation.Rectangle.Center.Distance(i)).First();

					formation.MoveTo( target.X, target.Y, Formation.TACTICAL_ORDER, maxSpeed );
					return;
				}
			}
		}
    }
}
