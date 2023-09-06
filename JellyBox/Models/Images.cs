using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellyBox.Models
{
    /// <summary>
    /// Base class for groups of images.
    /// </summary>
    public class Images
    {
        public string Primary { get; set; }
        public string Art { get; set; }
        public string Backdrop { get; set; }
        public string Banner { get; set; }
        public string Logo { get; set; }
        public string Thumb { get; set; }
        public string Disc { get; set; }
        public string Box { get; set; }
        public string Screenshot { get; set; }
        public string Menu { get; set; }
        public string Chapter { get; set; }
        public string BoxRear { get; set; }
        public string Profile { get; set; }
    }

    public class ImageBlurHashes: Images { }

    public class ImageUris: Images { }
}
