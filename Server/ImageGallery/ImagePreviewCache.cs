using BTDB.Collections;
using Serilog;
using System.IO;
using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;

namespace Server.ImageGallery
{
    public class ImagePreviewCache
    : IImagePreviewCache
    {
        readonly LruCache<(string path, int width, int height), byte[]> _previewCache = new(200);
        readonly object _lock = new object();

        public byte[] ResizeAndGetImage(string path, int width, int height)
        {
            lock (_lock)
            {
                if (_previewCache.TryGetValue((path, width, height), out var res))
                {
                    return res;
                }
            }

            try
            {
                var imageData = File.ReadAllBytes(path);
                imageData = ResizeImage(imageData, width, height);
                lock (_lock)
                {
                    _previewCache[(path, width, height)] = imageData;
                }
                return imageData;
            }
            catch (Exception ex)
            {
                Log.Logger.Debug($"File {path} cannot be converted.{ex}");
                return null;
            }
        }

        static byte[] ResizeImage(byte[] data, int width, int height)
        {

            using var inputStream = new MemoryStream(data);
            using var image = Image.Load(inputStream);
            image.Mutate(x => x.Resize(width, 0, KnownResamplers.Lanczos3));
            using var outputStream=new MemoryStream();
            image.Save(outputStream, new PngEncoder());
            return outputStream.GetBuffer();
        }


    }
}
