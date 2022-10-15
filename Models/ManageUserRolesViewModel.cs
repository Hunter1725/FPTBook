using System.ComponentModel.DataAnnotations;
namespace FPTBook.Models;

public class ManageUserRolesViewModel
{
    public string RoleId { get; set; }
    public string RoleName { get; set; }
    public bool Selected { get; set; }
}