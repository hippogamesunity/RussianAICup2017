using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    /// <summary>
    /// Низкоуровневый генератор приказов. Лучше вместо него пользоваться методами объектов класса Formation
    /// </summary>
    class Command
    {
        #region SELECTION

        /// <summary>
        /// Выделить все объекты заданного типа
        /// </summary>
        /// <param name="type"></param>
        public static void SelectAll(VehicleType type)
        {
            Global.ActionQueue.Add( new Action { Action = ActionType.ClearAndSelect, Right = Global.World.Width, Bottom = Global.World.Height, VehicleType = type } );
        }

        /// <summary>
        /// Выделить все объекты
        /// </summary>
        /// <param name="world"></param>
		public static void SelectAll()
        {
            Global.ActionQueue.Add(new Action { Action = ActionType.ClearAndSelect, Right = Global.World.Width, Bottom = Global.World.Height });
        }

		/// <summary>
		/// Выделить объекты
		/// </summary>
		/// <param name="world"></param>
		public static void Select( Rect rect )
		{
			Global.ActionQueue.Add(new Action { Action = ActionType.ClearAndSelect, Left = rect.LeftTop.X, Right = rect.RightBottom.X, Top = rect.LeftTop.Y, Bottom = rect.RightBottom.Y });
		}

        #endregion

        #region GROUP

        /// <summary>
        /// Группировать выделенные юниты 
        /// </summary>
        /// <param name="group"></param>
		public static void Group(int group)
        {
            Global.ActionQueue.Add(new Action { Action = ActionType.Assign, Group = group });
        }

        #endregion

        #region SCALE

        /// <summary>
        /// Масштабирование относительно точки
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
		public static void Scale(double scale, double x, double y, Formation formation = null, int waitDuringTicks = 0, bool urgent = false )
        {
			Global.ActionQueue.Add(new Action { Action = ActionType.Scale, Factor = scale, X = x, Y = y, Formation = formation, WaitDuringTicks = waitDuringTicks, Urgent = urgent });
        }

		/// <summary>
		/// Масштабирование относительно точки
		/// </summary>
		/// <param name="scale"></param>
		/// <param name="position"></param>
		public static void Scale(double scale, Point position, Formation formation = null, int waitDuringTicks = 0, bool urgent = false)
        {
			Scale(scale, position.X, position.Y, formation, waitDuringTicks, urgent);
        }

		/// <summary>
		/// Масштабирование относительно центра масс юнитов игрока
		/// </summary>
		/// <param name="scale"></param>
		public static void ScaleRelative(double scale, Formation formation = null, int waitDuringTicks = 0, bool urgent = false)
        {
			Global.ActionQueue.Add(new Action { Action = ActionType.Scale, Factor = scale, Formation = formation, WaitDuringTicks = waitDuringTicks, Relative = true });
        }

        #endregion

		#region ROTATE

		/// <summary>
		/// Поворот вокруг точки на заданный угол
		/// </summary>
		/// <param name="angle"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public static void Rotate(double angle, double x, double y)
		{
			Global.ActionQueue.Add(new Action { Action = ActionType.Rotate, Angle = angle, X = x, Y = y, MaxSpeed = 1, MaxAngularSpeed = 1 });
		}

		/// <summary>
		/// Поворот вокруг точки на заданный угол
		/// </summary>
		/// <param name="angle"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public static void Rotate(double angle, Point position)
		{
			Rotate(angle, position.X, position.Y);
		}

		/// <summary>
		/// Поворот вокруг точки на заданный угол
		/// </summary>
		/// <param name="angle"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public static void RotateRelative(double angle, Formation formation = null, int waitDuringTicks = 0)
		{
			Global.ActionQueue.Add(new Action { Action = ActionType.Rotate, Angle = angle, MaxSpeed = 1, MaxAngularSpeed = 1, Relative = true, Formation = formation, WaitDuringTicks = waitDuringTicks });
		}

		#endregion

		#region MOVE

		/// <summary>
		/// Двигаться к точке с заданной скоростью
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="maxSpeed"></param>
		public static void Move(double x, double y, Formation formation = null, int waitDuringTicks = 0, double maxSpeed = 0)
		{
			Global.ActionQueue.Add(new Action { Action = ActionType.Move, X = x, Y = y, MaxSpeed = maxSpeed, Formation = formation, WaitDuringTicks = waitDuringTicks });
		}

		/// <summary>
		/// Двигаться к точке с заданной скоростью
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="maxSpeed"></param>
		public static void MoveRelative(double x, double y, Formation formation = null, int waitDuringTicks = 0, double maxSpeed = 0)
		{
			Global.ActionQueue.Add(new Action { Action = ActionType.Move, X = x, Y = y, MaxSpeed = maxSpeed, Formation = formation, WaitDuringTicks = waitDuringTicks, Relative = true });
		}

		#endregion
	}
}
