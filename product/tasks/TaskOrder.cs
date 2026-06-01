namespace DotnetAksMicroservices.Product.Tasks;

public record TaskOrder(string Id, string Title, string Details, string AssignedUserId, string Status);
