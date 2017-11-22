using System.Collections.Generic;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed partial class MyStrategy
    {
        private void Rush(World world, Player me)
        {
            switch (world.TickIndex % 120)
            {
                case 0:
                    SelectAll(VehicleType.Tank, world);
                    if (world.TickIndex < 60)
                    {
                        Scale(0.1, VehicleType.Tank, me);
                    }
                    else
                    {
                        MoveAll(VehicleType.Tank, new List<VehicleType> { VehicleType.Ifv, VehicleType.Arrv, VehicleType.Tank, VehicleType.Fighter, VehicleType.Helicopter }, me);
                    }
                    break;
                case 10:
                    SelectAll(VehicleType.Ifv, world);
                    if (world.TickIndex < 60)
                    {
                        Scale(0.1, VehicleType.Ifv, me);
                    }
                    else
                    {
                        MoveAll(VehicleType.Ifv, new List<VehicleType> { VehicleType.Helicopter, VehicleType.Fighter, VehicleType.Arrv, VehicleType.Ifv, VehicleType.Tank }, me);
                    }
                    break;
                case 20:
                    SelectAll(VehicleType.Arrv, world);
                    if (world.TickIndex < 60)
                    {
                        Scale(0.1, VehicleType.Arrv, me);
                    }
                    else
                    {
                        MoveAll(VehicleType.Arrv, new List<VehicleType> { VehicleType.Ifv, VehicleType.Arrv, VehicleType.Tank, VehicleType.Fighter, VehicleType.Helicopter }, me);
                    }
                    break;
                case 30:
                case 90:
                    SelectAll(VehicleType.Helicopter, world);
                    MoveAll(VehicleType.Helicopter, new List<VehicleType> { VehicleType.Tank, VehicleType.Arrv, VehicleType.Helicopter, VehicleType.Ifv, VehicleType.Fighter }, me);
                    break;
                case 40:
                case 100:
                    SelectAll(VehicleType.Fighter, world);
                    MoveAll(VehicleType.Fighter, new List<VehicleType> { VehicleType.Helicopter, VehicleType.Fighter }, me);
                    break;
            }
        }
    }
}