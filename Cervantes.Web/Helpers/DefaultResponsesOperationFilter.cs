using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cervantes.Web.Helpers;

public class DefaultResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var httpMethod = context.ApiDescription.HttpMethod?.ToUpper();
        
        // Remove the default 200 response that Swagger adds
        operation.Responses.Clear();
        
        switch (httpMethod)
        {
            case "POST":
                operation.Responses.Add("201", new OpenApiResponse { Description = "Created" });
                operation.Responses.Add("400", new OpenApiResponse { Description = "Bad Request" });
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
                break;
                
            case "GET":
                operation.Responses.Add("200", new OpenApiResponse { Description = "Success" });
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
                operation.Responses.Add("404", new OpenApiResponse { Description = "Not Found" });
                break;
                
            case "PUT":
                operation.Responses.Add("204", new OpenApiResponse { Description = "Success" });
                operation.Responses.Add("400", new OpenApiResponse { Description = "Bad Request" });
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
                operation.Responses.Add("404", new OpenApiResponse { Description = "Not Found" });
                break;
                
            case "DELETE":
                operation.Responses.Add("204", new OpenApiResponse { Description = "No Content" });
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
                operation.Responses.Add("404", new OpenApiResponse { Description = "Not Found" });
                break;
                
            default:
                operation.Responses.Add("200", new OpenApiResponse { Description = "Success" });
                break;
        }
    }
}