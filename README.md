Melody Music Library

Melody Music Library is a web application built with ASP.NET Core MVC, designed to manage albums, artists, songs, genres, and playback functionality similar to popular music streaming services.

Features
Album Management: Add, edit, and delete albums with cover art and track listings.
Artist Profiles: Create and manage artist profiles with descriptions.
Song Catalog: Organize songs by albums and artists, with duration details.
Genre Classification: Categorize music by genres with visual representation.
Playback Feature: Play music directly within the application.
User Authentication: Register and authenticate users with ASP.NET Core Identity.
Admin Dashboard: Manage all aspects of the music library with role-based authorization.

Technologies Used
ASP.NET Core MVC: Framework for building web applications.
Entity Framework Core: Object-relational mapping (ORM) framework for database interactions.
Bootstrap: Front-end framework for responsive and mobile-first design.
SQL Server: Database management system for data storage.
Select2: JavaScript plugin for enhancing select boxes for artist selection.
Audio Player Integration: Capability to play music tracks seamlessly.

Getting Started
To run this project locally, follow these steps:

Clone the repository:
git clone https://github.com/mihaelalazetic/MelodyMusicLibrary.git

Set up the database:
Ensure you have SQL Server installed.
Update the connection string in appsettings.json with your SQL Server instance details.
Add migrations:

Open NuGet Package Manager Console.

Run the following command to add migrations:
Add-Migration InitialCreate
Run the following command to update the database:
Update-Database

Run the application

Open your web browser:

Navigate to https://localhost:5001 to view the application.
