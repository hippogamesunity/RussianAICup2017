using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public partial class MyStrategy
    {
        private void NuclearStrike()
        {
            if (Global.Me.NextNuclearStrikeTickIndex == -1 && Global.World.TickIndex % 60 == 0)
            {
                var myUnits = Global.Units.Values.Where(i => i.PlayerId == Global.Me.Id).ToList();
                var enemyUnits = Global.Units.Values.Where(i => i.PlayerId != Global.Me.Id).ToList();
                var evaluations = new Dictionary<VehicleWrapper, double>();

                foreach (var target in enemyUnits)
                {
                    var visors = myUnits.Where(i => i.CanSee(target)).ToList();

                    if (visors.Count == 0) continue;

                    var affected = Global.Units.Values.Where(i => i.Distance(target) < Global.Game.TacticalNuclearStrikeRadius).ToList();

                    if (affected.Count <= 50) continue;

                    var damageToMe = affected.Where(i => i.PlayerId == Global.Me.Id)
                        .Sum(i => Math.Min(i.Durability, Global.Game.MaxTacticalNuclearStrikeDamage * (1 - i.Distance(target) / Global.Game.TacticalNuclearStrikeRadius)));
                    var damageToEnemy = affected.Where(i => i.PlayerId != Global.Me.Id)
                        .Sum(i => Math.Min(i.Durability, Global.Game.MaxTacticalNuclearStrikeDamage * (1 - i.Distance(target) / Global.Game.TacticalNuclearStrikeRadius)));
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
                    var visor = myUnits.Where(i => i.CanSee(target)).OrderBy(i => i.Distance(target)).Last();

                    var affectedEnemies = Global.Units.Values.Where(i => i.PlayerId != Global.Me.Id && i.Distance(target) < Global.Game.TacticalNuclearStrikeRadius).ToList();
                    var direction = new Position(affectedEnemies.Average(i => i.Direction.X), affectedEnemies.Average(i => i.Direction.Y));
                    var prediction = new Position(30 * direction.X, 30 * direction.Y);

                    Global.ActionQueue.Add(new Action
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