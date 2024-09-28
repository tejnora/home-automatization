using Server.Core;
using System.Collections.Generic;

namespace Server.ImageGallery.Responses
{
    public class ImageInfo
    {
        public string Name { get; set; }
        public string Src { get; set; }
    }
    public class ImagesListResponse : Define.IResponse
    {
        public IList<ImageInfo> Images { get; set; }
    }
}
