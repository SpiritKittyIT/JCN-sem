using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Logic
{
    public class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Coordinate operator +(Coordinate a, Coordinate b)
        {
            return new Coordinate(a.X + b.X, a.Y + b.Y);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Coordinate other)
            {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public static readonly Coordinate Up = new(0, -1);
        public static readonly Coordinate Down = new(0, 1);
        public static readonly Coordinate Left = new(-1, 0);
        public static readonly Coordinate Right = new(1, 0);
    }

    public class CoordinateEventArgs : EventArgs
    {
        public Coordinate Coordinate { get; }
        public CoordinateEventArgs(Coordinate coordinate)
        {
            Coordinate = coordinate;
        }
    }
}
