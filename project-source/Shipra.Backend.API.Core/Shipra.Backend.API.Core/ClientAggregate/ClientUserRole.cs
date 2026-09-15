using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Constants;

namespace Shipra.Backend.API.Core.ClientAggregate;
public partial class ClientUserRole
{
  public int ClientUserRoleId { get; set; }
  public ClientId? ClientId { get; set; }
  public string? RoleName { get; set; }
  public string? RoleDescription { get; set; }
  public bool? Active { get; set; }
  public bool? IsDefault { get; set; }

  public static ClientUserRole Create(string? roleName, string? roleDescription, ClientId clientId, bool? isDefault)
  {
    return new ClientUserRole { RoleName = roleName, RoleDescription = roleDescription, ClientId = clientId, Active = true, IsDefault = isDefault };
  }
  public static ClientUserRole AddDefault()
  {
    return new ClientUserRole()
    {
      ClientUserRoleId = 0,
      RoleName = ShipraConstants.DropDownPlaceHolderName
    };
  }

}
