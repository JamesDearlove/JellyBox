using Jellyfin.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellyBox.Models
{
    public class TvShowSeries : BaseMediaItem
    {
        // Not currently required.
        public string Studio { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public override string DisplayYear
        {
            get
            {
                if (PremiereDate != null)
                {
                    var endingString = EndDate == null ? "Present" : EndDate.Value.Year.ToString();
                    return $"{PremiereDate.Value.Year} - {endingString}";
                }
                else
                {
                    return null;
                }
            }
        }

        public TvShowSeries(BaseItemDto sdkBaseItem) : base(sdkBaseItem)
        {
            EndDate = sdkBaseItem.EndDate;
            //Studio = sdkBaseItem.Studios
        }
    }
}
