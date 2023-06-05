using NUnit.Framework;



namespace GameModel.Tests
{
    [TestFixture]
    public class CoordinatesTests
    {
        [TestCase(1, 2, Direction.Up, 1, 1)]
        [TestCase(1, 2, Direction.Down, 1, 3)]
        [TestCase(1, 2, Direction.Left, 0, 2)]
        [TestCase(1, 2, Direction.Right, 2, 2)]
        [TestCase(-1, -1, Direction.Right, 0, -1)]
        public void GetInDirection(int x, int y, Direction direction, int expectedX, int expectedY)
        {
            Assert.AreEqual(new Coordinates(x, y).GetInDirection(direction), new Coordinates(expectedX, expectedY));
        }

        [Test]
        public void GetInDirectionVsGet__()
        {
            Assert.AreEqual(new Coordinates(1, 2).GetInDirection(Direction.Up), new Coordinates(1, 2).GetUp());
            Assert.AreEqual(new Coordinates(1, 2).GetInDirection(Direction.Down), new Coordinates(1, 2).GetDown());
            Assert.AreEqual(new Coordinates(1, 2).GetInDirection(Direction.Left), new Coordinates(1, 2).GetLeft());
            Assert.AreEqual(new Coordinates(1, 2).GetInDirection(Direction.Right), new Coordinates(1, 2).GetRight());
        }


        [TestCase(1, 2, 1, 1, Direction.Up)]
        [TestCase(1, 2, 1, 3, Direction.Down)]
        [TestCase(1, 2, 0, 2, Direction.Left)]
        [TestCase(1, 2, 2, 2, Direction.Right)]
        [TestCase(1, 1, 2, 2, null)]
        [TestCase(1, 1, 1, 3, null)]

        public void GetDirection(int x1, int y1, int x2, int y2, Direction? expectedDirecion)
        {
            Assert.AreEqual(new Coordinates(x1, y1).GetDirection(new Coordinates(x2, y2)), expectedDirecion);
        }


        [Test]
        public void IsNextVsInDirection([Values] Direction direction)
        {
            Coordinates coor = new Coordinates(2, 3);
            Assert.True(coor.GetInDirection(direction).IsNext(coor));
        }

        [TestCase(1, 3, 1, 4)]
        [TestCase(-2, -3, -3, -3)]
        public void IsNext(int x1, int y1, int x2, int y2)
        {
            Assert.True(new Coordinates(x1, y1).IsNext(new Coordinates(x2, y2)));
        }


        [TestCase(2, 3, 3, 4)]
        [TestCase(4, 3, 2, 3)]
        [TestCase(1, 7, 1, 5)]
        public void NotIsNext(int x1, int y1, int x2, int y2)
        {
            Assert.False(new Coordinates(x1, y1).IsNext(new Coordinates(x2, y2)));
        }


        [TestCase(3, 4, 3, 5)]
        [TestCase(3, 4, 3, 3)]
        [TestCase(3, 4, 4, 5)]
        [TestCase(3, 4, 4, 4)]
        [TestCase(3, 4, 4, 3)]
        [TestCase(3, 4, 2, 5)]
        [TestCase(3, 4, 2, 4)]
        [TestCase(3, 4, 2, 3)]

        public void IsAdjacent(int x1, int y1, int x2, int y2)
        {
            Assert.True(new Coordinates(x1, y1).IsAdjacent(new Coordinates(x2, y2)));
        }


        [TestCase(3, 4, 3, 4)]
        [TestCase(3, 4, 3, 6)]
        [TestCase(3, 4, 1, 4)]
        [TestCase(3, 4, 0, 3)]
        public void NotIsAdjacent(int x1, int y1, int x2, int y2)
        {
            Assert.False(new Coordinates(x1, y1).IsAdjacent(new Coordinates(x2, y2)));
        }
     }

     [TestFixture]
     class CoordinatesChainTests
     {
        [Test]
        public void Includes()
        {
            CoordinatesChain chain = new CoordinatesChain();
            chain.Add(new Coordinates(3, 4));
            chain.Add(new Coordinates(3, 5));

            Assert.True(chain.Includes(new Coordinates(3, 5)));
            Assert.False(chain.Includes(new Coordinates(3, 8)));
        }

        [Test]
        public void Add()
        {
            CoordinatesChain chain1 = new CoordinatesChain();
            chain1.Add(new Coordinates(3, 4));
            chain1.Add(new Coordinates(3, 5));
            Assert.Throws<ArgumentException>(() => chain1.Add(new Coordinates(4, 6)));

            CoordinatesChain chain2 = new CoordinatesChain();
            chain2.Add(new Coordinates(3, 4));
            chain2.Add(new Coordinates(3, 5));
            Assert.Throws<ArgumentException>(() => chain2.Add(new Coordinates(3, 4)));
        }

        [Test]
        public void Last()
        {
            CoordinatesChain chain = new CoordinatesChain();
            Assert.Throws<InvalidOperationException>(() => { _ = chain.Last; });

            chain.Add(new Coordinates(3, 4));
            chain.Add(new Coordinates(3, 5));

            Assert.AreEqual(chain.Last, new Coordinates(3, 5));
        }

        [Test]
        public void IsStraight()
        {
            CoordinatesChain chain = new CoordinatesChain();
            Assert.True(chain.IsStraight());

            chain.Add(new Coordinates(3, 4));
            Assert.True(chain.IsStraight());

            chain.Add(new Coordinates(3, 5));
            Assert.True(chain.IsStraight());

            chain.Add(new Coordinates(3, 6));
            Assert.True(chain.IsStraight());

            chain.Add(new Coordinates(4, 6));
            Assert.False(chain.IsStraight());
        }

        [Test]
        public void GetLastDirection()
        {
            CoordinatesChain chain = new CoordinatesChain();
            Assert.AreEqual(chain.GetLastDirection(), null);

            chain.Add(new Coordinates(3, 4));
            Assert.AreEqual(chain.GetLastDirection(), null);

            chain.Add(new Coordinates(3, 5));
            Assert.AreEqual(chain.GetLastDirection(), Direction.Down);
        }
    }
}
