using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using MPlayer.Interfaces;

namespace MPlayer.ViewModels
{
    public class OptionsViewModel : Screen, IMainScreen
    {
        public MainScreens Order
        {
            get { return MainScreens.Options; }
        }

        public string DisplayName
        {
            get
            {
                return "OPTIONS";
            }
        }
    }
}
