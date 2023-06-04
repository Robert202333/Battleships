using NUnit.Framework;

namespace GameModel.Tests
{
    [TestFixture]
    public class ShipTests
    {
        [Test]
        public void WasSunk()
        {
            Ship ship = new Ship("Destroyer");
            ShipComponent[] shipComponents =
            {
                new ShipComponent(ship, new Square(new Coordinates(2, 2))),
                new ShipComponent(ship, new Square(new Coordinates(2, 3))),
                new ShipComponent(ship, new Square(new Coordinates(2, 4))),
            };

            ship.AddComponents(shipComponents);
            Assert.False(ship.WasSunk);

            shipComponents[0].WasHit = true;
            Assert.False(ship.WasSunk);

            shipComponents[1].WasHit = true;
            Assert.False(ship.WasSunk);

            shipComponents[2].WasHit = true;
            Assert.True(ship.WasSunk);
        }

    }
}
