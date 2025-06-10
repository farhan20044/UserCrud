using Microsoft.AspNetCore.Mvc;
using UserCrud.Models.Dto;
using UserCrud.Helpers;
using UserCrud.Services;
using System;
using System.Linq;

public abstract class BaseController : ControllerBase
{
    [NonAction]
    public override OkObjectResult Ok(object value)
    {
        if (value is ApiResponse<object>)
        {
            return base.Ok(value);
        }

        var response = ApiResponse<object>.SuccessResponse(value);
        return base.Ok(response);
    }

    [NonAction]
    public OkObjectResult Ok(string message)
    {
        var response = ApiResponse<object>.SuccessResponse(null, message);
        return base.Ok(response);
    }

    [NonAction]
    public OkObjectResult Ok(object value, string message)
    {
        var response = ApiResponse<object>.SuccessResponse(value, message);
        return base.Ok(response);
    }


    [NonAction]
    public BadRequestObjectResult BadRequest(Exception ex)
    {
        var response = ApiResponse<object>.FailureResponse(ex.Message);
        return base.BadRequest(response);
    }

    [NonAction]
    public NotFoundObjectResult NotFound(Exception ex)
    {
        var response = ApiResponse<object>.FailureResponse(ex.Message);
        return base.NotFound(response);
    }
}