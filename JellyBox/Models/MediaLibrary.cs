using Jellyfin.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellyBox.Models
{
    public class MediaLibrary : BaseItem
    {
        public string CollectionType { get; set; }

        public MediaLibrary(BaseItemDto sdkBaseItem) : base(sdkBaseItem)
        { 
            CollectionType = sdkBaseItem.CollectionType;
        }
    }
}
