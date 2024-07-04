using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MelodyMusicLibrary.Data;
using MelodyMusicLibrary.Models;
using MelodyMusicLibrary.ViewModels;

namespace MelodyMusicLibrary.Controllers
{
    public class SongsController : Controller
    {
        private readonly MelodyContext _context;

        public SongsController(MelodyContext context)
        {
            _context = context;
        }

        // GET: Songs
        public async Task<IActionResult> Index()
        {
            var melodyContext = _context.Song.Include(s => s.Album);
            return View(await melodyContext.ToListAsync());
        }

        // GET: Songs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Song
                .Include(s => s.Album)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // GET: Songs/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new SongCreateViewModel();

            // Populate SelectList for albums
            viewModel.AlbumList = new SelectList(await _context.Album.OrderBy(a => a.Title)
                                                                     .Select(a => new SelectListItem
                                                                     {
                                                                         Value = a.Id.ToString(),
                                                                         Text = a.Title
                                                                     }).ToListAsync(), "Value", "Text");

            // Populate SelectList for artists
            viewModel.ArtistList = new SelectList(await _context.Artist.OrderBy(a => a.Name)
                                                                       .Select(a => new SelectListItem
                                                                       {
                                                                           Value = a.Id.ToString(),
                                                                           Text = a.Name
                                                                       }).ToListAsync(), "Value", "Text");

            // Populate SelectList for genres
            viewModel.GenreList = new SelectList(await _context.Genre.OrderBy(g => g.Name)
                                                                     .Select(g => new SelectListItem
                                                                     {
                                                                         Value = g.Id.ToString(),
                                                                         Text = g.Name
                                                                     }).ToListAsync(), "Value", "Text");

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SongCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var song = new Song
                {
                    Title = viewModel.Title,
                    Duration = viewModel.Duration,
                    AlbumId = viewModel.AlbumId
                };

                // Add selected artists to the song
                if (viewModel.SelectedArtists != null)
                {
                    song.SongArtists = new List<SongArtist>();
                    foreach (var artistId in viewModel.SelectedArtists)
                    {
                        song.SongArtists.Add(new SongArtist { ArtistId = artistId });
                    }
                }

                // Add selected genres to the song
                if (viewModel.SelectedGenres != null)
                {
                    song.SongGenres = new List<SongGenre>();
                    foreach (var genreId in viewModel.SelectedGenres)
                    {
                        song.SongGenres.Add(new SongGenre { GenreId = genreId });
                    }
                }

                _context.Add(song);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate SelectLists if model state is invalid
            viewModel.AlbumList = new SelectList(await _context.Album.OrderBy(a => a.Title)
                                                                     .Select(a => new SelectListItem
                                                                     {
                                                                         Value = a.Id.ToString(),
                                                                         Text = a.Title
                                                                     }).ToListAsync(), "Value", "Text");

            viewModel.ArtistList = new SelectList(await _context.Artist.OrderBy(a => a.Name)
                                                                       .Select(a => new SelectListItem
                                                                       {
                                                                           Value = a.Id.ToString(),
                                                                           Text = a.Name
                                                                       }).ToListAsync(), "Value", "Text");

            viewModel.GenreList = new SelectList(await _context.Genre.OrderBy(g => g.Name)
                                                                     .Select(g => new SelectListItem
                                                                     {
                                                                         Value = g.Id.ToString(),
                                                                         Text = g.Name
                                                                     }).ToListAsync(), "Value", "Text");

            return View(viewModel);
        }

        // GET: Songs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Song.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            ViewData["AlbumId"] = new SelectList(_context.Album, "Id", "Title", song.AlbumId);
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Duration,AlbumId")] Song song)
        {
            if (id != song.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(song);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongExists(song.Id))
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
            ViewData["AlbumId"] = new SelectList(_context.Album, "Id", "Title", song.AlbumId);
            return View(song);
        }

        // GET: Songs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Song
                .Include(s => s.Album)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var song = await _context.Song.FindAsync(id);
            if (song != null)
            {
                _context.Song.Remove(song);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SongExists(int id)
        {
            return _context.Song.Any(e => e.Id == id);
        }

        // SongsController.cs

        public IActionResult ListByAlbum(int id)
        {
            var album = _context.Album
                .Include(a => a.Songs)
                .FirstOrDefault(a => a.Id == id);

            if (album == null)
            {
                return NotFound();
            }

            return View(album.Songs);
        }

    }
}
