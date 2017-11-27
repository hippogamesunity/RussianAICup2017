using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;



namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    /*TODO:
     * Создание группы не по одному типу юнитов, а по их сочетанию
     * Создание группы по выделенной в данной момент области
     * 
     * Тип приказа: моментальный, тактический, стратегический. Моментальный приказ прерывается любым новым приказом группе, т.е.
     * не меняет ее TickIndex. Тактический приказ действует в течение короткого срока порядка нескольких секунд, после чего его
     * может заменить новый приказ в стеке. Стратегический приказ действует до окончания своего срока и не прерывается.
     * 
     * 
     * Изменение габаритного контейнера группы если она масштабируется
     * */



    /// <summary>
    /// Группа юнитов. Имеет методы по управлению. Для группы очередь гарантирует независимую последовательность приказов, 
    /// временные задержки на их выполнение и то, что приказ, переданный данной группе, будет выполнен именно ей.
    /// </summary>
    public class Formation
    {
		/// <summary>
		/// Единоразовый приказ. Любой новый приказ отменит его
		/// </summary>
		public static int MOMENT_ORDER = 0;

		/// <summary>
		/// Тактический приказ. Через 2 сек его действие можно отменить новым
		/// </summary>
		public static int TACTICAL_ORDER = 120;

		/// <summary>
		/// Стратегический приказ. Не дает выполняться другим заданиям до тех пор пока группа его не выполнит.
		/// </summary>
		public static int STRATEGICAL_ORDER = 20000;

        /// <summary>
        /// Входящие в группу юниты
        /// </summary>
        public List<VehicleWrapper> Units;

        /// <summary>
        /// Габаритный контейнер, который группа занимает на карте
        /// </summary>
        public Rect Rectangle;

		/// <summary>
		/// Центр масс группы
		/// </summary>
		public Point MassCenter = new Point(0,0);

		/// <summary>
		/// Плотность группы (кол-во юнитов в кругу ограниченном центром и самым дальним юнитом)
		/// </summary>
		public double Density { get {
			if (Units.Count == 0)
				return 0;
			var radius = Rectangle.Center.Distance( Units.OrderBy(i => Rectangle.Center.Distance(i)).Last() );
			var sq = Math.PI * radius * radius;
			return Units.Count / sq;
		} }

        /// <summary>
        /// Тип юнитов в группе
        /// </summary>
        private VehicleType Type;

		/// <summary>
		/// Индекс отряда. -1 = все юниты, 0 = отряд задается типом юнитов, >1 = номер отряда
		/// </summary>
		private int GroupIndex = -1;

        /// <summary>
        /// Внутренний таймер группы - новые приказы не посылаются группе, пока ее TickIndex > TickIndex'a мира
        /// </summary>
        public int TickIndex = 0;

		/// <summary>
		/// Флаг, готова ли формация выполнить новый приказ
		/// </summary>
		public bool Busy { get { return Global.World.TickIndex < TickIndex; } }

        /// <summary>
        /// Создание группы на основе типа юнитов
        /// </summary>
        /// <param name="type"></param>
        public Formation(VehicleType type)
        {
            Type = type;
			GroupIndex = 0;
        }

		/// <summary>
        /// Создание группы на основе типа юнитов
        /// </summary>
        /// <param name="type"></param>
        public Formation()
        {
        }

        /// <summary>
        /// Обновление данных формации. Нужно вызывать каждый тик
        /// </summary>
        public void Update()
        {
			if ( GroupIndex == -1 )
				Units = Global.MyUnits;
			else
				Units = Global.Units.Values.Where(j => j.Type == Type && j.PlayerId == Global.Me.Id).ToList();
            Rectangle = new Rect(Units);
			if (Units.Count > 0)
				MassCenter = new Point(Units.Average(i => i.X), Units.Average(i => i.Y));
			else
				MassCenter = Point.Zero;
        }

        /// <summary>
        /// Перемещение группы в точку с заданными координатами. 
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
		public void MoveTo(double X, double Y, int WaitDuringTicks = 20000, double MaxSpeed = 0)
        {
            // Дистанция до цели
            double dist = MassCenter.Distance(X, Y);
			if (MaxSpeed > 0)
				dist /= MaxSpeed;
			else
				dist /= 0.6;
			int tickIndex = (int)(dist * 1.1);

			if ( tickIndex > WaitDuringTicks )
				tickIndex = WaitDuringTicks;

            // Перемещение группы по заданному вектору. Группа задает расчетное время ( расстояние * константа ) за которое она переместится. Пока это время
            // не истечет, группа не будет получать новых приказов (за исключением urgent)
            Command.MoveRelative(X, Y, this, tickIndex, MaxSpeed);
        }

        /// <summary>
        /// Масштабирование группы
        /// </summary>
        /// <param name="value"></param>
		public void Scale(double value, int WaitDuringTicks = 20000)
        {
            // Дистанция от центра до угла 
            double dist = Rectangle.Center.Distance( Rectangle.LeftTop );
			int tickIndex = (int)(dist * 1.5 * value);
			if ( tickIndex > WaitDuringTicks )
				tickIndex = WaitDuringTicks;
            Command.ScaleRelative(value, this, tickIndex);
        }

		/// <summary>
		/// Срочное масштабирование группы (уклонение от атаки)
		/// </summary>
		/// <param name="value"></param>
		public void UrgentScale(double value, double X, double Y)
		{
			Command.Scale(value, X, Y, this, 0, true);
		}

		/// <summary>
		/// Имеются ли в очереди приказы для данной группы. Если имеются новых не добавлять
		/// </summary>
		/// <returns></returns>
		public bool HasOrders()
		{
			return Global.ActionQueue.FirstOrDefault(i => i.Formation == this) != null;
		}

		/// <summary>
		/// Возвращает команду для выбора текущей группы
		/// </summary>
		/// <returns></returns>
		public Action GetSelectionAction()
		{
			var Action = new Action { Action = ActionType.ClearAndSelect, Left = 0, Right = Global.World.Width, Top = 0, Bottom = Global.World.Height };
			if (GroupIndex == 0)
				Action.VehicleType = Type;
			if (GroupIndex > 0)
				Action.Group = GroupIndex;
			return Action;
		}
	}
}