using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModel
{   public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    };

    public struct Coordinates
    {
        public uint X { get; set; }
        public uint Y { get; set; }

        internal Coordinates(uint x, uint y)
        {
            X = x;
            Y = y;
        }

        internal Coordinates GetUp()
        {
            return new Coordinates(X, Y - 1);
        }

        internal Coordinates GetDown()
        {
            return new Coordinates(X, Y + 1);
        }

        internal Coordinates GetRight()
        {
            return new Coordinates(X + 1, Y);
        }

        internal Coordinates GetLeft()
        {
            return new Coordinates(X - 1, Y);
        }

        internal Coordinates GetInDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Up => GetUp(),
                Direction.Right => GetRight(),
                Direction.Down => GetDown(),
                Direction.Left => GetLeft(),
                _ => throw new NotImplementedException()
            };
        }

        internal Direction? GetDirection(Coordinates coordinates)
        {
            if (Math.Abs((int)X - coordinates.X) + Math.Abs((int)Y - coordinates.Y) != 1)
                return null;
            if (X < coordinates.X)
                return Direction.Right;
            else if (Y < coordinates.Y)
                return Direction.Down;
            if (X > coordinates.X)
                return Direction.Left;
            else // (Y > coordinates.Y)
                return Direction.Up;
        }

    }
}
