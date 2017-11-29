using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
	public partial class MyStrategy
	{
		static Formation rightFormation, leftFormation, middleFormation, mainAviaUnits, addAviaUnits;
		static Formation[] rightGroups, leftGroups, middleGroups;
		static int FirstFormation;

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

			mainAviaUnits.MoveTo(250, 250);
			mainAviaUnits.Scale(1.7);

			addAviaUnits.TickIndex += 240;
			addAviaUnits.MoveTo(50, 255);
			addAviaUnits.Scale(1.7);
			addAviaUnits.MoveTo(250, 255);

			#endregion

			#region Формируется пехота

			rightFormation = Tactic.Tanks;
			if ( Tactic.Ifvs.MassCenter.X > rightFormation.MassCenter.X ||
				( Tactic.Ifvs.MassCenter.X == rightFormation.MassCenter.X && Tactic.Ifvs.MassCenter.Y > rightFormation.MassCenter.Y))
				rightFormation = Tactic.Ifvs;
			if ( Tactic.Arrvs.MassCenter.X > rightFormation.MassCenter.X ||
				(Tactic.Arrvs.MassCenter.X == rightFormation.MassCenter.X  && Tactic.Arrvs.MassCenter.Y > rightFormation.MassCenter.Y))
				rightFormation = Tactic.Arrvs;

			leftFormation = rightFormation == Tactic.Tanks ? Tactic.Ifvs : Tactic.Tanks;
			if (Tactic.Ifvs.MassCenter.X < leftFormation.MassCenter.X ||
				(Tactic.Ifvs.MassCenter.X == leftFormation.MassCenter.X  && Tactic.Ifvs.MassCenter.Y < leftFormation.MassCenter.Y))
				leftFormation = Tactic.Ifvs;
			if (rightFormation != Tactic.Arrvs && ( Tactic.Arrvs.MassCenter.X < leftFormation.MassCenter.X || (
				Tactic.Arrvs.MassCenter.X == leftFormation.MassCenter.X  && Tactic.Arrvs.MassCenter.Y < leftFormation.MassCenter.Y)))
				leftFormation = Tactic.Arrvs;

			middleFormation = (rightFormation == Tactic.Tanks || leftFormation == Tactic.Tanks) ?
				((rightFormation == Tactic.Ifvs || leftFormation == Tactic.Ifvs) ? Tactic.Arrvs : Tactic.Ifvs) : Tactic.Tanks;

			if ( rightFormation == Tactic.Tanks )
				rightGroups = Tactic.TanksGroup;
			if (rightFormation == Tactic.Ifvs)
				rightGroups = Tactic.IfvsGroup;
			if (rightFormation == Tactic.Arrvs)
				rightGroups = Tactic.ArrvsGroup;

			if ( leftFormation == Tactic.Tanks)
				leftGroups = Tactic.TanksGroup;
			if (leftFormation == Tactic.Ifvs)
				leftGroups = Tactic.IfvsGroup;
			if (leftFormation == Tactic.Arrvs)
				leftGroups = Tactic.ArrvsGroup;

			if (middleFormation == Tactic.Tanks)
				middleGroups = Tactic.TanksGroup;
			if (middleFormation == Tactic.Ifvs)
				middleGroups = Tactic.IfvsGroup;
			if (middleFormation == Tactic.Arrvs)
				middleGroups = Tactic.ArrvsGroup;			

			rightFormation.MoveTo(200, 200);
			leftFormation.TickIndex = Global.World.TickIndex + 100;
			leftFormation.MoveTo(50, 50);
			middleFormation.TickIndex = Global.World.TickIndex + 400;
			middleFormation.MoveTo(middleFormation.MassCenter.X, 125);
			middleFormation.MoveTo(125, 125);

			#endregion
		}

		public void MergeFormations()
		{
			bool busy = false;
			for (int i = 0; i < 5; i++)
			{
				if (Tactic.TanksGroup[i].Busy || Tactic.IfvsGroup[i].Busy || Tactic.ArrvsGroup[i].Busy)
				{
					busy = true;
					break;
				}
			}
			if (!rightFormation.Busy && !leftFormation.Busy && !middleFormation.Busy)
			{
				if (FirstFormation == 0)
				{
					for (int i = 0; i < 5; i++)
					{
						Tactic.TanksGroup[i].MoveTo(Tactic.TanksGroup[i].MassCenter.X + 60 * i, Tactic.TanksGroup[i].MassCenter.Y);
						Tactic.TanksGroup[i].MoveTo(Tactic.TanksGroup[i].MassCenter.X + 60 * i, Tactic.TanksGroup[0].MassCenter.Y);
						Tactic.IfvsGroup[i].MoveTo(Tactic.IfvsGroup[i].MassCenter.X + 60 * i, Tactic.IfvsGroup[i].MassCenter.Y);
						Tactic.IfvsGroup[i].MoveTo(Tactic.IfvsGroup[i].MassCenter.X + 60 * i, Tactic.IfvsGroup[0].MassCenter.Y);
						Tactic.ArrvsGroup[i].MoveTo(Tactic.ArrvsGroup[i].MassCenter.X + 60 * i, Tactic.ArrvsGroup[i].MassCenter.Y);
						Tactic.ArrvsGroup[i].MoveTo(Tactic.ArrvsGroup[i].MassCenter.X + 60 * i, Tactic.ArrvsGroup[0].MassCenter.Y);
					}
					FirstFormation = Global.World.TickIndex + 60;
				}
				else if (!busy && Global.World.TickIndex > FirstFormation)
				{
					leftFormation.MoveTo(rightFormation.MassCenter.X, rightFormation.MassCenter.Y - 22);
					middleFormation.MoveTo(rightFormation.MassCenter.X, rightFormation.MassCenter.Y - 11 );
					mainAviaUnits.MoveTo(rightFormation.MassCenter.X, rightFormation.MassCenter.Y - 11);
					addAviaUnits.MoveTo(rightFormation.MassCenter.X, rightFormation.MassCenter.Y - 11);
					Tactic.FormationsReady = Global.World.TickIndex + 1000;
				}
			}
		}

		public void MergeFormations2()
		{
			bool busy = false;
			for (int i = 0; i < 5; i++)
			{
				if (Tactic.TanksGroup[i].Busy || Tactic.IfvsGroup[i].Busy || Tactic.ArrvsGroup[i].Busy)
				{
					busy = true;
					break;
				}
			}
			if (!rightFormation.Busy && !leftFormation.Busy && !middleFormation.Busy)
			{
				if (FirstFormation == 0)
				{
					for (int i = 0; i < 5; i++)
					{
						Tactic.TanksGroup[i].MoveTo(Tactic.TanksGroup[i].MassCenter.X, middleFormation.MassCenter.Y + 33 * i);
						Tactic.IfvsGroup[i].MoveTo(Tactic.IfvsGroup[i].MassCenter.X, middleFormation.MassCenter.Y + 11 + 33 * i);
						Tactic.ArrvsGroup[i].MoveTo(Tactic.ArrvsGroup[i].MassCenter.X, middleFormation.MassCenter.Y + 22 + 33 * i);
						//Tactic.IfvsGroup[i].MoveTo(Tactic.IfvsGroup[i].MassCenter.X + 60 * i, Tactic.IfvsGroup[i].MassCenter.Y);
						//Tactic.IfvsGroup[i].MoveTo(Tactic.IfvsGroup[i].MassCenter.X + 60 * i, Tactic.IfvsGroup[0].MassCenter.Y);
						//Tactic.ArrvsGroup[i].MoveTo(Tactic.ArrvsGroup[i].MassCenter.X + 60 * i, Tactic.ArrvsGroup[i].MassCenter.Y);
						//Tactic.ArrvsGroup[i].MoveTo(Tactic.ArrvsGroup[i].MassCenter.X + 60 * i, Tactic.ArrvsGroup[0].MassCenter.Y);
					}
					FirstFormation = Global.World.TickIndex + 60;
				}
				else if (!busy && Global.World.TickIndex > FirstFormation)
				{
					leftFormation.MoveTo(middleFormation.MassCenter.X, leftFormation.MassCenter.Y);
					rightFormation.MoveTo(middleFormation.MassCenter.X, rightFormation.MassCenter.Y);
					mainAviaUnits.MoveTo(middleFormation.MassCenter.X, middleFormation.MassCenter.Y);
					addAviaUnits.MoveTo(middleFormation.MassCenter.X, middleFormation.MassCenter.Y);
					Tactic.FormationsReady = Global.World.TickIndex + 400;
				}
			}
		}
	}
}


/*rightFormation = Tactic.Tanks;
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
leftFormation.MoveTo(30, 238);

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
//Tactic.Arrvs.Scale(0.4);*/