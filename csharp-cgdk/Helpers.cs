using System;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
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

        public double Distance(VehicleWrapper unit)
        {
            var x = X - unit.X;
            var y = Y - unit.Y;

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