using HistoryOfChanges;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("***** History Of Changes *****");

using (ApplicationContext db = new ApplicationContext())
{
    Console.WriteLine("\n=> Create DB");

    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();

    User tom = new User { Name = "Tom" };

    db.Users.Add(tom);
    db.SaveChanges();
}

using (ApplicationContext db = new ApplicationContext())
{
    Console.WriteLine("\n=> Changes user");

    User? user = db.Users.FirstOrDefault();

    if (user != null)
    {
        user.Name = "Marcus";
        db.SaveChanges();
        user.Name = "Ruslan";
        db.SaveChanges();
    }
}

using (ApplicationContext db = new ApplicationContext())
{
    Console.WriteLine("\n=> Get user info");

    User? user = db.Users.FirstOrDefault(u => u.Id == 1);

    if (user != null)
    {
        var userEntry = db.Entry(user);
        var createdAt = userEntry.Property<DateTime>("PeriodStart").CurrentValue;
        var deletedAt = userEntry.Property<DateTime>("PeriodEnd").CurrentValue;

        Console.WriteLine($"User: {user.Name}");
        Console.WriteLine($"CreatedAt: {createdAt}");
        Console.WriteLine($"DeletedAt: {deletedAt}");
    }
}

using (ApplicationContext db = new ApplicationContext())
{
    Console.WriteLine("\n=> Get user history");

    var history = db.Users.TemporalAll()
        .Where(u => u.Id == 1)
        .OrderBy(u => EF.Property<DateTime>(u, "PeriodStart"))
        .Select(u => new
        {
            User = u,
            Start = EF.Property<DateTime>(u, "PeriodStart"),
            End = EF.Property<DateTime>(u, "PeriodEnd")
        }).ToList();

    Console.WriteLine("User #1");

    foreach (var item in history)
    {
        Console.WriteLine($"{item.User.Name} from {item.Start} to {item.End}");
    }
}

using (ApplicationContext db = new ApplicationContext())
{
    Console.WriteLine("\n=> Change first user");

    User? user = db.Users.FirstOrDefault();

    if (user != null)
    {
        using (var transaction = db.Database.BeginTransaction())
        {
            db.Users.Remove(user);
            db.SaveChanges();
            user.Name = "Tom";
            db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Users] ON");
            db.Users.Add(user);
            db.SaveChanges();
            db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Users] OFF");
            db.Database.CommitTransaction();
        }
    }
}

using (ApplicationContext db = new ApplicationContext())
{
    Console.WriteLine("\n=> Get user info");

    User? user = db.Users.FirstOrDefault(u => u.Id == 1);

    if (user != null)
    {
        var userEntry = db.Entry(user);
        var createdAt = userEntry.Property<DateTime>("PeriodStart").CurrentValue;
        var deletedAt = userEntry.Property<DateTime>("PeriodEnd").CurrentValue;

        Console.WriteLine($"User: {user.Name}");
        Console.WriteLine($"CreatedAt: {createdAt}");
        Console.WriteLine($"DeletedAt: {deletedAt}");
    }
}