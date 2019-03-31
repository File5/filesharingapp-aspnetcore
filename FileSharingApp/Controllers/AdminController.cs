using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileSharingApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FileSharingApp.Data;
using Microsoft.AspNetCore.Authorization;

namespace FileSharingApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public static readonly string adminRoleName = "Admin";
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context) {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var Users = _userManager.Users.ToList();
            IEnumerable<UserWithRole> UsersRoles = await Task.WhenAll(Users.Select(async i => new UserWithRole(i, await _userManager.IsInRoleAsync(i, adminRoleName))));
            return View(UsersRoles);
        }

        public async Task<IActionResult> ChangeRole(string id) {
            var adminRole = await _roleManager.FindByNameAsync(adminRoleName);

            if (adminRole == null) {
                adminRole = new IdentityRole(adminRoleName);
                await _roleManager.CreateAsync(adminRole);
            }

            var user = await _userManager.FindByIdAsync(id);
            bool isAdmin = await _userManager.IsInRoleAsync(user, adminRoleName);

            if (isAdmin) {
                await _userManager.RemoveFromRoleAsync(user, adminRoleName);
            } else {
                await _userManager.AddToRoleAsync(user, adminRoleName);
            }

            return RedirectToAction("Index");
        }

        private async Task<AppUser> GetCurrentUser() {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
    }
    public class UserWithRole {
        public AppUser User { get; set; }
        public bool IsAdmin { get; set; }

        public UserWithRole() { }
        public UserWithRole(AppUser user, bool isAdmin) {
            User = user;
            IsAdmin = isAdmin;
        }
    }
}