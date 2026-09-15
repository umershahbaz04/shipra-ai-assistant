using System;
using System.Collections.Generic;
using System.Text;

namespace Shipra.Backend.API.Application.Common;

public class PagedQueryBase : QueryBase
{
  public int Page { get; set; }
  public int PageSize { get; set; } = 50;

  public string OrderBy { get; set; } = "CreatedDate";
  public string SortBy { get; set; } = "desc";
  public string? SearchKeyword { get; set; }

  public DateTime DateTime { get; set; }
  public DateTime FromDate { get; set; }
  public DateTime ToDate { get; set; }
}
