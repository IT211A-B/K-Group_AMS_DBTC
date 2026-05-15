using Backend.Backend;
using Backend.Backend.Model;
using Microsoft.AspNetCore.Identity;

namespace Backend.Backend.Seeder
{
    public static class RolePermissionandPermission
    {

        /*
        
        1. This Put Seeds to the:
              - Permission Entity
              - AspNet Roles

        2. Put All permissions in the admin or the super user
        */

        public static async Task SeedRolePermissionNPermissionAsync(this RoleManager<IdentityRole> roleManager,DatabaseLibrary context)
        {
            // Seed Roles
            var roles = new[] { "Admin", "Teacher", "Student" };

            foreach (var role in roles)
            {
                // Save this 3 Roles
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed Permissions
            if (!context.Permissions.Any())
            {
                context.Permissions.AddRange(
                    new Permission { String_Name = "User.Create" },
                    new Permission { String_Name = "User.Delete" },
                    new Permission { String_Name = "Student.Create" },
                    new Permission { String_Name = "Student.Delete" },
                    new Permission { String_Name = "Teacher.Create"},
                    new Permission { String_Name = "Teacher.Delete" }

                );

                await context.SaveChangesAsync();
            }

            /*
              
            This gives admin every permission that currently exist
            (Permission on the top is currently the only existing once)
             
             */
            var adminRole = await roleManager.FindByNameAsync("Admin");

            if (!context.RolePermissions.Any())
            {
                var permissions = context.Permissions.ToList();

                foreach (var permission in permissions)
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        Role_ID = adminRole!.Id, // I am sure, like super sure there is Admin in Rolemanager
                        Permission_ID = permission.Permission_ID,
                    });
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
