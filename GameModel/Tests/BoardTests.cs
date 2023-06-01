using NUnit.Framework;

namespace GameModel.Tests
{
    [TestFixture]
    public class CoordinateDescriptorTests
    {
        [Test]
        public void GetDescription()
        {
            CoordinateDescriptor descriptor = new CoordinateDescriptor(new string[] { "A", "B", "C", "D" });
            Assert.AreEqual(descriptor.GetDescription(0), "A");
            Assert.AreEqual(descriptor.GetDescription(2), "C");
            Assert.AreNotEqual(descriptor.GetDescription(3), "G");
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = descriptor.GetDescription(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = descriptor.GetDescription(4));
        }

        [Test]
        public void GetCoordinate()
        {
            CoordinateDescriptor descriptor = new CoordinateDescriptor(new string[] { "A", "B", "C", "D" });
            Assert.AreEqual(descriptor.GetCoordinate("A"), 0);
            Assert.AreEqual(descriptor.GetCoordinate("D"), 3);
            Assert.AreNotEqual(descriptor.GetCoordinate("C"), 1);
            Assert.Throws<InvalidCoordinatesDescriptionException>(() => _ = descriptor.GetCoordinate("J"));
            Assert.Throws<InvalidCoordinatesDescriptionException>(() => _ = descriptor.GetCoordinate("xx"));
        }

        [Test]
        public void Size()
        {
            CoordinateDescriptor descriptor = new CoordinateDescriptor(new string[] { "A", "B", "C", "D" });
            Assert.AreEqual(descriptor.Size, 4);
        }

    }


    [TestFixture]
    public class BoardTests
    {
        private Board CreateBoard()
        {
            CoordinateDescriptor horizontalDescriptor = new CoordinateDescriptor(new string[] { "A", "B", "C", "D", "E" });
            CoordinateDescriptor verticalDescriptor = new CoordinateDescriptor(new string[] { "1", "2", "3", "4" });
            return new Board(horizontalDescriptor, verticalDescriptor);
        }

        [Test]
        public void GetSquare()
        {
            Board board = CreateBoard();

            Square square1 = board.GetSquare(1, 3);
            Assert.AreEqual(1, square1.Coordinates.X);
            Assert.AreEqual(3, square1.Coordinates.Y);

            Square square2 = board.GetSquare(2, 1);
            Assert.AreEqual(2, square2.Coordinates.X);
            Assert.AreEqual(1, square2.Coordinates.Y);

            Square square3 = board.GetSquare(0, 3);
            Assert.AreEqual(0, square3.Coordinates.X);
            Assert.AreNotEqual(1, square3.Coordinates.Y);

            Square square4 = board.GetSquare(0, 0);
            Assert.AreEqual(0, square4.Coordinates.X);
            Assert.AreEqual(0, square4.Coordinates.Y);

            Square square5 = board.GetSquare(board.HorizontalDescriptor.Size - 1, board.VerticalDescriptor.Size - 1);
            Assert.AreEqual(board.HorizontalDescriptor.Size - 1, square5.Coordinates.X);
            Assert.AreEqual(board.VerticalDescriptor.Size - 1, square5.Coordinates.Y);


            Assert.Throws<CoordinateOutOfBoardException>(() => _ = board.GetSquare(5, 3));
            Assert.Throws<CoordinateOutOfBoardException>(() => _ = board.GetSquare(1, -1));
            Assert.Throws<CoordinateOutOfBoardException>(() => _ = board.GetSquare(4, 4));
        }

        [Test]
        public void ConvertToCoordinates()
        {
            Board board = CreateBoard();

            Assert.AreEqual(new Coordinates(0, 1), board.ConvertToCoordinates("A", "2"));
            Assert.AreEqual(new Coordinates(2, 0), board.ConvertToCoordinates("C", "1"));

            Assert.Throws<InvalidCoordinatesDescriptionException>(() => _ = board.ConvertToCoordinates("1", "C"));
            Assert.Throws<InvalidCoordinatesDescriptionException>(() => _ = board.ConvertToCoordinates("X", "1"));
        }

        [Test]
        public void VisitSquares()
        {
            Board board = CreateBoard();

            int visitedFieldCount = 0;
            board.VisitSquares((square, x, y) => { visitedFieldCount++; });
            Assert.AreEqual(20, visitedFieldCount);


            visitedFieldCount = 0;
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

    }
}
