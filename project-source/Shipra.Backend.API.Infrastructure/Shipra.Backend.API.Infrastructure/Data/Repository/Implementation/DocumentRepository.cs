using System.Dynamic;
using Ardalis.Specification.EntityFrameworkCore;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DocumentAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Infrastructure.Services.Interface;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class DocumentRepository : IDocumentRepository
{
  private readonly IDbContextService _dbContextService;
  private readonly ShipraMasterDbContext _shipraMasterDbContext;
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public DocumentRepository(IDbContextService dbContextService, ShipraMasterDbContext shipraMasterDbContext, DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dbContextService = dbContextService;
    _shipraMasterDbContext = shipraMasterDbContext;
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }

  public async Task<List<DocumentSize>> GetAllDocumentSize()
  {
    var data = await _context.DocumentSizes.ToListAsync();
    return data;
  }

  public async Task<List<DocumentTemplate>> GetAllDocumentTemplate()
  {
    var data = await _context.DocumentTemplates.ToListAsync();
    return data;
  }
  public async Task<DocumentTemplate> GetDocumentTemplateById(int? documentTemplateId, ClientId? clientId)
  {
    var cId = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(cId!))
    {
      var data = await _context.DocumentTemplates.FirstOrDefaultAsync(x => x.DocumentTemplateId == documentTemplateId);
      return data!;
    }
  }

  public async Task<List<DocumentTemplateConfig>> GetAllDocumentTemplateConfigByClient(ClientId? clientId)
  {
    var data = await _context.DocumentTemplateConfigs.Where(x => x.ClientId == clientId).ToListAsync();
    return data;
  }

  public async Task<List<DocumentType>> GetAllDocumentType()
  {
    var data = await _context.DocumentTypes.ToListAsync();
    return data;
  }

  public async Task<DocumentTemplateConfig> GetDocumentTemplateConfigById(DocumentTemplateConfigId documentTemplateConfigId, ClientId? clientId)
  {
    var data = await _context.DocumentTemplateConfigs.FirstOrDefaultAsync(x => x.ClientId == clientId && x.DocumentTemplateConfigId == documentTemplateConfigId);
    return data!;
  }

  public async Task<bool> UpdateDocumentTemplateConfig(DocumentTemplateConfig documentTemplate)
  {
    _context.DocumentTemplateConfigs.Update(documentTemplate);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<bool> CreateDocumentTemplateConfig(DocumentTemplateConfig oDocumentTemplateConfig)
  {
    await _context.DocumentTemplateConfigs.AddAsync(oDocumentTemplateConfig);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<DocumentTemplateConfig> GetDocumentTemplateConfigByClientId(ClientId? clientId)
  {
    var cId = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(cId!))
    {
      var target = await _context.DocumentTemplateConfigs.Where(x => x.ClientId == clientId).FirstOrDefaultAsync();
      return target!;
    }
  }
  public async Task<dynamic> GetValidateDocumentSetting(int documentTypeId, string? clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();
      var query = $@"SELECT dt.DocumentTemplateId
                            FROM dbo.DocumentTemplateConfig AS dtc
                                INNER JOIN dbo.DocumentTemplate AS dt
                                    ON dt.DocumentTemplateId = dtc.DocumentTemplateId
                                INNER JOIN dbo.DocumentType AS dt2
                                    ON dt2.DocumentTypeId = dt.DocumentTypeId ";
      string whereStart = $"WHERE ( 1=1 AND (dtc.ClientId = '{clientId}') AND dt.DocumentTypeId = {documentTypeId} AND ISNULL(dtc.IsAskEveryTime, 0) = 0 ";
      string whereEnd = ")";

      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "crr.CreatedOn");


      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);

      dynamic result = new ExpandoObject();
      var dataList = data.ToList();

      return dataList.FirstOrDefault()!;
    }

  }
}
