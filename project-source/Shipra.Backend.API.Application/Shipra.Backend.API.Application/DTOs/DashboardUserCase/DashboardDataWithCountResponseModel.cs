using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.DashboardUserCase;
public class DashboardDataWithCountResponseModel
{
  public int Count { get; set; }
  public List<dynamic>? list { get; set; } = new List<dynamic>();
} 
