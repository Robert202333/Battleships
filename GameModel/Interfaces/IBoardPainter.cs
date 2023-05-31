using System;

namespace GameModel
{
    public interface IBoardPainter
    {
        void PaintAll(Board board, bool debugMode);        
        void PaintShotResult(Tuple<Square, ShotResult> shotResult, Board board, bool debugMode);
        void Clear();
        void OnSettingsChange(uint horizontalSize, uint verticalSize);
    }
}
