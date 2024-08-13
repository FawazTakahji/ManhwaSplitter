using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Pbm;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Qoi;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using ErrorOr;
using NLog;

namespace ManhwaSplitter.Core.Utilities;

public static class Images
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public static async Task<ErrorOr<bool>> IsValid(string path)
    {
        try
        {
            ImageInfo info = await Image.IdentifyAsync(path);

            return info.Metadata.DecodedImageFormat is not null;
        }
        catch (InvalidImageContentException)
        {
            return false;
        }
        catch (UnknownImageFormatException)
        {
            return false;
        }
        catch (Exception ex)
        {
            return Error.Failure(description: $"An error occurred while identifying the file \"{Path.GetFileName(path)}\".",
                metadata: new()
                {
                    { "Exception", ex }
                });
        }
    }

    public static async Task<ErrorOr<Image>> Load(string path)
    {
        try
        {
            return await Image.LoadAsync(path);
        }
        catch (UnknownImageFormatException)
        {
            return Error.Failure(description: $"The file format for \"{Path.GetFileName(path)}\" is not supported, The following formats are supported:"
                                              + "\n" + "PNG, PBM, WEBP, TIFF, JPEG, QOI, TGA, BMP.");
        }
        catch (Exception ex)
        {
            return Error.Failure(description: $"An error occurred while loading the file \"{Path.GetFileName(path)}\".",
                metadata: new()
                {
                    { "Exception", ex }
                });
        }
    }

    public static List<Image> Split(Image image, int maxHeight)
    {
        List<Image> images = [];

        int numberOfImages = (int)Math.Ceiling(image.Height / (double)maxHeight);
        for (int i = 0; i < numberOfImages; i++)
        {
            int startY = i * maxHeight;
            int endY = startY + maxHeight;
            if (endY > image.Height)
                endY = image.Height;

            Rectangle cropRectangle = new(0, startY, image.Width, endY - startY);
            Image croppedImage = image.Clone(clone => clone.Crop(cropRectangle));
            images.Add(croppedImage);
        }

        return images;
    }

    public static async Task<ErrorOr<List<string>>> Save(List<Image> images, string directory, string name, string extension, CancellationToken token = default)
    {
        List<string> paths = [];
        IImageEncoder encoder = GetEncoder(images[0]);
        int numberOfDigits = images.Count.ToString().Length;

        try
        {
            foreach (Image image in images)
            {
                token.ThrowIfCancellationRequested();
                int imageIndex = images.IndexOf(image) + 1;
                string nameIndex = imageIndex.ToString($"D{numberOfDigits}");
                string imageName = $"{name}_{nameIndex}{extension}";
                string path = Path.Combine(directory, imageName);

                try
                {
                    await image.SaveAsync(path, encoder, cancellationToken: token);
                    paths.Add(path);
                }
                catch (Exception ex)
                {
                    return Error.Failure(description: $"An error occurred while saving the image \"{imageName}\".",
                        metadata: new()
                        {
                            { "Exception", ex }
                        });
                }
            }
        }
        catch (OperationCanceledException)
        {
            foreach (string savedImage in paths)
            {
                try
                {
                    File.Delete(savedImage);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"An error occurred while deleting the file \"{savedImage}\".");
                }
            }

            throw;
        }

        return paths;
    }

    public static IImageEncoder GetEncoder(Image image)
    {
        IImageFormat? format = image.Metadata.DecodedImageFormat;

        return format switch
        {
            PngFormat => new PngEncoder(),
            PbmFormat => new PbmEncoder(),
            WebpFormat => new WebpEncoder(),
            TiffFormat => new TiffEncoder(),
            JpegFormat => new JpegEncoder(),
            QoiFormat => new QoiEncoder(),
            TgaFormat => new TgaEncoder(),
            BmpFormat => new BmpEncoder(),
            _ => new PngEncoder()
        };
    }
}