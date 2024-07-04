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
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace MelodyMusicLibrary.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly MelodyContext _context;

        public AlbumsController(MelodyContext context)
        {
            _context = context;
        }

        // GET: Albums
        public async Task<IActionResult> Index(AlbumSearchViewModel searchModel)
        {
            // Query albums from database
            var query = _context.Album
                                .Include(a => a.AlbumArtists).ThenInclude(aa => aa.Artist)
                                .Include(a => a.AlbumGenres).ThenInclude(ag => ag.Genre)
                                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchModel.Title))
            {
                query = query.Where(a => a.Title.Contains(searchModel.Title));
            }

            if (!string.IsNullOrEmpty(searchModel.Artist))
            {
                query = query.Where(a => a.AlbumArtists.Any(aa => aa.Artist.Name.Contains(searchModel.Artist)));
            }

            if (!string.IsNullOrEmpty(searchModel.Genre))
            {
                query = query.Where(a => a.AlbumGenres.Any(ag => ag.Genre.Name.Contains(searchModel.Genre)));
            }

            // Populate SelectList for genres
            var genres = await _context.Genre.OrderBy(g => g.Name)
                                              .Select(g => new SelectListItem
                                              {
                                                  Value = g.Name,
                                                  Text = g.Name
                                              }).ToListAsync();
            genres.Insert(0, new SelectListItem { Value = "", Text = "Genre" });
            searchModel.Genres = new SelectList(genres, "Value", "Text");

            // Populate SelectList for artists
            var artists = await _context.Artist.OrderBy(a => a.Name)
                                                .Select(a => new SelectListItem
                                                {
                                                    Value = a.Name,
                                                    Text = a.Name
                                                }).ToListAsync();
            artists.Insert(0, new SelectListItem { Value = "", Text = "Artist" });
            searchModel.Artists = new SelectList(artists, "Value", "Text");

            // Retrieve albums matching the query
            searchModel.Albums = await query.ToListAsync();

            return View(searchModel);
        }

        // GET: Albums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Album
            .Include(a => a.AlbumArtists)
                .ThenInclude(aa => aa.Artist)
            .Include(a => a.AlbumGenres)
                .ThenInclude(ag => ag.Genre)
            .Include(a => a.Songs)
            .FirstOrDefaultAsync(a => a.Id == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // GET: Albums/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var artists = await _context.Artist.OrderBy(a => a.Name)
                                               .Select(a => new SelectListItem
                                               {
                                                   Value = a.Id.ToString(),
                                                   Text = a.Name
                                               }).ToListAsync();

            var genres = await _context.Genre.OrderBy(g => g.Name)
                                             .Select(g => new SelectListItem
                                             {
                                                 Value = g.Id.ToString(),
                                                 Text = g.Name
                                             }).ToListAsync();

            var viewModel = new AlbumCreateViewModel
            {
                ArtistList = new SelectList(artists, "Value", "Text"),
                GenresList = new SelectList(genres, "Value", "Text")
            };

            return View(viewModel);
        }

        // POST: Albums/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AlbumCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Map ViewModel to Album model
                var album = new Album
                {
                    Title = viewModel.Title,
                    ReleaseDate = viewModel.ReleaseDate,
                    CoverUrl = viewModel.CoverUrl
                };

                // Add selected artists to the album
                if (viewModel.SelectedArtist != null)
                {
                    foreach (var artistId in viewModel.SelectedArtist)
                    {
                        album.AlbumArtists.Add(new AlbumArtist { ArtistId = artistId });
                    }
                }

                // Add selected genres to the album
                if (viewModel.SelectedGenres != null)
                {
                    foreach (var genreId in viewModel.SelectedGenres)
                    {
                        album.AlbumGenres.Add(new AlbumGenre { GenreId = genreId });
                    }
                }

                _context.Add(album);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If model state is not valid, re-render the view with errors
            viewModel.ArtistList = new SelectList(await _context.Artist.OrderBy(a => a.Name)
                                                                       .Select(a => new SelectListItem
                                                                       {
                                                                           Value = a.Id.ToString(),
                                                                           Text = a.Name
                                                                       }).ToListAsync(), "Value", "Text");

            viewModel.GenresList = new SelectList(await _context.Genre.OrderBy(g => g.Name)
                                                                     .Select(g => new SelectListItem
                                                                     {
                                                                         Value = g.Id.ToString(),
                                                                         Text = g.Name
                                                                     }).ToListAsync(), "Value", "Text");

            return View(viewModel);
        }

        // GET: Albums/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Album.Where(a => a.Id == id)
                .Include(a => a.AlbumArtists)
                .Include(a => a.AlbumGenres)
                .FirstOrDefaultAsync();
            if (album == null)
            {
                return NotFound();
            }

            var artists = _context.Artist.AsEnumerable().OrderBy(s => s.Name);
            var genres = _context.Genre.AsEnumerable().OrderBy(s => s.Name);

            AlbumEditViewModel viewmodel = new()
            {
                Album = album,
                ArtistList = new MultiSelectList(artists, "Id", "Name"),
                SelectedArtist = album.AlbumArtists.Select(aa => aa.ArtistId),
                GenresList = new MultiSelectList(genres, "Id", "Name"),
                SelectedGenres = album.AlbumGenres.Select(ag => ag.GenreId)
            };
            return View(viewmodel);
        }



        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AlbumEditViewModel viewmodel)
        {
            if (id != viewmodel.Album.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var albumToUpdate = await _context.Album
                        .Include(a => a.AlbumArtists)
                        .Include(a => a.AlbumGenres)
                        .FirstOrDefaultAsync(a => a.Id == id);

                    if (albumToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Update album details
                    albumToUpdate.Title = viewmodel.Album.Title;
                    albumToUpdate.ReleaseDate = viewmodel.Album.ReleaseDate;
                    albumToUpdate.CoverUrl = viewmodel.Album.CoverUrl;

                    // Update AlbumArtists
                    var newArtistList = viewmodel.SelectedArtist ?? Enumerable.Empty<int>();
                    var prevArtistList = albumToUpdate.AlbumArtists.Select(aa => aa.ArtistId).ToList();
                    var toBeRemovedArtists = albumToUpdate.AlbumArtists.Where(aa => !newArtistList.Contains(aa.ArtistId)).ToList();

                    foreach (var artistId in newArtistList)
                    {
                        if (!prevArtistList.Contains(artistId))
                        {
                            albumToUpdate.AlbumArtists.Add(new AlbumArtist { ArtistId = artistId, AlbumId = id });
                        }
                    }
                    foreach (var albumArtist in toBeRemovedArtists)
                    {
                        albumToUpdate.AlbumArtists.Remove(albumArtist);
                    }

                    // Update AlbumGenres
                    var newGenreList = viewmodel.SelectedGenres ?? Enumerable.Empty<int>();
                    var prevGenreList = albumToUpdate.AlbumGenres.Select(ag => ag.GenreId).ToList();
                    var toBeRemovedGenres = albumToUpdate.AlbumGenres.Where(ag => !newGenreList.Contains(ag.GenreId)).ToList();

                    foreach (var genreId in newGenreList)
                    {
                        if (!prevGenreList.Contains(genreId))
                        {
                            albumToUpdate.AlbumGenres.Add(new AlbumGenre { GenreId = genreId, AlbumId = id });
                        }
                    }
                    foreach (var albumGenre in toBeRemovedGenres)
                    {
                        albumToUpdate.AlbumGenres.Remove(albumGenre);
                    }

                    _context.Update(albumToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExists(viewmodel.Album.Id))
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
            return View(viewmodel);
        }


        // GET: Albums/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Album
                .Include(a => a.AlbumArtists)
                .ThenInclude(aa => aa.Artist)
                .Include(a => a.AlbumGenres)
                .ThenInclude(ag => ag.Genre)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // POST: Albums/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var album = await _context.Album.FindAsync(id);
            if (album != null)
            {
                _context.Album.Remove(album);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlbumExists(int id)
        {
            return _context.Album.Any(e => e.Id == id);
        }

        // GET: Albums/GenreDetails/5
        public async Task<IActionResult> GenreDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genre
                  .Include(g => g.AlbumGenres)
                  .ThenInclude(ag => ag.Album)
                      .ThenInclude(a => a.Songs)
                  .FirstOrDefaultAsync(g => g.Id == id);

            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // GET: Albums/AddSongsToAlbum/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSongsToAlbum(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Album
                .Include(a => a.Songs)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
            {
                return NotFound();
            }

            var model = new AddSongsToAlbumViewModel
            {
                AlbumId = album.Id,
                AlbumTitle = album.Title,
                AvailableSongs = await _context.Song.Where(s => s.AlbumId != id).ToListAsync()
            };

            return View(model);
        }

        // POST: Albums/AddSongsToAlbum/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSongsToAlbum(AddSongsToAlbumViewModel model)
        {
            if (ModelState.IsValid)
            {
                var album = await _context.Album
                    .Include(a => a.Songs)
                    .FirstOrDefaultAsync(a => a.Id == model.AlbumId);

                if (album == null)
                {
                    return NotFound();
                }

                if (model.SelectedSongs != null && model.SelectedSongs.Any())
                {
                    foreach (var songId in model.SelectedSongs)
                    {
                        var song = await _context.Song.FindAsync(songId);
                        if (song != null)
                        {
                            // Add song to album if it doesn't already exist
                            if (!album.Songs.Any(s => s.Id == songId))
                            {
                                album.Songs.Add(song);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Details), new { id = model.AlbumId });
                }
            }

            // If model state is not valid or no songs were selected, reload the view with errors
            model.AvailableSongs = await _context.Song.ToListAsync(); // Refresh available songs list
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveSongFromAlbum(int? id, int? songId)
        {
            if (id == null || songId == null)
            {
                return NotFound();
            }

            var album = await _context.Album
                .Include(a => a.Songs)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
            {
                return NotFound();
            }

            var songToRemove = album.Songs.FirstOrDefault(s => s.Id == songId);

            if (songToRemove == null)
            {
                return NotFound();
            }

            try
            {
                album.Songs.Remove(songToRemove);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException /* ex */)
            {
                // Log the error (uncomment ex variable name and write a log)
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(SongsInAlbum), new { id = album.Id }); // Redirect to the album's song list
        }

        public async Task<IActionResult> SongsInAlbum(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Album
                .Include(a => a.Songs)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
            {
                return NotFound();
            }

            return View(album.Songs.ToList());
        }
    }
}
