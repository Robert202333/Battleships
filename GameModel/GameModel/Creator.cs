using System.Diagnostics;


namespace GameModel
{
    internal class CoordinatesChain
    {
        public List<Coordinates> Chain { get; } = new List<Coordinates>();

        internal CoordinatesChain()
        {

        }

        internal void Add(Coordinates coordinates)
        {
            Chain.Add(coordinates);
        }

        internal bool Occupies(Coordinates coordinates)
        {
            return Chain.Any(coord => coord.Equals(coordinates));
        }

        internal Coordinates Last
        {
            get
            {
                return Chain.LastOrDefault();
            }
        }

        internal Direction? GetLastDirection()
        {
            if (Chain.Count < 2)
                return null;
            return Chain[Chain.Count - 2].GetDirection(Last);
        }
    }

    static internal class ComponentCreationStrategies
    {
        private static Random random = new Random();

        private static Direction GetRandomDirection()
        {
            return (Direction)random.Next(4);
        }

        internal static Func<CoordinatesChain, Predicate<Coordinates>, bool> Bent =
            (coordinatesChain, coordinatesValidator) =>
        {
            bool isDirectionValid(Direction direction)
            {
                var nextCoordinates = coordinatesChain.Last.GetInDirection(direction);
                return coordinatesValidator(nextCoordinates) && !coordinatesChain.Occupies(nextCoordinates);
            };

            Direction[] availableDirections = Enum.GetValues(typeof(Direction)).Cast<Direction>().
                Where(isDirectionValid).
                ToArray();

            if (availableDirections.Length == 0)
                return false;

            var direction = availableDirections[random.Next(availableDirections.Length)];
            coordinatesChain.Add(coordinatesChain.Last.GetInDirection(direction));
            return true;
        };

        internal static Func<CoordinatesChain, Predicate<Coordinates>, bool> Straight =
            (coordinatesChain, coordinatesValidator) =>
        {
            var direction = coordinatesChain.GetLastDirection() ?? GetRandomDirection();
            var nextCoordinations = coordinatesChain.Last.GetInDirection(direction);

            if (!coordinatesValidator(nextCoordinations))
                return false;

            coordinatesChain.Add(nextCoordinations);
            return true;
        };
    }


    internal class ShipCreator
    {
        private const int CreateShipTaskNum = 4;
        private const int CreationMaxTime = 6; // seconds

        private static Random random = new Random();
        private ThreadLocal<bool[,]>? threadLocalBoard;

        private CancellationTokenSource? CancelSource;

        private uint HorizontalSize { get { return settings.HorizontalSize; } }
        private uint VerticalSize { get { return settings.VerticalSize; } }

        Settings settings;

        internal ShipCreator(Settings settings)
        {
            this.settings = settings;
        }



        internal List<Ship> ExecuteCreateShips()
        {
            bool debugCreationMode = false;
            if (debugCreationMode)
            {
                // No multithred approach
                CancelSource = new CancellationTokenSource();
                return CreateShips();
            }
            else
            {
                CancelSource = new CancellationTokenSource();
                CancelSource.CancelAfter(CreationMaxTime * 1000);

                List<Task<List<Ship>>> tasks = new List<Task<List<Ship>>>();
                for (int i = 0; i < CreateShipTaskNum; i++)
                    tasks.Add(new Task<List<Ship>>(() => CreateShips()));

                tasks.ForEach(task => task.Start());

                int finishedTask = Task.WaitAny(tasks.ToArray());

                CancelSource.Cancel();

                if (tasks[finishedTask].Result.Count == 0)
                    throw new ShipCreationException();
                return tasks[finishedTask].Result;
            }
        }

        private List<Ship> CreateShips()
        {
            try
            {
                InitSquares();
                List<Ship> ships = new List<Ship>();

                settings.ShipDescriptions.ForEach(shipDescription =>
                {
                    for (int i = 0; i < shipDescription.Count; i++)
                    {
                        CancelSource!.Token.ThrowIfCancellationRequested();

                        ships.Add(CreateShip(shipDescription.Name, shipDescription.Size));
                    }
                });
                return ships;
            }
            catch (OperationCanceledException)
            {
                return new List<Ship>();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<Ship>();
            }
        }

