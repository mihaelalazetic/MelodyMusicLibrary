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
            var songs = _context.Song
             .Include(s => s.Album)
             .Include(s => s.SongArtists)
                 .ThenInclude(sa => sa.Artist)
             .ToList();

            var viewModelList = songs.Select(song => new SongViewModel(song)).ToList();

            return View(viewModelList);
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
                 .Include(s => s.SongArtists)
                .ThenInclude(sa => sa.Artist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            var viewModelList = new SongViewModel(song);

            return View(viewModelList);
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
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Song
                .Include(s => s.SongArtists)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (song == null)
            {
                return NotFound();
            }

            var viewModel = new SongEditViewModel
            {
                Id = song.Id,
                Title = song.Title,
                Duration = song.Duration,
                AlbumId = song.AlbumId,
                ArtistList = await _context.Artist.OrderBy(a => a.Name)
                                                  .Select(a => new SelectListItem
                                                  {
                                                      Value = a.Id.ToString(),
                                                      Text = a.Name
                                                  }).ToListAsync(),
                SelectedArtistId = song.SongArtists.FirstOrDefault()?.ArtistId ?? 0 // Assuming only one artist can be selected
            };

            ViewData["AlbumId"] = new SelectList(_context.Album, "Id", "Title", song.AlbumId);

            return View(viewModel);
        }



        // POST: Songs/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SongEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var song = await _context.Song
                        .Include(s => s.SongArtists)
                        .FirstOrDefaultAsync(s => s.Id == id);

                    if (song == null)
                    {
                        return NotFound();
                    }

                    song.Title = viewModel.Title;
                    song.Duration = viewModel.Duration;
                    song.AlbumId = viewModel.AlbumId;

                    // Update selected artist
                    if (viewModel.SelectedArtistId != 0)
                    {
                        // Clear existing artists and add the new one
                        song.SongArtists.Clear();
                        song.SongArtists.Add(new SongArtist { ArtistId = viewModel.SelectedArtistId });
                    }
                    else
                    {
                        song.SongArtists.Clear(); // No artist selected, clear the association
                    }

                    _context.Update(song);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // If ModelState is not valid, re-populate the dropdowns
            viewModel.ArtistList = await _context.Artist.OrderBy(a => a.Name)
                                                        .Select(a => new SelectListItem
                                                        {
                                                            Value = a.Id.ToString(),
                                                            Text = a.Name
                                                        }).ToListAsync();

            ViewData["AlbumId"] = new SelectList(_context.Album, "Id", "Title", viewModel.AlbumId);
            return View(viewModel);
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
