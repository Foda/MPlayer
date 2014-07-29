using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPlayer.Interfaces
{
    public interface IMainScreen
    {
        MainScreens Order { get; }
    }

    public enum MainScreens
    {
        Library = 0,
        NowPlaying = 1,
        Options = 2
    }
}
