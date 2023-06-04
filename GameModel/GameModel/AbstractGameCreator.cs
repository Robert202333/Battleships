namespace GameModel
{
    // Only way to create ships and 
    public abstract class AbstractGameCreator
    {
        public AbstractGameCreator() { }
        protected Ship CreateShip(string name, IEnumerable<Coordinates> coordinatesChain, Board board)
        {
            var ship = new Ship(name);
            var shipComponents = coordinatesChain.Select(coordinates =>
            {
                var square = board.GetSquare(coordinates.X, coordinates.Y);
                var shipComponent = new ShipComponent(ship, square);
                square.ShipComponent = shipComponent;
                return shipComponent;
            });
            ship.AddComponents(shipComponents);
            return ship;
        }

        private Board CreateBoard(Settings settings)
        {
            static string GenerateDescription(ushort coordinate, CoordinateDescriptionType coordinateDescriptionType)
            {
                return coordinateDescriptionType == CoordinateDescriptionType.Number ?
                    (coordinate + 1).ToString() :
                    ((char)('A' + coordinate)).ToString();
            }

            var horizontalDescriptions = Enumerable.Range(0, (int)settings.HorizontalSize).
                Select((number) => GenerateDescription((ushort)number, settings.HorizontalCoordinateDescriptionType));

            var verticallDescriptions = Enumerable.Range(0, (int)settings.VerticalSize).
                Select((number) => GenerateDescription((ushort)number, settings.VerticalCoordinateDescriptionType));

            return new Board(new CoordinateDescriptor(horizontalDescriptions), new CoordinateDescriptor(verticallDescriptions));
        }

        protected abstract IEnumerable<Ship> CreateShips(Board board, Settings settings);

        internal Game Execute(Settings settings)
        {
            var board = CreateBoard(settings);
            var ships = CreateShips(board, settings);
            return new Game(board, ships);
        }
    }
}
