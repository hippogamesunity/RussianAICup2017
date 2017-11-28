using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public static class Actions
    {
        public static void SelectAll(VehicleType type)
        {
            Global.ActionQueue.Add(new Action { Action = ActionType.ClearAndSelect, Right = Global.World.Width, Bottom = Global.World.Height, VehicleType = type });
        }

        public static void SelectAll()
        {
            Global.ActionQueue.Add(new Action { Action = ActionType.ClearAndSelect, Right = Global.World.Width, Bottom = Global.World.Height });
        }

        public static void Group(int group)
        {
            Global.ActionQueue.Add(new Action { Action = ActionType.Assign, Group = group });
        }

        public static void Scale(double scale, VehicleType type, Player me)
        {
            var units = Global.Units.Values.Where(i => i.Type == type && i.PlayerId == me.Id).ToList();

            if (units.Count == 0) return;

            var move = new Action { Action = ActionType.Scale, Factor = scale, X = units.Average(i => i.X), Y = units.Average(i => i.Y) };

            Global.ActionQueue.Add(move);
        }

        public static void Scale(double scale, double x, double y)
        {
            Global.ActionQueue.Add(new Action { Action = ActionType.Scale, Factor = scale, X = x, Y = y });
        }

        public static void Scale(double scale, Position position)
        {
            Global.ActionQueue.Add(new Action { Action = ActionType.Scale, Factor = scale, X = position.X, Y = position.Y });
        }

        public static void Scale(double scale, Player me)
        {
            var units = Global.Units.Values.Where(i => i.PlayerId == me.Id).ToList();
            var move = new Action { Action = ActionType.Scale, Factor = scale, X = units.Average(i => i.X), Y = units.Average(i => i.Y) };

            Global.ActionQueue.Add(move);
        }

        public static void Move(double x, double y, double maxSpeed = 0)
        {
            Global.ActionQueue.Add(new Action { Action = ActionType.Move, X = x, Y = y, MaxSpeed = maxSpeed });
        }

        public static void Move(Position offset, double maxSpeed = 0)
        {
            Global.ActionQueue.Add(new Action { Action = ActionType.Move, X = offset.X, Y = offset.Y, MaxSpeed = maxSpeed });
        }

        public static void Rotate(double angle, double x, double y)
        {
            Global.ActionQueue.Add(new Action { Action = ActionType.Rotate, Angle = angle, X = x, Y = y, MaxSpeed = 1, MaxAngularSpeed = 1 });
        }

        public static void Rotate(double angle, Position position)
        {
            Global.ActionQueue.Add(new Action { Action = ActionType.Rotate, Angle = angle, X = position.X, Y = position.Y, MaxSpeed = 1, MaxAngularSpeed = 1 });
        }

        public static void MoveAll(VehicleType type, List<VehicleType> targetTypes, Player me, bool self = false, double maxSpeed = 0)
        {
            var move = new Action();
            var myUnits = Global.Units.Values.Where(i => i.Type == type && i.PlayerId == me.Id).ToList();

            if (myUnits.Count == 0) return;

            foreach (var targetType in targetTypes)
            {
                var targets = Global.Units.Values.Where(i => i.Type == targetType && (self ? i.PlayerId == me.Id : i.PlayerId != me.Id)).ToList();

                if (targets.Any())
                {
                    var myCenter = new Position(myUnits.Average(i => i.X), myUnits.Average(i => i.Y));
                    var targetsCenter = new Position(targets.Average(i => i.X), targets.Average(i => i.Y));
                    var target = targets.OrderBy(i => targetsCenter.Distance(i)).First();

                    move.Action = ActionType.Move;
                    move.X = target.X - myCenter.X;
                    move.Y = target.Y - myCenter.Y;

                    if (maxSpeed > 0)
                    {
                        move.MaxSpeed = maxSpeed;
                    }

                    Global.ActionQueue.Add(move);

                    return;
                }
            }
        }
    }
}