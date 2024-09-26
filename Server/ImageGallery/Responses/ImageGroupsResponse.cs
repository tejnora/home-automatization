using System.Collections.Generic;
using Server.Core;

namespace Server.ImageGallery.Responses
{
    public class ImageGroupsResponse : Define.IResponse
    {
        public IList<string> ImagesGroups { get; set; }
    }
}
