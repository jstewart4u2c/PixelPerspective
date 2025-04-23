**About Pixel Perspective**
- Pixel Perspective is a video game review site that also functions as a social media. Users can create and edit an account, search games, add them to their library, and review them. Added games and written reviews will show up dynamically on the user's profile page. This was designed as a capstone project and is maintained by a team of three. 

**Steps for Deployment**
1. Install <a href="https://visualstudio.microsoft.com/downloads/" target="_blank">Visual Studio 2022</a> and <a href="https://www.microsoft.com/en-us/sql-server/sql-server-downloads" target="_blank">SQL Server</a>.
2. Clone the repository to your local system.
3. Open the solution (the .sln file within the project) in Visual Studio.
4. On the Visual Studio toolbar, click Tools -> NuGet Package Manager -> Package Manager Console
5. Within the Package Manager Console, enter "update-database" without the quotes to create a new local database.
6. Enter "add-migration InitialCreate" to generate SQL for the local database.
7. Once the migration is created, enter "update-database" once again to apply the migration.
8. To view the database and ensure it was created properly, click View -> SQL Server Object Explorer
9. Find the LocalPixelPerspective database within the Object Explorer and expand the tables folder to ensure tables were created properly.
   
**Priority List**

High Priority

Establish a database Complete
Establish users/admins in the database Complete
Establish games in the database Complete (API)
Wireframes for what we want pages to look like (User profile, game profile, home page) Complete

Medium Priority

Ubuntu SQL Server (Express) Complete (Azure)
Support for user profiles Complete
Account required to make posts and comment Complete
Game profile page support Complete
Friends System (Add, unadd, block) Partially Complete
Admin Portal Complete
Game Search Function Complete

Low Priority

Steam API integration to view steam reviews for a game Incomplete
Reviewbomb prevention Incomplete
like/dislike system Complete
Users online status (active, idle, offline) Incomplete
Trending games on home page Incomplete
Trending reviews on home page Incomplete

