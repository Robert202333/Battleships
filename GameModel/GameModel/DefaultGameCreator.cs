using System.Diagnostics;


namespace GameModel
{
    using ShipCreationData = Tuple<CoordinatesChain, ShipDescription>;


    static internal class ComponentCreationStrategies
    {
        private static Direction GetRandomDirection(Random random)
        {
            return (Direction)random.Next(4);
        }

        internal static Func<CoordinatesChain, Predicate<Coordinates>, Random, bool> Bent =
            (coordinatesChain, coordinatesValidator, random) =>
        {
            bool isDirectionValid(Direction direction)
            {
                var nextCoordinates = coordinatesChain.Last.GetInDirection(direction);
                return coordinatesValidator(nextCoordinates) && !coordinatesChain.Includes(nextCoordinates);
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

        internal static Func<CoordinatesChain, Predicate<Coordinates>, Random, bool> Straight =
            (coordinatesChain, coordinatesValidator, random) =>
        {
            var direction = coordinatesChain.GetLastDirection() ?? GetRandomDirection(random);
            var nextCoordinations = coordinatesChain.Last.GetInDirection(direction);

            if (!coordinatesValidator(nextCoordinations))
                return false;

            coordinatesChain.Add(nextCoordinations);
            return true;
        };
    }


    internal class ShipsCreator
    {
        private readonly Random random = new Random();
        private readonly bool[,] board;

        private readonly CancellationToken cancellationToken;

        private int HorizontalSize { get { return settings.HorizontalSize; } }
        private int VerticalSize { get { return settings.VerticalSize; } }

        private readonly Settings settings;

        internal ShipsCreator(Settings settings, CancellationToken cancellationToken, int randomSeed = 0)
        {
            this.settings = settings;
            this.cancellationToken = cancellationToken;

            random = (randomSeed != 0) ? new Random(randomSeed) : new Random();

            board = new bool[VerticalSize, HorizontalSize];
            for (int y = 0; y < board.GetLength(0); y++)
            {
                for (int x = 0; x < board.GetLength(1); x++)
                {
                    board[y, x] = true;
                }
            }
        }


        internal List<ShipCreationData> Execute()
        {
            try
            {
                List<ShipCreationData> shipCreationDatas = new();

                settings.ShipDescriptions.ForEach(shipDescription =>
                {
                    for (int i = 0; i < shipDescription.Count; i++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        shipCreationDatas.Add(Tuple.Create(CreateShipPosition(shipDescription.Size), shipDescription));
                    }
                });
                return shipCreationDatas;
            }
            catch (OperationCanceledException)
            {
                return new();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new();
            }
        }

        private CoordinatesChain CreateShipPosition(int size)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                CoordinatesChain? shipPosition = TryCreateShipPosition(size,
                    settings.StraightShips ? ComponentCreationStrategies.Straight : ComponentCreationStrategies.Bent);

                if (shipPosition != null)
                    return shipPosition;
            }
        }

        private bool IsSquareAllowed(Coordinates coor)
        {
            return AreCoordinatesValid(coor) && IsSquareAvailable(coor);
        }

        private CoordinatesChain? TryCreateShipPosition(int size, Func<CoordinatesChain, Predicate<Coordinates>, Random, bool> addCoordinatesToChain)
        {
            CoordinatesChain shipPosition = new CoordinatesChain();
            shipPosition.Add(GetRandomSquare());

            for (int i = 1; i < size; i++)
            {
                if (!addCoordinatesToChain(shipPosition, IsSquareAllowed, random))
                    return null;
            }

            shipPosition.Chain.ForEach(SetShipSquareUnavailable);
            return shipPosition;
        }

        private void SetShipSquareUnavailable(Coordinates coor)
        {
            SetSquareUnavailable(coor);
            if (!settings.ShipsCanStick)
                SetAllAdjacentSquaresUnavailable(coor);
        }

        private Coordinates GetRandomSquare()
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                int x = random.Next(HorizontalSize);
                int y = random.Next(VerticalSize);
                if (IsSquareAvailable(x, y))
                    return new Coordinates(x, y);
            }
        }

        private bool IsSquareAvailable(Coordinates coordinates)
        {
            return IsSquareAvailable(coordinates.X, coordinates.Y);
        }

        private bool IsSquareAvailable(int x, int y)
        {
            return board[y, x];
        }

        private void SetSquareUnavailable(Coordinates coor)
        {
            SetSquareUnavailable(coor.X, coor.Y);
        }

        private void SetSquareUnavailable(int x, int y)
        {
            board[y, x] = false;
        }

        private void SetAllAdjacentSquaresUnavailable(Coordinates coor)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    int x = coor.X + i;
                    int y = coor.Y + j;

                    if (!AreCoordinatesValid(x, y))
                        continue;

                    SetSquareUnavailable(x, y);
                }
            }
        }
        private bool AreCoordinatesValid(Coordinates squareCoordinates)
        {
            return AreCoordinatesValid((int)squareCoordinates.X, (int)squareCoordinates.Y);
        }

        private bool AreCoordinatesValid(int x, int y)
        {
            return x >= 0 && x < HorizontalSize && y >= 0 && y < VerticalSize;
        }
    }

    internal class ShipCreationDatasProvider
    {
        private const int CreateShipTaskNum = 4;
        private const int CreationMaxTime = 6; // seconds

        private readonly Settings settings;

        internal ShipCreationDatasProvider(Settings settings)
        {
            this.settings = settings;
        }

        internal List<ShipCreationData> Execute()
        {
            bool debugCreationMode = false;
            if (debugCreationMode)
            {
                // No multithread approach
                using CancellationTokenSource cancelSource = new CancellationTokenSource();
                return CreateShipCreationDatas(cancelSource.Token);
            }
            else
            {
                using CancellationTokenSource cancelSource = new CancellationTokenSource();
                cancelSource.CancelAfter(CreationMaxTime * 1000);

                Task<List<ShipCreationData>>[] tasks =
                    new Task<List<ShipCreationData>>[CreateShipTaskNum];

                for (int i = 0; i < CreateShipTaskNum; i++)
                    tasks[i] = new(() => CreateShipCreationDatas(cancelSource.Token));

                foreach (var task in tasks)
                    task.Start();

                int finishedTask = Task.WaitAny(tasks);

                cancelSource.Cancel();

                if (tasks[finishedTask].Result.Count == 0)
                    throw new ShipCreationException();

                return tasks[finishedTask].Result;
            }
        }

        private List<ShipCreationData> CreateShipCreationDatas(CancellationToken cancellationToken)
        {
            ShipsCreator creator = new(settings, cancellationToken);
            return creator.Execute();
        }
    }


    public class DefaultGameCreator : AbstractGameCreator
    {
        public DefaultGameCreator() { }

        protected override IEnumerable<Ship> CreateShips(Board board, Settings settings)
        {
            ShipCreationDatasProvider provider = new(settings);
            var shipCreationDatas = provider.Execute();

            return shipCreationDatas.Select(shipCreationData =>
            {
                var (shipPosition, shipDescription) = shipCreationData;
                return CreateShip(shipDescription.Name, shipPosition.Chain, board);
            });
        }
    }
}
