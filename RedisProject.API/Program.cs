using Microsoft.EntityFrameworkCore;
using RedisProject.API.Models;
using RedisProject.API.Repositories;
using RedisProject.API.Services;
using RedisProject.Cache;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository>(sp =>
{
    var appDbContext= sp.GetRequiredService<AppDbContext>();
    var productRepository= new ProductRepository(appDbContext);
    var redisService= sp.GetRequiredService<RedisService>();

    return new ProductRepositoryWithCacheDecorator(productRepository, redisService);
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("myDatabase");
});

builder.Services.AddSingleton<RedisService>(sp=>
{
    return new RedisService(builder.Configuration["CacheOptions:Url"]);
});

builder.Services.AddSingleton<IDatabase>(sp =>
{
    var redisService = sp.GetRequiredService<RedisService>();
    return redisService.GetDb(0);
});

var app = builder.Build();

using (var scope= app.Services.CreateScope())
{
    var dbContext= scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
