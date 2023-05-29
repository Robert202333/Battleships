using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModel
{
    public interface IGameCreator
    {
        Game Create(Settings settings);
    }
}
