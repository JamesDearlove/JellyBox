using Jellyfin.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellyBox.Models
{
    public class Movie : BaseMediaItem
    {
        public Movie(BaseItemDto sdkBaseItem) : base(sdkBaseItem) { }
    }
}
