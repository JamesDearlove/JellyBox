using Jellyfin.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellyBox.Models
{
    public class TvShowSeason : BaseMediaItem
    {
        public string SeasonName { get; set; }
        public int Season { get; set; }
        public Guid? Series { get; set; }
        public string SeriesName { get; set; }

        //public override string SubtitleText
        //{
        //    get
        //    {
        //        return SeriesName;
        //    }
        //}

        public TvShowSeason(BaseItemDto sdkBaseItem) : base(sdkBaseItem)
        {
            SeasonName = sdkBaseItem.SeasonName;
            //Released = sdkBaseItem.AirTime;
            Series = sdkBaseItem.SeriesId;
            SeriesName = sdkBaseItem.SeriesName;
        }
    }
}
