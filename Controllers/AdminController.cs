using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FPTBook.Models;
using FPTBook.Areas.Identity.Data;

using System.Linq;
using System.Threading.Tasks;

using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace FPTBook.Controllers;

public class AdminController : Controller
{
    // private readonly ILogger<AdminController> _logger;
    
    // public AdminController(ILogger<AdminController> logger)
    // {
    //     _logger = logger;
    // }

    private readonly FPTBookIdentityDbContext _context;
    private readonly IWebHostEnvironment hostEnvironment;

    public AdminController(FPTBookIdentityDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            hostEnvironment = environment;
        }


    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ManageUser()
    {
        return View();
    }
    public IActionResult ManageCategory()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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