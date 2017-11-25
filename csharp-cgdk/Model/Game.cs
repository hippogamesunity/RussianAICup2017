using System;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model {
    public partial class Game {
        private readonly long randomSeed;
        private readonly int tickCount;
        private readonly double worldWidth;
        private readonly double worldHeight;
        private readonly bool isFogOfWarEnabled;
        private readonly int victoryScore;
        private readonly int facilityCaptureScore;
        private readonly int vehicleEliminationScore;
        private readonly int actionDetectionInterval;
        private readonly int baseActionCount;
        private readonly int additionalActionCountPerControlCenter;
        private readonly int maxUnitGroup;
        private readonly int terrainWeatherMapColumnCount;
        private readonly int terrainWeatherMapRowCount;
        private readonly double plainTerrainVisionFactor;
        private readonly double plainTerrainStealthFactor;
        private readonly double plainTerrainSpeedFactor;
        private readonly double swampTerrainVisionFactor;
        private readonly double swampTerrainStealthFactor;
        private readonly double swampTerrainSpeedFactor;
        private readonly double forestTerrainVisionFactor;
        private readonly double forestTerrainStealthFactor;
        private readonly double forestTerrainSpeedFactor;
        private readonly double clearWeatherVisionFactor;
        private readonly double clearWeatherStealthFactor;
        private readonly double clearWeatherSpeedFactor;
        private readonly double cloudWeatherVisionFactor;
        private readonly double cloudWeatherStealthFactor;
        private readonly double cloudWeatherSpeedFactor;
        private readonly double rainWeatherVisionFactor;
        private readonly double rainWeatherStealthFactor;
        private readonly double rainWeatherSpeedFactor;
        private readonly double vehicleRadius;
        private readonly int tankDurability;
        private readonly double tankSpeed;
        private readonly double tankVisionRange;
        private readonly double tankGroundAttackRange;
        private readonly double tankAerialAttackRange;
        private readonly int tankGroundDamage;
        private readonly int tankAerialDamage;
        private readonly int tankGroundDefence;
        private readonly int tankAerialDefence;
        private readonly int tankAttackCooldownTicks;
        private readonly int tankProductionCost;
        private readonly int ifvDurability;
        private readonly double ifvSpeed;
        private readonly double ifvVisionRange;
        private readonly double ifvGroundAttackRange;
        private readonly double ifvAerialAttackRange;
        private readonly int ifvGroundDamage;
        private readonly int ifvAerialDamage;
        private readonly int ifvGroundDefence;
        private readonly int ifvAerialDefence;
        private readonly int ifvAttackCooldownTicks;
        private readonly int ifvProductionCost;
        private readonly int arrvDurability;
        private readonly double arrvSpeed;
        private readonly double arrvVisionRange;
        private readonly int arrvGroundDefence;
        private readonly int arrvAerialDefence;
        private readonly int arrvProductionCost;
        private readonly double arrvRepairRange;
        private readonly double arrvRepairSpeed;
        private readonly int helicopterDurability;
        private readonly double helicopterSpeed;
        private readonly double helicopterVisionRange;
        private readonly double helicopterGroundAttackRange;
        private readonly double helicopterAerialAttackRange;
        private readonly int helicopterGroundDamage;
        private readonly int helicopterAerialDamage;
        private readonly int helicopterGroundDefence;
        private readonly int helicopterAerialDefence;
        private readonly int helicopterAttackCooldownTicks;
        private readonly int helicopterProductionCost;
        private readonly int fighterDurability;
        private readonly double fighterSpeed;
        private readonly double fighterVisionRange;
        private readonly double fighterGroundAttackRange;
        private readonly double fighterAerialAttackRange;
        private readonly int fighterGroundDamage;
        private readonly int fighterAerialDamage;
        private readonly int fighterGroundDefence;
        private readonly int fighterAerialDefence;
        private readonly int fighterAttackCooldownTicks;
        private readonly int fighterProductionCost;
        private readonly double maxFacilityCapturePoints;
        private readonly double facilityCapturePointsPerVehiclePerTick;
        private readonly double facilityWidth;
        private readonly double facilityHeight;
        private readonly int baseTacticalNuclearStrikeCooldown;
        private readonly int tacticalNuclearStrikeCooldownDecreasePerControlCenter;
        private readonly double maxTacticalNuclearStrikeDamage;
        private readonly double tacticalNuclearStrikeRadius;
        private readonly int tacticalNuclearStrikeDelay;

        public Game(long randomSeed, int tickCount, double worldWidth, double worldHeight, bool isFogOfWarEnabled,
                int victoryScore, int facilityCaptureScore, int vehicleEliminationScore, int actionDetectionInterval,
                int baseActionCount, int additionalActionCountPerControlCenter, int maxUnitGroup,
                int terrainWeatherMapColumnCount, int terrainWeatherMapRowCount, double plainTerrainVisionFactor,
                double plainTerrainStealthFactor, double plainTerrainSpeedFactor, double swampTerrainVisionFactor,
                double swampTerrainStealthFactor, double swampTerrainSpeedFactor, double forestTerrainVisionFactor,
                double forestTerrainStealthFactor, double forestTerrainSpeedFactor, double clearWeatherVisionFactor,
                double clearWeatherStealthFactor, double clearWeatherSpeedFactor, double cloudWeatherVisionFactor,
                double cloudWeatherStealthFactor, double cloudWeatherSpeedFactor, double rainWeatherVisionFactor,
                double rainWeatherStealthFactor, double rainWeatherSpeedFactor, double vehicleRadius,
                int tankDurability, double tankSpeed, double tankVisionRange, double tankGroundAttackRange,
                double tankAerialAttackRange, int tankGroundDamage, int tankAerialDamage, int tankGroundDefence,
                int tankAerialDefence, int tankAttackCooldownTicks, int tankProductionCost, int ifvDurability,
                double ifvSpeed, double ifvVisionRange, double ifvGroundAttackRange, double ifvAerialAttackRange,
                int ifvGroundDamage, int ifvAerialDamage, int ifvGroundDefence, int ifvAerialDefence,
                int ifvAttackCooldownTicks, int ifvProductionCost, int arrvDurability, double arrvSpeed,
                double arrvVisionRange, int arrvGroundDefence, int arrvAerialDefence, int arrvProductionCost,
                double arrvRepairRange, double arrvRepairSpeed, int helicopterDurability, double helicopterSpeed,
                double helicopterVisionRange, double helicopterGroundAttackRange, double helicopterAerialAttackRange,
                int helicopterGroundDamage, int helicopterAerialDamage, int helicopterGroundDefence,
                int helicopterAerialDefence, int helicopterAttackCooldownTicks, int helicopterProductionCost,
                int fighterDurability, double fighterSpeed, double fighterVisionRange, double fighterGroundAttackRange,
                double fighterAerialAttackRange, int fighterGroundDamage, int fighterAerialDamage,
                int fighterGroundDefence, int fighterAerialDefence, int fighterAttackCooldownTicks,
                int fighterProductionCost, double maxFacilityCapturePoints,
                double facilityCapturePointsPerVehiclePerTick, double facilityWidth, double facilityHeight,
                int baseTacticalNuclearStrikeCooldown, int tacticalNuclearStrikeCooldownDecreasePerControlCenter,
                double maxTacticalNuclearStrikeDamage, double tacticalNuclearStrikeRadius,
                int tacticalNuclearStrikeDelay) {
            this.randomSeed = randomSeed;
            this.tickCount = tickCount;
            this.worldWidth = worldWidth;
            this.worldHeight = worldHeight;
            this.isFogOfWarEnabled = isFogOfWarEnabled;
            this.victoryScore = victoryScore;
            this.facilityCaptureScore = facilityCaptureScore;
            this.vehicleEliminationScore = vehicleEliminationScore;
            this.actionDetectionInterval = actionDetectionInterval;
            this.baseActionCount = baseActionCount;
            this.additionalActionCountPerControlCenter = additionalActionCountPerControlCenter;
            this.maxUnitGroup = maxUnitGroup;
            this.terrainWeatherMapColumnCount = terrainWeatherMapColumnCount;
            this.terrainWeatherMapRowCount = terrainWeatherMapRowCount;
            this.plainTerrainVisionFactor = plainTerrainVisionFactor;
            this.plainTerrainStealthFactor = plainTerrainStealthFactor;
            this.plainTerrainSpeedFactor = plainTerrainSpeedFactor;
            this.swampTerrainVisionFactor = swampTerrainVisionFactor;
            this.swampTerrainStealthFactor = swampTerrainStealthFactor;
            this.swampTerrainSpeedFactor = swampTerrainSpeedFactor;
            this.forestTerrainVisionFactor = forestTerrainVisionFactor;
            this.forestTerrainStealthFactor = forestTerrainStealthFactor;
            this.forestTerrainSpeedFactor = forestTerrainSpeedFactor;
            this.clearWeatherVisionFactor = clearWeatherVisionFactor;
            this.clearWeatherStealthFactor = clearWeatherStealthFactor;
            this.clearWeatherSpeedFactor = clearWeatherSpeedFactor;
            this.cloudWeatherVisionFactor = cloudWeatherVisionFactor;
            this.cloudWeatherStealthFactor = cloudWeatherStealthFactor;
            this.cloudWeatherSpeedFactor = cloudWeatherSpeedFactor;
            this.rainWeatherVisionFactor = rainWeatherVisionFactor;
            this.rainWeatherStealthFactor = rainWeatherStealthFactor;
            this.rainWeatherSpeedFactor = rainWeatherSpeedFactor;
            this.vehicleRadius = vehicleRadius;
            this.tankDurability = tankDurability;
            this.tankSpeed = tankSpeed;
            this.tankVisionRange = tankVisionRange;
            this.tankGroundAttackRange = tankGroundAttackRange;
            this.tankAerialAttackRange = tankAerialAttackRange;
            this.tankGroundDamage = tankGroundDamage;
            this.tankAerialDamage = tankAerialDamage;
            this.tankGroundDefence = tankGroundDefence;
            this.tankAerialDefence = tankAerialDefence;
            this.tankAttackCooldownTicks = tankAttackCooldownTicks;
            this.tankProductionCost = tankProductionCost;
            this.ifvDurability = ifvDurability;
            this.ifvSpeed = ifvSpeed;
            this.ifvVisionRange = ifvVisionRange;
            this.ifvGroundAttackRange = ifvGroundAttackRange;
            this.ifvAerialAttackRange = ifvAerialAttackRange;
            this.ifvGroundDamage = ifvGroundDamage;
            this.ifvAerialDamage = ifvAerialDamage;
            this.ifvGroundDefence = ifvGroundDefence;
            this.ifvAerialDefence = ifvAerialDefence;
            this.ifvAttackCooldownTicks = ifvAttackCooldownTicks;
            this.ifvProductionCost = ifvProductionCost;
            this.arrvDurability = arrvDurability;
            this.arrvSpeed = arrvSpeed;
            this.arrvVisionRange = arrvVisionRange;
            this.arrvGroundDefence = arrvGroundDefence;
            this.arrvAerialDefence = arrvAerialDefence;
            this.arrvProductionCost = arrvProductionCost;
            this.arrvRepairRange = arrvRepairRange;
            this.arrvRepairSpeed = arrvRepairSpeed;
            this.helicopterDurability = helicopterDurability;
            this.helicopterSpeed = helicopterSpeed;
            this.helicopterVisionRange = helicopterVisionRange;
            this.helicopterGroundAttackRange = helicopterGroundAttackRange;
            this.helicopterAerialAttackRange = helicopterAerialAttackRange;
            this.helicopterGroundDamage = helicopterGroundDamage;
            this.helicopterAerialDamage = helicopterAerialDamage;
            this.helicopterGroundDefence = helicopterGroundDefence;
            this.helicopterAerialDefence = helicopterAerialDefence;
            this.helicopterAttackCooldownTicks = helicopterAttackCooldownTicks;
            this.helicopterProductionCost = helicopterProductionCost;
            this.fighterDurability = fighterDurability;
            this.fighterSpeed = fighterSpeed;
            this.fighterVisionRange = fighterVisionRange;
            this.fighterGroundAttackRange = fighterGroundAttackRange;
            this.fighterAerialAttackRange = fighterAerialAttackRange;
            this.fighterGroundDamage = fighterGroundDamage;
            this.fighterAerialDamage = fighterAerialDamage;
            this.fighterGroundDefence = fighterGroundDefence;
            this.fighterAerialDefence = fighterAerialDefence;
            this.fighterAttackCooldownTicks = fighterAttackCooldownTicks;
            this.fighterProductionCost = fighterProductionCost;
            this.maxFacilityCapturePoints = maxFacilityCapturePoints;
            this.facilityCapturePointsPerVehiclePerTick = facilityCapturePointsPerVehiclePerTick;
            this.facilityWidth = facilityWidth;
            this.facilityHeight = facilityHeight;
            this.baseTacticalNuclearStrikeCooldown = baseTacticalNuclearStrikeCooldown;
            this.tacticalNuclearStrikeCooldownDecreasePerControlCenter = tacticalNuclearStrikeCooldownDecreasePerControlCenter;
            this.maxTacticalNuclearStrikeDamage = maxTacticalNuclearStrikeDamage;
            this.tacticalNuclearStrikeRadius = tacticalNuclearStrikeRadius;
            this.tacticalNuclearStrikeDelay = tacticalNuclearStrikeDelay;
        }

        public long RandomSeed { get { return randomSeed; } }
        public int TickCount { get { return tickCount; } }
        public double WorldWidth { get { return worldWidth; } }
        public double WorldHeight { get { return worldHeight; } }
        public bool IsFogOfWarEnabled { get { return isFogOfWarEnabled; } }
        public int VictoryScore { get { return victoryScore; } }
        public int FacilityCaptureScore { get { return facilityCaptureScore; } }
        public int VehicleEliminationScore { get { return vehicleEliminationScore; } }
        public int ActionDetectionInterval { get { return actionDetectionInterval; } }
        public int BaseActionCount { get { return baseActionCount; } }
        public int AdditionalActionCountPerControlCenter { get { return additionalActionCountPerControlCenter; } }
        public int MaxUnitGroup { get { return maxUnitGroup; } }
        public int TerrainWeatherMapColumnCount { get { return terrainWeatherMapColumnCount; } }
        public int TerrainWeatherMapRowCount { get { return terrainWeatherMapRowCount; } }
        public double PlainTerrainVisionFactor { get { return plainTerrainVisionFactor; } }
        public double PlainTerrainStealthFactor { get { return plainTerrainStealthFactor; } }
        public double PlainTerrainSpeedFactor { get { return plainTerrainSpeedFactor; } }
        public double SwampTerrainVisionFactor { get { return swampTerrainVisionFactor; } }
        public double SwampTerrainStealthFactor { get { return swampTerrainStealthFactor; } }
        public double SwampTerrainSpeedFactor { get { return swampTerrainSpeedFactor; } }
        public double ForestTerrainVisionFactor { get { return forestTerrainVisionFactor; } }
        public double ForestTerrainStealthFactor { get { return forestTerrainStealthFactor; } }
        public double ForestTerrainSpeedFactor { get { return forestTerrainSpeedFactor; } }
        public double ClearWeatherVisionFactor { get { return clearWeatherVisionFactor; } }
        public double ClearWeatherStealthFactor { get { return clearWeatherStealthFactor; } }
        public double ClearWeatherSpeedFactor { get { return clearWeatherSpeedFactor; } }
        public double CloudWeatherVisionFactor { get { return cloudWeatherVisionFactor; } }
        public double CloudWeatherStealthFactor { get { return cloudWeatherStealthFactor; } }
        public double CloudWeatherSpeedFactor { get { return cloudWeatherSpeedFactor; } }
        public double RainWeatherVisionFactor { get { return rainWeatherVisionFactor; } }
        public double RainWeatherStealthFactor { get { return rainWeatherStealthFactor; } }
        public double RainWeatherSpeedFactor { get { return rainWeatherSpeedFactor; } }
        public double VehicleRadius { get { return vehicleRadius; } }
        public int TankDurability { get { return tankDurability; } }
        public double TankSpeed { get { return tankSpeed; } }
        public double TankVisionRange { get { return tankVisionRange; } }
        public double TankGroundAttackRange { get { return tankGroundAttackRange; } }
        public double TankAerialAttackRange { get { return tankAerialAttackRange; } }
        public int TankGroundDamage { get { return tankGroundDamage; } }
        public int TankAerialDamage { get { return tankAerialDamage; } }
        public int TankGroundDefence { get { return tankGroundDefence; } }
        public int TankAerialDefence { get { return tankAerialDefence; } }
        public int TankAttackCooldownTicks { get { return tankAttackCooldownTicks; } }
        public int TankProductionCost { get { return tankProductionCost; } }
        public int IfvDurability { get { return ifvDurability; } }
        public double IfvSpeed { get { return ifvSpeed; } }
        public double IfvVisionRange { get { return ifvVisionRange; } }
        public double IfvGroundAttackRange { get { return ifvGroundAttackRange; } }
        public double IfvAerialAttackRange { get { return ifvAerialAttackRange; } }
        public int IfvGroundDamage { get { return ifvGroundDamage; } }
        public int IfvAerialDamage { get { return ifvAerialDamage; } }
        public int IfvGroundDefence { get { return ifvGroundDefence; } }
        public int IfvAerialDefence { get { return ifvAerialDefence; } }
        public int IfvAttackCooldownTicks { get { return ifvAttackCooldownTicks; } }
        public int IfvProductionCost { get { return ifvProductionCost; } }
        public int ArrvDurability { get { return arrvDurability; } }
        public double ArrvSpeed { get { return arrvSpeed; } }
        public double ArrvVisionRange { get { return arrvVisionRange; } }
        public int ArrvGroundDefence { get { return arrvGroundDefence; } }
        public int ArrvAerialDefence { get { return arrvAerialDefence; } }
        public int ArrvProductionCost { get { return arrvProductionCost; } }
        public double ArrvRepairRange { get { return arrvRepairRange; } }
        public double ArrvRepairSpeed { get { return arrvRepairSpeed; } }
        public int HelicopterDurability { get { return helicopterDurability; } }
        public double HelicopterSpeed { get { return helicopterSpeed; } }
        public double HelicopterVisionRange { get { return helicopterVisionRange; } }
        public double HelicopterGroundAttackRange { get { return helicopterGroundAttackRange; } }
        public double HelicopterAerialAttackRange { get { return helicopterAerialAttackRange; } }
        public int HelicopterGroundDamage { get { return helicopterGroundDamage; } }
        public int HelicopterAerialDamage { get { return helicopterAerialDamage; } }
        public int HelicopterGroundDefence { get { return helicopterGroundDefence; } }
        public int HelicopterAerialDefence { get { return helicopterAerialDefence; } }
        public int HelicopterAttackCooldownTicks { get { return helicopterAttackCooldownTicks; } }
        public int HelicopterProductionCost { get { return helicopterProductionCost; } }
        public int FighterDurability { get { return fighterDurability; } }
        public double FighterSpeed { get { return fighterSpeed; } }
        public double FighterVisionRange { get { return fighterVisionRange; } }
        public double FighterGroundAttackRange { get { return fighterGroundAttackRange; } }
        public double FighterAerialAttackRange { get { return fighterAerialAttackRange; } }
        public int FighterGroundDamage { get { return fighterGroundDamage; } }
        public int FighterAerialDamage { get { return fighterAerialDamage; } }
        public int FighterGroundDefence { get { return fighterGroundDefence; } }
        public int FighterAerialDefence { get { return fighterAerialDefence; } }
        public int FighterAttackCooldownTicks { get { return fighterAttackCooldownTicks; } }
        public int FighterProductionCost { get { return fighterProductionCost; } }
        public double MaxFacilityCapturePoints { get { return maxFacilityCapturePoints; } }
        public double FacilityCapturePointsPerVehiclePerTick { get { return facilityCapturePointsPerVehiclePerTick; } }
        public double FacilityWidth { get { return facilityWidth; } }
        public double FacilityHeight { get { return facilityHeight; } }
        public int BaseTacticalNuclearStrikeCooldown { get { return baseTacticalNuclearStrikeCooldown; } }
        public int TacticalNuclearStrikeCooldownDecreasePerControlCenter { get { return tacticalNuclearStrikeCooldownDecreasePerControlCenter; } }
        public double MaxTacticalNuclearStrikeDamage { get { return maxTacticalNuclearStrikeDamage; } }
        public double TacticalNuclearStrikeRadius { get { return tacticalNuclearStrikeRadius; } }
        public int TacticalNuclearStrikeDelay { get { return tacticalNuclearStrikeDelay; } }
    }
}