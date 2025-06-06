using Catalog.API;
using Catalog.Application;
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Products;
using Catalog.Application.Products.Commands.CreateProduct;
using Catalog.Application.Products.Commands.DeleteProduct;
using Catalog.Application.Products.Commands.UpdateProduct;
using Catalog.Application.Products.Queries.GetAllProducts;
using Catalog.Application.Products.Queries.GetPagedProducts;
using Catalog.Application.Products.Queries.GetProductsById;
using Catalog.Domain.Entities;
using Catalog.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
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

builder.Services.AddScoped<IApplicationDbContext, AppDbContext>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAllProductsHandler).Assembly));

builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();

builder.Services.AddFluentValidationAutoValidation();


builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddValidatorsFromAssemblyContaining<ProductDtoValidator>();
builder.Services.AddMemoryCache();

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
builder.Services.AddHealthChecks();

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

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");

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

app.MapGet("/products/paged", GetPagedProducts)
    .WithName("GetPagedProducts");

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

async Task<IResult> GetProducts(IMediator mediator)
{
    var products = await mediator.Send(new GetAllProductsQuery());
    return Results.Ok(ApiResponse<IEnumerable<ProductDto>>.Ok(products));
}

async Task<IResult> GetPagedProducts(IMediator mediator, int pageNumber = 1, int pageSize = 10)
{
    var products = await mediator.Send(new GetPagedProductsQuery(pageNumber, pageSize));
    return Results.Ok(ApiResponse<IEnumerable<ProductDto>>.Ok(products));
}

async Task<IResult> GetProductById(Guid id, IMediator mediator)
{
    var product = await mediator.Send(new GetProductByIdQuery(id));
    return Results.Ok(ApiResponse<ProductDto>.Ok(product));
}

async Task<IResult> CreateProduct([FromBody] ProductDto product, IMediator mediator)
{
    var result = await mediator.Send(new CreateProductCommand(product));
    return Results.Ok(ApiResponse<ProductDto>.Ok(result));
}

async Task<IResult> UpdateProduct([FromBody] ProductDto updatedProduct, IMediator mediator)
{
    await mediator.Send(new UpdateProductCommand(updatedProduct));
    return Results.Ok(ApiResponse<bool>.Ok(true));
}

async Task<IResult> DeleteProduct(Guid id, IMediator mediator)
{
    await mediator.Send(new DeleteProductCommand(id));
    return Results.Ok(ApiResponse<bool>.Ok(true));
}