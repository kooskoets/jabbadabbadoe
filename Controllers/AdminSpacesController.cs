using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JabbadabbadoeBooking.Data;
using JabbadabbadoeBooking.Models;
using JabbadabbadoeBooking.Services;

namespace JabbadabbadoeBooking.Controllers;

[Authorize(AuthenticationSchemes = "AdminBasic", Policy = "AdminOnly")]
public class AdminSpacesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IImageStorageService _storage;

    public AdminSpacesController(ApplicationDbContext db, IImageStorageService storage)
    {
        _db = db;
        _storage = storage;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _db.Spaces.Include(s => s.Photos).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var space = await _db.Spaces.Include(s => s.Photos).FirstOrDefaultAsync(s => s.Id == id);
        if (space == null) return NotFound();
        return View(space);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Space space, List<IFormFile> files)
    {
        if (!ModelState.IsValid) return View(space);
        _db.Spaces.Add(space);
        await _db.SaveChangesAsync();

        foreach (var file in files)
        {
            var path = await _storage.SaveImageAsync(file, space.Id);
            _db.SpacePhotos.Add(new SpacePhoto { SpaceId = space.Id, FilePath = path });
        }
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var space = await _db.Spaces.Include(s => s.Photos).FirstOrDefaultAsync(s => s.Id == id);
        if (space == null) return NotFound();
        return View(space);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Space space, List<IFormFile> files)
    {
        if (id != space.Id) return NotFound();
        if (!ModelState.IsValid) return View(space);

        _db.Update(space);
        await _db.SaveChangesAsync();

        foreach (var file in files)
        {
            var path = await _storage.SaveImageAsync(file, space.Id);
            _db.SpacePhotos.Add(new SpacePhoto { SpaceId = space.Id, FilePath = path });
        }
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var space = await _db.Spaces.Include(s => s.Photos).FirstOrDefaultAsync(s => s.Id == id);
        if (space == null) return NotFound();
        return View(space);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var space = await _db.Spaces.Include(s => s.Photos).FirstOrDefaultAsync(s => s.Id == id);
        if (space == null) return NotFound();

        foreach (var photo in space.Photos)
        {
            _storage.DeleteImage(photo.FilePath);
        }

        _db.Spaces.Remove(space);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
