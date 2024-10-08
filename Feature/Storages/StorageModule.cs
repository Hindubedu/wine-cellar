using System.Security.Claims;
using Carter;
using Carter.OpenApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WineCellar.Domain;
using WineCellar.Persistence;

namespace WineCellar.Feature.Storages;

public class StorageRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public uint Capacity { get; set; }
    public int CellarId { get; set; }
}

public class StorageModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/storage/add",
                (HttpContext context, StorageRequest storage, ApplicationDbContext dbContext) =>
                {
                    if (storage.Temperature == 0)
                    {
                        storage.Temperature = dbContext
                            .Cellars.First(x => x.Id == storage.CellarId)
                            .Temperature;
                    }

                    var newStorage = new Storage(storage);
                    var cellar = dbContext
                        .Cellars.Include(c => c.Storages)
                        .Include(c => c.Users)
                        .FirstOrDefault(c =>
                            c.Id == storage.CellarId
                            && c.Users.Any(u => u.Id == context.GetUserId())
                        );
                    if (cellar is null)
                    {
                        return Results.BadRequest();
                    }
                    cellar.Storages.Add(newStorage);

                    dbContext.SaveChanges();

                    return Results.Ok(newStorage);
                }
            )
            .WithTags("Storage")
            .WithName("AddStorage")
            .IncludeInOpenApi()
            .RequireAuthorization();

        app.MapGet(
                "/storages",
                (HttpContext context, ApplicationDbContext dbContext) =>
                {
                    var storages = dbContext
                        .Storages.Where(storage =>
                            storage.Cellar.Users.FirstOrDefault(user =>
                                user.Id == context.GetUserId()
                            ) != null
                        )
                        .ToList();
                    return storages;
                }
            )
            .Produces<List<Storage>>()
            .RequireAuthorization()
            .WithTags("Storage")
            .WithName("GetStorages")
            .IncludeInOpenApi();

        app.MapDelete(
                "/storage/delete/{storageId:int}",
                (HttpContext context, ApplicationDbContext dbContext, int storageId) =>
                {
                    var existingStorage = dbContext.Storages.Find(storageId);
                    if (existingStorage is null)
                    {
                        return Results.NotFound("Storage not found");
                    }
                    dbContext.Remove(existingStorage);
                    dbContext.SaveChanges();

                    return Results.Ok();
                }
            )
            .Produces<OkResult>()
            .RequireAuthorization()
            .WithTags("Storage")
            .WithName("DeleteStorage")
            .IncludeInOpenApi();

        app.MapPost(
                "/storage/update",
                (HttpContext context, ApplicationDbContext dbContext, StorageRequest userStorage) =>
                {
                    var existingStorage = dbContext.Storages.FirstOrDefault(StorageModule =>
                        StorageModule.Id == userStorage.Id
                    );
                    if (existingStorage is null)
                    {
                        return Results.NotFound("Storage not found");
                    }
                    StorageMutator.MutateStorage(userStorage, existingStorage);

                    dbContext.SaveChanges();

                    return Results.Ok(existingStorage);
                }
            )
            .Produces<Storage>()
            .RequireAuthorization()
            .WithTags("Storage")
            .WithName("UpdateStorage")
            .IncludeInOpenApi();

        app.MapGet(
                "storage/{storageId:int}",
                (HttpContext context, ApplicationDbContext dbContext, int storageId) =>
                {
                    var cellar = dbContext.Cellars.FirstOrDefault(cellar =>
                        cellar.Users.FirstOrDefault(user => user.Id == context.GetUserId()) != null
                    );

                    if (cellar is null)
                    {
                        return Results.NotFound("Cellar not found");
                    }

                    var storage = dbContext.Storages.Find(storageId);
                    return Results.Ok(storage);
                }
            )
            .Produces<Storage>()
            .RequireAuthorization()
            .WithTags("Storage")
            .WithName("GetStorage")
            .IncludeInOpenApi();

        app.MapGet(
                "/storage/{storageId:int}/wines",
                (HttpContext context, ApplicationDbContext dbContext, int storageId) =>
                {
                    var wines = dbContext
                        .Storages.Include(storage => storage.Wines)
                        .Where(storage =>
                            storage.Cellar.Users.FirstOrDefault(user =>
                                user.Id == context.GetUserId()
                            ) != null
                        )
                        .First(storage => storage.Id == storageId)
                        .Wines;
                    return wines;
                }
            )
            .Produces<List<Wine>>()
            .RequireAuthorization()
            .WithTags("Storage")
            .WithName("GetWinesInStorage")
            .IncludeInOpenApi();
    }
}
