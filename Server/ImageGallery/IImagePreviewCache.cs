namespace Server.ImageGallery
{
    public interface IImagePreviewCache
    {
        byte[] ResizeAndGetImage(string imagePath, int width, int height);
    }
}
