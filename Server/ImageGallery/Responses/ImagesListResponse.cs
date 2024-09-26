using Server.Core;
using System.Collections.Generic;

namespace Server.ImageGallery.Responses
{
    public class ImagesListResponse : Define.IResponse
    {
        public IList<string> Images { get; set; }
    }
}
