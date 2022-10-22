using Microsoft.AspNetCore.Identity;
using FPTBook.Enums;
namespace FPTBook.Areas.Identity.Data{


public static class ContextSeed
{
    public static async Task SeedRolesAsync(UserManager<BookUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        //Seed Roles
        await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Admin.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Enums.Roles.StoreOwner.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Customer.ToString()));
    }

    public static async Task SeedSuperAdminAsync(UserManager<BookUser> userManager, RoleManager<IdentityRole> roleManager)
{
    //Seed Default User
    var defaultUser = new BookUser 
    { 
        UserName = "Admin@gmail.com", 
        Email = "Admin@gmail.com",
        Name = "Le Hoang Du",
        Address = "Can Tho",
        DOB = new DateTime(2022-09-25) ,
        EmailConfirmed = true, 
        PhoneNumber = "0909090909",
        PhoneNumberConfirmed = true 
        
    };
    if (userManager.Users.All(u => u.Id != defaultUser.Id))
    {
        var user = await userManager.FindByEmailAsync(defaultUser.Email);
        if(user==null)
        {
            await userManager.CreateAsync(defaultUser, "Admin@123");
            await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Admin.ToString());
            await userManager.AddToRoleAsync(defaultUser, Enums.Roles.StoreOwner.ToString());
            await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Customer.ToString());
        }
    }
}
}
}