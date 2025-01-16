using System.Diagnostics;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Profescipta.Data;
using Profescipta.Models;
using Profescipta.ViewModel;

namespace Profescipta.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly AppDbContext _dbContext;
        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult ExportToExcel()
        {
            // Ambil data dari database (sesuaikan query dengan kebutuhan Anda)
            var orders = _dbContext.SO_ORDER
                .Select(o => new
                {
                    o.SO_ORDER_ID,
                    o.ORDER_NO,
                    o.ORDER_DATE,
                    o.ADDRESS,
                    CustomerName = o.COM_CUSTOMER.CUSTOMER_NAME
                })
                .ToList();

            // Buat workbook menggunakan ClosedXML
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Orders");
                var currentRow = 1;

                // Header
                worksheet.Cell(currentRow, 1).Value = "Order ID";
                worksheet.Cell(currentRow, 2).Value = "Order No";
                worksheet.Cell(currentRow, 3).Value = "Order Date";
                worksheet.Cell(currentRow, 4).Value = "Address";
                worksheet.Cell(currentRow, 5).Value = "Customer Name";

                // Data
                foreach (var order in orders)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = order.SO_ORDER_ID;
                    worksheet.Cell(currentRow, 2).Value = order.ORDER_NO;
                    worksheet.Cell(currentRow, 3).Value = order.ORDER_DATE.ToString("yyyy-MM-dd");
                    worksheet.Cell(currentRow, 4).Value = order.ADDRESS;
                    worksheet.Cell(currentRow, 5).Value = order.CustomerName;
                }

                // Simpan workbook ke MemoryStream
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    // Kembalikan file sebagai respons
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
                }
            }
        }
        public ActionResult Index(string searchTerm, DateTime? searchDate)
        {
            var ordersQuery = _dbContext.SO_ORDER
        .Include(o => o.COM_CUSTOMER)
        .AsQueryable();

            // Filter berdasarkan string (ORDER_NO atau CUSTOMER_NAME)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ordersQuery = ordersQuery.Where(o =>
                    o.ORDER_NO.Contains(searchTerm) ||
                    o.COM_CUSTOMER.CUSTOMER_NAME.Contains(searchTerm));
            }

            // Filter berdasarkan tanggal (ORDER_DATE)
            if (searchDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.ORDER_DATE.Date == searchDate.Value.Date);
            }

            var orders = ordersQuery
                .Select(o => new OrderViewModel
                {
                    SO_ORDER = o,
                    SO_ITEM = o.SO_ITEM != null ? o.SO_ITEM.ToList() : new List<SO_ITEM>() // Assuming SO_ITEM is a collection
                })
                .ToList();

            return View(orders);
        }

        public ActionResult Create()
        {
            ViewBag.Customers = _dbContext.COM_CUSTOMER.ToList();
            return View(new OrderViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine($"Order No: {model.SO_ORDER.ORDER_NO}");
                Console.WriteLine($"Order Items Count: {model.SO_ITEM.Count}");
                _dbContext.SO_ORDER.Add(model.SO_ORDER);

                foreach (var item in model.SO_ITEM)
                {
                    Console.WriteLine($"Item Name: {item.ITEM_NAME}, Quantity: {item.QUANTITY}, Price: {item.PRICE}");
                    item.SO_ORDER_ID = model.SO_ORDER.SO_ORDER_ID;
                    _dbContext.SO_ITEM.Add(item);
                }

                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                // Tampilkan error jika model tidak valid
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            ViewBag.Customers = _dbContext.COM_CUSTOMER.ToList();
            return View(model);
        }

        // UPDATE: Display form to edit an existing order
        public ActionResult Edit(long id)
        {
            var order = _dbContext.SO_ORDER.Find(id);
            if (order == null) return NotFound();

            var model = new OrderViewModel
            {
                SO_ORDER = order,
                SO_ITEM = _dbContext.SO_ITEM.Where(i => i.SO_ORDER_ID == id).ToList()
            };

            ViewBag.Customers = _dbContext.COM_CUSTOMER.ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingOrder = _dbContext.SO_ORDER.Find(model.SO_ORDER.SO_ORDER_ID);
                if (existingOrder == null) return NotFound();

                // Update order fields
                existingOrder.ORDER_NO = model.SO_ORDER.ORDER_NO;
                existingOrder.ORDER_DATE = model.SO_ORDER.ORDER_DATE;
                existingOrder.COM_CUSTOMER_ID = model.SO_ORDER.COM_CUSTOMER_ID;
                existingOrder.ADDRESS = model.SO_ORDER.ADDRESS;

                // Remove existing items and add new ones
                var existingItems = _dbContext.SO_ITEM.Where(i => i.SO_ORDER_ID == model.SO_ORDER.SO_ORDER_ID).ToList();
                _dbContext.SO_ITEM.RemoveRange(existingItems);

                foreach (var item in model.SO_ITEM)
                {
                    item.SO_ORDER_ID = model.SO_ORDER.SO_ORDER_ID;
                    _dbContext.SO_ITEM.Add(item);
                }

                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Customers = _dbContext.COM_CUSTOMER.ToList();
            return View(model);
        }

        // DELETE: Delete an existing order
        public ActionResult Delete(long id)
        {
            var order = _dbContext.SO_ORDER.Find(id);
            if (order == null) return NotFound();

            var items = _dbContext.SO_ITEM.Where(i => i.SO_ORDER_ID == id).ToList();
            _dbContext.SO_ITEM.RemoveRange(items);

            _dbContext.SO_ORDER.Remove(order);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        // DETAILS: Display details of an order
        public ActionResult Details(long id)
        {
            var order = _dbContext.SO_ORDER.Find(id);
            if (order == null) return NotFound();

            var model = new OrderViewModel
            {
                SO_ORDER = order,
                SO_ITEM = _dbContext.SO_ITEM.Where(i => i.SO_ORDER_ID == id).ToList()
            };

            return View(model);
        }
    

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
