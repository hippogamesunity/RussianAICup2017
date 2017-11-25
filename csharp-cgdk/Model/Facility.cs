namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model {
    public partial class Facility {
        private readonly long id;
        private readonly FacilityType type;
        private readonly long ownerPlayerId;
        private readonly double left;
        private readonly double top;
        private readonly double capturePoints;
        private readonly VehicleType? vehicleType;
        private readonly int productionProgress;

        public Facility(long id, FacilityType type, long ownerPlayerId, double left, double top, double capturePoints,
                VehicleType? vehicleType, int productionProgress) {
            this.id = id;
            this.type = type;
            this.ownerPlayerId = ownerPlayerId;
            this.left = left;
            this.top = top;
            this.capturePoints = capturePoints;
            this.vehicleType = vehicleType;
            this.productionProgress = productionProgress;
        }

        public long Id{
            get { return id; }
        }
        public FacilityType Type{
            get { return type; }
        }
        public long OwnerPlayerId{
            get { return ownerPlayerId; }
        }
        public double Left{
            get { return left; }
        }
        public double Top{
            get { return top; }
        }
        public double CapturePoints{
            get { return capturePoints; }
        }
        public VehicleType? VehicleType{
            get { return vehicleType; }
        }
        public int ProductionProgress
        {
            get { return productionProgress; }
        }
    }
}