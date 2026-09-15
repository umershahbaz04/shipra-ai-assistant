using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.CatalougeAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface ICatalogueRepository
{
  Task<bool> CreateCatalogue(Catalogue catalogue);
  Task<List<CatalogueDatabase>> GetAllCatalogueDatabases();
  Task<bool> UpdateCatalogueDatabase(CatalogueDatabase catalogueDatabase);
  Task<List<Catalogue>> GetAllCatalogues();
  Task<Catalogue?> GetCatalogByClientId(string? clientId);
}
