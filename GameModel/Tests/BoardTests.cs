using NUnit.Framework;

namespace GameModel.Tests
{
    [TestFixture]
    public class CoordinateDescriptorTests
    {
        CoordinateDescriptor descriptor = new CoordinateDescriptor(new string[] { "A", "B", "C", "D" });

        [Test]
        public void GetDescription()
        {
            Assert.AreEqual(descriptor.GetDescription(0), "A");
            Assert.AreEqual(descriptor.GetDescription(2), "C");
            Assert.AreNotEqual(descriptor.GetDescription(3), "G");
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = descriptor.GetDescription(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = descriptor.GetDescription(4));
        }

        [Test]
        public void GetCoordinate()
        {
            Assert.AreEqual(descriptor.GetCoordinate("A"), 0);
            Assert.AreEqual(descriptor.GetCoordinate("D"), 3);
            Assert.AreNotEqual(descriptor.GetCoordinate("C"), 1);
            Assert.Throws<InvalidCoordinatesDescriptionException>(() => _ = descriptor.GetCoordinate("J"));
            Assert.Throws<InvalidCoordinatesDescriptionException>(() => _ = descriptor.GetCoordinate("xx"));
        }

        [Test]
        public void Size()
        {
            Assert.AreEqual(descriptor.Size, 4);
        }

    }


    [TestFixture]
    public class BoardTests
    {
        private Board board = CreateBoard();

        [TestCase(1, 3)]
        [TestCase(2, 1)]
        [TestCase(0, 3)]
        [TestCase(0, 0)]
        [TestCase(4, 3)]
        public void GetSquare(int x, int y)
        {
            Square square = board.GetSquare(x, y);
            Assert.AreEqual(x, square.Coordinates.X);
            Assert.AreEqual(y, square.Coordinates.Y);
        }


        [TestCase(5, 3)]
        [TestCase(1, -1)]
        [TestCase(4, 4)]
        public void GetSquareForInvalidCoordinates(int x, int y)
        {
            Assert.Throws<CoordinateOutOfBoardException>(() => _ = board.GetSquare(x, y));
        }

        [TestCase("A", "2", 0, 1)]
        [TestCase("C", "1", 2, 0)]
        public void ConvertToCoordinates(string xDescr, string yDescr, int expectedX, int expectedY)
        {
            Assert.AreEqual(new Coordinates(expectedX, expectedY), board.ConvertToCoordinates(xDescr, yDescr));
        }

        [TestCase("X", "1")]
        [TestCase("1", "C")]
        public void ConvertToCoordinatesInvalid(string xDescr, string yDescr)
        {
            Assert.Throws<InvalidCoordinatesDescriptionException>(() => _ = board.ConvertToCoordinates(xDescr, yDescr));
        }


        [Test]
        public void VisitSquares()
        {
            int visitedFieldCount = 0;
            board.VisitSquares((square, x, y) => { visitedFieldCount++; });
            Assert.AreEqual(20, visitedFieldCount);
        }

        [Test]
        public void VisitSquaresWithPredicate()
        {
            int visitedFieldCount = 0;
            const int coordinateX = 3;

            void counter(Square square, int x, int y)
            {
                Assert.AreEqual(coordinateX, square.Coordinates.X);
                visitedFieldCount++;
            }

            static bool onlyHorizontalX(Square square)
            {
                return square.Coordinates.X == coordinateX;
            }

            board.VisitSquares(counter, onlyHorizontalX);
            Assert.AreEqual(4, visitedFieldCount);
        }


        static private Board CreateBoard()
        {
            CoordinateDescriptor horizontalDescriptor = new CoordinateDescriptor(new string[] { "A", "B", "C", "D", "E" });
            CoordinateDescriptor verticalDescriptor = new CoordinateDescriptor(new string[] { "1", "2", "3", "4" });
            return new Board(horizontalDescriptor, verticalDescriptor);
        }

    }
}
