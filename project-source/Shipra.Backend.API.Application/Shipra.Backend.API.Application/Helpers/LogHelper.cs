using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.Helpers;
public static class LogHelper
{
  private static readonly object _lock = new object();

  public static void Write(string message, string clientId = "General")
  {
    try
    {
      // Base logs folder
      var baseLogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

      // Create sub-folder for each client
      var clientFolder = Path.Combine(baseLogDirectory, clientId);
      if (!Directory.Exists(clientFolder))
      {
        Directory.CreateDirectory(clientFolder);
      }

      // File name per day: RefreshlogFile_yyyy-MM-dd.txt
      var fileName = $"RefreshlogFile_{DateTime.UtcNow:yyyy-MM-dd}.txt";
      var logFile = Path.Combine(clientFolder, fileName);

      // Final log message
      var logMessage = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} | {message}";

      lock (_lock)
      {
        File.AppendAllText(logFile, logMessage + Environment.NewLine);
      }
    }
    catch
    {
      // Prevent log writing errors from breaking main process
    }
  }
}

