using DoctorAppointmentWeb.Api.Exceptions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DoctorAppointmentWebApi.ExceptionHandler;

/// <summary>
/// Обработчик исключений для API.
/// </summary>
public class ExceptionHandlers : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        if (exception is InvalidArgumentException)
        {
            context.Result = new BadRequestObjectResult(new
            {
                Status = "error",
                Message = exception.Message
            });
        }
        else if (exception is NotFoundException)
        {
            context.Result = new NotFoundObjectResult(new
            {
                Status = "error",
                Message = exception.Message
            });
        }
        else if (exception is UnauthorizedAccessExceptions)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                Status = "error",
                Message = exception.Message
            });
        }
        else if (exception is ForbiddenException)
        {
            context.Result = new ObjectResult(new
            {
                Status = "error",
                Message = exception.Message
            })
            {
                StatusCode = 403
            };
        }
        else if (exception is ConflictException)
        {
            context.Result = new ConflictObjectResult(new
            {
                Status = "error",
                Message = exception.Message
            });
        }
        else if (exception is ValidationException)
        {
            context.Result = new BadRequestObjectResult(new
            {
                Status = "error",
                Message = exception.Message
            });
        }
        else
        {
            context.Result = new ObjectResult(new
            {
                Status = "error",
                Message = "Произошла непредвиденная ошибка."
            })
            {
                StatusCode = 500
            };
        }
    }
}