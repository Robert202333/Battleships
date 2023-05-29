using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GameModel;

namespace Battleships.Painter
{
    internal enum ShipComponentState
    {
        Debug,
        Hit,
        Sunk
    };
    internal class CanvasCoordCalculator
    {
        private const int margin = 2;
        private const int componentMargin = 1;

        private double canvasWidth = 0;
        private double canvasHeight = 0;
        private uint horizontalSize = 0;
        private uint verticalSize = 0;


        private double squareWidth = 0;
        private double squareHeight = 0;

        private double coordinateWidth = 0;
        private double coordinateHeight = 0;
        private double missDotRadius = 0;

        internal uint HorizontalSize { get { return horizontalSize; } }
        internal uint VerticalSize { get { return verticalSize; } }
        internal double ShipComponentWidth { get { return squareWidth - 2 * componentMargin; } }
        internal double ShipComponentHeight { get { return squareHeight - 2 * componentMargin; } }
        internal double ShipComponentMargin { get { return componentMargin; } }

        internal double MissDotRadius { get { return missDotRadius; } }

        internal double CoordinateSize { get { return Math.Min(coordinateWidth, coordinateHeight) / 2; } }
        internal double Left { get { return margin; } }
        internal double Right { get { return canvasWidth - margin; } }

        internal double Top { get { return margin; } }
        internal double Bottom { get { return canvasHeight - margin; } }


        internal double LeftMap { get { return Left + coordinateWidth; } }
        internal double RightMap { get { return Right - coordinateWidth; } }

        internal double TopMap { get { return Top + coordinateHeight; } }
        internal double BottomMap { get { return Bottom - coordinateHeight; } }


        internal CanvasCoordCalculator()
        {
        }

        internal void OnResizeCanvas(double canvasWidth, double canvasHeight)
        {
            if (canvasWidth < 10 || canvasHeight < 10)
                return;

            this.canvasWidth = canvasWidth;
            this.canvasHeight = canvasHeight;
            UpdateObjectsSize();
        }

        internal void OnResetSettings(uint horizontalSize, uint verticalSize)
        {
            this.horizontalSize = horizontalSize;
            this.verticalSize = verticalSize;
            UpdateObjectsSize();
        }


        internal double GetSquareLeft(uint squareXCoordinate)
        {
            return LeftMap + squareXCoordinate * squareWidth;
        }
        internal double GetSquareTop(uint squareYCoordinate)
        {
            return TopMap + squareYCoordinate * squareHeight;
        }

        internal double GetMissDotCenterX(uint squareXCoordinate)
        {
            return LeftMap + (squareXCoordinate + 0.5) * squareWidth - missDotRadius / 2;
        }
        internal double GetMissDotCenterY(uint squareYCoordinate)
        {
            return TopMap + (squareYCoordinate + 0.5) * squareHeight - missDotRadius / 2;
        }

        internal double GetHorizontalCoordinateCenterX(uint squareXCoordinate)
        {
            return LeftMap + (squareXCoordinate + 0.35) * squareWidth;
        }

        internal double GetHorizontalCoordinateCenterY(bool upper)
        {
            return upper ? Top + coordinateHeight * 0.15 : Bottom - coordinateHeight * 0.85;
        }

        internal double GetVerticalCoordinateCenterX(bool left)
        {
            return left ? Left + coordinateWidth / 3 : Right - coordinateWidth * 0.66;
        }

        internal double GetVerticalCoordinateCenterY(uint squareYCoordinate)
        {
            return TopMap + (squareYCoordinate + 0.15) * squareHeight;
        }


        private void UpdateObjectsSize()
        {
            // Assumed that rect for coordinate display is the same size as for square

            squareWidth = (canvasWidth - 2 * margin) / (horizontalSize + 2); // Coordinatess included
            squareHeight = (canvasHeight - 2 * margin) / (verticalSize + 2); // Coordinates included

            coordinateWidth = squareWidth;
            coordinateHeight = squareHeight;
            missDotRadius = Math.Min(squareWidth, squareHeight) / 2;
        }
    }

