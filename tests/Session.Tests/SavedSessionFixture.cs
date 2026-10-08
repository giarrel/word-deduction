using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

internal static class SavedSessionFixture
{
    public static void RewritePrimary(string directory, Action<JObject> edit)
    {
        string path = Path.Combine(directory, "session.json");
        var envelope = JObject.Parse(File.ReadAllText(path));
        var payload = JObject.Parse((string)envelope["Payload"]);
        edit(payload);
        string json = payload.ToString(Formatting.None);
        envelope["Payload"] = json;
        envelope["Checksum"] = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(json)));
        File.WriteAllText(path, envelope.ToString());
    }
}
