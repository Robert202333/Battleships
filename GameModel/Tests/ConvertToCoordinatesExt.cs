using System;
using NUnit.Framework;

namespace GameModel.Tests
{
    public class ConvertToCoordinatesExt
    {
        [Test]
        public void ConvertToCoordinates()
        {

            var (xCoor, yCoor) = "A1".ConvertToCoordinates();
            Assert.AreEqual("A", xCoor);
            Assert.AreEqual("1", yCoor);

            (xCoor, yCoor) = "1A".ConvertToCoordinates();
            Assert.AreEqual("1", xCoor);
            Assert.AreEqual("A", yCoor);

            (xCoor, yCoor) = "11A".ConvertToCoordinates();
            Assert.AreEqual("11", xCoor);
            Assert.AreEqual("A", yCoor);

            (xCoor, yCoor) = "A11".ConvertToCoordinates();
            Assert.AreEqual("A", xCoor);
            Assert.AreEqual("11", yCoor);

            (xCoor, yCoor) = "  A11  ".ConvertToCoordinates();
            Assert.AreEqual("A", xCoor);
            Assert.AreEqual("11", yCoor);


            Assert.Throws<InvalidDescriptionException>(() => _ = "".ConvertToCoordinates());
            Assert.Throws<InvalidDescriptionException>(() => _ = "A".ConvertToCoordinates());
            Assert.Throws<InvalidDescriptionException>(() => _ = "AA".ConvertToCoordinates());
            Assert.Throws<InvalidDescriptionException>(() => _ = "A 1".ConvertToCoordinates());
            Assert.Throws<InvalidDescriptionException>(() => _ = "1".ConvertToCoordinates());
            Assert.Throws<InvalidDescriptionException>(() => _ = "11".ConvertToCoordinates());
            Assert.Throws<InvalidDescriptionException>(() => _ = "111A".ConvertToCoordinates());
            Assert.Throws<InvalidDescriptionException>(() => _ = "A111".ConvertToCoordinates());
            Assert.Throws<InvalidDescriptionException>(() => _ = "A1A".ConvertToCoordinates());
            Assert.Throws<InvalidDescriptionException>(() => _ = "1AA".ConvertToCoordinates());
            Assert.Throws<InvalidDescriptionException>(() => _ = "AA1".ConvertToCoordinates());
            Assert.Throws<InvalidDescriptionException>(() => _ = "AA11".ConvertToCoordinates());
            Assert.Throws<InvalidDescriptionException>(() => _ = "11AA".ConvertToCoordinates());
        }

    }
}

