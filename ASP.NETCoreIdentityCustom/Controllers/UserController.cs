using ASP.NETCoreIdentityCustom.Areas.Identity.Data;
using ASP.NETCoreIdentityCustom.Core.Repositories;
using ASP.NETCoreIdentityCustom.Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static ASP.NETCoreIdentityCustom.Core.Constants;

namespace ASP.NETCoreIdentityCustom.Controllers
{
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(IUnitOfWork unitOfWork, SignInManager<ApplicationUser> signInManager)
        {
            _unitOfWork = unitOfWork;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            var users = _unitOfWork.User.GetUsers();
            return View(users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = _unitOfWork.User.GetUser(id);
            var roles = _unitOfWork.Role.GetRoles();

            var userRoles = await _signInManager.UserManager.GetRolesAsync(user);

            var roleItems = roles.Select(role =>
                new SelectListItem(
                    role.Name,
                    role.Id,
                    userRoles.Any(ur => ur.Contains(role.Name)))).ToList();

            var vm = new EditUserViewModel
            {
                User = user,
                Roles = roleItems
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync(EditUserViewModel data)
        {

            var user = _unitOfWork.User.GetUser(data.User.Id);

            if(user == null)
            {
                return NotFound();
            }

            var userRolesInDb = await _signInManager.UserManager.GetRolesAsync(user);

            // Loop through the roles in ViewModel
            //Check if the Role is Assigned In DB
            //If Assigned -> Do Nothing
            //If Not Assigned -> Add Role

            var rolesToAdd = new List<string>();
            var rolesToDelete = new List<string>();

            foreach (var role in data.Roles)
            {
                var assignedInDb = userRolesInDb.FirstOrDefault(ur => ur == role.Text);

                if (role.Selected)
                {
                    if(assignedInDb == null)
                    {
                        //Add Role
                        rolesToAdd.Add(role.Text);
                        //await _signInManager.UserManager.AddToRoleAsync(user, role.Text);
                    }
                }
                else
                {
                    if(assignedInDb != null)
                    {
                        //Remove Role
                        rolesToDelete.Add(role.Text);
                        //await _signInManager.UserManager.RemoveFromRoleAsync(user, role.Text);
                    }
                }
            }

            if (rolesToAdd.Any())
            {
                await _signInManager.UserManager.AddToRolesAsync(user, rolesToAdd);
            }

            if (rolesToDelete.Any())
            {
                await _signInManager.UserManager.RemoveFromRolesAsync(user, rolesToDelete);
            }

            user.FirstName = data.User.FirstName;
            user.LastName = data.User.LastName;
            user.Email = data.User.Email;


            _unitOfWork.User.UpdateUser(user);


            return RedirectToAction("Edit", new { id = user.Id });
        }

        //public async Task<IActionResult> Edit(string id)
        //{
        //    var user = _unitOfWork.User.GetUser(id);
        //    var roles = _unitOfWork.Role.GetRoles();

        //    var userRoles = await _signInManager.UserManager.GetRolesAsync(user);

        //    //var roleItems = new List<SelectListItem>();
        //    //foreach (var role in roles)
        //    //{
        //    //    var hasRole = userRoles.Any(ur => ur.Contains(role.Name));

        //    //    roleItems.Add(new SelectListItem(role.Name, role.Id, hasRole));
        //    //}


        //    var roleItems = roles.Select(role => 
        //        new SelectListItem(
        //            role.Name, 
        //            role.Id, 
        //            userRoles.Any(ur => ur.Contains(role.Name))));


        //    var vm = new EditUserViewModel
        //    {
        //        User = user,
        //        Roles = roleItems
        //    };

        //    return View(vm);
        //}
    }
}
