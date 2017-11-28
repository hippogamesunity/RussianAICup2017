using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public class ActionQueue : List<Action>
    {
		public void Process()
		{
		    if (this.Any() && Global.Me.RemainingActionCooldownTicks == 0)
		    {
		        for (var i = 0; i < Count; i++)
                {
                    if (this[i].Urgent && this[i].Ready && i > 0 && this[0].Action == ActionType.ClearAndSelect)
                    {
                        Execute(this[i], Global.Move);
                        return;
                    }
                }

                Execute(this.FirstOrDefault(i => i.Ready), Global.Move);
		    }
        }

		private void Execute( Action action, Move move )
		{
            move.Action = action.Action;
            move.X = action.X;
            move.Y = action.Y;
            move.Angle = action.Angle;
            move.Right = action.Right;
            move.Bottom = action.Bottom;
            move.VehicleId = action.VehicleId;
            move.VehicleType = action.VehicleType;
            move.MaxSpeed = action.MaxSpeed;
            move.Factor = action.Factor;
            move.Group = action.Group;
            move.FacilityId = action.FacilityId;
            
            if (action.Callback != null) action.Callback();

			Remove(action);
		}
    }
}
