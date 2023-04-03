using ASP.NETCoreIdentityCustom.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NETCoreIdentityCustom.Controllers
{
    public class RoleController : Controller
    {
        //[Authorize(Policy = "EmployeeOnly")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "RequireManager")]
        public IActionResult Manager()
        {
            return View();
        }

        //[Authorize(Policy = "RequireAdmin")]
        //[Authorize(Roles = "Administrator,Manager")]
        [Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
        public IActionResult Admin()
        {
            return View();
        }

    }
}
