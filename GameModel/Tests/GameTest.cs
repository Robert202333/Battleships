using NUnit.Framework;

namespace GameModel.Tests
{
    // Ignores ship creation configuration
    internal class TestGameCreator : AbstractGameCreator
    {
        protected override IEnumerable<Ship> CreateShips(Board board, Settings settings)
        {
            CoordinatesChain coordinatesChain = new CoordinatesChain();
            coordinatesChain.Add(new Coordinates(1, 1));
            coordinatesChain.Add(new Coordinates(1, 2));
            coordinatesChain.Add(new Coordinates(1, 3));

            List<Ship> ships = new List<Ship>()
            {
                CreateShip("Sumbarine", coordinatesChain.Chain, board)
            };

            return ships;
        }
    }
    internal class GameTest
    {
        Game CraeteGame()
        {
            Settings settings = new Settings();
            settings.VerticalSize = 10;
            settings.HorizontalSize = 5;

            var creator = new TestGameCreator();
            return creator.Execute(settings);
        }

        [Test]
        public void ProcessShot()
        {
            Game game = CraeteGame();

            Assert.Throws<InvalidCoordinatesDescriptionException>(() => _ = game.ProcessShot("Z", "1"));
            Assert.Throws<InvalidCoordinatesDescriptionException>(() => _ = game.ProcessShot("A", "11"));

            var (square, result) = game.ProcessShot("A", "1");
            Assert.AreEqual(result, ShotResult.Miss);
            Assert.Null(square.ShipComponent);
            Assert.True(square.WasHit);

            (square, result) = game.ProcessShot("A", "1");
            Assert.AreEqual(result, ShotResult.Repeated);


            (square, result) = game.ProcessShot("B", "2");
            Assert.AreEqual(result, ShotResult.Hit);
            Assert.NotNull(square.ShipComponent);
            Assert.True(square.ShipComponent?.WasHit ?? false);
            Assert.False(square.ShipComponent?.Ship.WasSunk ?? true);

            (square, result) = game.ProcessShot("B", "3");
            Assert.AreEqual(result, ShotResult.Hit);
            Assert.NotNull(square.ShipComponent);
            Assert.True(square.ShipComponent?.WasHit ?? false);
            Assert.False(square.ShipComponent?.Ship.WasSunk ?? true);

            (square, result) = game.ProcessShot("B", "2");
            Assert.AreEqual(result, ShotResult.Repeated);

            (square, result) = game.ProcessShot("B", "4");
            Assert.True((result & ShotResult.Hit) != 0);
            Assert.NotNull(square.ShipComponent);
            Assert.True(square.ShipComponent?.WasHit ?? false);

            Assert.True((result & ShotResult.ShipSunk) != 0);
            Assert.True(square.ShipComponent?.Ship.WasSunk ?? false);

            Assert.True((result & ShotResult.GameEnd) != 0);
        }
    }
}
