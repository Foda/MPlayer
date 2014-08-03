using Caliburn.Micro;
using MPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPlayer.ViewModels
{
    public class PlayQueueViewModel : Screen
    {
        private IObservableCollection<SongModel> _queueSongs;
        public IObservableCollection<SongModel> QueueSongs
        {
            get
            {
                return _queueSongs;
            }
            set
            {
                _queueSongs = value;
                NotifyOfPropertyChange(() => QueueSongs);
            }
        }

        public PlayQueueViewModel()
        {
            QueueSongs = new BindableCollection<SongModel>();
            
            /*QueueSongs.Add(new SongModel()
            {
                Album = "TestAlbum",
                Title = "TestTitle"
            });

            QueueSongs.Add(new SongModel()
            {
                Album = "TestAlbum2",
                Title = "TestTitle2"
            });

            QueueSongs.Add(new SongModel()
            {
                Album = "TestAlbum3",
                Title = "TestTitle3"
            });*/
        }
    }
}
