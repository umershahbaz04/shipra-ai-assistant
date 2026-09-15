using Microsoft.AspNetCore.Http;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats;
public static class ImageValidationUtils
{
  public static bool IsFileSizeValid(IFormFile file, int maxSizeInKb)
  {
    return file != null && file.Length <= maxSizeInKb * 1024;
  }

  public static bool HasValidDimensions(IFormFile file, int maxWidth, int maxHeight)
  {
    try
    {
      using var stream = file.OpenReadStream();
      using var image = Image.Load<Rgba32>(stream); // ✅ FIXED: this is the correct call
      return image.Width <= maxWidth && image.Height <= maxHeight;
    }
    catch
    {
      return false; // Not a valid image or unsupported format
    }
  }
  public static bool IsSupportedContentType(IFormFile file)
  {
    var allowedTypes = new[] { "image/jpeg", "image/png" };
    return allowedTypes.Contains(file.ContentType);
  }
}
