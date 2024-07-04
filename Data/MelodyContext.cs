using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MelodyMusicLibrary.Models;
using System.Reflection.Emit;

namespace MelodyMusicLibrary.Data
{
    public class MelodyContext : DbContext
    {
        public MelodyContext(DbContextOptions<MelodyContext> options)
            : base(options)
        {
        }

        public DbSet<MelodyMusicLibrary.Models.Album> Album { get; set; } = default!;
        public DbSet<MelodyMusicLibrary.Models.Artist> Artist { get; set; } = default!;
        public DbSet<MelodyMusicLibrary.Models.Genre> Genre { get; set; } = default!;
        public DbSet<MelodyMusicLibrary.Models.Song> Song { get; set; } = default!;

        public DbSet<AlbumArtist> AlbumArtists { get; set; } = default!;
        public DbSet<AlbumGenre> AlbumGenres { get; set; } = default!;
        public DbSet<SongArtist> SongArtists { get; set; } = default!;
        public DbSet<SongGenre> SongGenres { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Configure Album - Artist relationship (AlbumArtist)
            modelBuilder.Entity<AlbumArtist>()
                .HasKey(aa => new { aa.AlbumId, aa.ArtistId });

            modelBuilder.Entity<AlbumArtist>()
                .HasOne(aa => aa.Album)
                .WithMany(a => a.AlbumArtists)
                .HasForeignKey(aa => aa.AlbumId);

            modelBuilder.Entity<AlbumArtist>()
                .HasOne(aa => aa.Artist)
                .WithMany(a => a.AlbumArtists)
                .HasForeignKey(aa => aa.ArtistId);

            // Configure Album - Genre relationship (AlbumGenre)
            modelBuilder.Entity<AlbumGenre>()
                .HasKey(ag => new { ag.AlbumId, ag.GenreId });

            modelBuilder.Entity<AlbumGenre>()
                .HasOne(ag => ag.Album)
                .WithMany(a => a.AlbumGenres)
                .HasForeignKey(ag => ag.AlbumId);

            modelBuilder.Entity<AlbumGenre>()
                .HasOne(ag => ag.Genre)
                .WithMany(g => g.AlbumGenres)
                .HasForeignKey(ag => ag.GenreId);

            // Configure Song - Artist relationship (SongArtist)
            modelBuilder.Entity<SongArtist>()
                .HasKey(sa => new { sa.SongId, sa.ArtistId });

            modelBuilder.Entity<SongArtist>()
                .HasOne(sa => sa.Song)
                .WithMany(s => s.SongArtists)
                .HasForeignKey(sa => sa.SongId);

            modelBuilder.Entity<SongArtist>()
                .HasOne(sa => sa.Artist)
                .WithMany(a => a.SongArtists)
                .HasForeignKey(sa => sa.ArtistId);

            // Configure Song - Genre relationship (SongGenre)
            modelBuilder.Entity<SongGenre>()
                .HasKey(sg => new { sg.SongId, sg.GenreId });

            modelBuilder.Entity<SongGenre>()
                .HasOne(sg => sg.Song)
                .WithMany(s => s.SongGenres)
                .HasForeignKey(sg => sg.SongId);

            modelBuilder.Entity<SongGenre>()
                .HasOne(sg => sg.Genre)
                .WithMany(g => g.SongGenres)
                .HasForeignKey(sg => sg.GenreId);
        }
    }
}
