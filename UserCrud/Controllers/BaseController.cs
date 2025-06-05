using Microsoft.AspNetCore.Mvc;
using UserCrud.Models.Dto;
using UserCrud.Helpers;
using UserCrud.Services;
using System;
using System.Linq;

public abstract class BaseController : ControllerBase
{
    public override OkObjectResult Ok(object value)
    {
        if (value is object)
        {
            return base.Ok(value);
        }

        var response = ApiResponse<object>.SuccessResponse(value);
        return base.Ok(response);
    }

    public IActionResult Ok(string message)
    {
        var response = ApiResponse<object>.SuccessResponse(null, message);
        return base.Ok(response);
    }

    public IActionResult Ok(object value, string message)
    {
        var response = ApiResponse<object>.SuccessResponse(value, message);
        return base.Ok(response);
    }
}