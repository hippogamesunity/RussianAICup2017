using System;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed partial class MyStrategy
    {
        private readonly int[] _intervals = { 920, -1, -1, -1, -1, -1 };

        private void Hurricane(World world, Player me)
        {
            Compress(world, me);

            if (world.TickIndex == _intervals[0])
            {
                var middle = Middle(me);

                SelectAll(VehicleType.Tank, world);
                Scale(GetRandom(1, 1.1) / 300f, middle);

                SelectAll(VehicleType.Ifv, world);
                Scale(GetRandom(1, 1.1), middle);

                SelectAll(VehicleType.Arrv, world);
                Scale(GetRandom(1, 1.1), middle);

                SelectAll(VehicleType.Helicopter, world);
                Scale(GetRandom(1, 1.1), middle);

                SelectAll(VehicleType.Fighter, world);
                Scale(GetRandom(1, 1.1), middle);

                _intervals[1] = WaitSeconds(world, GetRandom(0.5, 1));
            }

            if (world.TickIndex == _intervals[1])
            {
                Rotate(world, Middle(me));

                _intervals[2] = WaitSeconds(world, GetRandom(1, 2));
            }

            if (world.TickIndex == _intervals[2])
            {
                SelectAll(world);
                Scale(0.1, Middle(me));

                _intervals[3] = WaitSeconds(world, GetRandom(1, 1.5));
            }

            if (world.TickIndex == _intervals[3])
            {
                Rotate(world, Middle(me));

                _intervals[4] = WaitSeconds(world, GetRandom(1, 1.5));
            }

            if (world.TickIndex == _intervals[4])
            {
                var middle = Middle(me);

                SelectAll(VehicleType.Helicopter, world);
                Scale(GetRandom(1, 1.2), middle);
                SelectAll(VehicleType.Fighter, world);
                Scale(GetRandom(1, 1.2), middle);

                _intervals[5] = WaitSeconds(world, GetRandom(0.5, 1));
            }

            if (world.TickIndex == _intervals[5])
            {
                var enemy = Units.Values.Where(i => i.PlayerId != me.Id).ToList();

                if (enemy.Count == 0) return;

                var middle = Middle(me);
                var target = enemy.OrderBy(i => middle.Distance(i)).First();
                var direction = new Position(target.X, target.Y) - Middle(me);

                SelectAll(world);
                Move(direction.X, direction.Y, 0.3);

                _intervals[0] = WaitSeconds(world, GetRandom(2, 4));
            }
        }

        private void Rotate(World world, Position middle)
        {
            SelectAll(VehicleType.Tank, world);
            Rotate(Math.PI - GetRandom(0, 1), middle);

            SelectAll(VehicleType.Ifv, world);
            Rotate(Math.PI - GetRandom(0, 1), middle);

            SelectAll(VehicleType.Arrv, world);
            Rotate(Math.PI - GetRandom(0, 1), middle);

            SelectAll(VehicleType.Helicopter, world);
            Rotate(Math.PI - GetRandom(0, 1), middle);

            SelectAll(VehicleType.Fighter, world);
            Rotate(Math.PI - GetRandom(0, 1), middle);
        }

        private void Compress(World world, Player me)
        {
            if (world.TickIndex == 0)
            {
                SelectAll(world);
                Scale(0.75f, me);
            }

            foreach (var tick in new[] { 60, 120, 180, 240, 300, 360, 420, 480, 540, 600, 660, 720 })
            if (world.TickIndex == tick)
            {
                Rotate(Math.PI, Middle(me));
            }

            foreach (var tick in new[] { 780, 840 })
            if (world.TickIndex == tick)
            {
                Scale(0.25f, me);
            }
        }

		private void EvadeNuclearStrike(World world, Player me)
		{
			var enemy = world.GetOpponentPlayer();

			if (enemy.NextNuclearStrikeTickIndex - world.TickIndex == 29)
			{
				SelectAll(world);
				Scale(2, enemy.NextNuclearStrikeX, enemy.NextNuclearStrikeY);
				WaitTicks(world, enemy.NextNuclearStrikeTickIndex - world.TickIndex + 10);
				Scale(0.1, me);
				ActionQueue[ActionQueue.Count - 1].Urgent = true;
				ActionQueue[ActionQueue.Count - 2].Urgent = true;
				ActionQueue[ActionQueue.Count - 3].Urgent = true;
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
					var direction = new Position(affectedEnemies.Average(i => i.Direction.X), affectedEnemies.Average(i => i.Direction.Y));
					var prediction = new Position(30 * direction.X, 30 * direction.Y);

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

        
	}
}