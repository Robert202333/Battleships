namespace GameModel
{
    public class CoordinateDescriptor
    {
        private readonly string[] descriptions;
        private readonly Dictionary<string, uint> descriptionToCoordinateMap = new Dictionary<string, uint>();

        public uint Size
        {
            get { return (uint)descriptions.Length; }
        }

        internal CoordinateDescriptor(IEnumerable<string> descriptions)
        {
            this.descriptions = descriptions.ToArray();
            for (uint i = 0; i < this.descriptions.Length; i++)
                descriptionToCoordinateMap.Add(this.descriptions[i], i);
        }


        public string GetDescription(uint coordinate)
        {
            if (coordinate > descriptions.Length)
                throw new ArgumentOutOfRangeException();

            return descriptions[coordinate];
        }

        internal uint GetCoordinate(string description)
        {
            if (descriptionToCoordinateMap.TryGetValue(description, out var coordinateValue))
                return coordinateValue;

            throw new InvalidCoordinatesDescriptionException(CreateDescriptionOutOfRangeMessage(description));
        }


        private string CreateDescriptionOutOfRangeMessage(string passedDescription)
        {
            string validDescriptions = string.Join(",", descriptions);
            return $"Passed {passedDescription} is not a valid coordinate, please use one of the following: {validDescriptions}";
        }
    }

    public class Square
    {
        public ShipComponent? ShipComponent { get; internal set; } = null;
        public bool WasHit { get; internal set; } = false;
        public Coordinates Coordinates{ get; init; }

        internal Square(Coordinates coordinates)
        {
            Coordinates = coordinates;
        }
    }

    public class Board
    {
        private readonly Square[,] squares;
        public CoordinateDescriptor VerticalDescriptor { get; init; }
        public CoordinateDescriptor HorizontalDescriptor { get; init; }

        public Square GetSquare(uint x, uint y)
        {
            return squares[y, x];
        }

        internal Board(CoordinateDescriptor horizontalDescriptor, CoordinateDescriptor verticalDescriptor)
        {
            VerticalDescriptor = verticalDescriptor;
            HorizontalDescriptor = horizontalDescriptor;

            squares = new Square[VerticalDescriptor.Size, HorizontalDescriptor.Size];
            for (uint y = 0; y < squares.GetLength(0); y++)
                for (uint x = 0; x < squares.GetLength(1); x++)
                    squares[y, x] = new Square(new Coordinates(x, y));
        }

        internal Coordinates ConvertToCoordinates(string xDescription, string yDescription)
        {
            return new Coordinates(HorizontalDescriptor.GetCoordinate(xDescription), 
                                   VerticalDescriptor.GetCoordinate(yDescription));
        }
        public void VisitSquares(Action<Square, uint, uint> action, Predicate<Square>? predicate = null)
        {
            for (uint y = 0; y < squares.GetLength(0); y++)
            {
                for (uint x = 0; x < squares.GetLength(1); x++)
                {
                    var square = GetSquare(x, y);
                    if (predicate == null || predicate(square))
                        action(square, x, y);
                }
            }
        }
    }
}
