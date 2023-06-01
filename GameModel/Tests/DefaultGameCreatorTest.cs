using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GameModel.Tests
{
    [TestFixture]
    public class ShipsCreatorTest
    {
        private void AssertCoordinates(List<CoordinatesChain> chains, Settings settings)
        {
            void assertCoordinates(Coordinates coordinates)
            {
                Assert.True(coordinates.X >= 0);
                Assert.True(coordinates.X < settings.HorizontalSize);
                Assert.True(coordinates.Y >= 0);
                Assert.True(coordinates.Y < settings.VerticalSize);
            }
            chains.ForEach(chain => chain.Chain.ForEach(coordinates => assertCoordinates(coordinates)));
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
                if (sizeToCountMap.ContainsKey(chain.Chain.Count))
                    sizeToCountMap[chain.Chain.Count] = sizeToCountMap[chain.Chain.Count] + 1;
                else
                    sizeToCountMap[chain.Chain.Count] = 1;
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

        [Test]
        public void CreateStraightAndStick()
        {
            CancellationTokenSource ctSource = new CancellationTokenSource();

            Settings settings = new Settings();
            settings.StraightShips = true;
            settings.ShipsCanStick = true;

            var shipsCraetor = new ShipsCreator(settings, ctSource.Token, 34543);
            var result = shipsCraetor.Execute();
            AssertChains(result.Select(shipCreationData => shipCreationData.Item1).ToList(), settings);
        }

        [Test]
        public void CreateStraight()
        {
            CancellationTokenSource ctSource = new CancellationTokenSource();

            Settings settings = new Settings();
            settings.StraightShips = true;
            settings.ShipsCanStick = false;

            var shipsCraetor = new ShipsCreator(settings, ctSource.Token, 54321);
            var result = shipsCraetor.Execute();
            AssertChains(result.Select(shipCreationData => shipCreationData.Item1).ToList(), settings);
        }

        [Test]
        public void CreateBent()
        {
            CancellationTokenSource ctSource = new CancellationTokenSource();

            Settings settings = new Settings();
            settings.StraightShips = false;
            settings.ShipsCanStick = false;

            var shipsCraetor = new ShipsCreator(settings, ctSource.Token, 23432);
            var result = shipsCraetor.Execute();
            AssertChains(result.Select(shipCreationData => shipCreationData.Item1).ToList(), settings);
        }

        [Test]
        public void CreateBentAndStick()
        {
            CancellationTokenSource ctSource = new CancellationTokenSource();

            Settings settings = new Settings();
            settings.StraightShips = false;
            settings.ShipsCanStick = true;

            var shipsCraetor = new ShipsCreator(settings, ctSource.Token, 12345);
            var result = shipsCraetor.Execute();
            AssertChains(result.Select(shipCreationData => shipCreationData.Item1).ToList(), settings);
        }


        [Test]
        public void CreateStraightAndStickRandom()
        {
            CancellationTokenSource ctSource = new CancellationTokenSource();

            Settings settings = new Settings();
            settings.StraightShips = true;
            settings.ShipsCanStick = true;

            var shipsCraetor = new ShipsCreator(settings, ctSource.Token);
            var result = shipsCraetor.Execute();
            AssertChains(result.Select(shipCreationData => shipCreationData.Item1).ToList(), settings);
        }

        [Test]
        public void CreateStraightRandom()
        {
            CancellationTokenSource ctSource = new CancellationTokenSource();

            Settings settings = new Settings();
            settings.StraightShips = true;
            settings.ShipsCanStick = false;

            var shipsCraetor = new ShipsCreator(settings, ctSource.Token);
            var result = shipsCraetor.Execute();
            AssertChains(result.Select(shipCreationData => shipCreationData.Item1).ToList(), settings);
        }

        [Test]
        public void CreateBentRandom()
        {
            CancellationTokenSource ctSource = new CancellationTokenSource();

            Settings settings = new Settings();
            settings.StraightShips = false;
            settings.ShipsCanStick = false;

            var shipsCraetor = new ShipsCreator(settings, ctSource.Token);
            var result = shipsCraetor.Execute();
            AssertChains(result.Select(shipCreationData => shipCreationData.Item1).ToList(), settings);
        }

        [Test]
        public void CreateBentAndStickRandom()
        {
            CancellationTokenSource ctSource = new CancellationTokenSource();

            Settings settings = new Settings();
            settings.StraightShips = false;
            settings.ShipsCanStick = true;

            var shipsCraetor = new ShipsCreator(settings, ctSource.Token);
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
            catch(Exception)
            {
                return DefaultGameCreatorErrorResult;
            }
            return DefaultGameCreatorNormalResult;
        }

        [Test]
        public void Execue()
        {
            Task<int>[] tasks =
            {
                new Task<int>(() => { return Guard(GuardWaitingTimeNormal); }),
                new Task<int>(() => { return DefaultGameCreatorFun(); })
            };

            foreach (var task in tasks)
                task.Start();

            int finishedTaskIndex = Task.WaitAny(tasks);
            Assert.AreEqual(DefaultGameCreatorNormalResult, tasks[finishedTaskIndex].Result);
        }


        [Test]
        public void InterruptedWhenNoAvailableSquare()
        {
            Task<int>[] tasks =
            {
                new Task<int>(() => { return Guard(GuardWaitingTimeNormal); }),
                new Task<int>(() => { return DefaultGameCreatorFun(4, 4, 10, 1); }) // too many ships
            };

            foreach (var task in tasks)
                task.Start();

            int finishedTaskIndex = Task.WaitAny(tasks);
            Assert.AreEqual(DefaultGameCreatorInterruptedResult, tasks[finishedTaskIndex].Result);
        }

        [Test]
        public void InterruptedWhenCanNotFinishShip()
        {
            Task<int>[] tasks =
            {
                new Task<int>(() => { return Guard(GuardWaitingTimeNormal); }),
                new Task<int>(() => { return DefaultGameCreatorFun(4, 4, 1, 5); }) // ship too long
            };

            foreach(var task in tasks)
                task.Start();

            int finishedTaskIndex = Task.WaitAny(tasks);
            Assert.AreEqual(DefaultGameCreatorInterruptedResult, tasks[finishedTaskIndex].Result);
        }

        [Test]
        public void InterruptedTooEarly()
        {
            Task<int>[] tasks =
            {
                new Task<int>(() => { return Guard(GuardWaitingTimeShort); }),
                new Task<int>(() => { return DefaultGameCreatorFun(4, 4, 1, 5); })
            };

            foreach (var task in tasks)
                task.Start();

            int finishedTaskIndex = Task.WaitAny(tasks);
            Assert.AreEqual(GuardResult, tasks[finishedTaskIndex].Result);
        }
    }
}
