using NUnit.Framework;

namespace GameModel.Tests
{
    internal record struct CreateParameters(bool StraightShips, bool ShipsCanStick);


    [TestFixture]
    public class ShipsCreatorTest
    {
        private Settings settings = new Settings();

        private CancellationTokenSource? cancellationTokentSource;

        private Dictionary<CreateParameters, int> randomSeedsMap = new();

        private void AssertCoordinates(List<CoordinatesChain> chains, Settings settings)
        {
            void assertCoordinates(Coordinates coordinates)
            {
                Assert.True(coordinates.X >= 0);
                Assert.True(coordinates.X < settings.HorizontalSize);
                Assert.True(coordinates.Y >= 0);
                Assert.True(coordinates.Y < settings.VerticalSize);
            }
            chains.ForEach(chain => chain.Chain.ForEach(assertCoordinates));
        }

        private void AssertNotIntersect(List<CoordinatesChain> chains, Settings settings)
        {
            void assertNotIntersect(CoordinatesChain chain1, CoordinatesChain chain2)
            {
                chain1.Chain.ForEach(coordinates => Assert.False(chain2.Includes(coordinates)));
            }

            for (int i = 0; i < chains.Count - 1; i++)
            {
                for (int j = i + 1; j < chains.Count; j++)
                {
                    assertNotIntersect(chains[i], chains[j]);
                }
            }
        }

        private void AssertStraight(List<CoordinatesChain> chains, Settings settings)
        {
            if (settings.StraightShips)
            {
                chains.ForEach(chain => Assert.True(chain.IsStraight()));
            }
        }

        private void AssertNotStick(List<CoordinatesChain> chains, Settings settings)
        {
            void assertNotStick(CoordinatesChain chain1, CoordinatesChain chain2)
            {
                chain1.Chain.ForEach(coordinates1 =>
                {
                    chain2.Chain.ForEach(coordinates2 => Assert.False(coordinates2.IsAdjacent(coordinates1)));
                });
            }

            if (!settings.ShipsCanStick)
            {
                for (int i = 0; i < chains.Count - 1; i++)
                {
                    for (int j = i + 1; j < chains.Count; j++)
                    {
                        assertNotStick(chains[i], chains[j]);
                    }
                }
            }
        }

        private void AssertCountAndSize(List<CoordinatesChain> chains, Settings settings)
        {
            Dictionary<int, int> sizeToCountMap = new Dictionary<int, int>();
            chains.ForEach(chain =>
            {
                if (!sizeToCountMap.ContainsKey(chain.Chain.Count))
                    sizeToCountMap[chain.Chain.Count] = 0;
                sizeToCountMap[chain.Chain.Count]++;
            });

            settings.ShipDescriptions.ForEach(shipDefinition =>
            {
                Assert.True(sizeToCountMap.ContainsKey(shipDefinition.Size));
                Assert.AreEqual(sizeToCountMap[shipDefinition.Size], shipDefinition.Count);
            });
        }

        private void AssertChains(List<CoordinatesChain> chains, Settings settings)
        {
            AssertCountAndSize(chains, settings);
            AssertCoordinates(chains, settings);
            AssertNotIntersect(chains, settings);
            AssertStraight(chains, settings);
            AssertNotStick(chains, settings);
        }

        [OneTimeSetUp]
        public void Init()
        {
            randomSeedsMap.Add(new CreateParameters(true, true), 12345);
            randomSeedsMap.Add(new CreateParameters(true, false), 54321);
            randomSeedsMap.Add(new CreateParameters(false, true), 12321);
            randomSeedsMap.Add(new CreateParameters(false, false), 54345);
        }

        [SetUp]
        public void InitTest()
        {
            cancellationTokentSource = new CancellationTokenSource();
        }

        [TearDown]
        public void FinishTest()
        {
            cancellationTokentSource!.Dispose();
        }

