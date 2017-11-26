using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    /// <summary>
    /// Глобальные свойства, описывающие игру
    /// </summary>
    public static class Global
    {
        /// <summary>
        /// Игрок
        /// </summary>
        public static Player Me;

        /// <summary>
        /// Мир
        /// </summary>
        public static World World;

        /// <summary>
        /// Игровые характеристики
        /// </summary>
        public static Game Game;

        /// <summary>
        /// Ход
        /// </summary>
        public static Move Move;

        /// <summary>
        /// Список юнитов в игре
        /// </summary>
        public static readonly Dictionary<long, VehicleWrapper> Units = new Dictionary<long, VehicleWrapper>();

		public static List<VehicleWrapper> MyUnits;

        /// <summary>
        /// Очередь приказов
        /// </summary>
        public static readonly ActionQueue ActionQueue = new ActionQueue();

		/// <summary>
		/// Текущая выделенная формация
		/// </summary>
		public static Formation SelectedFormation;

        /// <summary>
        /// Обновление глобальной информации игры
        /// </summary>
        /// <param name="me"></param>
        /// <param name="world"></param>
        /// <param name="game"></param>
        /// <param name="move"></param>
        public static void Update(Player me, World world, Game game, Move move)
        {
            Me = me;
            World = world;
            Game = game;
            Move = move;

            foreach (var newVehicle in world.NewVehicles)
            {
                Units.Add( newVehicle.Id, new VehicleWrapper(newVehicle) );
            }

            foreach (var vehicleUpdate in World.VehicleUpdates)
            {
                if (vehicleUpdate.Durability > 0)
                {
                    Units[vehicleUpdate.Id].Update(vehicleUpdate);
                }
                else
                {
                    Units.Remove(vehicleUpdate.Id);
                }
            }

			MyUnits = Units.Values.Where(i => i.PlayerId == Me.Id).ToList();

			Tactic.UpdateFormations();
        }
    }

    /// <summary>
    /// Расширение для класса-игрового действия
    /// </summary>
    public class Action : Move
    {
        /// <summary>
        /// Срочное действие, которое надо выполнить вне очереди
        /// </summary>
        public bool Urgent;

        /// <summary>
        /// Действие заказывает ход игры до которого не требуется запускать следующее действие
        /// </summary>
        public int WaitForWorldTick;

        /// <summary>
        /// Условие при котором действие должно быть выполнено
        /// </summary>
        public Func<bool> Condition;

        /// <summary>
        /// Колбек выполняемый по завершении действия
        /// </summary>
        public System.Action Callback;

        /// <summary>
        /// Группа, к которой относится действие. 
        /// </summary>
        public Formation Formation;

        /// <summary>
        /// Относительное время ожидания выполнения действия в тиках. Отличается по логике работы от WaitForWorldTick:
        /// Когда действие запускается на выполнение, значение WaitDuringTicks + World.TickIndex присваиваются в TickIndex
        /// группы, вызвавшей действие. До наступления хода = TickIndex другие приказы в очереди для данной группы находятся 
        /// в состоянии ожидания.
        /// </summary>
        public int WaitDuringTicks;

        /// <summary>
        /// Готовность действия к выполнению. Учитывает только выполнение условия.
        /// </summary>
        public bool Ready
        {
            get
            {
                return Condition == null || Condition();
            }
        }

		/// <summary>
		/// Если действие относительное, то Х и У для него не передаются приказом, а вычисляются в момент выполнения действия
		/// </summary>
		public bool Relative = false;
    }

    /// <summary>
    /// Обертка для юнита
    /// </summary>
    public class VehicleWrapper
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public long Id;

        /// <summary>
        /// Тип юнита
        /// </summary>
        public VehicleType Type;

        /// <summary>
        /// Х-координата
        /// </summary>
        public double X;

        /// <summary>
        /// Y-координата
        /// </summary>
        public double Y;

        /// <summary>
        /// Количество ХП
        /// </summary>
        public int Durability;

        /// <summary>
        /// Идентификатор игрока которому принадлежит юнит
        /// </summary>
        public long PlayerId;

        /// <summary>
        /// Ссылка на юнит
        /// </summary>
        public Vehicle Vehicle;

        /// <summary>
        /// Направление движения юнита (разница между текущей и предыдущей точкой местоположения)
        /// </summary>
        public Point Direction;

        /// <summary>
        /// 
        /// </summary>
        public Position Position = new Position(0, 0);

        /// <summary>
        /// Движется ли юнит. Юнит предположительно неподвижен, если дистанция на которую он передвинулся за ход
        /// меньше четверти от его скорости.
        /// </summary>
        public bool IsMoving { get { return Direction.Distance(Point.Zero) >= Vehicle.MaxSpeed / 4; } }

        /// <summary>
        /// Создание юнита
        /// </summary>
        /// <param name="vehicle"></param>
        public VehicleWrapper(Vehicle vehicle)
        {
            Id = vehicle.Id;
            Type = vehicle.Type;
            X = vehicle.X;
            Y = vehicle.Y;
            Durability = vehicle.Durability;
            PlayerId = vehicle.PlayerId;
            Vehicle = vehicle;
        }

        /// <summary>
        /// Обновление изменяемых данных юнита
        /// </summary>
        /// <param name="vehicleUpdate"></param>
        public void Update(VehicleUpdate vehicleUpdate)
        {
            Direction = new Point(vehicleUpdate.X, vehicleUpdate.Y) - new Point(X, Y);
            X = vehicleUpdate.X;
            Y = vehicleUpdate.Y;
            Position.X = X;
            Position.Y = Y;
            Durability = vehicleUpdate.Durability;
        }

        public double Distance(VehicleWrapper target)
        {
            return Position.Distance(target.Position);
        }

        public bool CanSee(VehicleWrapper target) // 2.4 Типы местности и погоды
        {
            return Position.Distance(target.Position) <= Vehicle.VisionRange * VisionFactor * target.StealthFactor;
        }

        public TerrainType TerrainType
        {
            get { return Global.World.TerrainByCellXY[(int) Math.Floor(X / 32)][(int) Math.Floor(Y / 32)]; }
        }

        public WeatherType WeatherType
        {
            get { return Global.World.WeatherByCellXY[(int) Math.Floor(X / 32)][(int) Math.Floor(Y / 32)]; }
        }

        public double VisionFactor
        {
            get
            {
                if (Vehicle.Type == VehicleType.Tank || Vehicle.Type == VehicleType.Ifv || Vehicle.Type == VehicleType.Arrv)
                {
                    switch (TerrainType)
                    {
                        case TerrainType.Forest: return Global.Game.ForestTerrainVisionFactor;
                        case TerrainType.Plain: return Global.Game.PlainTerrainVisionFactor;
                        case TerrainType.Swamp: return Global.Game.SwampTerrainVisionFactor;
                    }
                }
                else
                {
                    switch (WeatherType)
                    {
                        case WeatherType.Clear: return Global.Game.ClearWeatherVisionFactor;
                        case WeatherType.Cloud: return Global.Game.CloudWeatherVisionFactor;
                        case WeatherType.Rain: return Global.Game.RainWeatherVisionFactor;
                    }
                }

                return 1;
            }
        }

        public double StealthFactor
        {
            get
            {
                if (Vehicle.Type == VehicleType.Tank || Vehicle.Type == VehicleType.Ifv || Vehicle.Type == VehicleType.Arrv)
                {
                    switch (TerrainType)
                    {
                        case TerrainType.Forest: return Global.Game.ForestTerrainStealthFactor;
                        case TerrainType.Plain: return Global.Game.PlainTerrainStealthFactor;
                        case TerrainType.Swamp: return Global.Game.SwampTerrainStealthFactor;
                    }
                }
                else
                {
                    switch (WeatherType)
                    {
                        case WeatherType.Clear: return Global.Game.ClearWeatherStealthFactor;
                        case WeatherType.Cloud: return Global.Game.CloudWeatherStealthFactor;
                        case WeatherType.Rain: return Global.Game.RainWeatherStealthFactor;
                    }
                }

                return 1;
            }
        }
    }
}
