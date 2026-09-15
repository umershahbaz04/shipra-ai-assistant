using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.StoresAggregate;
public class StoreUploadSampleFile
{
  public int StoreUploadSampleFileId { get; private set; }
  public int? CountryId { get; private set; }
  public string? FilePath { get; private set; }
}

