using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using MPlayer.Interfaces;
using MPlayer.Models;

namespace MPlayer.ViewModels
{
    public class NowPlayingViewModel : Screen, IMainScreen
    {
        public MainScreens Order
        {
            get { return MainScreens.NowPlaying; }
        }

        public string DisplayName
        {
            get
            {
                return "NOW PLAYING";
            }
        }

        public PlaybackControlsViewModel PlaybackControlsViewModel { get; set; }
        public PlayQueueViewModel PlayQueueViewModel { get; set; }


        public NowPlayingViewModel()
        {
            PlaybackControlsViewModel = new PlaybackControlsViewModel(new UserSettingsModel());
            PlayQueueViewModel = new PlayQueueViewModel();
        }
    }
}
