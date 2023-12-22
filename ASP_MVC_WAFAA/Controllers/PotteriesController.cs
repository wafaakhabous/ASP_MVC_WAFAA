using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP_MVC_WAFAA.Data;
using Microsoft.Extensions.Caching.Memory;  // Add this line for MemoryCache
using ASP_MVC_WAFAA.Models;
using Microsoft.AspNetCore.Authorization;

namespace ASP_MVC_WAFAA.Controllers
{
    [Authorize]
    public class PotteriesController : Controller
    {
        private readonly ASP_MVC_WAFAAContext _context;
        private readonly ILogger<PotteriesController> _logger;
        private readonly IMemoryCache _cache;  // Add this line for MemoryCache
     //   public IMemoryCache _cache { get; private set; }  // Add this line for MemoryCache

        public PotteriesController(ASP_MVC_WAFAAContext context,  ILogger<PotteriesController> logger, IMemoryCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        // GET: Potteries
        public async Task<IActionResult> Index(string searchString)
{
    List<Pottery> potteries;

    // Check if a search string is provided
    if (!string.IsNullOrEmpty(searchString))
    {
        // Perform a case-insensitive search on the Title property
        potteries = await _context.Pottery
            .Where(p => EF.Functions.Like(p.Title, $"%{searchString}%"))
            .ToListAsync();

    }
    else
    {
        // Try to get the list of potteries from cache
        if (!_cache.TryGetValue("PotteriesList", out potteries))
        {
            // If not found in cache, query from the database
            potteries = await _context.Pottery.ToListAsync();

            // Cache the result for 5 minutes (adjust the duration as needed)
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            // Set the result in cache
            _cache.Set("PotteriesList", potteries, cacheEntryOptions);
            _logger.LogInformation("*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*Potteries list loaded from the database and cached.");
        }
        else
        {
            _logger.LogInformation("*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*Potteries list loaded from cache.");
        }
    }

    return View(potteries);
}


        // GET: Potteries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pottery == null)
            {
                return NotFound();
            }

            var pottery = await _context.Pottery
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pottery == null)
            {
                return NotFound();
            }

            return View(pottery);
        }

        // GET: Potteries/Create
        public IActionResult Create()
        {
            return View();
        }
        //*******************************************************************************************************
        //*******************************************************************************************************


        //*******************************************************************************************************
        // POST: Potteries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Price,ImageFile")] Pottery pottery, IFormFile imageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Handle image file upload
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Save the image to the wwwroot/images folder
                        var imagePath = $"wwwroot/images/{Guid.NewGuid().ToString()}_{imageFile.FileName}";
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        // Set the ImageUrl property of the Pottery model
                        pottery.ImageUrl = $"/images/{Path.GetFileName(imagePath)}";
                    }

                    // Add the pottery to the database
                    _context.Add(pottery);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*" +
                        "*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-Pottery created successfully: {PotteryId}*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*" +
                        "*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-**-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*", pottery.Id);
                    _cache.Remove("PotteriesList");
                   /* _cache.Set("PotteriesList", potteries, cacheEntryOptions);
                    cacheKeys.Keys.AddRange(potteries.Select(p => p.Id.ToString())); // Assuming Id is a unique identifier
                    _cache.Set("AllCacheKeys", cacheKeys);*/
                    /////////////////////////////////////////////////// HNA KAN7EYDOHA MN CACHE BACH I TREFRECHA F METHODE INDEX  ! ! ! ! 
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Log ModelState errors for debugging
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            _logger.LogError($"Model Error: {error.ErrorMessage}");
                        }
                    }

                    _logger.LogWarning("*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-**-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*Invalid ModelState during pottery creation");

                    // If ModelState is not valid, return to the Create view with the invalid model
                    return View(pottery);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-**-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*Exception occurred during pottery creation");

                return View(pottery);
            }
        }
        //*******************************************************************************************************
        //*******************************************************************************************************
        //*******************************************************************************************************



        // GET: Potteries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pottery == null)
            {
                return NotFound();
            }

            var pottery = await _context.Pottery.FindAsync(id);
            if (pottery == null)
            {
                return NotFound();
            }
            return View(pottery);
        }
        private bool PotteryExists(int id)
        {
            return _context.Pottery.Any(e => e.Id == id);
        }

        // POST: Potteries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Price")] Pottery pottery)
        {
            if (id != pottery.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pottery);
                    await _context.SaveChangesAsync();
                    _cache.Remove("PotteriesList");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PotteryExists(pottery.Id))
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
            return View(pottery);
        }

        // GET: Potteries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pottery == null)
            {
                return NotFound();
            }

            var pottery = await _context.Pottery
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pottery == null)
            {
                return NotFound();
            }

            return View(pottery);
        }

        // POST: Potteries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pottery == null)
            {
                return Problem("Entity set 'ASP_MVC_WAFAAContext.Pottery'  is null.");
            }
            var pottery = await _context.Pottery.FindAsync(id);
            if (pottery != null)
            {
                _context.Pottery.Remove(pottery);
            }
            
            await _context.SaveChangesAsync();
            _cache.Remove("PotteriesList");
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult AddToCart(int potteryId)
        {
            var pottery = _context.Pottery.Find(potteryId);

            if (pottery != null)
            {
                var cart = HttpContext.Session.Get<Cart>("Cart") ?? new Cart();

                // Check if the item already exists in the cart
                var existingItem = cart.Items.FirstOrDefault(item => item.PotteryId == pottery.Id);

                if (existingItem != null)
                {
                    // If the item exists, update its quantity
                    existingItem.Quantity++;
                }
                else
                {
                    // If the item doesn't exist, add it to the cart
                    cart.Items.Add(new CartItem
                    {
                        PotteryId = pottery.Id,
                        PotteryTitle = pottery.Title,
                        PotteryPrice = pottery.Price,
                        Quantity = 1 // You can set the default quantity as needed
                    });
                }

                HttpContext.Session.Set("Cart", cart);

                return RedirectToAction("Index", "Potteries");
            }

            return NotFound();
        }

        /*********************** REGARDER LE CACHE ICIIIIIII *******************************/
        public IActionResult ViewCacheContents()
        {
            // Créez une instance de la classe pour contenir les clés du cache
            var cacheKeys = _cache.Get<CacheKeyHolder>("AllCacheKeys") ?? new CacheKeyHolder();
            // Récupérez toutes les clés du cache
            var allKeys = cacheKeys.Keys;

            // Affichez les clés dans la sortie de journalisation (log)
            foreach (var key in allKeys)
            {
                var cachedItem = _cache.Get<object>(key);
                _logger.LogInformation($"Cache Key: {key}, Value: {cachedItem}");
            }
            // var allKeysList = allKeys.ToList(); // Convert to List<string>
            var allKeysList = allKeys.Select(key => key.ToString()).ToList(); // Convert to List<string>

            // Vous pouvez également retourner les clés ou le contenu du cache à la vue si vous préférez
            return View(allKeysList);
            //return Content(allKeysList.ToString+"Cache contents displayed in the log. Check your logging output.");
        }
    }
}
