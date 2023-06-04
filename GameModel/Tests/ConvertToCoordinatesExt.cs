using NUnit.Framework;

namespace GameModel.Tests
{
    public class ConvertToCoordinatesExt
    {
        [Test]
        public void ConvertToCoordinates()
        {

            var (xCoor, yCoor) = "A1".ConvertToCoordinates()!;
            Assert.AreEqual("A", xCoor);
            Assert.AreEqual("1", yCoor);

            (xCoor, yCoor) = "1A".ConvertToCoordinates()!;
            Assert.AreEqual("1", xCoor);
            Assert.AreEqual("A", yCoor);

            (xCoor, yCoor) = "11A".ConvertToCoordinates()!;
            Assert.AreEqual("11", xCoor);
            Assert.AreEqual("A", yCoor);

            (xCoor, yCoor) = "A11".ConvertToCoordinates()!;
            Assert.AreEqual("A", xCoor);
            Assert.AreEqual("11", yCoor);

            (xCoor, yCoor) = "  A11  ".ConvertToCoordinates()!;
            Assert.AreEqual("A", xCoor);
            Assert.AreEqual("11", yCoor);


            Assert.Null("".ConvertToCoordinates());
            Assert.Null("A".ConvertToCoordinates());
            Assert.Null("AA".ConvertToCoordinates());
            Assert.Null("A 1".ConvertToCoordinates());
            Assert.Null("1".ConvertToCoordinates());
            Assert.Null("11".ConvertToCoordinates());
            Assert.Null("111A".ConvertToCoordinates());
            Assert.Null("A111".ConvertToCoordinates());
            Assert.Null("A1A".ConvertToCoordinates());
            Assert.Null("1AA".ConvertToCoordinates());
            Assert.Null("AA1".ConvertToCoordinates());
            Assert.Null("AA11".ConvertToCoordinates());
            Assert.Null("11AA".ConvertToCoordinates());
        }

    }
}