        [TestCase(true, true, 12345, TestName = "Straight and can stick")]
        [TestCase(true, false, 12321, TestName = "Straight and can't stick")]
        [TestCase(false, true, 54321, TestName = "Bent and can stick")]
        [TestCase(false, false, 54345, TestName = "Bent and can't stick")]
        public void Create([Values] bool straightShips, [Values] bool shipsCanStiick, int randomSeed)
        {
            settings.StraightShips = straightShips;
            settings.ShipsCanStick = shipsCanStiick;

            var shipsCraetor = new ShipsCreator(settings, cancellationTokentSource!.Token, randomSeed);
            var result = shipsCraetor.Execute();
            AssertChains(result.Select(shipCreationData => shipCreationData.Item1).ToList(), settings);
        }



        [TestCase(true, true, TestName = "Random Straight and can stick")]
        [TestCase(true, false, TestName = "Random Straight and can't stick")]
        [TestCase(false, true, TestName = "Random Bent and can stick")]
        [TestCase(false, false, TestName = "Random Bent and can't stick")]
        public void CreateRandom(bool straightShips, bool shipsCanStiick)
        {
            settings.StraightShips = straightShips;
            settings.ShipsCanStick = shipsCanStiick;

            var shipsCraetor = new ShipsCreator(settings, cancellationTokentSource!.Token);
            var result = shipsCraetor.Execute();
            AssertChains(result.Select(shipCreationData => shipCreationData.Item1).ToList(), settings);
        }

    }

    [TestFixture]
    public class DefaultGameCreatorTest
    {
        const int GuardResult = 0;
        const int DefaultGameCreatorInterruptedResult = 1;
        const int DefaultGameCreatorNormalResult = 2;
        const int DefaultGameCreatorErrorResult = 3;
        const int GuardWaitingTimeNormal = 8; // 2 additional seconds as safety margin
        const int GuardWaitingTimeShort = 1; // checks if test works

        private static int Guard(int seconds)
        {
            Thread.Sleep(seconds * 1000);
            return GuardResult;
        }

        private static int DefaultGameCreatorFun(int horizontalSize = 10, int verticalSize = 10, int destroyerCount = 2, int destroyerSize = 2)
        {
            var creator = new DefaultGameCreator();
            Settings settings = new();

            // Invalid settings, ship is too long 
            settings.HorizontalSize = horizontalSize;
            settings.VerticalSize = verticalSize;
            settings.ShipDescriptions[0].Count = destroyerCount;
            settings.ShipDescriptions[0].Size = destroyerSize;

            try
            {
                creator.Execute(settings);
            }
            catch (ShipCreationException)
            {
                return DefaultGameCreatorInterruptedResult;
            }
            catch (Exception)
            {
                return DefaultGameCreatorErrorResult;
            }
            return DefaultGameCreatorNormalResult;
        }

        [TestCase(GuardWaitingTimeNormal, 10, 10, 2, 2, DefaultGameCreatorNormalResult, TestName = "Create ships")]
        [TestCase(GuardWaitingTimeNormal, 4, 4, 10, 1, DefaultGameCreatorInterruptedResult, TestName = "Interrupted when no available square")]
        [TestCase(GuardWaitingTimeNormal, 4, 4, 1, 5, DefaultGameCreatorInterruptedResult, TestName = "interrupted when ship is too long")]
        [TestCase(GuardWaitingTimeShort, 4, 4, 1, 5, GuardResult, TestName = "Interrupted too early")]
        public void Execute(int guardTime, int horizontalBoardSize, int verticalBoardSize, int destroyerCount, int destroyerSize, int expectedResult)
        {
            Task<int>[] tasks =
            {
                new Task<int>(() => { return Guard(guardTime); }),
                new Task<int>(() => { return DefaultGameCreatorFun(horizontalBoardSize, verticalBoardSize, destroyerCount, destroyerSize); })
            };

            foreach (var task in tasks)
                task.Start();

            int finishedTaskIndex = Task.WaitAny(tasks);
            Assert.AreEqual(expectedResult, tasks[finishedTaskIndex].Result);
        }
    }
}
