using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.MyClasses
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

        public VehicleWrapper(Vehicle vehicle)
        {
            Id = vehicle.Id;
            Type = vehicle.Type;
            X = vehicle.X;
            Y = vehicle.Y;
            Durability = vehicle.Durability;
            PlayerId = vehicle.PlayerId;
        }

        public void Update(VehicleUpdate vehicleUpdate)
        {
            X = vehicleUpdate.X;
            Y = vehicleUpdate.Y;
            Durability = vehicleUpdate.Durability;
        }
    }
}