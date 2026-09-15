using iText.Html2pdf;
using Nancy.Extensions;

namespace Shipra.Backend.API.Application.Helpers.Reporting;
public class PDFDocumentGenerator
{
  public static string ContentType = "application/pdf";
  public static byte[] GeneratePDF(string html)
  {

    using (var stream = new MemoryStream())
    {
      HtmlConverter.ConvertToPdf(html, stream);
      return stream.ToArray();
    }
  }
  public static void ConvertHtmlToPdf(string htmlContent, string pdfFilePath)
  {

    using (var outputStream = new FileStream(pdfFilePath, FileMode.Create))
    {
      HtmlConverter.ConvertToPdf(htmlContent, outputStream);
    }
  }
  public static string ReplacePlaceholdersInTemplate(string templatePath, Dictionary<string, object> replacements, string listItems)
  {
    var templateContent = File.ReadAllText(templatePath);

    foreach (var replacement in replacements)
    {
      object obj = !string.IsNullOrEmpty(replacement.Value?.ToString()) ? replacement.Value! : "";
      if (obj != null)
      {
        Type objectType = obj.GetType();

        if (objectType == typeof(List<object>))
        {
          templateContent = templateContent.Replace("{{" + replacement.Key + "}}", listItems);
          //foreach (var item in (dynamic)replacement.Value!)
          //{
          //  Dictionary<string, object> listReplacements = PDFDocumentGenerator.ConvertDynamicToDictionary(replacement.Value!);
          //  if (listReplacements.Count() > 0)
          //  {
          //    //ReplacePlaceholdersInTemplate(templatePath, listReplacements);
          //  }
          //}
        }
        else
        {
          var value = !string.IsNullOrEmpty(replacement.Value?.ToString()) ? replacement.Value?.ToString() : "";
          templateContent = templateContent.Replace("{{" + replacement.Key + "}}", value);
        }
      }
    }

    return templateContent;
  }

  private static Dictionary<string, object> ConvertDynamicListToDictionary(dynamic dynamicList)
  {
    var dictionary = new Dictionary<string, object>(System.StringComparer.OrdinalIgnoreCase);

    // Iterating over the list
    foreach (var dapperRow in dynamicList)
    {
      // Extracting key-value pairs from each row
      foreach (var property in dapperRow)
      {
        string strKey = property.Key;
        string key = strKey.ToCamelCase();
        object value = property.Value;

        // Adding key-value pair to the dictionary
        dictionary[key] = value;
      }
    }

    return dictionary;
  }

  public static string ReplacePlaceholdersInTemplate1(string templatePath, Dictionary<string, string> replacements)
  {
    var templateContent = File.ReadAllText(templatePath);

    foreach (var replacement in replacements)
    {
      templateContent = templateContent.Replace("{{" + replacement.Key + "}}", replacement.Value);
    }

    return templateContent;
  }

  public static byte[] ReadAllBytes(string pdfFilePath)
  {
    return File.ReadAllBytes(pdfFilePath);
  }
  public static byte[] ReadAllBytesFromStream(Stream instream)
  {
    if (instream is MemoryStream)
      return ((MemoryStream)instream).ToArray();

    using (var memoryStream = new MemoryStream())
    {
      instream.CopyTo(memoryStream);
      return memoryStream.ToArray();
    }
  }
  public static Dictionary<string, object> ConvertDynamicToDictionary(dynamic dynamicObject)
  {
    var dictionary = new Dictionary<string, object>(System.StringComparer.OrdinalIgnoreCase);
    var dynamicDict = (IDictionary<string, object>)dynamicObject;

    foreach (var keyValuePair in dynamicDict)
    {
      dictionary.Add(keyValuePair.Key.ToCamelCase(), keyValuePair.Value);
    }

    return dictionary;
  }
  public static byte[] ConvertStreamToByteArray(Stream? stream)
  {
    if (stream == null)
      return Array.Empty<byte>(); // Return an empty byte array if stream is null

    if (stream.CanSeek)
      stream.Position = 0; // Reset position if seekable

    using (MemoryStream memoryStream = new MemoryStream())
    {
      stream.CopyTo(memoryStream);
      return memoryStream.ToArray();
    }
  }

  public static string ConvertStreamToBase64(Stream? stream)
  {
    if (stream == null)
      return string.Empty; // Return an empty string if stream is null

    using (MemoryStream memoryStream = new MemoryStream())
    {
      stream.CopyTo(memoryStream);
      return Convert.ToBase64String(memoryStream.ToArray());
    }
  }

}
