using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace DoctorAppointmentWebApi.Filters;

public class SwaggerOperationFromInterfaceFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var methodInfo = context.MethodInfo;
        var interfaceMethods = methodInfo.DeclaringType?.GetInterfaces()
            .SelectMany(i => i.GetMethods())
            .Where(iMethod => iMethod.Name == methodInfo.Name);

        if (interfaceMethods == null) return;

        foreach (var interfaceMethod in interfaceMethods)
        {
            var swaggerOperationAttribute = interfaceMethod
                .GetCustomAttributes<Swashbuckle.AspNetCore.Annotations.SwaggerOperationAttribute>()
                .FirstOrDefault();

            if (swaggerOperationAttribute != null)
            {
                operation.Summary = swaggerOperationAttribute.Summary;
                operation.Description = swaggerOperationAttribute.Description;
                operation.OperationId = swaggerOperationAttribute.OperationId ?? operation.OperationId;
            }

            var swaggerResponseAttributes = interfaceMethod
                .GetCustomAttributes<Swashbuckle.AspNetCore.Annotations.SwaggerResponseAttribute>();

            foreach (var responseAttribute in swaggerResponseAttributes)
            {
                var statusCode = responseAttribute.StatusCode.ToString();

                // Проверяем, существует ли уже ответ с таким статусом
                if (!operation.Responses.ContainsKey(statusCode))
                {
                    operation.Responses.Add(statusCode, new OpenApiResponse
                    {
                        Description = responseAttribute.Description
                    });
                }
                else
                {
                    // Обновляем описание, если ответ уже существует
                    operation.Responses[statusCode].Description = responseAttribute.Description;
                }
            }
        }
    }
}