using NUnit.Framework;

namespace GameModel.Tests
{
    [TestFixture]
    public class ConvertToCoordinatesExt
    {

        [Test, Sequential]
        public void ConvertToCoordinatesValid(
            [Values("A1", "1A", "A11", "11A", "  A11  ")] string coordText,
            [Values("A",  "1",  "A",   "11",  "A")] string expectedXCoor,
            [Values("1",  "A",  "11",  "A",  "11")] string expectedYCoor)
        {
            var (xCoor, yCoor) = coordText.ConvertToCoordinates()!;
            Assert.AreEqual(expectedXCoor, xCoor);
            Assert.AreEqual(expectedYCoor, yCoor);

        }


        [Test]
        public void ConvertToCoordinatesInvalid([Values("", "A", "AA", "A 1", "1", "11", "111A", "A111", 
                                                        "A1A", "1AA", "AA1", "AA11", "11AA")] string coord)
        {
            Assert.Null(coord.ConvertToCoordinates());
        }
    }
}

