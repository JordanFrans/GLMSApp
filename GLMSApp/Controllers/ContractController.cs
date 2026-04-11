using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GLMSApp.Data;

namespace GLMSApp.Controllers
{
    public class ContractController : Controller
    {
        private readonly GLMSAppContext _context;

        public ContractController(GLMSAppContext context)
        {
            _context = context;
        }

        // GET: Contract
        public async Task<IActionResult> Index()
        {
            var contracts = _context.Contract.Include(c => c.Client);
            return View(await contracts.ToListAsync());
        }

        // GET: Contract/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var contract = await _context.Contract
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contract == null)
                return NotFound();

            return View(contract);
        }

        // GET: Contract/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Set<Client>(), "Id", "Id");
            return View();
        }

        // POST: Contract/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartDate,EndDate,Status,ClientId,FilePath")] Contract contract)
        {
            // ✅ Workflow Rule
            if (contract.Status == "Expired" || contract.Status == "On Hold")
            {
                return BadRequest("Invalid contract status.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(_context.Set<Client>(), "Id", "Id", contract.ClientId);
            return View(contract);
        }

        // GET: Contract/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var contract = await _context.Contract.FindAsync(id);
            if (contract == null)
                return NotFound();

            ViewData["ClientId"] = new SelectList(_context.Set<Client>(), "Id", "Id", contract.ClientId);
            return View(contract);
        }

        // POST: Contract/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate,Status,ClientId,FilePath")] Contract contract)
        {
            if (id != contract.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(contract);
        }

        // GET: Contract/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var contract = await _context.Contract
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contract == null)
                return NotFound();

            return View(contract);
        }

        // POST: Contract/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contract.FindAsync(id);

            if (contract != null)
                _context.Contract.Remove(contract);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ✅ FILE UPLOAD
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, int id)
        {
            if (file != null && file.Length > 0)
            {
                var path = Path.Combine("wwwroot/files", file.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var contract = await _context.Contract.FindAsync(id);
                contract.FilePath = file.FileName;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // ✅ LINQ SEARCH
        public IActionResult Search(DateTime? startDate, string status)
        {
            var contracts = _context.Contract.AsQueryable();

            if (startDate.HasValue)
                contracts = contracts.Where(c => c.StartDate >= startDate);

            if (!string.IsNullOrEmpty(status))
                contracts = contracts.Where(c => c.Status == status);

            return View(contracts.ToList());
        }

        // ✅ API INTEGRATION
        public async Task<decimal> ConvertToZAR(decimal amount)
        {
            var client = new HttpClient();
            var response = await client.GetStringAsync("https://api.exchangerate-api.com/v4/latest/USD");

            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
            decimal rate = data.rates.ZAR;

            return amount * rate;
        }
    }
}