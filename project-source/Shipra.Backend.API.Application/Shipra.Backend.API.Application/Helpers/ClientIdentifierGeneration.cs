namespace Shipra.Backend.API.Application.Helpers;

public class ClientIdentifierGeneration
{
  public static int NextIdentifier(int? clientIdentifier)
  {
    int newVal = 0;
    if (clientIdentifier > 0)
    {
      newVal = clientIdentifier.GetValueOrDefault() + 1;
    }
    else
    {
      //its a default value if we have no client exist 
      newVal = 100;
    }
    return newVal;
  }
}
