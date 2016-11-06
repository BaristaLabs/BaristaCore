using System.IO;

namespace BaristaLabs.BaristaCore.Http
{
    public interface ISourceFileProvider
    {
        string Prefix
        {
            get;
        }

        Stream GetFileContentsAsync();
    }
}
