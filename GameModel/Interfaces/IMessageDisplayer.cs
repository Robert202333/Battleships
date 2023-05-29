using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModel
{
    public interface IMessageDisplayer
    {
        void ShowWarning(string type, string message);
        void ShowError(string type, string message);
        void ShowInformation(string type, string message);
    }
}
