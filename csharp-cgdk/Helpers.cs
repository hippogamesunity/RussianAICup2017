using System;
using System.Dynamic;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    /// <summary>
    /// Vehicle wrapper
    /// </summary>
    public class VehicleWrapper
    {
        public long Id;
        public VehicleType Type;
        public double X;
        public double Y;
        public int Durability;
        public long PlayerId;
        public Vehicle Vehicle;

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

        public void Update(VehicleUpdate vehicleUpdate)
        {
            X = vehicleUpdate.X;
            Y = vehicleUpdate.Y;
            Durability = vehicleUpdate.Durability;
        }
    }

    public class Position
    {
        public double X;
        public double Y;

        public static Position Zero => new Position(0, 0);

        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double Distance(Position other)
        {
            var x = X - other.X;
            var y = Y - other.Y;

            return Math.Sqrt(x * x + y * y);
        }

        public static Position operator +(Position a, Position b)
        {
            return new Position(a.X + b.X, a.Y + b.Y);
        }

        public static Position operator -(Position a, Position b)
        {
            return new Position(a.X - b.X, a.Y - b.Y);
        }
    }
}
