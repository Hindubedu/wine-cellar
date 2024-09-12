using WineCellar.Domain;
using WineCellar.Feature.Cellars;

public static class CellarMutator
{
    public static void MutateCellar(CellarRequest request, Cellar cellar)
    {
        cellar.Name = request.Name;
        cellar.Location = request.Location;
        cellar.Temperature = request.Temperature;
    }
}
