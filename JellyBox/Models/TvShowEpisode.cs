using Jellyfin.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace JellyBox.Models
{
    public class TvShowEpisode : BaseMediaItem
    {
        public string SeasonName { get; set; }
        public DateTime Released { get; set; }

        // TODO: Maybe needs it's own model.
        public string[] Genres { get; set; }

        public Guid? Season { get; set; }
        public Guid? Series { get; set; }
        public string SeriesName { get; set; }

        public int? SeasonNumber { get; set; }
        public int? EpisodeNumber { get; set; }

        public override string PlaybackTitle => SeriesName;
        // TODO: Get season and ep number in.
        public override string PlaybackSubtitle
        {
            get
            {
                if (SeasonNumber != null && EpisodeNumber != null)
                {
                    return $"S{SeasonNumber}:E{EpisodeNumber} - {Name}";
                }
                else
                {
                    return Name;
                }
            }

        }

        public TvShowEpisode(BaseItemDto sdkBaseItem) : base(sdkBaseItem)
        {
            SeasonName = sdkBaseItem.SeasonName;
            //Released = sdkBaseItem.AirTime;
            Season = sdkBaseItem.SeasonId;
            Series = sdkBaseItem.SeriesId;
            SeriesName = sdkBaseItem.SeriesName;
            SeasonNumber = sdkBaseItem.ParentIndexNumber;
            EpisodeNumber = sdkBaseItem.IndexNumber;
        }
    }
}
