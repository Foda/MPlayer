using Caliburn.Micro;
using MPlayer.Interfaces;

namespace MPlayer.ViewModels
{
    public class ShellViewModel : Conductor<IMainScreen>.Collection.OneActive, IShell 
    {
        public LibraryViewModel LibraryViewModel { get; set; }
        public NowPlayingViewModel NowPlayingViewModel { get; set; }
        public OptionsViewModel OptionsViewModel { get; set; }

        IMainScreen _screen;
        public IMainScreen MainScreen
        {
            get { return _screen; }
            set
            {
                _screen = value;
                NotifyOfPropertyChange(() => MainScreen);
            }
        }

        public ShellViewModel()
        {
            LibraryViewModel = new LibraryViewModel();
            NowPlayingViewModel = new NowPlayingViewModel();
            OptionsViewModel = new OptionsViewModel();

            MainScreen = NowPlayingViewModel;
        }
    }
}