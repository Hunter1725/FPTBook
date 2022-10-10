using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPTBook.Models;

// using OfficeOpenXml;
// using OfficeOpenXml.Style;


namespace FPTBook.Controllers
{
    public class UsersController : Controller
    {
        private readonly FPTBookContext _context;
        private readonly IWebHostEnvironment hostEnvironment;

        public UsersController(FPTBookContext context, IWebHostEnvironment environment)
        {
            _context = context;
            hostEnvironment = environment;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return _context.User != null ?
                        View(await _context.User.ToListAsync()) :
                        Problem("Entity set 'FPTBookContext.User'  is null.");
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password,HomeAddress,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password,HomeAddress,Role")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'FPTBookContext.User'  is null.");
            }
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (_context.User?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult ReportDemo()
        {
            var data = _context.OrderItem.Include(s => s.Book)
                        .GroupBy(s => s.Book.Title)
                        .Select(g => new { Title = g.Key, Total = g.Sum(s => s.Quantity*s.Book.Price), TotalQuantity= g.Sum(s => s.Quantity)})
                        .ToList();

            string[] labels = new string[data.Count];
            string[] totalquantity = new string[data.Count];
            string[] totals = new string[data.Count];


            for (int i = 0; i < data.Count; i++)
            {
                labels[i] = data[i].Title;
                totalquantity[i] = data[i].TotalQuantity.ToString();
                totals[i] = data[i].Total.ToString();

            }

            ViewData["labels"] = string.Format("'{0}'", String.Join("','", labels));
            ViewData["totalquantity"] = String.Join(",", totalquantity);
            ViewData["totals"] = String.Join(",", totals);

            return View(data);
        }

        public IActionResult ExportBookList()
        {
            // Get the book list 

            var queryBook = _context.Book.Include(m => m.Category);
            List<Book> books= queryBook.ToList();

            var queryOrderItem = _context.OrderItem.Include(b => b.Book)
                                                    .GroupBy(s => s.Book.Title)
                                                    .Select(g => new { Title = g.Key, Category = g.Select(s=>s.Book.Category.Name), Total = g.Sum(s => s.Quantity*s.Book.Price), TotalQuantity= g.Sum(s => s.Quantity)})
                                                    .ToList();                                                           
            var stream = new MemoryStream();
            using (var xlPackage = new ExcelPackage(stream))
            {
                var worksheet = xlPackage.Workbook.Worksheets.Add("Books");
                var namedStyle = xlPackage.Workbook.Styles.CreateNamedStyle("HyperLink");
                namedStyle.Style.Font.UnderLine = true;                
                const int startRow = 5;
                var row = startRow;

                //Create Headers and format them
                worksheet.Cells["A1"].Value = "Report on the number of books sold";
                using (var r = worksheet.Cells["A1:D1"])
                {
                    r.Merge = true;
                    r.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous;
                 }

                worksheet.Cells["A4"].Value = "Title";
                worksheet.Cells["B4"].Value = "Category";
                worksheet.Cells["C4"].Value = "Total Quantity";
                worksheet.Cells["D4"].Value = "Total Price";
                worksheet.Cells["A4:D4"].Style.Font.Bold = true;

                row = 5;    
                foreach (var OrderItem in queryOrderItem)
                {
                        worksheet.Cells[row, 1].Value = OrderItem.Title;
                        worksheet.Cells[row, 2].Value = OrderItem.Category;
                        worksheet.Cells[row, 3].Value = OrderItem.TotalQuantity;
                        worksheet.Cells[row, 4].Value = OrderItem.Total;

                        row++;
                }  
                // save the new spreadsheet
                xlPackage.Save();
                // Response.Clear();
                
            }
            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "books.xlsx");
        }
    }
}
