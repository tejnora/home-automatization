﻿using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using Serilog;
using Server.Core;
using Server.ImageGallery.Queries;
using Server.ImageGallery.Responses;

namespace Server.ImageGallery;

public class ImageGalleryQueryHandler
    : Define.IQuery<ListOfImageGroupsQuery, ImageGroupsResponse>
    , Define.IQuery<ImagesListQuery, ImagesListResponse>
    , Define.IQuery<ImageQuery, ImageDataResponse>
{
    readonly ServerOptions _options;
    Dictionary<string, string> _imagesGroups = new();

    public ImageGalleryQueryHandler(ServerOptions options)
    {
        _options = options;
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
                imageData = ResizeImage(File.ReadAllBytes(path), request.Width, request.Height);
            }
            return new ImageDataResponse { Data = imageData, Name = request.ImageName };
        }
        catch (Exception ex)
        {
            Log.Logger.Debug($"File {path} cannot be converted.{ex}");
        }
        return new ImageDataResponse { Data = null, Name = request.ImageName };
    }

    public static byte[] ResizeImage(byte[] data, int width, int height)
    {
        using var ms = new MemoryStream(data);
        var imgPhoto = Image.FromStream(ms);
        var sourceWidth = imgPhoto.Width;
        var sourceHeight = imgPhoto.Height;
        var sourceX = 0;
        var sourceY = 0;
        var destX = 0;
        var destY = 0;

        float nPercent = 0;
        float nPercentW = 0;
        float nPercentH = 0;

        nPercentW = ((float)width / (float)sourceWidth);
        nPercentH = ((float)height / (float)sourceHeight);
        if (nPercentH < nPercentW)
        {
            nPercent = nPercentH;
            destX = Convert.ToInt16((width - (sourceWidth * nPercent)) / 2);
        }
        else
        {
            nPercent = nPercentW;
            destY = Convert.ToInt16((height - (sourceHeight * nPercent)) / 2);
        }

        var destWidth = (int)(sourceWidth * nPercent);
        var destHeight = (int)(sourceHeight * nPercent);

        var newImageBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        newImageBitmap.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

        var grPhoto = Graphics.FromImage(newImageBitmap);
        grPhoto.Clear(Color.Transparent);
        grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
        grPhoto.DrawImage(imgPhoto, new Rectangle(destX, destY, destWidth, destHeight), new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), GraphicsUnit.Pixel);
        grPhoto.Dispose();
        var converter = new ImageConverter();
        return (byte[])converter.ConvertTo(newImageBitmap, typeof(byte[]));
    }
}
