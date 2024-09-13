using Carter;
using Carter.OpenApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                            .Cellars.Include(x => x.Storages)
                            .Where(cellar =>
                                cellar.Users.FirstOrDefault(user => user.Id == context.GetUserId())
                                != null
                            )
                            .ToList();

                        if (cellars.Count == 0)
                        {
                            return Results.NotFound("No cellars found");
                        }
                        return Results.Ok(cellars);
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
                        if (cellar is null)
                        {
                            return Results.BadRequest("Cellar not found");
                        }
                        return Results.Ok(cellar);
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

            app.MapPost(
                    "/cellar/edit",
                    (HttpContext context, CellarRequest request, ApplicationDbContext dbContext) =>
                    {
                        var exsistingCellar = dbContext.Cellars.FirstOrDefault(x =>
                            x.Id == request.Id
                        );
                        if (exsistingCellar is null)
                        {
                            return Results.NotFound("Cellar not found");
                        }
                        CellarMutator.MutateCellar(request, exsistingCellar);
                        dbContext.SaveChanges();

                        return Results.Ok(exsistingCellar);
                    }
                )
                .WithTags("Cellar")
                .WithName("UpdateCellar")
                .IncludeInOpenApi()
                .RequireAuthorization();

            app.MapDelete(
                    "/cellar/delete/{cellarId:int}",
                    (HttpContext context, ApplicationDbContext dbContext, int cellarId) =>
                    {
                        var existingCellar = dbContext.Cellars.Find(cellarId);
                        if (existingCellar is null)
                        {
                            return Results.NotFound("Cellar not found");
                        }
                        dbContext.Remove(existingCellar);
                        dbContext.SaveChanges();
                        return Results.Ok("Storage deleted successfully.");
                    }
                )
                .Produces<OkResult>()
                .RequireAuthorization()
                .WithTags("Cellar")
                .WithName("DeleteCellar")
                .IncludeInOpenApi();

            app.MapGet(
                    "/cellar/{cellarId:int}/storages",
                    (HttpContext context, ApplicationDbContext dbContext, int cellarId) =>
                    {
                        var cellar = dbContext
                            .Cellars.Include(x => x.Storages)
                            .FirstOrDefault(cellar => cellar.Id == cellarId);
                        if (cellar is null)
                        {
                            return Results.NotFound();
                        }
                        return Results.Ok(cellar.Storages);
                    }
                )
                .Produces<List<Storage>>()
                .RequireAuthorization()
                .WithTags("Cellar")
                .WithName("GetCellarStorages")
                .IncludeInOpenApi();
        }
    }
}
