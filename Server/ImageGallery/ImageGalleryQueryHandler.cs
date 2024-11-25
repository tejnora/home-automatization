using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Serilog;
using Server.Core;
using Server.ImageGallery.Queries;
using Server.ImageGallery.Responses;
using ImageInfo = Server.ImageGallery.Responses.ImageInfo;

namespace Server.ImageGallery;

public class ImageGalleryQueryHandler
    : Define.IQuery<ListOfImageGroupsQuery, ImageGroupsResponse>
        , Define.IQuery<ImagesListQuery, ImagesListResponse>
        , Define.IQuery<ImageQuery, ImageDataResponse>
{
    readonly ServerOptions _options;
    readonly IImagePreviewCache _imageCache;
    readonly Dictionary<string, string> _imagesGroups = new();

    public ImageGalleryQueryHandler(ServerOptions options, IImagePreviewCache imageCache)
    {
        _options = options;
        _imageCache = imageCache;
        try
        {
            _imagesGroups = Directory.GetDirectories(options.ImagesRootDirectory)
                .ToDictionary(v => Path.GetFileName(v), v => v);
        }
        catch (Exception ex)
        {
            Log.Error($"Directory with images can not be iterated. {ex}");
        }
    }

    public ImageGroupsResponse Consume(IQueryContext consumeContext, ListOfImageGroupsQuery request)
    {
        return new ImageGroupsResponse() { ImagesGroups = _imagesGroups.Keys.ToList() };
    }

    public ImagesListResponse Consume(IQueryContext consumeContext, ImagesListQuery request)
    {
        var path = _imagesGroups[request.ImagesGroup];
        var images = Directory.GetFiles(path);
        return new ImagesListResponse
        {
            Images = images.Select((imagePath) =>
            {
                var imageName = Path.GetFileName(imagePath);
                return new ImageInfo() { Name = imageName, Src = HttpUtility.HtmlEncode(imageName) };
            }).ToList()
        };
    }

    public ImageDataResponse Consume(IQueryContext consumeContext, ImageQuery request)
    {
        var path = Path.Combine(_imagesGroups[request.ImagesGroup], HttpUtility.HtmlDecode(request.ImageName));
        try
        {
            var imageData = File.ReadAllBytes(path);
            if (request.Height != -1 && request.Width != -1)
            {
                imageData = _imageCache.ResizeAndGetImage(path, request.Width, request.Height);
            }

            return new ImageDataResponse { Data = imageData, Name = request.ImageName };
        }
        catch (Exception ex)
        {
            Log.Logger.Debug($"File {path} cannot be converted.{ex}");
        }

        return new ImageDataResponse { Data = null, Name = request.ImageName };
    }
}
