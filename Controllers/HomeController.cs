using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FPTBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using FPTBook.Utils;
using FPTBook.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FPTBook.Controllers;

public class HomeController : Controller
{
    private readonly FPTBookIdentityDbContext _context;
    private readonly UserManager<BookUser> _userManager;

    public HomeController(FPTBookIdentityDbContext context, UserManager<BookUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // public HomeController(ILogger<HomeController> logger)
    // {
    //     _logger = logger;
    // }

    // public IActionResult Index()
    // {
    //     return View();
    // }


    public async Task<IActionResult> Index(string searchString)
    {
        // var fPTBookContext = _context.Book.Include(b => b.Category);
        // return View(await fPTBookContext.ToListAsync());
        var fPTBookContext = from m in _context.Book.Include(a => a.Category)
                                                    .Include(b => b.Author)
                                                    .Include(c => c.Publisher)
                             select m;

        if (!String.IsNullOrEmpty(searchString))
        {
            fPTBookContext = fPTBookContext.Where(s => s.Title!.Contains(searchString));
        }

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
    [Authorize(Roles = "Customer, StoreOwner, Admin")]
    public IActionResult Cart()
    {
        return View();
    }
    [Authorize(Roles = "Customer, StoreOwner, Admin")]
    public IActionResult Profile()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    [Authorize(Roles = "Customer, StoreOwner, Admin")]
    [HttpPost]
    public IActionResult AddBook(int id, string name, string poster, string author, decimal price, int quantity)
    {
        ShoppingCart myCart;
        // If the cart is not in the session, create one and put it there
        // Otherwise, get it from the session
        if (HttpContext.Session.GetObject<ShoppingCart>("cart") == null)
        {
            myCart = new ShoppingCart();
            HttpContext.Session.SetObject("cart", myCart);
        }
        myCart = (ShoppingCart)HttpContext.Session.GetObject<ShoppingCart>("cart");
        var newItem = myCart.AddItem(id, name, poster, author, price, quantity);
        HttpContext.Session.SetObject("cart", myCart);
        ViewData["newItem"] = newItem;
        return View();
    }

    [Authorize(Roles = "Customer, StoreOwner, Admin")]
    public async Task<IActionResult> CheckOut()
    {
        ShoppingCart cart = (ShoppingCart)HttpContext.Session.GetObject<ShoppingCart>("cart");
        ViewData["uid"] = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var users = await _userManager.Users.Where(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToListAsync();
        var userRolesViewModel = new List<UserRolesViewModel>();
        foreach (BookUser user in users)
        {
            var thisViewModel = new UserRolesViewModel();
            thisViewModel.UserId = user.Id;
            thisViewModel.Name = user.Name;
            thisViewModel.PhoneNumber = user.PhoneNumber;
            thisViewModel.Address = user.Address;
            userRolesViewModel.Add(thisViewModel);
        }

        if (cart != null)
        {
            ViewData["myItems"] = cart.Items;
            return View(userRolesViewModel);
        }
        else
        {
            return View("EmptyCart");
        }
    }

    [Authorize(Roles = "Customer, StoreOwner, Admin")]
    public IActionResult PlaceOrder(decimal total, string fullname, string address, string phone, string cusid)
    {
        ShoppingCart cart = (ShoppingCart)HttpContext.Session.GetObject<ShoppingCart>("cart");
        Order myOrder = new Order();
        myOrder.OrderTime = DateTime.Now;
        myOrder.Total = total;
        myOrder.Fullname = fullname;
        myOrder.Address = address;
        myOrder.UserID = cusid;
        myOrder.Phone = phone;
        myOrder.State = "Delivering";
        _context.Order.Add(myOrder);
        _context.SaveChanges();//this generates the Id for Order

        foreach (var item in cart.Items)
        {
            OrderItem myOrderItem = new OrderItem();
            myOrderItem.BookID = item.ID;
            myOrderItem.Quantity = item.Quantity;
            myOrderItem.OrderID = myOrder.Id;//id of saved order above

            _context.OrderItem.Add(myOrderItem);
        }
        _context.SaveChanges();
        //empty shopping cart
        cart = new ShoppingCart();
        HttpContext.Session.SetObject("cart", cart);
        return View();
    }
    [HttpPost]
    public RedirectToActionResult EditOrder(int id, int quantity)
    {
        ShoppingCart cart = (ShoppingCart)HttpContext.Session.GetObject<ShoppingCart>("cart");
        cart.EditItem(id, quantity);
        HttpContext.Session.SetObject("cart", cart);

        return RedirectToAction("CheckOut", "Home");
    }
    [HttpPost]
    public RedirectToActionResult RemoveOrderItem(int id)
    {
        ShoppingCart cart = (ShoppingCart)HttpContext.Session.GetObject<ShoppingCart>("cart");
        cart.RemoveItem(id);
        HttpContext.Session.SetObject("cart", cart);

        return RedirectToAction("CheckOut", "Home");
    }

    [Authorize(Roles = "Customer, StoreOwner, Admin")]
    public async Task<IActionResult> Record()
    {
        var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return _context.Order != null ?
                    View(await _context.Order
                    .Where(o => o.UserID == userID)
                    .ToListAsync()) :
                    Problem("Entity set 'FPTBookContext.Order'  is null.");
    }
    [Authorize(Roles = "Customer, StoreOwner, Admin")]
    public async Task<IActionResult> OrderDetail(int id)
    {
        var fPTBookContext = _context.OrderItem.Where(e => e.Order.Id == id).Include(b => b.Book).Include(o => o.Order).Include(c => c.Book.Author);
        return View(await fPTBookContext.ToListAsync());
    }
}
