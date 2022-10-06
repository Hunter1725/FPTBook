using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FPTBook.Models;

namespace FPTBook.Controllers;

public class StoreOwnerController : Controller
{
    private readonly ILogger<StoreOwnerController> _logger;

    public StoreOwnerController(ILogger<StoreOwnerController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ManageBook()
    {
        return View();
    }
    public IActionResult ManageAuthor()
    {
        return View();
    }
    public IActionResult ManagePublisher()
    {
        return View();
    }
    public IActionResult RequestCategory()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
