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
    var defaultUserAdmin = new BookUser 
    { 
        UserName = "Admin@gmail.com", 
        Email = "Admin@gmail.com",
        Name = "NGUYEN NGOC HAI",
        Address = "Can Tho",
        DOB = new DateTime(2008, 3, 9, 16, 5, 7, 123),
        EmailConfirmed = true, 
        PhoneNumber = "0909090909",
        PhoneNumberConfirmed = true 
        
    };
    if (userManager.Users.All(u => u.Id != defaultUserAdmin.Id))
    {
        var user = await userManager.FindByEmailAsync(defaultUserAdmin.Email);
        if(user==null)
        {
            await userManager.CreateAsync(defaultUserAdmin, "Admin@123");
            await userManager.AddToRoleAsync(defaultUserAdmin, Enums.Roles.Admin.ToString());
        }
    }

    //Seed Default User
    var defaultUserStoreOwner = new BookUser 
    { 
        UserName = "StoreOwner@gmail.com", 
        Email = "StoreOwner@gmail.com",
        Name = "Le Hoang Du",
        Address = "Can Tho",
        DOB = new DateTime(2002, 3, 11, 21, 5, 8, 123),
        EmailConfirmed = true, 
        PhoneNumber = "0909090909",
        PhoneNumberConfirmed = true 
        
    };
    if (userManager.Users.All(u => u.Id != defaultUserStoreOwner.Id))
    {
        var user = await userManager.FindByEmailAsync(defaultUserStoreOwner.Email);
        if(user==null)
        {
            await userManager.CreateAsync(defaultUserStoreOwner, "Admin@123");
            await userManager.AddToRoleAsync(defaultUserStoreOwner, Enums.Roles.StoreOwner.ToString());
        }
    }
}
}
}