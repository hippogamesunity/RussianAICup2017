using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
	public partial class MyStrategy
	{
		static Formation rightFormation, leftFormation, middleFormation, mainAviaUnits, addAviaUnits;
		static bool mergedM2R = false, mergedL2R;

		public void CreateFormations()
		{
			#region Формируются эскадрильи

			mainAviaUnits = Tactic.Fighters;
			addAviaUnits = Tactic.Helicopters;
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

			addAviaUnits.TickIndex += 60;
			addAviaUnits.MoveTo(50, 205);
			addAviaUnits.Scale(1.7);
			addAviaUnits.MoveTo(200, 205);

			#endregion

			#region Формируется пехота

			rightFormation = Tactic.Tanks;
			if (Tactic.Ifvs.MassCenter.X > rightFormation.MassCenter.X)
				rightFormation = Tactic.Ifvs;
			if (Tactic.Arrvs.MassCenter.X > rightFormation.MassCenter.X)
				rightFormation = Tactic.Arrvs;

			leftFormation = rightFormation == Tactic.Tanks ? Tactic.Ifvs : Tactic.Tanks;
			if (Tactic.Ifvs.MassCenter.Y > leftFormation.MassCenter.Y && rightFormation != Tactic.Ifvs )
				leftFormation = Tactic.Ifvs;
			if (Tactic.Arrvs.MassCenter.Y > leftFormation.MassCenter.Y && rightFormation != Tactic.Arrvs)
				leftFormation = Tactic.Arrvs;

			middleFormation = ( rightFormation == Tactic.Tanks || leftFormation == Tactic.Tanks ) ?
				((rightFormation == Tactic.Ifvs || leftFormation == Tactic.Ifvs) ? Tactic.Arrvs : Tactic.Ifvs ) : Tactic.Tanks;

			rightFormation.Scale(0.8);
			rightFormation.MoveTo(250, rightFormation.MassCenter.Y);
			rightFormation.MoveTo(250, 250);
			rightFormation.Scale(2.5);

			leftFormation.Scale(0.8);
			leftFormation.MoveTo(leftFormation.MassCenter.X, 250);
			leftFormation.MoveTo(30, 240);

			middleFormation.TickIndex += 150;
			middleFormation.Scale(0.8);
			middleFormation.MoveTo(110, middleFormation.MassCenter.Y);
			middleFormation.MoveTo(110, 244);
			middleFormation.Scale(2.5);

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

		public void MergeFormations()
		{
			if (!mergedM2R && !rightFormation.Busy && !middleFormation.Busy)
			{
				middleFormation.MoveTo(250, 244);
				mergedM2R = true;
			} 
			else if (!mergedL2R && !rightFormation.Busy && !middleFormation.Busy && Global.World.TickIndex > 900 )
			{
				middleFormation.Scale(0.1);
				rightFormation.Scale(0.1);
				mergedM2R = true;
			}
		}
	}
}
