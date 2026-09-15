using System;

namespace Shipra.Backend.API.Web.Common;

public enum FailureType
{
  Unspecified,
  EntityNotFound,
  ValidationFailure,
  ExternalServiceFailure,
  UnexpectedFailure,
  BusinessRuleViolation,
  BadConfiguration,
  NotImplemented
}
