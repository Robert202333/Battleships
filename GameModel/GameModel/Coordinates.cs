
// All classes are public to allow using them in custom GameCreator classes
namespace GameModel
{   public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    };

    public readonly struct Coordinates
    {
        public int X { get; init; }
        public int Y { get; init; }

        public Coordinates(int x, int y)
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
            if (!IsNext(coordinates))
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

        public bool IsNext(Coordinates coordinates)
        {
            return Math.Abs(X - coordinates.X) + Math.Abs(Y - coordinates.Y) == 1;
        }

        public bool IsAdjacent(Coordinates coordinates)
        {
            return Math.Max(Math.Abs(X - coordinates.X), Math.Abs(Y - coordinates.Y)) == 1;
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
            if (Chain.Count > 0 && (!Last.IsNext(coordinates) || Includes(coordinates)))
                throw new ArgumentException();

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
                return Chain.Last();
            }
        }

        public Direction? GetLastDirection()
        {
            if (Chain.Count < 2)
                return null;
            return Chain[Chain.Count - 2].GetDirection(Last);
        }

        public bool IsStraight()
        {
            Direction? direction = GetLastDirection();
            if (direction == null)
                return true;

            // Chekcing from back
            for (int i = Chain.Count - 2; i > 0; i--)
                if (Chain[i - 1].GetDirection(Chain[i]) != direction)
                    return false;
            return true;
        }

    }
}
