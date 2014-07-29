using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPlayer.Models
{
    public class AlbumModel
    {
        public string Title { get; set; }

        public List<SongModel> Songs { get; set; }

        public string Artist { get; set; }

        public string ArtworkUrl { get; set; }
    }
}