    public class BoardPainter : IBoardPainter
    {
        private readonly Canvas canvas;
        private readonly CanvasCoordCalculator ccc = new();

        static private SolidColorBrush CoordinateDescriptionBrush = Brushes.Black;
        static private SolidColorBrush MissedBrush = Brushes.White;
        static private SolidColorBrush LineBrush = Brushes.Black;
        static private SolidColorBrush HitShipComponentBrush = Brushes.Yellow;
        static private SolidColorBrush DebugShipComponentBrush = Brushes.LightGray;
        static private SolidColorBrush SunkShipComponentBrush = Brushes.Red;

        public BoardPainter(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void OnCanvasResize()
        {
            ccc.OnResizeCanvas(canvas.ActualWidth, canvas.ActualHeight);
        }

        public void OnSettingsChange(uint horizontalSize, uint verticalSize)
        {
            ccc.OnResetSettings(horizontalSize, verticalSize);
        }

        public void PaintShotResult(Tuple<Coordinates, ShotResult, ShipComponent?> shotResult, Game game, bool debugMode)
        {
            var (coordinates, result, component) = shotResult;

            if (result == ShotResult.Miss)
            {
                PaintMissDot(coordinates.X, coordinates.Y);
                return;
            }

            if ((result & ShotResult.ShipSunk) != 0)
                PaintSunkShip(component!.Ship);
            else
                PaintShipComponent(component!, ShipComponentState.Hit);
        }

        public void Clear()
        {
            canvas.Children.Clear();
        }

        public void PaintAll(Game game, bool debugMode)
        {
            Clear();

            PaintCoordinatesDescriptions(game.Board);
            PaintLines();
            if (debugMode)
                PaintDebugShipComponents(game.Board);
            PaintHitShipComponents(game.Board);
            PaintMissDots(game.Board);
        }

        private void PaintSunkShip(Ship ship)
        {
            foreach (var component in ship.Components)
                PaintShipComponent(component, ShipComponentState.Sunk);
        }


        private void PaintShipComponent(ShipComponent component, ShipComponentState state)
        {
            PaintShipComponent(component.Coordinates.X, component.Coordinates.Y, GetShipComponentBrush(state));
        }

        private void PaintMissDot(uint xCoor, uint yCoor)
        {
            double x = ccc.GetMissDotCenterX(xCoor);
            double y = ccc.GetMissDotCenterY(yCoor);

            var ellipse = new Ellipse();
            ellipse.Stroke = MissedBrush;
            ellipse.Fill = MissedBrush;
            ellipse.Width = ccc.MissDotRadius;
            ellipse.Height = ccc.MissDotRadius;
            ellipse.VerticalAlignment = VerticalAlignment.Center;
            ellipse.HorizontalAlignment = HorizontalAlignment.Center;
            ellipse.SetValue(Canvas.LeftProperty, x);
            ellipse.SetValue(Canvas.TopProperty, y);
            canvas.Children.Add(ellipse);
        }

        private void PaintCoordinatesDescriptions(Board board)
        {
            var textSize = ccc.CoordinateSize;
            var upperHorizontalCoordinateCenterY = ccc.GetHorizontalCoordinateCenterY(true);
            var lowerHorizontalCoordinateCenterY = ccc.GetHorizontalCoordinateCenterY(false);
            for (ushort i = 0; i < board.HorizontalDescriptor.Size; i++)
            {
                string coordinateDescription = board.HorizontalDescriptor.GetDescription(i);
                PaintText(ccc.GetHorizontalCoordinateCenterX(i), upperHorizontalCoordinateCenterY, textSize, coordinateDescription);
                PaintText(ccc.GetHorizontalCoordinateCenterX(i), lowerHorizontalCoordinateCenterY, textSize, coordinateDescription);
            }

            var leftVerticalCoordinateCenterY = ccc.GetVerticalCoordinateCenterX(true);
            var righttVerticalCoordinateCenterY = ccc.GetVerticalCoordinateCenterX(false);
            for (ushort i = 0; i < board.VerticalDescriptor.Size; i++)
            {
                string coordinateDescription = board.VerticalDescriptor.GetDescription(i);
                PaintText(leftVerticalCoordinateCenterY, ccc.GetVerticalCoordinateCenterY(i), textSize, coordinateDescription);
                PaintText(righttVerticalCoordinateCenterY, ccc.GetVerticalCoordinateCenterY(i), textSize, coordinateDescription);
            }
        }

        private void PaintLines()
        {
            for (uint i = 0; i <= ccc.HorizontalSize; i++)
                PaintLine(ccc.GetSquareLeft(i), ccc.TopMap, ccc.GetSquareLeft(i), ccc.BottomMap);

            for (uint i = 0; i <= ccc.VerticalSize; i++)
                PaintLine(ccc.LeftMap, ccc.GetSquareTop(i), ccc.RightMap, ccc.GetSquareTop(i));
        }

        private void PaintDebugShipComponents(Board board)
        {
            static bool isShipComponent(Square square)
            {
                return square.ShipComponent != null;
            }

            void paintDebugShipComponent(Square square, uint x, uint y)
            {
                PaintShipComponent(square.ShipComponent!, ShipComponentState.Debug);
            }

            board.VisitSquares(paintDebugShipComponent, isShipComponent);
        }

        private SolidColorBrush GetShipComponentBrush(ShipComponentState state)
        {
            return state switch
            {
                ShipComponentState.Debug => DebugShipComponentBrush,
                ShipComponentState.Hit => HitShipComponentBrush,
                ShipComponentState.Sunk => SunkShipComponentBrush,
                _ => throw new NotImplementedException()
            };
        }
        private void PaintHitShipComponents(Board board)
        {
            void paint(Square square, uint x, uint y)
            {
                PaintShipComponent(square.ShipComponent!, square.ShipComponent!.Ship.IsSunk ? ShipComponentState.Sunk : ShipComponentState.Hit);
            };

            static bool hasHitShipComponent(Square square)
            {
                return square.ShipComponent != null && square.ShipComponent.WasHit;
            };

            board.VisitSquares(paint, hasHitShipComponent);
        }

        private void PaintMissDots(Board board)
        {
            static bool hasMissedShot(Square square)
            {
                return square.WasHit && square.ShipComponent == null;
            }

            board.VisitSquares((square, x, y) => PaintMissDot(x, y), hasMissedShot);
        }

        private void PaintLine(double x1, double y1, double x2, double y2)
        {
            var line = new Line();
            line.Stroke = LineBrush;
            line.StrokeThickness = 1;
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            canvas.Children.Add(line);
        }

        private void PaintText(double x, double y, double height, string text)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Width = height * 2;
            textBlock.Height = height * 2;
            textBlock.FontSize = height;
            textBlock.Foreground = CoordinateDescriptionBrush;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.SetValue(Canvas.LeftProperty, x);
            textBlock.SetValue(Canvas.TopProperty, y);
            canvas.Children.Add(textBlock);
        }

        private void PaintShipComponent(uint xCoor, uint yCoor, SolidColorBrush brush)
        {
            double x = ccc.GetSquareLeft(xCoor) + ccc.ShipComponentMargin;
            double y = ccc.GetSquareTop(yCoor) + ccc.ShipComponentMargin;

            var rectangle = new Rectangle();
            rectangle.Stroke = brush;
            rectangle.Fill = brush;
            rectangle.Width = ccc.ShipComponentWidth;
            rectangle.Height = ccc.ShipComponentHeight;

            rectangle.SetValue(Canvas.LeftProperty, x);
            rectangle.SetValue(Canvas.TopProperty, y);

            canvas.Children.Add(rectangle);
        }
    }
}
