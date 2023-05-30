using System.Diagnostics;


namespace GameModel
{
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


    internal class CoordinatesChainsCreator
    {
        private const int CreateShipTaskNum = 4;
        private const int CreationMaxTime = 6; // seconds

        private static Random random = new Random();
        private ThreadLocal<bool[,]>? threadLocalBoard;

        private CancellationTokenSource? CancelSource;

        private uint HorizontalSize { get { return settings.HorizontalSize; } }
        private uint VerticalSize { get { return settings.VerticalSize; } }

        private readonly Settings settings;

        internal CoordinatesChainsCreator(Settings settings)
        {
            this.settings = settings;
        }



        internal List<Tuple<CoordinatesChain, ShipDescription>> ExecuteCreateCoordinatesChains()
        {
            bool debugCreationMode = false;
            if (debugCreationMode)
            {
                // No multithred approach
                CancelSource = new CancellationTokenSource();
                return CreateCoordinatesChains();
            }
            else
            {
                CancelSource = new CancellationTokenSource();
                CancelSource.CancelAfter(CreationMaxTime * 1000);

                List<Task<List<Tuple<CoordinatesChain, ShipDescription>>>> tasks = new ();
                for (int i = 0; i < CreateShipTaskNum; i++)
                    tasks.Add(new (() => CreateCoordinatesChains()));

                tasks.ForEach(task => task.Start());

                int finishedTask = Task.WaitAny(tasks.ToArray());

                CancelSource.Cancel();

                if (tasks[finishedTask].Result.Count == 0)
                    throw new ShipCreationException();
                return tasks[finishedTask].Result;
            }
        }

        private List<Tuple<CoordinatesChain, ShipDescription>> CreateCoordinatesChains()
        {
            try
            {
                InitSquares();
                List<Tuple<CoordinatesChain, ShipDescription>> coordinatesChains = new ();

                settings.ShipDescriptions.ForEach(shipDescription =>
                {
                    for (int i = 0; i < shipDescription.Count; i++)
                    {
                        CancelSource!.Token.ThrowIfCancellationRequested();

                        coordinatesChains.Add(Tuple.Create(CreateCoordinatesChain(shipDescription.Size), shipDescription));
                    }
                });
                return coordinatesChains;
            }
            catch (OperationCanceledException)
            {
                return new ();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new ();
            }
        }

        private CoordinatesChain CreateCoordinatesChain(uint size)
        {
            while (true)
            {
                CancelSource!.Token.ThrowIfCancellationRequested();

                CoordinatesChain? coordinateChain = TryCreateCoordinatesChain(size,
                    settings.StrightShips ? ComponentCreationStrategies.Straight : ComponentCreationStrategies.Bent);

                if (coordinateChain != null)
                    return coordinateChain;
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

        private CoordinatesChain? TryCreateCoordinatesChain(uint size, Func<CoordinatesChain, Predicate<Coordinates>, bool> addComponent)
        {
            CoordinatesChain coordinatesChain = new CoordinatesChain();
            coordinatesChain.Add(GetRandomSquare());

            for (int i = 1; i < size; i++)
            {
                if (!addComponent(coordinatesChain, IsSquareAllowed))
                    return null;
            }

            coordinatesChain.Chain.ForEach(SetShipSquareUnavailable);
            return coordinatesChain;
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

    public class DefaultGameCreator : GameCreator
    {
        public DefaultGameCreator() {}



        protected override IEnumerable<Ship> CreateShips(Board board, Settings settings)
        {
            var coordinatesChains = CreateCoordinatesChains(settings);

            return coordinatesChains.Select(coordinateChain =>
            {
                var (chain, shipDescription) = coordinateChain;
                return CreateShip(shipDescription.Name, chain.Chain, board);
            });
        }

        private static List<Tuple<CoordinatesChain, ShipDescription>> CreateCoordinatesChains(Settings settings)
        {
            CoordinatesChainsCreator ccCraetor = new CoordinatesChainsCreator(settings);
            return ccCraetor.ExecuteCreateCoordinatesChains();
        }
    }
}
