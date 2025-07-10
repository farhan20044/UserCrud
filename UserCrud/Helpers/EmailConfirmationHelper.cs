using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using UserCrud.Models.Dto;

namespace UserCrud.Helpers
{
    public static class EmailConfirmationHelper
    {
        /// <summary>
        /// Generates an email confirmation link for application user
        /// </summary>
        /// <param name="controller">The controller instance (for Url.Action)</param>
        /// <param name="user">The user to generate the link for</param>
        /// <param name="token">The confirmation token</param>
        /// <returns>The confirmation link URL or null if generation fails</returns>
        public static string? GenerateEmailConfirmationLink(ControllerBase controller, ApplicationUser user, string token)
        {
            return controller.Url.Action(
                "ConfirmEmailGet",
                "Auth",
                new { userId = user.Id, token = token },
                protocol: controller.HttpContext.Request.Scheme);
        }

        /// <summary>
        /// Generates an email confirmation link for UserDto
        /// </summary>
        /// <param name="controller">The controller instance (for Url.Action)</param>
        /// <param name="user">The user DTO to generate the link for</param>
        /// <param name="token">The confirmation token</param>
        /// <returns>The confirmation link URL or null if generation fails</returns>
        public static string? GenerateEmailConfirmationLink(ControllerBase controller, UserDto user, string token)
        {
            return controller.Url.Action(
                "ConfirmEmailGet",
                "Auth",
                new { userId = user.Id, token = token },
                protocol: controller.HttpContext.Request.Scheme);
        }
    }
} 