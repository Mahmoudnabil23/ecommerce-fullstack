using E_Commerce_Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce_Backend.Data.Seeders
{
    public class IdentitySeeder
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public IdentitySeeder(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedRolesAsync()
        {
            string[] roles = { "Admin", "Seller", "Customer" };

            foreach (var role in roles)
            {
                var exists = await _roleManager.RoleExistsAsync(role);

                if (!exists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public async Task SeedAdminAsync()
        {
            var email = "admin@site.com";

            var admin = await _userManager.FindByEmailAsync(email);

            if (admin != null)
                return;

            admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = "Admin User"
            };

            var result = await _userManager.CreateAsync(admin, "Admin@123");

            if (!result.Succeeded)
                throw new Exception("Failed to create admin user");

            await _userManager.AddToRoleAsync(admin, "Admin");
        }

    }
}
