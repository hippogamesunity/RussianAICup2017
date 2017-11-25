using System;
using System.Security.Cryptography;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using System.Collections.Generic;
using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
	public class Helpers
	{
		
		private static Point Middle(Player player)
		{
			var units = Global.Units.Values.Where(i => i.PlayerId == player.Id).ToList();

			return new Point(units.Average(i => i.X), units.Average(i => i.Y));
		}

		private double GetDistance(VehicleWrapper from, VehicleWrapper to)
		{
			var x = to.X - from.X;
			var y = to.Y - from.Y;

			return Math.Sqrt(x * x + y * y);
		}

		private double GetRandom(double min, double max)
		{
			return CRandom.GetRandom((int)(min * 1000), (int)(max * 1000)) / 1000d;
		}
	}
}
