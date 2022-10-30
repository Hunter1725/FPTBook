using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using FPTBook.Models;

namespace FPTBook.Areas.Identity.Data;

// Add profile data for application users by adding properties to the BookUser class
public class BookUser : IdentityUser
{
    [PersonalData]
    public string  Name { get; set; }

    [PersonalData]
    public DateTime  DOB { get; set; }

    [PersonalData]
    public string  Address { get; set; }

    [PersonalData]
    public ICollection<Order>? Order { get; set; }

}

