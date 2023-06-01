using NUnit.Framework;



namespace GameModel.Tests
{
    [TestFixture]
    public class CoordinatesTests
    {
        [Test]
        public void GetInDirection()
        {
            Assert.AreEqual(new Coordinates(1, 2).GetInDirection(Direction.Up), new Coordinates(1, 1));
            Assert.AreEqual(new Coordinates(1, 2).GetInDirection(Direction.Down), new Coordinates(1, 3));
            Assert.AreEqual(new Coordinates(1, 2).GetInDirection(Direction.Left), new Coordinates(0, 2));
            Assert.AreEqual(new Coordinates(1, 2).GetInDirection(Direction.Right), new Coordinates(2, 2));
            Assert.AreEqual(new Coordinates(-1, -1).GetInDirection(Direction.Right), new Coordinates(0, -1));

            Assert.AreEqual(new Coordinates(1, 2).GetInDirection(Direction.Up), new Coordinates(1, 2).GetUp());
            Assert.AreEqual(new Coordinates(1, 2).GetInDirection(Direction.Down), new Coordinates(1, 2).GetDown());
            Assert.AreEqual(new Coordinates(1, 2).GetInDirection(Direction.Left), new Coordinates(1, 2).GetLeft());
            Assert.AreEqual(new Coordinates(1, 2).GetInDirection(Direction.Right), new Coordinates(1, 2).GetRight());

        }

        [Test]
        public void GetDirection()
        {
            Assert.AreEqual(new Coordinates(1, 2).GetDirection(new Coordinates(1, 1)), Direction.Up);
            Assert.AreEqual(new Coordinates(1, 2).GetDirection(new Coordinates(1, 3)), Direction.Down);
            Assert.AreEqual(new Coordinates(1, 2).GetDirection(new Coordinates(0, 2)), Direction.Left);
            Assert.AreEqual(new Coordinates(1, 2).GetDirection(new Coordinates(2, 2)), Direction.Right);
            Assert.AreEqual(new Coordinates(1, 1).GetDirection(new Coordinates(2, 2)), null);
            Assert.AreEqual(new Coordinates(1, 1).GetDirection(new Coordinates(1, 3)), null);
        }

        [Test]
        public void IsNext()
        {
            Coordinates coor = new Coordinates(2, 3);
            Assert.True(coor.GetInDirection(Direction.Up).IsNext(coor));
            Assert.True(coor.GetInDirection(Direction.Down).IsNext(coor));
            Assert.True(coor.GetInDirection(Direction.Right).IsNext(coor));
            Assert.True(coor.GetInDirection(Direction.Left).IsNext(coor));

            Assert.True(new Coordinates(2, 3).IsNext(new Coordinates(2, 4)));
            Assert.True(new Coordinates(-2, -3).IsNext(new Coordinates(-3, -3)));

            Assert.False(new Coordinates(2, 3).IsNext(new Coordinates(3, 4)));
            Assert.False(new Coordinates(4, 3).IsNext(new Coordinates(2, 3)));
            Assert.False(new Coordinates(1, 7).IsNext(new Coordinates(1, 5)));
        }

        [Test]
        public void IsAdjacent()
        {
            Assert.True(new Coordinates(3, 4).IsAdjacent(new Coordinates(3, 5)));
            Assert.True(new Coordinates(3, 4).IsAdjacent(new Coordinates(3, 3)));
            Assert.True(new Coordinates(3, 4).IsAdjacent(new Coordinates(4, 5)));
            Assert.True(new Coordinates(3, 4).IsAdjacent(new Coordinates(4, 4)));
            Assert.True(new Coordinates(3, 4).IsAdjacent(new Coordinates(4, 3)));
            Assert.True(new Coordinates(3, 4).IsAdjacent(new Coordinates(2, 5)));
            Assert.True(new Coordinates(3, 4).IsAdjacent(new Coordinates(2, 4)));
            Assert.True(new Coordinates(3, 4).IsAdjacent(new Coordinates(2, 3)));

            Assert.False(new Coordinates(3, 4).IsAdjacent(new Coordinates(3, 4)));

            Assert.False(new Coordinates(3, 4).IsAdjacent(new Coordinates(3, 6)));
            Assert.False(new Coordinates(3, 4).IsAdjacent(new Coordinates(1, 4)));
            Assert.False(new Coordinates(3, 4).IsAdjacent(new Coordinates(0, 3)));
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
