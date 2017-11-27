using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
	public partial class MyStrategy
	{
		public void CreateFormations()
		{
			#region Формируются эскадрильи

			var mainAviaUnits = Tactic.Fighters;
			var addAviaUnits = Tactic.Helicopters;
			if (
				(mainAviaUnits.MassCenter.X == addAviaUnits.MassCenter.X && mainAviaUnits.MassCenter.Y < addAviaUnits.MassCenter.Y) ||
				(mainAviaUnits.MassCenter.Y == addAviaUnits.MassCenter.Y && mainAviaUnits.MassCenter.X < addAviaUnits.MassCenter.X) ||
				(mainAviaUnits.MassCenter.Y < addAviaUnits.MassCenter.Y && mainAviaUnits.MassCenter.X < addAviaUnits.MassCenter.X)
			)
			{
				mainAviaUnits = Tactic.Helicopters;
				addAviaUnits = Tactic.Fighters;
			}

			mainAviaUnits.MoveTo(200, 200);
			mainAviaUnits.Scale(1.7);

			addAviaUnits.MoveTo(50, 205);
			addAviaUnits.Scale(1.7);
			addAviaUnits.MoveTo(200, 205);

			#endregion

			#region Формируется пехота

			var rightFormation = Tactic.Tanks;
			if (Tactic.Ifvs.MassCenter.X > rightFormation.MassCenter.X)
				rightFormation = Tactic.Ifvs;
			if (Tactic.Arrvs.MassCenter.X > rightFormation.MassCenter.X)
				rightFormation = Tactic.Arrvs;

			var leftFormation = rightFormation == Tactic.Tanks ? Tactic.Ifvs : Tactic.Tanks;
			if (Tactic.Ifvs.MassCenter.Y > leftFormation.MassCenter.Y && rightFormation != Tactic.Ifvs )
				leftFormation = Tactic.Ifvs;
			if (Tactic.Arrvs.MassCenter.Y > leftFormation.MassCenter.Y && rightFormation != Tactic.Arrvs)
				leftFormation = Tactic.Arrvs;

			rightFormation.Scale(0.1);
			rightFormation.MoveTo(250, rightFormation.MassCenter.Y);
			rightFormation.MoveTo(250, 250);

			leftFormation.Scale(0.1);
			leftFormation.MoveTo(leftFormation.MassCenter.X, 250);
			leftFormation.MoveTo(50, 250);

			//Tactic.Tanks.Scale(5);
			//Tactic.Tanks.MoveTo(250, 250);
			//Tactic.Tanks.Scale(0.4);
			//Tactic.Ifvs.Scale(2.5);
			//Tactic.Ifvs.MoveTo(250, 250);
			//Tactic.Ifvs.Scale(0.4);
			//Tactic.Arrvs.Scale(2.5);
			//Tactic.Arrvs.MoveTo(250, 250);
			//Tactic.Arrvs.Scale(0.4);

			#endregion
		}
	}
}
