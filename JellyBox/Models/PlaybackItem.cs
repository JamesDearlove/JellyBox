using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellyBox.Models
{
    public class PlaybackItem
    {
        public BaseMediaItem MediaItem { get; set; }
        public bool ResumePlayback { get; set; }

        public PlaybackItem(BaseMediaItem mediaItem, bool resumePlayback)
        {
            MediaItem = mediaItem;
            ResumePlayback = resumePlayback;
        }
    }
}
