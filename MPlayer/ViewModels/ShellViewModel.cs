using Caliburn.Micro;
using MPlayer.Interfaces;

namespace MPlayer.ViewModels
{
    public class ShellViewModel : Conductor<IMainScreen>.Collection.OneActive, IShell 
    {
        private BindableCollection<IMainScreen> _items;
        public BindableCollection<IMainScreen> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                NotifyOfPropertyChange(() => Items);
            }
        }

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
            Items = new BindableCollection<IMainScreen>();
            Items.Add(new LibraryViewModel());
            Items.Add(new NowPlayingViewModel());
            Items.Add(new OptionsViewModel());

            MainScreen = Items[0];
        }
    }
}