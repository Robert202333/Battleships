using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Battleships.GameModel;

namespace Battleships.GameModel
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

        private double indexWidth = 0;
        private double indexHeight = 0;
        private double missDotRadius = 0;

        internal uint HorizontalSize { get { return horizontalSize; } }
        internal uint VerticalSize { get { return verticalSize; } }
        internal double ShipComponentWidth { get { return squareWidth - 2 * componentMargin; } }
        internal double ShipComponentHeight { get { return squareHeight - 2 * componentMargin; } }
        internal double ShipComponentMargin { get { return componentMargin; } }

        internal double MissDotRadius { get { return missDotRadius; } }

        internal double CoordinateSize {  get { return Math.Min(indexWidth, indexHeight) / 2; } }
        internal double Left { get { return margin; } }
        internal double Right { get { return canvasWidth - margin; } }

        internal double Top { get { return margin; } }
        internal double Bottom { get { return canvasHeight - margin; } }


        internal double LeftMap {  get { return Left + indexWidth; } }
        internal double RightMap { get { return Right - indexWidth; } }

        internal double TopMap { get { return Top + indexHeight; } }
        internal double BottomMap { get { return Bottom - indexHeight; } }


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

        internal double GetHorizontalIndexCenterX(uint squareXCoordinate)
        {
            return LeftMap + (squareXCoordinate + 0.35) * squareWidth;
        }

        internal double GetHorizontalIndexCenterY(bool upper)
        {
            return upper ? Top + indexHeight * 0.15: Bottom - indexHeight * 0.85;
        }

        internal double GetVerticalIndexCenterX(bool left)
        {
            return left ? Left + indexWidth / 3 : Right - indexWidth * 0.66;
        }

        internal double GetVerticalIndexCenterY(uint squareYCoordinate)
        {
            return TopMap + (squareYCoordinate + 0.15) * squareHeight;
        }


        private void UpdateObjectsSize()
        {
            // Assumed that rect for coordinate display is the same size as for square

            squareWidth = (canvasWidth - 2 * margin) / (horizontalSize + 2); // Indexes included
            squareHeight = (canvasHeight - 2 * margin) / (verticalSize + 2); // Indexes included

            indexWidth = squareWidth;
            indexHeight = squareHeight;
            missDotRadius = Math.Min(squareWidth, squareHeight) / 2;
        }
    }

    public class BoardPainter
    {
        private readonly Canvas canvas;
        private readonly CanvasCoordCalculator ccc = new();

        static private SolidColorBrush IndexDescriptionBrush = Brushes.Black;
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

        internal void Clear()
        {
            canvas.Children.Clear();
        }

        internal void PaintAll(Board board, bool debugMode)
        {
            Clear();

            PaintCoordinatesDescriptions(board);
            PaintLines();
            if (debugMode)
                PaintDebugShipComponents(board);
            PaintHitShipComponents(board);
            PaintMissDots(board);
        }

        internal void PaintSunkShip(Ship ship)
        {
            ship.Components.Select(component => component.Coordinates).ToList().ForEach(coordinates =>
            {
                PaintShipComponent(coordinates.X, coordinates.Y, ShipComponentState.Sunk);
            });
        }


        internal void PaintShipComponent(uint x, uint y, ShipComponentState state)
        {
            PaintShipComponent(x, y, GetShipComponentBrush(state));
        }

        internal void PaintMissDot(uint xCoor, uint yCoor)
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
            var upperHorizontalIndexCenterY = ccc.GetHorizontalIndexCenterY(true);
            var lowerHorizontalIndexCenterY = ccc.GetHorizontalIndexCenterY(false);
            for (ushort  i = 0; i < board.HorizontalDescriptor.Size; i++)
            {
                string indexDescription = board.HorizontalDescriptor.GetDescription(i);
                PaintText(ccc.GetHorizontalIndexCenterX(i), upperHorizontalIndexCenterY, textSize, indexDescription);
                PaintText(ccc.GetHorizontalIndexCenterX(i), lowerHorizontalIndexCenterY, textSize, indexDescription);
            }

            var leftVerticalIndexCenterY = ccc.GetVerticalIndexCenterX(true);
            var righttVerticalIndexCenterY = ccc.GetVerticalIndexCenterX(false);
            for (ushort  i = 0; i < board.VerticalDescriptior.Size; i++)
            {
                string indexDescription = board.VerticalDescriptior.GetDescription(i);
                PaintText(leftVerticalIndexCenterY, ccc.GetVerticalIndexCenterY(i), textSize, indexDescription);
                PaintText(righttVerticalIndexCenterY, ccc.GetVerticalIndexCenterY(i), textSize, indexDescription);
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
            board.VisitSquares((square, x, y) =>
            {
                if (square.ShipComponenrt != null)
                    PaintShipComponent(x, y, ShipComponentState.Debug);
            });
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
            var paint = (Square square, uint x, uint y) =>
            {
                PaintShipComponent(x, y, square.ShipComponenrt!.Ship.IsSunk ? ShipComponentState.Sunk : ShipComponentState.Hit);
            };

            Predicate<Square> predicate = (Square square) =>
            {
                return (square.ShipComponenrt != null && square.ShipComponenrt.WasHit);
            };

            board.VisitSquares(paint, predicate);
        }

        private void PaintMissDots(Board board)
        {
            Predicate<Square> missedShotSquarePredicate = (Square square) =>
            {
                return square.WasHit && square.ShipComponenrt == null;
            };

            board.VisitSquares((square, x, y) => PaintMissDot(x, y), missedShotSquarePredicate);
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
            textBlock.Foreground = IndexDescriptionBrush;
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
