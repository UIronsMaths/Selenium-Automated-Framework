using System;
using Newtonsoft.Json;
using NUnit.Framework.Internal;

public class UserData
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Expected { get; set; }
}

public class JsonReader
{
    public static TestSettings settings = ConfigurationManager.Settings;
    public static List<UserData> ReadUsers(string file = "users.json")
    {
        string fullPath = Path.Combine(settings.BaseDirectory, file);
        string json = File.ReadAllText(fullPath);
        return JsonConvert.DeserializeObject<List<UserData>>(json);
    }
}