using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    /// <summary>
    /// Очередь приказов
    /// </summary>
    public class ActionQueue : List<Action>
    {
		/// <summary>
		/// Метка ожидания, устарела, нужно заменить
		/// </summary>
		private static int _wait = -1;

        /// <summary>
        /// Добавляет в очередь задержку на заданное число тиков
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public int WaitTicks(int ticks)
        {
            var action = new Action { WaitForWorldTick = Global.World.TickIndex + ticks };
            Add(action);
            return action.WaitForWorldTick;
        }
        
        /// <summary>
        /// Добавляет в очередь задержку на нужное число секунд
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public int WaitSeconds(double seconds)
        {
            var ticks = (int)(seconds * 60);
            return WaitTicks(ticks);
        }

		/// <summary>
		/// Обработка очереди
		/// </summary>
		/// <param name="world"></param>
		/// <param name="me"></param>
		/// <param name="move"></param>
		public void Process()
		{
			// Если в очереди етсь приказы и мы имеем право действовать
			if ( this.Any() && Global.Me.RemainingActionCooldownTicks == 0)
			{
				// Если существуют срочные приказы они выполняются без задержек
				var action = this.FirstOrDefault(i => i.Urgent && i.Ready);
				if (action != null)
				{
					Execute(action, Global.Move);
					return;
				}

				// Если метка ожидания пройдена 
				if ( Global.World.TickIndex >= _wait )
				{
					// Ищется готовое действие заданное не для группы либо для непустой группы готовой действовать
					action = this.FirstOrDefault(i => (i.Ready && (i.Formation == null || (!i.Formation.Busy && i.Formation.Units.Count > 0 ))));
					if (action != null)
					{
						_wait = -1;

						// Если текущая выделенная формация не совпадает с той для которой выполняется действие,
						// то надо вместо заданного приказа выполнить приказ на выделение
						if (action.Formation != null && Global.SelectedFormation != action.Formation)
						{
							Global.SelectedFormation = action.Formation;
							action = action.Formation.GetSelectionAction();
						}
						Execute(action, Global.Move);
						return;
					}
				}
			}
		}

		/// <summary>
		/// Выполнить приказ
		/// </summary>
		/// <param name="action"></param>
		/// <param name="move"></param>
		private void Execute( Action action, Move move )
		{
			if (action == null) return;

			if (action.WaitForWorldTick > 0)
			{
				_wait = action.WaitForWorldTick;
			}
			else
			{
				move.Action = action.Action;
				move.X = action.X;
				move.Y = action.Y;
				if (action.Relative)
				{
					var units = Global.MyUnits;
					if (action.Formation != null)
						units = action.Formation.Units;
					move.X = units.Average(i => i.X);
					move.Y = units.Average(i => i.Y);
					if (move.Action == ActionType.Move)
					{
						move.X -= action.X;
						move.Y -= action.Y;
					}
				}
				move.Angle = action.Angle;
				move.Right = action.Right;
				move.Bottom = action.Bottom;
				move.VehicleId = action.VehicleId;
				move.VehicleType = action.VehicleType;
				move.MaxSpeed = action.MaxSpeed;
				move.Factor = action.Factor;
				move.Group = action.Group;
				move.FacilityId = action.FacilityId;

				// Если приказ пришел от группы, то надо сдвинуть таймер группы на заданное число тиков
				if (action.Formation != null)
				{
					action.Formation.TickIndex = Global.World.TickIndex + action.WaitDuringTicks;
					if (action.Action == ActionType.ClearAndSelect)
						Global.SelectedFormation = action.Formation;
				}
			}

			if (action.Callback != null) action.Callback();

			Remove(action);
		}
    }
}
