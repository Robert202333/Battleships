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
            {
                descriptionToCoordinateMap.Add(this.descriptions[i], i);
            }
        }


        public string GetDescription(uint coordinate)
        {
            if (coordinate > descriptions.Length)
                throw new ArgumentOutOfRangeException();

            return descriptions[coordinate];
        }

        internal uint GetCoordinate(string description)
        {
            bool success = descriptionToCoordinateMap.TryGetValue(description, out var coordinateValue);
            if (success)
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
        public ShipComponent? ShipComponent { get; set; } = null;
        public bool WasHit { get; set; } = false;

        internal Square()
        {
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
            for (int y = 0; y < squares.GetLength(0); y++)
                for (int x = 0; x < squares.GetLength(1); x++)
                    squares[y, x] = new Square();
        }

        internal Coordinates ConvertToCoordinates(string xDescription, string yDescription)
        {
            return new Coordinates(HorizontalDescriptor.GetCoordinate(xDescription), 
                                   VerticalDescriptor.GetCoordinate(yDescription));
        }
        internal ShipComponent? ProcessShot(uint x, uint y)
        {
            if (y > squares.GetLength(0) || x > squares.GetLength(1))
                throw new ArgumentOutOfRangeException();

            var square = GetSquare(x, y);
            if (square.WasHit)
                throw new RepeatedHitException();

            square.WasHit = true;
            if (square.ShipComponent != null)
                square.ShipComponent.WasHit = true;

            return square.ShipComponent;
        }

        internal void ApplyShipComponents(IEnumerable<ShipComponent> components)
        {
            foreach(var component in components)
            {
                var coordinates = component.Coordinates;
                var square = GetSquare(coordinates.X, coordinates.Y);
                square.ShipComponent = component;
            }
        }

        public void VisitSquares(Action<Square, uint, uint> action, Predicate<Square>? predicate = null)
        {
            for (uint y = 0; y < squares.GetLength(0); y++)
                for (uint x = 0; x < squares.GetLength(1); x++)
                {
                    var square = GetSquare(x, y);
                    if (predicate == null || predicate(square))
                        action(square, x, y);
                }
        }
    }
}
