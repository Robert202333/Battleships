using System;

namespace GameModel
{
    public interface IBoardPainter
    {
        void PaintAll(Game game, bool debugMode);        
        void PaintShotResult(Tuple<Square, ShotResult> shotResult, Game game, bool debugMode);
        void Clear();
        void OnSettingsChange(uint horizontalSize, uint verticalSize);
    }
}
