using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
    public sealed partial class MyStrategy
    {
        private Dictionary<VehicleType, Position> _positions;
        private Position _target = new Position(150, 150);

        private void SuperHurricane(World world, Player me)
        {
            Compress(world, me);

            if (world.TickIndex >= 540 && world.TickIndex % 60 == 0)
            {
                var second = (world.TickIndex - 540) / 60 % 14;

                switch (second)
                {
                    case 2:
                    case 12:
                    {
                        var enemy = Units.Values.Where(i => i.PlayerId != me.Id).ToList();

                        if (enemy.Count == 0) break;

                        var center = new Position(enemy.Average(i => i.X), enemy.Average(i => i.Y));
                        var target = enemy.Select(i => new Position(i.X, i.Y)).OrderBy(i => i.Distance(center)).First();
                        var direction = target - _target;

                        Move(direction.X, direction.Y, 0.3);

                        break;
                    }
                    case 3:
                    case 13: break;
                    case 4:
                    {
                        var myUnits = Units.Values.Where(i => i.PlayerId == me.Id).ToList();

                        _target = new Position(myUnits.Average(i => i.X), myUnits.Average(i => i.Y));
                        Rotate(Math.PI, _target.X, _target.Y);

                        break;
                    }
                    case 6: 
                    case 7: Scale(0.1, _target.X, _target.Y); break;
                    case 9: Scale(2, _target.X, _target.Y); break;
                    default: Rotate(Math.PI, _target.X, _target.Y); break;
                }
            }
        }

        private void Compress(World world, Player me)
        {
            if (world.TickIndex == 0)
            {
                var groups = new Dictionary<VehicleType, List<VehicleWrapper>>
                {
                    { VehicleType.Tank, new List<VehicleWrapper>() },
                    { VehicleType.Ifv, new List<VehicleWrapper>() },
                    { VehicleType.Arrv, new List<VehicleWrapper>() },
                    { VehicleType.Helicopter, new List<VehicleWrapper>() },
                    { VehicleType.Fighter, new List<VehicleWrapper>() }
                };

                foreach (var unit in Units.Values.Where(i => i.PlayerId == me.Id))
                {
                    groups[unit.Type].Add(unit);
                }

                _positions = new Dictionary<VehicleType, Position>
                {
                    { VehicleType.Tank, new Position(groups[VehicleType.Tank].Average(i => i.X), groups[VehicleType.Tank].Average(i => i.Y)) },
                    { VehicleType.Ifv, new Position(groups[VehicleType.Ifv].Average(i => i.X), groups[VehicleType.Ifv].Average(i => i.Y)) },
                    { VehicleType.Arrv, new Position(groups[VehicleType.Arrv].Average(i => i.X), groups[VehicleType.Arrv].Average(i => i.Y)) },
                    { VehicleType.Helicopter, new Position(groups[VehicleType.Helicopter].Average(i => i.X), groups[VehicleType.Helicopter].Average(i => i.Y)) },
                    { VehicleType.Fighter, new Position(groups[VehicleType.Fighter].Average(i => i.X), groups[VehicleType.Fighter].Average(i => i.Y)) }
                };

                _positions = _positions.OrderBy(i => i.Value.Distance(_target)).ToDictionary(i => i.Key, i => i.Value);
            }

            if (world.TickIndex <= 300)
            for (var i = 0; i < _positions.Count; i++)
            {
                if (world.TickIndex == 60 * i)
                {
                    var type = _positions.ElementAt(i).Key;
                    var position = _positions.ElementAt(i).Value;

                    SelectAll(type, world);
                    Move(_target.X - position.X, _target.Y - position.Y);
                }
            }

            if (world.TickIndex == 540)
            {
                SelectAll(world);
            }
        }
    }
}