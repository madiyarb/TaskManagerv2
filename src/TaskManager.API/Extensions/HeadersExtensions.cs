using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TaskManager.API.Extensions;

public class HeadersExtensions : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Authorization",
            In = ParameterLocation.Header,
            Required = false,
            Description = "DESCRIPTION",
            Schema = new OpenApiSchema
            {
                Type = "string"
            }
        });
    }
}