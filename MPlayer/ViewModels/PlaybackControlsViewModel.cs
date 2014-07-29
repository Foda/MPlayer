using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using MPlayer.Models;

namespace MPlayer.ViewModels
{
    public class PlaybackControlsViewModel : Screen
    {
        private bool _isVolumeControlVisible = false;
        public bool IsVolumeControlVisible
        {
            get
            {
                return _isVolumeControlVisible;
            }
            set
            {
                _isVolumeControlVisible = value;
                NotifyOfPropertyChange(() => IsVolumeControlVisible);
            }
        }

        public int Volume
        {
            get
            {
                return _userSettingsModel.Volume;
            }
            set
            {
                _userSettingsModel.Volume = value;
                NotifyOfPropertyChange(() => Volume);
            }
        }

        private UserSettingsModel _userSettingsModel;

        public PlaybackControlsViewModel(UserSettingsModel UserSettingsModel)
        {
            _userSettingsModel = UserSettingsModel;
        }

        public void ShowVolumeControl()
        {
            IsVolumeControlVisible = true;
        }

        public void HideVolumeControl()
        {
            IsVolumeControlVisible = false;
        }
    }
}
