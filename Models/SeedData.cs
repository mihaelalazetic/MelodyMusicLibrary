using MelodyMusicLibrary.Areas.Identity.Data;
using MelodyMusicLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MelodyMusicLibrary.Data
{
    public static class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<MelodyUser>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            var roleUserCheck = await RoleManager.RoleExistsAsync("User");
            if (!roleUserCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("User")); }

            MelodyUser user = await UserManager.FindByEmailAsync("admin@mvcmovie.com");
            if (user == null)
            {
                var User = new MelodyUser();
                User.Email = "admin@melody.com";
                User.UserName = "admin@melody.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin      
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
            MelodyUser user2 = await UserManager.FindByEmailAsync("admin@mvcmovie.com");
            if (user2 == null)
            {
                var User = new MelodyUser();
                User.Email = "mihaela@melody.com";
                User.UserName = "mihaela";
                string userPWD = "123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin      
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "User"); }
            }
        }
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MelodyContext(serviceProvider.GetRequiredService<DbContextOptions<MelodyContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();
                // Check if data already exists
                if (context.Album.Any() || context.Artist.Any() || context.Genre.Any() || context.Song.Any())
                {
                    return;   // DB has been seeded
                }

                // Seed genres
                var genres = new Genre[]
                {
                new Genre { Name = "Pop", ImageUrl="/images/genres/Pop.png" },
                new Genre { Name = "Rock", ImageUrl="/images/genres/Rock.png" },
                new Genre { Name = "Hip-Hop" , ImageUrl = "/images/genres/Hip-Hop.png"},
                new Genre { Name = "Electronic" , ImageUrl = "/images/genres/Electronic.png"},
                new Genre { Name = "Alternative" , ImageUrl = "/images/genres/Alternative.png"},
                new Genre { Name = "R&B" , ImageUrl = "/images/genres/R&B.png"}
                };
                foreach (Genre g in genres)
                {
                    context.Genre.Add(g);
                }
                context.SaveChanges();

                // Seed artists
                var artists = new Artist[]
                {
                new Artist { Name = "Taylor Swift", Description = "American singer-songwriter known for narrative songs about her personal life." },
                new Artist { Name = "Ed Sheeran", Description = "British singer-songwriter known for his acoustic pop style." },
                new Artist { Name = "Kendrick Lamar", Description = "American rapper known for his socially conscious lyrics." },
                new Artist { Name = "Daft Punk", Description = "French electronic music duo known for their innovative electronic sound." }
                };
                foreach (Artist a in artists)
                {
                    context.Artist.Add(a);
                }
                context.SaveChanges();

                // Seed albums
                var albums = new Album[]
                {
                new Album
                {
                    Title = "1989",
                    ReleaseDate = new DateTime(2014, 10, 27),
                    CoverUrl = "https://upload.wikimedia.org/wikipedia/en/f/f6/Taylor_Swift_-_1989.png"
                },
                new Album
                {
                    Title = "÷ (Divide)",
                    ReleaseDate = new DateTime(2017, 3, 3),
                    CoverUrl = "https://upload.wikimedia.org/wikipedia/en/4/45/Divide_cover.png"
                },
                new Album
                {
                    Title = "good kid, m.A.A.d city",
                    ReleaseDate = new DateTime(2012, 10, 22),
                    CoverUrl = "https://i.scdn.co/image/ab67616d0000b273d58e537cea05c2156792c53d"
                },
                new Album
                {
                    Title = "Random Access Memories",
                    ReleaseDate = new DateTime(2013, 5, 17),
                    CoverUrl = "https://upload.wikimedia.org/wikipedia/en/2/26/Daft_Punk_-_Random_Access_Memories.png"
                }
                };
                foreach (Album a in albums)
                {
                    context.Album.Add(a);
                }
                context.SaveChanges();

                // Seed songs
                var songs = new Song[]
                {
                new Song
                {
                    Title = "Shake It Off",
                    Duration = 219, // 3 minutes and 39 seconds
                    AlbumId = albums.Single(a => a.Title == "1989").Id
                },
                new Song
                {
                    Title = "Shape of You",
                    Duration = 233, // 3 minutes and 53 seconds
                    AlbumId = albums.Single(a => a.Title == "÷ (Divide)").Id
                },
                new Song
                {
                    Title = "HUMBLE.",
                    Duration = 177, // 2 minutes and 57 seconds
                    AlbumId = albums.Single(a => a.Title == "good kid, m.A.A.d city").Id
                },
                new Song
                {
                    Title = "Get Lucky",
                    Duration = 369, // 6 minutes and 9 seconds
                    AlbumId = albums.Single(a => a.Title == "Random Access Memories").Id
                }
                };
                foreach (Song s in songs)
                {
                    context.Song.Add(s);
                }
                context.SaveChanges();

                // Seed relationships (AlbumArtist, AlbumGenre, SongArtist, SongGenre)
                context.AlbumArtists.AddRange(
                    new AlbumArtist { AlbumId = albums.Single(a => a.Title == "1989").Id, ArtistId = artists.Single(a => a.Name == "Taylor Swift").Id },
                    new AlbumArtist { AlbumId = albums.Single(a => a.Title == "÷ (Divide)").Id, ArtistId = artists.Single(a => a.Name == "Ed Sheeran").Id },
                    new AlbumArtist { AlbumId = albums.Single(a => a.Title == "good kid, m.A.A.d city").Id, ArtistId = artists.Single(a => a.Name == "Kendrick Lamar").Id },
                    new AlbumArtist { AlbumId = albums.Single(a => a.Title == "Random Access Memories").Id, ArtistId = artists.Single(a => a.Name == "Daft Punk").Id }
                );

                context.AlbumGenres.AddRange(
                    new AlbumGenre { AlbumId = albums.Single(a => a.Title == "1989").Id, GenreId = genres.Single(g => g.Name == "Pop").Id },
                    new AlbumGenre { AlbumId = albums.Single(a => a.Title == "÷ (Divide)").Id, GenreId = genres.Single(g => g.Name == "Pop").Id },
                    new AlbumGenre { AlbumId = albums.Single(a => a.Title == "good kid, m.A.A.d city").Id, GenreId = genres.Single(g => g.Name == "Hip-Hop").Id },
                    new AlbumGenre { AlbumId = albums.Single(a => a.Title == "Random Access Memories").Id, GenreId = genres.Single(g => g.Name == "Electronic").Id }
                );

                context.SongArtists.AddRange(
                    new SongArtist { SongId = songs.Single(s => s.Title == "Shake It Off").Id, ArtistId = artists.Single(a => a.Name == "Taylor Swift").Id },
                    new SongArtist { SongId = songs.Single(s => s.Title == "Shape of You").Id, ArtistId = artists.Single(a => a.Name == "Ed Sheeran").Id },
                    new SongArtist { SongId = songs.Single(s => s.Title == "HUMBLE.").Id, ArtistId = artists.Single(a => a.Name == "Kendrick Lamar").Id },
                    new SongArtist { SongId = songs.Single(s => s.Title == "Get Lucky").Id, ArtistId = artists.Single(a => a.Name == "Daft Punk").Id }
                );

                context.SongGenres.AddRange(
                    new SongGenre { SongId = songs.Single(s => s.Title == "Shake It Off").Id, GenreId = genres.Single(g => g.Name == "Pop").Id },
                    new SongGenre { SongId = songs.Single(s => s.Title == "Shape of You").Id, GenreId = genres.Single(g => g.Name == "Pop").Id },
                    new SongGenre { SongId = songs.Single(s => s.Title == "HUMBLE.").Id, GenreId = genres.Single(g => g.Name == "Hip-Hop").Id },
                    new SongGenre { SongId = songs.Single(s => s.Title == "Get Lucky").Id, GenreId = genres.Single(g => g.Name == "Electronic").Id }
                );

                context.SaveChanges();
            }
        }
    }
}
