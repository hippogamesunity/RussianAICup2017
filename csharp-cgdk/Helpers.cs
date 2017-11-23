using System;
using System.Security.Cryptography;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class Action : Move
    {
        public bool Urgent;
        public int WaitForTick;
        public Func<bool> Condition;
        public System.Action Callback;

        public bool Ready => Condition == null || Condition();
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

    public static class CRandom
    {
        private static readonly byte[] Buffer = new byte[1024];
        private static int _bufferOffset = Buffer.Length;
        private static readonly RNGCryptoServiceProvider CryptoProvider = new RNGCryptoServiceProvider();

        public static int GetRandom()
        {
            if (_bufferOffset >= Buffer.Length)
            {
                FillBuffer();
            }

            var val = BitConverter.ToInt32(Buffer, _bufferOffset) & 0x7fffffff;

            _bufferOffset += sizeof(int);

            return val;
        }

        public static int GetRandom(int maxValue)
        {
            return GetRandom() % maxValue;
        }

        public static int GetRandom(int minValue, int maxValue)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException();
            }

            var range = maxValue - minValue;

            return minValue + GetRandom(range);
        }

        /// <summary>
        /// Chance 0-100
        /// </summary>
        public static bool Chance(int chance)
        {
            return GetRandom(0, 101) < chance;
        }

        /// <summary>
        /// Chance 0-1f
        /// </summary>
        public static bool Chance(float chance)
        {
            return Chance((int)(100 * chance));
        }

        private static void FillBuffer()
        {
            CryptoProvider.GetBytes(Buffer);
            _bufferOffset = 0;
        }
    }
}
