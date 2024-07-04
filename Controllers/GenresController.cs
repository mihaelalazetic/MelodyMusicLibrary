using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MelodyMusicLibrary.Models;
using MelodyMusicLibrary.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using MelodyMusicLibrary.Data;

public class GenresController : Controller
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly MelodyContext _context; // Assuming you have a DbContext for your application

    public GenresController(IWebHostEnvironment webHostEnvironment, MelodyContext context)
    {
        _webHostEnvironment = webHostEnvironment;
        _context = context;
    }

    // GET: Genres
    public IActionResult Index()
    {
        var genres = _context.Genre.ToList();
        return View(genres);
    }

    // GET: Genres/Details/5
    public IActionResult Details(int id)
    {
        var genre = _context.Genre.FirstOrDefault(g => g.Id == id);
        if (genre == null)
        {
            return NotFound();
        }
        return View(genre);
    }

    // GET: Genres/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Genres/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GenreViewModel model)
    {

        if (ModelState.IsValid)
        {
            string fileName = model.Name;
            await SaveImageFile(model.ImageFile, fileName);
            
            Genre genre = new Genre
            {
                Name = model.Name,
                ImageUrl = fileName != null ? "/images/genres/" + fileName : null
            };

            _context.Add(genre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // GET: Genre/Edit/5
    public IActionResult Edit(int id)
    {
        var genre = _context.Genre.FirstOrDefault(g => g.Id == id);
        if (genre == null)
        {
            return NotFound();
        }

        GenreViewModel model = new GenreViewModel
        {
            Id = genre.Id,
            Name = genre.Name,
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GenreViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        foreach (var key in ModelState.Keys)
        {
            var state = ModelState[key];
            foreach (var error in state.Errors)
            {
                Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
            }
        }
        if (ModelState.IsValid)
        {
            var genre = await _context.Genre.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            string fileName = null;
            if (model.ImageFile != null)
            {
                fileName = model.Name;
                await SaveImageFile(model.ImageFile,fileName);
                genre.ImageUrl = "/images/genres/" + fileName;
            }

            genre.Name = model.Name;

            _context.Update(genre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        else
        {
            // Log model state errors
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"Error: {error.ErrorMessage}");
            }
        }

        return View(model);
    }


    // GET: Genres/Delete/5
    public IActionResult Delete(int id)
    {
        var genre = _context.Genre.FirstOrDefault(g => g.Id == id);
        if (genre == null)
        {
            return NotFound();
        }

        return View(genre);
    }

    // POST: Genres/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var genre = _context.Genre.FirstOrDefault(g => g.Id == id);
        if (genre == null)
        {
            return NotFound();
        }

        _context.Genre.Remove(genre);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<string> SaveImageFile(IFormFile imageFile,string genreName)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            return null;
        }

        string wwwRootPath = _webHostEnvironment.WebRootPath;
        string fileName = genreName + Path.GetExtension(imageFile.FileName);
        string filePath = Path.Combine(wwwRootPath, "images", "genres", fileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(fileStream);
        }

        return fileName;
    }

}
