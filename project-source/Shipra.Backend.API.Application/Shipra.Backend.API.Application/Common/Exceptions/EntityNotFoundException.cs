namespace Shipra.Backend.API.Application.Common.Exceptions;
public class EntityNotFoundException : ApplicationException
{
  public dynamic Id { get; }
  public EntityNotFoundException(string entityName, dynamic guId)
            : base($"{entityName} with ID: {guId} not found.")
                => Id = guId;


}
public class InvalidIdTypeException : ApplicationException
{
  public dynamic Id { get; }
  public InvalidIdTypeException(dynamic guId)
            : base($"Invalid given ID with: {guId}.")
                => Id = guId;
}
