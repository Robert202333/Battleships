
// All classes are public to allow using them in custom GameCreator classes
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
        public uint X { get; init; }
        public uint Y { get; init; }

        public Coordinates(uint x, uint y)
        {
            X = x;
            Y = y;
        }

        public Coordinates GetUp()
        {
            return new Coordinates(X, Y - 1);
        }

        public Coordinates GetDown()
        {
            return new Coordinates(X, Y + 1);
        }

        public Coordinates GetRight()
        {
            return new Coordinates(X + 1, Y);
        }

        public Coordinates GetLeft()
        {
            return new Coordinates(X - 1, Y);
        }

        public Coordinates GetInDirection(Direction direction)
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

        public Direction? GetDirection(Coordinates coordinates)
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

    public class CoordinatesChain
    {
        public List<Coordinates> Chain { get; } = new List<Coordinates>();

        public CoordinatesChain()
        {

        }

        public void Add(Coordinates coordinates)
        {
            Chain.Add(coordinates);
        }

        public bool Includes(Coordinates coordinates)
        {
            return Chain.Any(coord => coord.Equals(coordinates));
        }

        public Coordinates Last
        {
            get
            {
                return Chain.LastOrDefault();
            }
        }

        public Direction? GetLastDirection()
        {
            if (Chain.Count < 2)
                return null;
            return Chain[Chain.Count - 2].GetDirection(Last);
        }
    }
}
