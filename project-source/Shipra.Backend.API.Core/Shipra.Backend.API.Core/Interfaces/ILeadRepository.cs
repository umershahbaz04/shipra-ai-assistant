using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.LeadAggregate;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Core.Interfaces;

public interface ILeadRepository
{
    Task<Lead?> CheckLeadExistsByPhoneOrProduct(ClientId clientId, string phoneNumber, string productName);
    Task<string?> GetNonCompletedLeadStatusByPhone(ClientId clientId, string phoneNumber);
    Task<Lead> CreateLead(Lead lead);
    Task<Lead?> UpdateLead(Lead lead);
    Task<dynamic> GetAllLeads(string clientId, int start, int length, string? search = null, int sortCol = 0, string? sortDir = null, string? leadStatusIds = null, string? salespersonIds = null, string? assignmentFilter = null, string? countryIds = null, string? startDate = null, string? endDate = null);
    Task<Lead?> GetLeadById(LeadId leadId);
    Task<bool> DeleteLeads(List<Lead> leads);
    Task<List<LeadStatusLookup>> GetAllLeadStatusForSelection();
    Task<dynamic> GetLeadContacts(string clientId, int start, int length, string? search);
    Task<dynamic> GetOrdersByContactMobile(string clientId, string mobileNumber);

    // Lead Tabs Config (mirrors GetAllShipmentGridClientSettingForDashboard)
    Task<List<LeadDashboardResponseModel>> GetAllLeadGridClientSettingForDashboard(string? clientId);

    // Lead Tabs Count (mirrors GetAllShipmentTabsCount)
    Task<dynamic> GetAllLeadTabsCount(string clientId, string? leadStatusIds = null, string? salespersonIds = null, string? search = null, string? assignmentFilter = null, string? countryIds = null, string? startDate = null, string? endDate = null);
    Task<dynamic> GetSalesPersonLeadStats(string clientId, string? leadStatusIds = null, string? salespersonIds = null, string? search = null, string? assignmentFilter = null, string? countryIds = null, string? startDate = null, string? endDate = null);

    // Lead Tab Management
    Task<bool> CreateLeadGridColumn(string columnName, string? dashboardStatusIdValues, ClientId clientId, EmployeeId employeeId);
    Task<bool> UpdateLeadTabDisplayOrder(List<(int leadGridColumnId, int displayOrder)> items);

    // Client Lead Status CRUD
    Task<bool> CreateClientLeadStatus(ClientLeadStatusLookup clientLeadStatus);
    Task<ClientLeadStatusLookup?> GetClientLeadStatusById(int clientLeadStatusId, ClientId clientId);
    Task<bool> UpdateClientLeadStatus(ClientLeadStatusLookup clientLeadStatus);
    Task<List<ClientLeadStatusLookup>> GetAllClientLeadStatusForSelection(ClientId clientId);
    Task<dynamic> GetAllClientLeadStatus(string clientId, int start, int length, string? search, int sortCol, string? sortDir);

    // Auto-seeds defaults from LeadStatusLookup into ClientLeadStatusLookup if client has none
    Task SeedDefaultLeadStatuses(ClientId clientId, EmployeeId employeeId);
    Task<int> GetDefaultClientLeadStatusId(ClientId clientId);

    // Lead Grid Column (Tab) management — mirrors Shipment Grid Column
    Task<LeadGridColumn?> GetLeadGridColumnById(int leadGridColumnId, ClientId clientId);
    Task<bool> UpdateLeadGridColumn(int leadGridColumnId, string columnName, string dashboardStatusValue, ClientId clientId, EmployeeId employeeId);
    Task<bool> DeleteLeadGridColumn(LeadGridColumn column);
}
