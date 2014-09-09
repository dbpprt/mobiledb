## MobileDB - a lightweight, fast and embeddable database engine 

You like **EntityFramework's** API and the way of accessing tables and save data? You asked your self why there is no simple json file database to use in small projects, apps, etc where you don't need a full database engine? **Then try out MobileDB**

### Features ###

- **Full LINQ support**
- Super-Fast data-access with a **Json**-Store (in-memory dataset synchronized with your underlaying filesystem)
- A **Bson**-Store for saving files with metadata (not kept in-memory - loaded on demand)
- EntityFramework inspired DbContext and EntitySets
- It relies on JSON.NET so it will serialize nearly any types 
- Primary keys for entities
- Extensive entity validation powered by [ExpressiveAnnotations](https://github.com/JaroslawWaliszko/ExpressiveAnnotations "ExpressiveAnnotations") (see Examples)
- It is **blazing fast**! Adding 100.000 items and reloading them from the filesystem takes about ~1sec
- **A friendly license for developers! (MIT licensed)**
- ......

### Supported Platforms ###

- PCL version for supported platforms 
- .NET 4.5+, 
- **(soon)** Windows Phone 8.0+, 
- Mono 4.5+
- Android (via Xamarin/Mono) (**untested**)
- iOS (via Xamarin/Mono) (**untested**)

### Why should i use a embedded database for my application?

You might also wonder about storing data in memory. Let's think about a blog with dozens of posts, comments and attached files?

The answer is really simple: think about the size of your data? Is it really more than some megabytes of data? Probably not. MobileDB comes with a BSON store who doesn't keep data in memory. So it's perfect to save your files (pictures, attachments) there and just reference it your other entities. 

**I created MobileDB because im to lazy to install a SQL-Server Express on every small application i use and i want a EntityFramework like experience when working with desktop and mobile applications.**

### A simple sample ###

So let's get started with a simple example

```csharp

// a simple movie entity containing some validation rules
// just for database consistency

public class Movie
{
	// we only want imdb ids here
	[AssertThat("StartsWith(MovieId, 'tt') == true")]
    [Identity]
	public string MovieId { get; set; }
	
	[AssertThat("Length(Name) >= 1")] 
    public string Name { get; set; }

    public string[] Genres { get; set; }
	
    public int Year { get; set; }

    // and some other properties
}


public class MovieContext : DbContext
{
    public MovieContext(string connectionString)
        : base(connectionString)
    {
    }

	// it's not required to define Json store, it's the default store
	//[Json]
    public IEntitySet<Movie> Movies { get; set; }
}


// so that's all... let's use our context :)

var context = ContextFactory.Create<MovieContext>()
    .WithPhysicalFilesystem("C:\\database\\")
    .Build();

var movies = context.Movies;

movies.Add(new Movie
{
    Genres = new[] { "Action" },
    MovieId = "tt0468569",
    Name = "The Dark Knight",
    Year = 2008
});

context.SaveChanges();

```

### Some other things you should know ###

- MobileDB is like EntityFramework, that means: 1 context per Thread (this is really handy in small webapplications with Unity or any other dependency injection framework
- Multiple contextes are safe to be accessed at the same time
- Entites are not bound to a schema => you can change it whenever you want
- The first time when you initialize a DbContext it takes about ~100ms to analyze the schema and to cache it but after the first load it's blazing fast
- MobileDB is under heavy development that means it i don't garantee that there won't be any API changes

### Other cool features ###

- You can plug-in any FileSystem or Store you want, everything is built on top of interfaces. The connection string specifies which components are used

```
// sample connection string generated for the example above by the ContextFactory
filesystem=MobileDB.FileSystem.PhysicalFileSystem;path=C:\\database\\
```

- This allows you to write your own filesystems if you need them. For example a zip-filesystem with zip-file encryption would be really easy
- You can define stores and MobileDB is able to load them for specific EntitySets

```csharp
[Store(typeof (JsonStore))]
public IEntitySet<SimpleEntityWithIdentity> Movies { get; set; }
```

- Adding your own validation attributes, etc
- More docs to come

### Known bugs :( ###

- Async functions are'nt implemented yet
- FileSystem implementation for Windows Phone (LocalStorage) is missing
- Test coverage is about 1%

### Roadmap ###

- See issues

### **Important:** I'm searching for contributors or other members who like to help me to make MobileDB even better 
