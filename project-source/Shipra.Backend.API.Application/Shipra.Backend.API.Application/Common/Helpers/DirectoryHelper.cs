using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.Common.Helpers;
public class DirectoryHelper
{
  public bool CheckDirectoryExistAndCreate(string fileUploadsFolder)
  {
    if (!Directory.Exists(fileUploadsFolder))
    { //check if the folder exists;
      Directory.CreateDirectory(fileUploadsFolder);
    }
    return true;
  }

  public string CombinePathPdfs(string fileUploadsFolder)
  {
    return System.IO.Path.Combine(fileUploadsFolder, $"Output_{DateTime.Now.ToString("dd-MM-yyyy-MM-HH-ss")}.pdf");
  }
  public static string GetRandomNameForPdf(string name = "")
  {
    return $"{name}_{DateTime.Now.ToString("dd-MM-yyyy-MM-HH-ss")}.pdf";
  }

  public bool FileExistsCreateAndClose(string path)
  {
    if (!File.Exists(path))
    {
      var myFile = File.Create(path);
      myFile.Close();
    }
    return true;
  }

  public string[] GetAllPdfFiles(string fileUploadsFolder)
  {
    return Directory.GetFiles(fileUploadsFolder, "*.pdf", SearchOption.TopDirectoryOnly);
  }

  public bool DeleteDirectroy(string fileUploadsFolder)
  {
    if (Directory.Exists(fileUploadsFolder))
    {
      Directory.Delete(fileUploadsFolder, true);
    }
    return true;
  }
}
