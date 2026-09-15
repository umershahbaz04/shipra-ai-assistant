using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DocumentAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IDocumentRepository
{ 
  Task<List<DocumentType>> GetAllDocumentType();
  Task<List<DocumentSize>> GetAllDocumentSize();
  Task<List<DocumentTemplate>> GetAllDocumentTemplate();
  Task<DocumentTemplate> GetDocumentTemplateById(int? documentTemplateId,ClientId? clientId);
  Task<List<DocumentTemplateConfig>> GetAllDocumentTemplateConfigByClient(ClientId? clientId);
  Task<DocumentTemplateConfig> GetDocumentTemplateConfigById(DocumentTemplateConfigId documentTemplateConfigId, ClientId? clientId);
  Task<bool> UpdateDocumentTemplateConfig(DocumentTemplateConfig documentTemplate);
  Task<bool> CreateDocumentTemplateConfig(DocumentTemplateConfig oDocumentTemplateConfig);
  Task<dynamic> GetValidateDocumentSetting(int documentTypeId, string? clientId);
  Task<DocumentTemplateConfig> GetDocumentTemplateConfigByClientId(ClientId? clientId);
}
