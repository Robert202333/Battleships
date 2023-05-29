using System;

namespace GameModel
{
    public interface IBoardPainter
    {
        void OnSettingsChange(uint horizontalSize, uint verticalSize);
        void PaintShotResult(Tuple<Coordinates, ShotResult, ShipComponent?> shotResult, Game game, bool debugMode);
        void Clear();
        void PaintAll(Game game, bool debugMode);
    }
}
