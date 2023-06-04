using NUnit.Framework;

namespace GameModel.Tests
{

    internal class TestMessageDisplayer : IMessageDisplayer
    {
        internal string ExpectedType { get; set; } = "";
        internal int ShowErrorCalls { get; set; }
        internal int ShowWarningCalls { get; set; }
        internal int ShowInformationCalls { get; set; }

        public void ShowError(string type, string message)
        {
            ShowErrorCalls++;
            if (!string.IsNullOrEmpty(ExpectedType))
                Assert.AreEqual(type, ExpectedType, "Message topic mismatch for Error");
        }

        public void ShowInformation(string type, string message)
        {
            ShowInformationCalls++;
            if (!string.IsNullOrEmpty(ExpectedType))
                Assert.AreEqual(type, ExpectedType, "Message topic mismatch for Information");
        }

        public void ShowWarning(string type, string message)
        {
            ShowWarningCalls++;
            if (!string.IsNullOrEmpty(ExpectedType))
                Assert.AreEqual(type, ExpectedType, "Message topic mismatch for Warning");
        }
    }

    internal class TestBoardPainter : IBoardPainter
    {
        public int ClearCalls { get; set; }
        public int OnSettingsChangeCalls { get; set; }
        public int PaintAllCalls { get; set; }
        public int PaintShotResultCalls { get; set; }

        public void Clear()
        {
            ClearCalls++;
        }

        public void OnSettingsChange(int horizontalSize, int verticalSize)
        {
            OnSettingsChangeCalls++;
        }

        public void PaintAll(Board board, bool debugMode)
        {
            PaintAllCalls++;
        }

        public void PaintShotResult(Tuple<Square, ShotResult> shotResult, Board board, bool debugMode)
        {
            PaintShotResultCalls++;
        }
    }



    [TestFixture]
    internal class GameEnvTests
    {
        [Test]
        public void MessagesAndPainterTest()
        {
            // Standard game on 10x10 board, with 1 ship.
            var messageDispalyer = new TestMessageDisplayer();
            var boardPainter = new TestBoardPainter();
            var gameEnv = new GameEnv(new TestGameCreator(), messageDispalyer);
            gameEnv.Painter = boardPainter;


            Assert.True(gameEnv.Restart());
            Assert.AreEqual(1, boardPainter.PaintAllCalls);

            messageDispalyer.ExpectedType = MessageType.ShotError;
            messageDispalyer.ShowWarningCalls = 0;

            Assert.False(gameEnv.ProcessShot("Z", "1"));
            Assert.AreEqual(1, messageDispalyer.ShowWarningCalls);
            Assert.AreEqual(0, boardPainter.PaintShotResultCalls);

            Assert.False(gameEnv.ProcessShot("A", "11"));
            Assert.AreEqual(2, messageDispalyer.ShowWarningCalls);
            Assert.AreEqual(0, boardPainter.PaintShotResultCalls);

            Assert.False(gameEnv.ProcessShot("A", "1"));
            Assert.AreEqual(2, messageDispalyer.ShowWarningCalls);
            Assert.AreEqual(1, boardPainter.PaintShotResultCalls);

            messageDispalyer.ExpectedType = MessageType.ShotResult;
            messageDispalyer.ShowInformationCalls = 0;

            Assert.False(gameEnv.ProcessShot("B", "2"));
            Assert.AreEqual(1, messageDispalyer.ShowInformationCalls);
            Assert.AreEqual(2, boardPainter.PaintShotResultCalls);

            Assert.False(gameEnv.ProcessShot("B", "3"));
            Assert.AreEqual(2, messageDispalyer.ShowInformationCalls);
            Assert.AreEqual(3, boardPainter.PaintShotResultCalls);

            messageDispalyer.ExpectedType = MessageType.ShotError;
            Assert.False(gameEnv.ProcessShot("B", "2"));
            Assert.AreEqual(2, messageDispalyer.ShowInformationCalls);
            Assert.AreEqual(3, messageDispalyer.ShowWarningCalls);
            Assert.AreEqual(3, boardPainter.PaintShotResultCalls);

            messageDispalyer.ExpectedType = MessageType.ShotResult;
            Assert.True(gameEnv.ProcessShot("B", "4"));
            Assert.AreEqual(3, messageDispalyer.ShowInformationCalls);
            Assert.AreEqual(4, boardPainter.PaintShotResultCalls);
        }

    }
}
