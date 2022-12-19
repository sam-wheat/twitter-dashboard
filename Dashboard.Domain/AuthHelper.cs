namespace Dashboard.Domain;

public static class AuthHelper
{
    public static BearerToken GetTwitterBearerToken(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            throw new Exception("Unable to get location of twitter bearer token file from appsettings.global.json.");

        if (!File.Exists(fileName))
            throw new Exception($"Twitter bearer token file {fileName} was not found.  Specifify the correct location and name of the file in appsettings.global.json.");

        return new(File.ReadAllText(fileName));
    }
}
