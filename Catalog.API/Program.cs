using Catalog.API;
using Catalog.Application;
using Catalog.Domain.Entities;
using Catalog.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("CatalogDb"));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@123ThatIsUsedInMySampleApp")),
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Catalog API";
        options.HideClientButton = true;
        options.HideModels = true;
    });
}

app.UseHttpsRedirection();

app.MapPost("/api/login", Login);

app.MapGet("/products", GetProducts)
    .WithName("GetProducts");

app.MapGet("/products/{id:Guid}", GetProductById)
    .WithName("GetProductById");

app.MapPost("/products", CreateProduct)
    .WithName("CreateProduct")
    .RequireAuthorization(policy => policy.RequireRole(UserRoles.Admin));

app.MapPut("/products", UpdateProduct)
    .WithName("UpdateProduct")
    .RequireAuthorization(policy => policy.RequireRole(UserRoles.Admin));

app.MapDelete("/products/{id:Guid}", DeleteProduct)
    .WithName("DeleteProduct")
    .RequireAuthorization(policy => policy.RequireRole(UserRoles.Admin));

app.Run();

IResult Login(UserLogin login)
{
    var user = InMemoryUserStore.Users.FirstOrDefault(u => u.Username == login.Username && u.Password == login.Password);

    if (user == null)
    {
        return Results.Unauthorized();
    }

    var claims = new[]
    {
        new Claim(ClaimTypes.Name, login.Username),
        new Claim(ClaimTypes.Role, user.Role)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@123ThatIsUsedInMySampleApp"));
    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
    );
    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(new { token = tokenString, role = user.Role });
}

async Task<IResult> GetProducts(IProductService productService)
{
    return Results.Ok(await productService.GetAllAsync());
}

async Task<IResult> GetProductById(IProductService productService, Guid id)
{
    return Results.Ok(await productService.GetByIdAsync(id));
}

async Task<IResult> CreateProduct(IProductService productService, ProductDto product)
{
    var result = await productService.AddAsync(product);
    return Results.Created($"/products/{result.Id}", result);
}

async Task<IResult> UpdateProduct(IProductService productService, ProductDto updatedProduct)
{
    await productService.UpdateAsync(updatedProduct);
    return Results.NoContent();
}

async Task<IResult> DeleteProduct(IProductService productService, Guid id)
{
    await productService.DeleteAsync(id);
    return Results.NoContent();
}