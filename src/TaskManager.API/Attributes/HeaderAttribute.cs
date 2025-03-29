using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TaskManager.API.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class HeaderAttribute : Attribute
{
    public string HeaderName { get; }
    public string HeaderDescription { get; }
    public Type Type { get; }
    public bool IsRequired { get; }

    public HeaderAttribute(string headerName, Type type, bool isRequired = false, string headerDescription = "")
    {
        HeaderName = headerName;
        Type = type;
        IsRequired = isRequired;
        HeaderDescription = headerDescription;
    }
}

internal class HeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var headerAttributes = context.MethodInfo.GetCustomAttributes(typeof(HeaderAttribute), false) as HeaderAttribute[];
        
        if (headerAttributes == null || headerAttributes.Length == 0)
            return;

        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();

        foreach (var headerAttribute in headerAttributes)
        {
            var openApiType = headerAttribute.Type.Name.ToLower() switch
            {
                "string" => "string",
                "int32" => "integer",
                "boolean" => "boolean",
                "datetime" => "string",
                _ => "string",
            };

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = headerAttribute.HeaderName,
                In = ParameterLocation.Header,
                Required = headerAttribute.IsRequired,
                Description = headerAttribute.HeaderDescription,
                Schema = new OpenApiSchema
                {
                    Type = openApiType
                }
            });
        }
    }
}