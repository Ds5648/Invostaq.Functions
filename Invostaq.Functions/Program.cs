using InvoStaq.Functions.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration["SqliteConnectionString"]
            ?? "Data Source=invostaq.db";

        services.AddDbContext<InvoiceDbContext>(options =>
            options.UseSqlite(connectionString));
    })
    .Build();

// Auto-create SQLite DB on startup
using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InvoiceDbContext>();
    db.Database.EnsureCreated();
}

await host.RunAsync();