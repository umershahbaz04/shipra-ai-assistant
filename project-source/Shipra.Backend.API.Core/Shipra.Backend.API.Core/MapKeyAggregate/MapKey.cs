namespace Shipra.Backend.API.Core.MapKeyAggregate;
public class MapKey
{
  public int MapkeyId { get; set; }
  public string? MapKey1 { get; set; }
}
public class MapKeysHistory
{
  public long MapKeysHistoryid { get; set; } 
  public int? MapkeyId { get; set; } 
  public DateTime? StartDate { get; set; } 
  public DateTime? EndDate { get; set; }

  public static MapKeysHistory Create(int mapkeyId, DateTime startDate, DateTime endDate)
  {
    return new MapKeysHistory
    {
      MapkeyId = mapkeyId,
      StartDate = startDate,
      EndDate = endDate
    };
  }
}
public class MapKeysInUse
{
  public int MapKeysInUseId { get; set; } 
  public int? MapkeyId { get; set; } 
  public long? MapKeysHistoryid { get; set; } 
  public DateTime? StartDate { get; set; } 
  public DateTime? EndDate { get; set; }

  public static MapKeysInUse Create(DateTime startDate, DateTime endDate, int mapkeyId, long mapKeysHistoryid)
  {
    return new MapKeysInUse
    {
      StartDate = startDate,
      EndDate = endDate,
      MapkeyId = mapkeyId,
      MapKeysHistoryid = mapKeysHistoryid
    };
  }

  public void Update(DateTime startDate, DateTime endDate, int? mapkeyId, long mapKeysHistoryid)
  {
    StartDate = startDate;
    EndDate = endDate;
    MapkeyId = mapkeyId;
    MapKeysHistoryid = mapKeysHistoryid;
  }
}
