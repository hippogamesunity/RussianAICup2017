using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    /// <summary>
    /// Точка в двумерном пространстве
    /// </summary>
    public class Point
    {
        /// <summary>
        /// Х-координата точки
        /// </summary>
        public double X;

        /// <summary>
        /// У-координата точки
        /// </summary>
        public double Y;

        /// <summary>
        /// Нулевая точка
        /// </summary>
        public static Point Zero { get { return new Point(0, 0); } }

        /// <summary>
        /// Точка пространства
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

		/// <summary>
		/// Дистанция от точки до другой заданной точки
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public double Distance( double x, double y )
		{
			var _x = X - x;
			var _y = Y - y;

			return Math.Sqrt(_x * _x + _y * _y);
		}

        /// <summary>
        /// Дистанция от точки до другой заданной точки
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double Distance(Point other)
        {
			return Distance( other.X, other.Y );
        }

        /// <summary>
        /// Дистанция от точки до юнита
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public double Distance(VehicleWrapper unit)
        {
            return Distance(unit.X, unit.Y);
        }

        /// <summary>
        /// Сложение координат двух точек 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        /// <summary>
        /// Вычитание координат точек
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }
    }

    /// <summary>
    /// Прямоугольник, задающийся двумя точками (левого верхнего и правого нижнего угла)
    /// </summary>
    public class Rect
    {
        /// <summary>
        /// Левый верхний угол прямоугольника (для незаданного содержит некорректное значение int.MaxValue)
        /// </summary>
        public Point LeftTop = new Point(int.MaxValue, int.MaxValue);

        /// <summary>
        /// Правый нижний угол прямоугольника (для незаданного содержит некорректное значение 0)
        /// </summary>
        public Point RightBottom = new Point(0, 0);

        /// <summary>
        /// Центр прямоугольника
        /// </summary>
        public Point Center
        {
            get { return new Point((RightBottom.X + LeftTop.X) / 2, (RightBottom.Y + LeftTop.Y) / 2); }
        }

		public Rect()
		{
		}

		/// <summary>
		/// Создает габаритный контейнер для заданных юнитов
		/// </summary>
		/// <param name="vehicles"></param>
		public Rect(List<VehicleWrapper> vehicles)
		{
			foreach (var vehicle in vehicles)
			{
				if (vehicle.X < LeftTop.X)
					LeftTop.X = vehicle.X;
				if (vehicle.Y < LeftTop.Y)
					LeftTop.Y = vehicle.Y;
				if (vehicle.X > RightBottom.X)
					RightBottom.X = vehicle.X;
				if (vehicle.Y > RightBottom.Y)
					RightBottom.Y = vehicle.Y;
			}
		}
    }

    /// <summary>
    /// Рандомизатор
    /// </summary>
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
