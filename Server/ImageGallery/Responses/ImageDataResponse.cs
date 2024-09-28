using Server.Core;

namespace Server.ImageGallery.Responses;

public class ImageDataResponse : Define.IResponse
{
    public byte[] Data { get; set; }
    public string Name { get; set; }
}