        private Ship CreateShip(string name, uint size)
        {
            while (true)
            {
                CancelSource!.Token.ThrowIfCancellationRequested();

                Ship? ship = TryCreateShip(name, size,
                    settings.StrightShips ? ComponentCreationStrategies.Straight : ComponentCreationStrategies.Bent);

                if (ship != null)
                    return ship;
            }
        }

        private void InitSquares()
        {
            threadLocalBoard = new ThreadLocal<bool[,]>(() => new bool[VerticalSize, HorizontalSize]);

            for (int y = 0; y < threadLocalBoard.Value!.GetLength(0); y++)
                for (int x = 0; x < threadLocalBoard.Value!.GetLength(1); x++)
                    threadLocalBoard.Value[y, x] = true;
        }

        private bool IsSquareAllowed(Coordinates coor)
        {
            return AreCoordinatesValid(coor) && IsSquareAvailable(coor);
        }

        private Ship? TryCreateShip(string name, uint size, Func<CoordinatesChain, Predicate<Coordinates>, bool> addComponent)
        {
            CoordinatesChain squareChain = new CoordinatesChain();
            squareChain.Add(GetRandomSquare());

            for (int i = 1; i < size; i++)
            {
                if (!addComponent(squareChain, IsSquareAllowed))
                    return null;
            }

            squareChain.Chain.ForEach(SetShipSquareUnavailable);
            return new Ship(name, squareChain.Chain);
        }

        private void SetShipSquareUnavailable(Coordinates coor)
        {
            SetSquareUnavailable(coor);
            if (!settings.ShipsCanStick)
                SetAllAdjacentSquaresUnavailable(coor);
        }

        Coordinates GetRandomSquare()
        {
            while (true)
            {
                CancelSource!.Token.ThrowIfCancellationRequested();

                int x = random.Next((int)HorizontalSize);
                int y = random.Next((int)VerticalSize);
                if (IsSquareAvailable((uint)x, (uint)y))
                    return new Coordinates((uint)x, (uint)y);
            }
        }

        private bool IsSquareAvailable(in Coordinates coordinates)
        {
            return IsSquareAvailable(coordinates.X, coordinates.Y);
        }

        private bool IsSquareAvailable(uint x, uint y)
        {
            return threadLocalBoard!.Value![y, x];
        }

        private void SetSquareUnavailable(in Coordinates coor)
        {
            SetSquareUnavailable(coor.X, coor.Y);
        }

        private void SetSquareUnavailable(uint x, uint y)
        {
            threadLocalBoard!.Value![y, x] = false;
        }

        private void SetAllAdjacentSquaresUnavailable(in Coordinates coor)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    int x = (int)coor.X + i;
                    int y = (int)coor.Y + j;

                    if (!AreCoordinatesValid(x, y))
                        continue;

                    SetSquareUnavailable((uint)x, (uint)y);
                }
            }
        }
        private bool AreCoordinatesValid(in Coordinates squareCoordinates)
        {
            return AreCoordinatesValid((int)squareCoordinates.X, (int)squareCoordinates.Y);
        }

        private bool AreCoordinatesValid(int x, int y)
        {
            return x >= 0 && x < HorizontalSize && y >= 0 && y < VerticalSize;
        }
    }

    public class DefaultGameCreator : IGameCreator
    {
        public DefaultGameCreator() {}

        public Game Create(Settings settings)
        {
            var ships = CreateShips(settings);

            var board = CreateBoard(settings);

            board.ApplyShipComponents(ships.Aggregate(new List<ShipComponent>(), (componentList, ship) =>
            {
                componentList.AddRange(ship.Components);
                return componentList;
            }));

            return new Game(board, ships);
        }

        private static Board CreateBoard(Settings settings)
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

        private static List<Ship> CreateShips(Settings settings)
        {
            ShipCreator shipSetter = new ShipCreator(settings);
            return shipSetter.ExecuteCreateShips();
        }
    }
}
