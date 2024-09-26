using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;
using Server.Core;
using Server.ImageGallery.Queries;
using Server.ImageGallery.Responses;

namespace Server.ImageGallery;

public class ImageGalleryQueryHandler
    : Define.IQuery<ListOfImageGroupsQuery, ImageGroupsResponse>
    , Define.IQuery<ImagesListQuery, ImagesListResponse>
{
    readonly ServerOptions _options;
    Dictionary<string, string> _imageGroups=new();

    public ImageGalleryQueryHandler(ServerOptions options)
    {
        _options = options;
        try
        {
            _imageGroups = Directory.GetDirectories(options.ImagesRootDirectory)
                .ToDictionary(v => Path.GetFileName(v), v => v);
        }
        catch (Exception ex)
        {
            Log.Error($"Directory with images can not be iterated. {ex}");
        }
    }
    public ImageGroupsResponse Consume(IQueryContext consumeContext, ListOfImageGroupsQuery request)
    {
        return new ImageGroupsResponse(){ImagesGroups = _imageGroups.Keys.ToList() };
    }

    public ImagesListResponse Consume(IQueryContext consumeContext, ImagesListQuery request)
    {
        var path = _imageGroups[request.NameOfGroup];
        var images = Directory.GetFiles(path);
        return new ImagesListResponse{Images = images };
    }
}