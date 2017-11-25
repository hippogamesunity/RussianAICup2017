using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed class MyStrategy : IStrategy
    {
        public static readonly Dictionary<long, VehicleWrapper> Units = new Dictionary<long, VehicleWrapper>();
        public static readonly List<Action> ActionQueue = new List<Action>();
        public static World World;
        public static Formation SelectedFormation;

        Formation Fighters = new Formation(VehicleType.Fighter);
        Formation Helicopters = new Formation(VehicleType.Helicopter);
        Formation Arrvs = new Formation(VehicleType.Arrv);
        Formation Ifvs = new Formation(VehicleType.Ifv);
        Formation Tanks = new Formation(VehicleType.Tank);
        
        int FormationsCreatedTick = 750;

        public class Action : Move
        {
            public bool Urgent;
            public int WaitForTick;
            public Func<bool> Condition;
            public System.Action Callback;
            public Formation Formation;
            public int WaitTicks;

            public bool Ready
            {
                get
                {
                    return Condition == null || Condition();
                }
            }
        }

        public void Move(Player me, World world, Game game, Move move)
        {
            World = world;
            UpdateUnits(world);
            CreateFormation(world, me);
            AntiNuclearStrike(world, me);
            if (AntiNuke == null)
            {
                Hurricane(world, me);
            }
            NuclearStrike(world, me, game);
            ProcessQueue(world, me, move);
        }

        #region Service

        private void UpdateUnits(World world)
        {
            foreach (var newVehicle in world.NewVehicles)
            {
                Units.Add(newVehicle.Id, new VehicleWrapper(newVehicle));
            }

            foreach (var vehicleUpdate in world.VehicleUpdates)
            {
                if (vehicleUpdate.Durability > 0)
                {
                    Units[vehicleUpdate.Id].Update(vehicleUpdate);
                }
                else
                {
                    Units.Remove(vehicleUpdate.Id);
                }
            }

            Fighters.Update();
            Helicopters.Update();
            Arrvs.Update();
            Ifvs.Update();
            Tanks.Update();
        }

        private static void ProcessQueue(World world, Player me, Move move)
        {
            if (ActionQueue.Any() && me.RemainingActionCooldownTicks == 0)
            {
                var action = ActionQueue.FirstOrDefault(i => i.Urgent && i.Ready);
                if (action != null)
                {
                    Execute(action, move);
                    return;
                }

                if (world.TickIndex >= _wait)
                {
                    action = ActionQueue.FirstOrDefault(i => (i.Ready && (i.Formation == null || i.Formation.TickIndex <= world.TickIndex)));
                    if (action != null)
                    {
                        _wait = -1;

                        if (action.Formation != null && SelectedFormation != action.Formation)
                        {
                            SelectedFormation = action.Formation;
                            action = new Action { Action = ActionType.ClearAndSelect, Left = 0, Right = World.Width, Top = 0, Bottom = World.Height, VehicleType = action.Formation.Type };
                        }
                        Execute(action, move);
                        return;
                    }
                }
            }
        }

        private static int _wait = -1;

        private static void Execute(Action action, Move move)
        {            
            if (action.WaitForTick > 0)
            {
                _wait = action.WaitForTick;
            }
            else
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

                if (action.Formation != null)
                {
                    action.Formation.TickIndex += action.WaitTicks;
                    if (action.Action == ActionType.ClearAndSelect)
                    {
                        SelectedFormation = action.Formation;
                    }
                }
            }
            ActionQueue.Remove(action);
        }

        #endregion

        #region Strategies

        private void CreateFormation(World world, Player me)
        {
            if (world.TickIndex == 0)
            {
                #region Формируются эскадрильи

                var mainAviaUnits = Fighters;
                var addAviaUnits = Helicopters; 
                if (
                    (mainAviaUnits.Rectangle.Center.X == addAviaUnits.Rectangle.Center.X && mainAviaUnits.Rectangle.Center.Y < addAviaUnits.Rectangle.Center.Y) ||
                    (mainAviaUnits.Rectangle.Center.Y == addAviaUnits.Rectangle.Center.Y && mainAviaUnits.Rectangle.Center.X < addAviaUnits.Rectangle.Center.X) ||
                    (mainAviaUnits.Rectangle.Center.Y < addAviaUnits.Rectangle.Center.Y && mainAviaUnits.Rectangle.Center.X < addAviaUnits.Rectangle.Center.X)
                )
                {
                    mainAviaUnits = Helicopters;
                    addAviaUnits = Fighters;
                }

                mainAviaUnits.MoveTo(200, 200);
                mainAviaUnits.Scale(1.7);

                addAviaUnits.MoveTo(50, 205);
                addAviaUnits.Scale(1.7);
                addAviaUnits.MoveTo(200, 205);

                #endregion

                #region Формируется пехота

                Tanks.Scale(5);
                Tanks.MoveTo(250, 250);
                Tanks.Scale(0.4);
                Ifvs.Scale(2.5);
                Ifvs.MoveTo(250, 250);
                Ifvs.Scale(0.4);
                Arrvs.Scale(2.5);
                Arrvs.MoveTo(250, 250);
                Arrvs.Scale(0.4);

                #endregion
            }
        }

        private void Hurricane(World world, Player me)
        {
            if (world.TickIndex == FormationsCreatedTick)
            {
                SelectAll(world);
            }

            Point hurricaneCenter = GetHurricaneCenter(me, world);
            bool formationMoved = false;
            if (GetDistance(hurricaneCenter, LastHurricaneCenter) > 40)
                formationMoved = true;
            if (formationMoveFreeze > 0)
                formationMoveFreeze--;
            LastHurricaneCenter = hurricaneCenter;

            if (world.TickIndex > FormationsCreatedTick && world.TickIndex % 180 == 0)
            {
                var units = Units.Values.Where(j => j.PlayerId == me.Id).ToList();
                Rotate(Math.PI, hurricaneCenter.X, hurricaneCenter.Y);
            }

            if (world.TickIndex > FormationsCreatedTick && world.TickIndex % 360 == 100)
            {
                Scale(0.2, hurricaneCenter.X, hurricaneCenter.Y);
            }

            if (world.TickIndex > FormationsCreatedTick + 300 && world.TickIndex < 10000)
            {
                if (!formationMoved && formationMoveFreeze == 0)
                {
                    var unit = GetNearestEnemy(hurricaneCenter, me);
                    if (unit != null)
                    {
                        var unitDistance = GetDistance(hurricaneCenter, unit);
                        if (unitDistance < 40)
                        {
                            CompressFormation(hurricaneCenter);
                        }
                        else
                        {
                            formationMoveFreeze = 100;
                            DecompressFormation(hurricaneCenter);
                            Move(unit.X - hurricaneCenter.X, unit.Y - hurricaneCenter.Y, 0.35);
                        }
                    }
                }
            }

            if (world.TickIndex > 10000)
            {
                Rush(true);
            }
        }

		private void Rush(bool expand)
		{
			if (_rushStartedTick == -1) _rushStartedTick = World.TickIndex;

			var tick = World.TickIndex - _rushStartedTick;

			if (tick == 0 && expand)
			{
				foreach (VehicleType vehicleType in Enum.GetValues(typeof(VehicleType)))
				{
					SelectAll(vehicleType, World);
					Scale(2, vehicleType, World.GetMyPlayer() );
				}
			}

			if (tick > 60 && World.TickIndex % 50 == 0) // 8 actions = 40 tics
			{
				if (_iteration % 9 > 6)
				{
					foreach (VehicleType vehicleType in Enum.GetValues(typeof(VehicleType)))
					{
						if (vehicleType == VehicleType.Arrv) continue;

						SelectAll(vehicleType, World);
						Scale(0.5, vehicleType, World.GetMyPlayer());
					}
				}
				else
				{
					SelectAll(VehicleType.Tank, World);
					MoveAll(VehicleType.Tank, new List<VehicleType> { VehicleType.Tank, VehicleType.Ifv, VehicleType.Arrv, VehicleType.Fighter, VehicleType.Helicopter }, World.GetMyPlayer() );

					SelectAll(VehicleType.Ifv, World);
					MoveAll(VehicleType.Ifv, new List<VehicleType> { VehicleType.Helicopter, VehicleType.Fighter, VehicleType.Arrv, VehicleType.Ifv, VehicleType.Tank }, World.GetMyPlayer() );

					SelectAll(VehicleType.Helicopter, World);
					MoveAll(VehicleType.Helicopter, new List<VehicleType> { VehicleType.Tank, VehicleType.Arrv, VehicleType.Helicopter, VehicleType.Ifv, VehicleType.Fighter }, World.GetMyPlayer() );

					SelectAll(VehicleType.Fighter, World);

					if (Units.Values.Any(i => i.PlayerId != World.GetMyPlayer().Id && (i.Type == VehicleType.Helicopter || i.Type == VehicleType.Fighter)))
					{
						MoveAll(VehicleType.Fighter, new List<VehicleType> { VehicleType.Helicopter, VehicleType.Fighter }, World.GetMyPlayer());
					}
					else
					{
						MoveAll(VehicleType.Fighter, new List<VehicleType> { VehicleType.Tank, VehicleType.Ifv, VehicleType.Arrv }, World.GetMyPlayer(), self: true);
					}

					var arrvs = Units.Values.Where(i => i.PlayerId == World.GetMyPlayer().Id && i.Type == VehicleType.Arrv).ToList();

					if (arrvs.Any())
					{
						var pos = new Point(arrvs.Average(i => i.X), arrvs.Average(i => i.Y));

						SelectAll(VehicleType.Arrv, World);
						Move(0.2 * World.Width - pos.X, 0.8 * World.Height - pos.Y);
					}
				}

				_iteration++;
			}
		}


		private void NuclearStrike(World world, Player me, Game game)
		{
			if (me.NextNuclearStrikeTickIndex == -1 && world.TickIndex % 60 == 0)
			{
				var myUnits = Units.Values.Where(i => i.PlayerId == me.Id).ToList();
				var enemyUnits = Units.Values.Where(i => i.PlayerId != me.Id).ToList();
				var evaluations = new Dictionary<VehicleWrapper, double>();

				foreach (var target in enemyUnits)
				{
					var visors = myUnits.Where(i => GetDistance(i, target) < 0.85 * i.Vehicle.VisionRange).ToList();

					if (visors.Count == 0) continue;

					var affected = Units.Values.Where(i => GetDistance(i, target) < game.TacticalNuclearStrikeRadius).ToList();

					if (affected.Count <= 50) continue;

					var damageToMe = affected.Where(i => i.PlayerId == me.Id)
						.Sum(i => Math.Min(i.Durability, game.MaxTacticalNuclearStrikeDamage * (1 - GetDistance(i, target) / game.TacticalNuclearStrikeRadius)));
					var damageToEnemy = affected.Where(i => i.PlayerId != me.Id)
						.Sum(i => Math.Min(i.Durability, game.MaxTacticalNuclearStrikeDamage * (1 - GetDistance(i, target) / game.TacticalNuclearStrikeRadius)));
					var efficiency = damageToEnemy - damageToMe;

					if (efficiency > 0)
					{
						evaluations.Add(target, efficiency);
					}
				}

				if (evaluations.Count == 0) return;

				var best = evaluations.OrderBy(i => i.Value).Last();
				var threshold = Math.Min(50 * 100, 0.20 * enemyUnits.Sum(i => i.Durability));

				if (best.Value > threshold)
				{
					var target = best.Key;
					var visor = myUnits.Where(i => GetDistance(i, target) < 0.85 * i.Vehicle.VisionRange).OrderBy(i => i.X + i.Y).First();
					var affectedEnemies = Units.Values.Where(i => i.PlayerId != me.Id && GetDistance(i, target) < game.TacticalNuclearStrikeRadius).ToList();
					var direction = new Point(affectedEnemies.Average(i => i.Direction.X), affectedEnemies.Average(i => i.Direction.Y));
					var prediction = new Point(30 * direction.X, 30 * direction.Y);

					ActionQueue.Add(new Action
					{
						Action = ActionType.TacticalNuclearStrike,
						X = target.X + prediction.X,
						Y = target.Y + prediction.Y,
						VehicleId = visor.Id,
						Urgent = true
					});
				}
			}
		}

        private void AntiNuclearStrike(World world, Player me)
        {
            Player opponent = world.GetOpponentPlayer();
            if (opponent.NextNuclearStrikeVehicleId != -1 && AntiNuke == null)
            {
                Scale(10, opponent.NextNuclearStrikeX, opponent.NextNuclearStrikeY);
                AntiNuke = new AntiNukeInfo();
                AntiNuke.NukePoint = new Point(opponent.NextNuclearStrikeX, opponent.NextNuclearStrikeY);
                AntiNuke.Tick = opponent.NextNuclearStrikeTickIndex;
            }
            else if (AntiNuke != null)
            {
                if (AntiNuke.Tick < world.TickIndex - 50)
                {
                    AntiNuke = null;
                }
                else if ( AntiNuke.Tick + 20 == world.TickIndex )
                {
                    Scale(0.2, AntiNuke.NukePoint.X, AntiNuke.NukePoint.Y);
                }
            }
        }

        #endregion

        #region Helpers

        private void MoveFormationTo(Rect unitsRectangle, int X, int Y)
        {
            int x = X - (int)unitsRectangle.Center.X;
            int y = Y - (int)unitsRectangle.Center.Y;
            Move(x, y);
        }

        public static int WaitTicks(int ticks)
        {
            var action = new Action { WaitForTick = World.TickIndex + ticks };

            ActionQueue.Add(action);

            return action.WaitForTick;
        }

        private static int WaitSeconds(World world, double seconds)
        {
            var ticks = (int)(seconds * 60);

            return WaitTicks(ticks);
        }

        private static void Select(VehicleType type, Rect rect, bool addToSelection)
        {
            ActionQueue.Add(new Action { Action = ActionType.AddToSelection, Left = rect.LeftTop.X, Right = rect.RightBottom.X, Top = rect.LeftTop.Y, Bottom = rect.RightBottom.Y, VehicleType = type });
        }

        private static void SelectAll(VehicleType type, World world)
        {
            ActionQueue.Add(new Action { Action = ActionType.ClearAndSelect, Right = world.Width, Bottom = world.Height, VehicleType = type });
        }

        private static void SelectAll(World world)
        {
            ActionQueue.Add(new Action { Action = ActionType.ClearAndSelect, Right = world.Width, Bottom = world.Height });
        }

        private static void Group(int group)
        {
            ActionQueue.Add(new Action { Action = ActionType.Assign, Group = group });
        }

        public static void Scale(double scale, VehicleType type, Player me)
        {
            var units = Units.Values.Where(i => i.Type == type && i.PlayerId == me.Id).ToList();
            var move = new Action { Action = ActionType.Scale, Factor = scale, X = units.Average(i => i.X), Y = units.Average(i => i.Y) };

            ActionQueue.Add(move);
        }

        public static void Scale(double scale, double x, double y)
        {
            ActionQueue.Add(new Action { Action = ActionType.Scale, Factor = scale, X = x, Y = y });
        }

        public static void Scale( double scale, double x, double y, Formation formation, int waitTicks )
        {
            ActionQueue.Add(new Action { Action = ActionType.Scale, Factor = scale, X = x, Y = y, Formation = formation, WaitTicks = waitTicks });
        }

        public static void Scale(double scale, Player me)
        {
            var units = Units.Values.Where(i => i.PlayerId == me.Id).ToList();
            var move = new Action { Action = ActionType.Scale, Factor = scale, X = units.Average(i => i.X), Y = units.Average(i => i.Y) };

            ActionQueue.Add(move);
        }

        public static void Move(double x, double y, double maxSpeed = 0)
        {
            ActionQueue.Add(new Action { Action = ActionType.Move, X = x, Y = y, MaxSpeed = maxSpeed });
        }

        public static void Move(double x, double y, Formation formation, int waitTicks, double maxSpeed = 0)
        {
            ActionQueue.Add(new Action { Action = ActionType.Move, X = x, Y = y, Formation = formation, WaitTicks = waitTicks, MaxSpeed = maxSpeed });
        }

        private static void Rotate(double angle, double x, double y)
        {
            ActionQueue.Add(new Action { Action = ActionType.Rotate, Angle = angle, X = x, Y = y, MaxSpeed = 1, MaxAngularSpeed = 1 });
        }

        private void MoveAll(VehicleType type, List<VehicleType> targetTypes, Player me, bool self = false, float maxSpeed = 0)
        {
            var move = new Action();
            var myUnits = Units.Values.Where(i => i.Type == type && i.PlayerId == me.Id).ToList();

            if (myUnits.Count == 0) return;

            foreach (var targetType in targetTypes)
            {
                var targets = Units.Values.Where(i => i.Type == targetType && (self ? i.PlayerId == me.Id : i.PlayerId != me.Id)).ToList();

                if (targets.Any())
                {
                    move.Action = ActionType.Move;
                    move.X = targets.Average(i => i.X) - myUnits.Average(i => i.X);
                    move.Y = targets.Average(i => i.Y) - myUnits.Average(i => i.Y);

                    if (maxSpeed > 0)
                    {
                        move.MaxSpeed = maxSpeed;
                    }

                    ActionQueue.Add(move);

                    return;
                }
            }
        }

        private double GetDistance(VehicleWrapper from, VehicleWrapper to)
        {
            var x = to.X - from.X;
            var y = to.Y - from.Y;

            return Math.Sqrt(x * x + y * y);
        }

        private double GetDistance(Point from, VehicleWrapper to)
        {
            var x = to.X - from.X;
            var y = to.Y - from.Y;

            return Math.Sqrt(x * x + y * y);
        }

        public static double GetDistance(Point from, Point to)
        {
            var x = to.X - from.X;
            var y = to.Y - from.Y;

            return Math.Sqrt(x * x + y * y);
        }

        public static Rect GetRectangle(List<VehicleWrapper> vehicles)
        {
            Rect result = new Rect();
            foreach (var vehicle in vehicles)
            {
                if (vehicle.X < result.LeftTop.X)
                    result.LeftTop.X = vehicle.X;
                if (vehicle.Y < result.LeftTop.Y)
                    result.LeftTop.Y = vehicle.Y;
                if (vehicle.X > result.RightBottom.X)
                    result.RightBottom.X = vehicle.X;
                if (vehicle.Y > result.RightBottom.Y)
                    result.RightBottom.Y = vehicle.Y;
            }
            return result;
        }

        #endregion

        #region Hurricane Strategy Helpers

        int formationCompression = 0;

        int formationMoveFreeze = 0;

        public Point LastHurricaneCenter = new Point();

        Point GetHurricaneCenter(Player me, World world)
        {
            var units = Units.Values.Where(j => j.PlayerId == me.Id).ToList();
            return new Point(units.Average(j => j.X), units.Average(j => j.Y));
        }

        void CompressFormation(Point center)
        {
            if (formationCompression > -1)
            {
                formationCompression--;
                Scale(0.2, center.X, center.Y);
            }
        }

        void DecompressFormation(Point center)
        {
            if (formationCompression < 1)
            {
                formationCompression++;
                Scale(2, center.X, center.Y);
            }
        }

        VehicleWrapper GetNearestEnemy(Point hurricanecenter, Player me)
        {
            VehicleWrapper result = null;
            double d = Double.MaxValue;
            var units = Units.Values.Where(i => i.PlayerId != me.Id).ToList();
            if (units.Count > 0)
            {
                foreach (var unit in units)
                {
                    var nd = GetDistance(hurricanecenter, unit);
                    if (nd < d)
                    {
                        d = nd;
                        result = unit;
                    }
                }
            }
            return result;
        }
        #endregion

        #region AntiNuke Strategy Helpers

        AntiNukeInfo AntiNuke = null;

        public class AntiNukeInfo
        {
            public Point NukePoint;

            public int Tick;
        }

        #endregion

		#region Rush Helpers

		private int _rushStartedTick = -1;
		private int _iteration;

		#endregion
	}

    public class Formation
    {
        public List<VehicleWrapper> Units;

        public Rect Rectangle;

        public VehicleType Type;

        public int TickIndex = 0;

        public Formation( VehicleType type )
        {
            Type = type;            
        }

        public void Update()
        {
            Units = MyStrategy.Units.Values.Where(j => j.Type == Type && j.PlayerId == MyStrategy.World.GetMyPlayer().Id).ToList();
            Rectangle = MyStrategy.GetRectangle(Units);
        }

        public void MoveTo( int X, int Y )
        {
            double dist = MyStrategy.GetDistance(Rectangle.Center, new Point(X, Y));
            int x = X - (int)Rectangle.Center.X;
            int y = Y - (int)Rectangle.Center.Y;
            MyStrategy.Move(x, y, this, (int)(dist * 1.5));

            Rectangle.LeftTop.X += x;
            Rectangle.LeftTop.Y += y;
            Rectangle.RightBottom.X += x;
            Rectangle.RightBottom.Y += y;
        }

        public void Scale(double value)
        {
            double dist = MyStrategy.GetDistance(Rectangle.LeftTop, Rectangle.Center);
            MyStrategy.Scale( value, Rectangle.Center.X, Rectangle.Center.Y, this, (int)(dist * 1.5 * value) );
        }
    }

    public class Rect
    {
        public Point LeftTop = new Point(int.MaxValue, int.MaxValue);

        public Point RightBottom = new Point(0, 0);

        public Point Center
        {
            get { return new Point((RightBottom.X + LeftTop.X) / 2, (RightBottom.Y + LeftTop.Y) / 2); }
        }
    }

    public class Point
    {
        public Point() { }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X;
        public double Y;

		public static Point operator -(Point a, Point b)
		{
			return new Point(a.X - b.X, a.Y - b.Y);
		}
    }

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
        public Vehicle Vehicle;
        public int[] Groups;
		public Point Direction;

        public VehicleWrapper(Vehicle vehicle)
        {
            Id = vehicle.Id;
            Type = vehicle.Type;
            X = vehicle.X;
            Y = vehicle.Y;
            Durability = vehicle.Durability;
            PlayerId = vehicle.PlayerId;
            Vehicle = vehicle;
            Groups = vehicle.Groups;
        }

        public void Update(VehicleUpdate vehicleUpdate)
        {
			Direction = new Point(vehicleUpdate.X, vehicleUpdate.Y) - new Point(X, Y);
            X = vehicleUpdate.X;
            Y = vehicleUpdate.Y;
            Durability = vehicleUpdate.Durability;
            Groups = vehicleUpdate.Groups;
        }
    }
}