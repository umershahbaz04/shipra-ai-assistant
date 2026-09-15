using System;
using System.Collections.Generic;
using System.Text;

namespace Shipra.Backend.API.Application.Common.Constants;

public static class ApplicationErrorConstants
{
  public const string GeneralError = "Something went wrong on server, Please reachout to support and refer to this error: ";

  public const string ProviderCustomerPracticeLocationDeactivated = "Provider's customer practice location has been deactivated and this appointment creation is no longer valid.";

  public const string PatientDeactivated = "Patient has been deactivated";

  public const string AppointmentOfferExpired = "Appointment Offer has been expired";

  public const string ProviderBlockout = "Provider went out of office and can't accept any new appointments.";

  public const string ProviderHasExistingAppointment = "Provider already has an existing appointment at provided date and time.";

  public const string PatientHasExistingAppointment = "Patient already has an existing appointment at provided date and time.";

  public const string ProviderCPLNotExist = "Patient already has an existing appointment at provided date and time.";

  #region Auth0 response

  public const string Auth0UserAreadyExist = "User with this email already exists.";
  public const string Auth0UserFailed = "User creation failed.";

  #endregion
}
