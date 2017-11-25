using System;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model {
    public partial class Player {
        private readonly long id;
        private readonly bool isMe;
        private readonly bool isStrategyCrashed;
        private readonly int score;
        private readonly int remainingActionCooldownTicks;
        private readonly int remainingNuclearStrikeCooldownTicks;
        private readonly long nextNuclearStrikeVehicleId;
        private readonly int nextNuclearStrikeTickIndex;
        private readonly double nextNuclearStrikeX;
        private readonly double nextNuclearStrikeY;

        public Player(long id, bool isMe, bool isStrategyCrashed, int score, int remainingActionCooldownTicks,
                int remainingNuclearStrikeCooldownTicks, long nextNuclearStrikeVehicleId,
                int nextNuclearStrikeTickIndex, double nextNuclearStrikeX, double nextNuclearStrikeY) {
            this.id = id;
            this.isMe = isMe;
            this.isStrategyCrashed = isStrategyCrashed;
            this.score = score;
            this.remainingActionCooldownTicks = remainingActionCooldownTicks;
            this.remainingNuclearStrikeCooldownTicks = remainingNuclearStrikeCooldownTicks;
            this.nextNuclearStrikeVehicleId = nextNuclearStrikeVehicleId;
            this.nextNuclearStrikeTickIndex = nextNuclearStrikeTickIndex;
            this.nextNuclearStrikeX = nextNuclearStrikeX;
            this.nextNuclearStrikeY = nextNuclearStrikeY;
        }

        public long Id{
            get { return id; }
        }
        public bool IsMe{
            get { return isMe; }
        }
        public bool IsStrategyCrashed{
            get { return isStrategyCrashed; }
        }
        public int Score{
            get { return score; }
        }
        public int RemainingActionCooldownTicks{
            get { return remainingActionCooldownTicks; }
        }
        public int RemainingNuclearStrikeCooldownTicks{
            get { return remainingNuclearStrikeCooldownTicks; }
        }
        public long NextNuclearStrikeVehicleId{
            get { return nextNuclearStrikeVehicleId; }
        }
        public int NextNuclearStrikeTickIndex{
            get { return nextNuclearStrikeTickIndex; }
        }
        public double NextNuclearStrikeX{
            get { return nextNuclearStrikeX; }
        }
        public double NextNuclearStrikeY
        {
            get { return nextNuclearStrikeY; }
        }
    }
}