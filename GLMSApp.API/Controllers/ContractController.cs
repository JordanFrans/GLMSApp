using GLMSApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GLMSApp.API.Controllers
{
    public class ContractsController : Controller
    {
        private readonly GLMSAppContext _context;

        public object Newtonsoft { get; private set; }

        public ContractsController(GLMSAppContext context)
        {
            _context = context;
        }

        // GET: Contracts
        public async Task<IActionResult> Index()
        {
            var gLMSAppContext = _context.Contract.Include(c => c.Client);
            return View(await gLMSAppContext.ToListAsync());
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contract
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // GET: Contracts/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Set<Client>(), "Id", "Id");
            return View();
        }

        // POST: Contracts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartDate,EndDate,Status,ClientId,FilePath")] Contract contract)
        {
            // 🔴 Workflow Rule
            if (contract.Status == "Expired" || contract.Status == "On Hold")
            {
                return BadRequest("Cannot create contract with invalid status.");
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

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contract.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            ViewData["ClientId"] = new SelectList(_context.Set<Client>(), "Id", "Id", contract.ClientId);
            return View(contract);
        }

        // POST: Contracts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate,Status,ClientId,FilePath")] Contract contract)
        {
            if (id != contract.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.Id))
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

            ViewData["ClientId"] = new SelectList(_context.Set<Client>(), "Id", "Id", contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contract
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contract.FindAsync(id);

            if (contract != null)
            {
                _context.Contract.Remove(contract);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 🔵 FILE UPLOAD
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

        // 🔵 SEARCH (LINQ)
        public IActionResult Search(DateTime? startDate, string status)
        {
            var contracts = _context.Contract.AsQueryable();

            if (startDate.HasValue)
                contracts = contracts.Where(c => c.StartDate >= startDate);

            if (!string.IsNullOrEmpty(status))
                contracts = contracts.Where(c => c.Status == status);

            return View(contracts.ToList());
        }

        // 🔵 EXTERNAL API (Currency Conversion)
        public async Task<decimal> ConvertToZAR(decimal amount)
        {
            using var client = new HttpClient();
            var response = await client.GetStringAsync("https://api.exchangerate-api.com/v4/latest/USD");
            using var doc = JsonDocument.Parse(response);
            var rate = doc.RootElement.GetProperty("rates").GetProperty("ZAR").GetDecimal();
            return amount * rate;
        }

        private bool ContractExists(int id)
        {
            return _context.Contract.Any(e => e.Id == id);
        }
    }
}