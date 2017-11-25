using System;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model {
    public partial class VehicleUpdate {
        private readonly long id;
        private readonly double x;
        private readonly double y;
        private readonly int durability;
        private readonly int remainingAttackCooldownTicks;
        private readonly bool isSelected;
        private readonly int[] groups;

        public VehicleUpdate(long id, double x, double y, int durability, int remainingAttackCooldownTicks,
                bool isSelected, int[] groups) {
            this.id = id;
            this.x = x;
            this.y = y;
            this.durability = durability;
            this.remainingAttackCooldownTicks = remainingAttackCooldownTicks;
            this.isSelected = isSelected;

            this.groups = new int[groups.Length];
            Array.Copy(groups, this.groups, groups.Length);
        }

        public long Id{
            get{ return id;  }
        }
        public double X{
            get{ return x;  }
        }
        public double Y{
            get{ return y;  }
        }
        public int Durability{
            get{ return durability;  }
        }
        public int RemainingAttackCooldownTicks{
            get{ return remainingAttackCooldownTicks;  }
        }
        public bool IsSelected
        {
            get { return isSelected; }
        }

        public int[] Groups {
            get {
                if (this.groups == null) {
                    return null;
                }

                int[] groups = new int[this.groups.Length];
                Array.Copy(this.groups, groups, this.groups.Length);
                return groups;
            }
        }
    }
}