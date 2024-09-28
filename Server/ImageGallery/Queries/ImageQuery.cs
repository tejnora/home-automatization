using Server.Authentication;
using Server.Core;
using Share;

namespace Server.ImageGallery.Queries
{
    [Session]
    public class ImageQuery : Define.IRequest
    {
        public string ImageName { get; set; }
        public string ImagesGroup { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
