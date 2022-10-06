using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FPTBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPTBook.Models;

namespace FPTBook.Controllers;

public class HomeController : Controller
{
    private readonly FPTBookContext _context;

    public HomeController(FPTBookContext context)
    {
        _context = context;
    }

    // public HomeController(ILogger<HomeController> logger)
    // {
    //     _logger = logger;
    // }

    // public IActionResult Index()
    // {
    //     return View();
    // }


    public async Task<IActionResult> Index()
    {
        var fPTBookContext = _context.Book.Include(b => b.Category);
        return View(await fPTBookContext.ToListAsync());
    }


    public async Task<IActionResult> Detail(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Category)
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

    public IActionResult Help()
    {
        return View();
    }
    public IActionResult About()
    {
        return View();
    }
    public IActionResult Login()
    {
        return View();
    }
    public IActionResult Register()
    {
        return View();
    }
    // public IActionResult Detail()
    // {
    //     return View();
    // }
    public IActionResult Cart()
    {
        return View();
    }
    public IActionResult Profile()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
