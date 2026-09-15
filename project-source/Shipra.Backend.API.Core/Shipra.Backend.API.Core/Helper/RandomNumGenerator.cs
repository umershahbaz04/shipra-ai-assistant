namespace Shipra.Backend.API.Core.Helper;

public class RandomNumGenerator
{
  public static int GetRandom1NumberFrom1To10()
  {
    Random random = new Random();
    return random.Next(1, 10); // Generates a number between 1 and 9 (inclusive)
  }
  public static int GetRandomThreeDigitNumber()
  {
    Random random = new Random();
    return random.Next(100, 1000); // Generates a number between 100 and 999 (inclusive)
  }
}
