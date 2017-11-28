using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public static class Global
    {
        public static Player Me;
        public static World World;
        public static Game Game;
        public static Move Move;
        public static readonly Dictionary<long, VehicleWrapper> Units = new Dictionary<long, VehicleWrapper>();
        public static List<VehicleWrapper> MyUnits;
        public static List<VehicleWrapper> EnemyUnits;
        public static readonly ActionQueue ActionQueue = new ActionQueue();

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
            EnemyUnits = Units.Values.Where(i => i.PlayerId != Me.Id).ToList();
        }
    }

    public class Action : Move
    {
        public bool Urgent;
        public Func<bool> Condition;
        public System.Action Callback;

        public bool Ready
        {
            get
            {
                return Condition == null || Condition();
            }
        }

		public bool Relative = false;
    }

    public class VehicleWrapper
    {
        public long Id;
        public VehicleType Type;
        public double X;
        public double Y;
        public int Durability;
        public long PlayerId;
        public Vehicle Vehicle;
        public Position Direction;
        public Position Position;
        public TerrainType TerrainType;
        public WeatherType WeatherType;

        public bool IsMoving { get { return Direction.Distance(Position.Zero) >= Vehicle.MaxSpeed / 4; } }

        public VehicleWrapper(Vehicle vehicle)
        {
            Id = vehicle.Id;
            Type = vehicle.Type;
            X = vehicle.X;
            Y = vehicle.Y;
            Position = new Position(vehicle.X, vehicle.Y);
            Durability = vehicle.Durability;
            PlayerId = vehicle.PlayerId;
            Vehicle = vehicle;
        }

        public void Update(VehicleUpdate vehicleUpdate)
        {
            Direction = new Position(vehicleUpdate.X - X, vehicleUpdate.Y - Y);
            X = vehicleUpdate.X;
            Y = vehicleUpdate.Y;
            Position.X = X;
            Position.Y = Y;
            TerrainType = Global.World.TerrainByCellXY[(int) Math.Floor(X / 32)][(int) Math.Floor(Y / 32)];
            WeatherType = Global.World.WeatherByCellXY[(int) Math.Floor(X / 32)][(int) Math.Floor(Y / 32)];
            Durability = vehicleUpdate.Durability;
        }

        public double Distance(VehicleWrapper target)
        {
            return Position.Distance(target.Position);
        }

        public double Distance(Position position)
        {
            return Position.Distance(position);
        }

        public bool CanSee(VehicleWrapper target, double gap = 1) // 2.4 Типы местности и погоды
        {
            return Position.Distance(target.Position) <= gap * Vehicle.VisionRange * VisionFactor * target.StealthFactor;
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