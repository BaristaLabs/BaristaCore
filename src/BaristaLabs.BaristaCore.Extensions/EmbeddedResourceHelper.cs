namespace BaristaLabs.BaristaCore
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    internal static class EmbeddedResourceHelper
    {
        public static async Task<string> LoadResource<T>(T type, string name)
        {
            var assembly = typeof(T).Assembly;
            var resourceStream = assembly.GetManifestResourceStream(name);
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
