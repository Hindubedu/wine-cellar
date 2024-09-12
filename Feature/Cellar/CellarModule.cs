using System;
using Carter;
using Carter.OpenApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WineCellar.Domain;
using WineCellar.Persistence;

namespace WineCellar.Feature.Cellars
{
    public class CellarRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public double Temperature { get; set; }
    }

    public class CellarModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(
                    "/cellars",
                    (HttpContext context, ApplicationDbContext dbContext) =>
                    {
                        var cellars = dbContext
                            .Cellars.Where(cellar =>
                                cellar.Users.FirstOrDefault(user => user.Id == context.GetUserId())
                                != null
                            )
                            .ToList();
                        return cellars;
                    }
                )
                .Produces<List<Cellar>>()
                .RequireAuthorization()
                .WithTags("Cellar")
                .WithName("GetCellars")
                .IncludeInOpenApi();

            app.MapGet(
                    "/cellar/{cellarId:int}",
                    (HttpContext context, ApplicationDbContext dbContext, int cellarId) =>
                    {
                        var cellar = dbContext.Cellars.FirstOrDefault(cellar =>
                            cellar.Id == cellarId
                        );
                        return cellar;
                    }
                )
                .Produces<Cellar>()
                .RequireAuthorization()
                .WithTags("Cellar")
                .WithName("GetCellar")
                .IncludeInOpenApi();

            app.MapPost(
                    "/cellar/add",
                    (HttpContext context, CellarRequest request, ApplicationDbContext dbContext) =>
                    {
                        Cellar cellar = new Cellar(request);
                        var user = dbContext.Users.FirstOrDefault(user =>
                            user.Id == context.GetUserId()
                        );
                        if (user is null)
                        {
                            return Results.BadRequest();
                        }
                        user.Cellars.Add(cellar);
                        dbContext.SaveChanges();
                        return Results.Ok();
                    }
                )
                .WithTags("Cellar")
                .WithName("AddCellar")
                .IncludeInOpenApi()
                .RequireAuthorization();
        }
    }
}
