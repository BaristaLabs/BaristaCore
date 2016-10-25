namespace BaristaLabs.BaristaCore.Http
{
    using System.IO;

    public interface ISourceFileProvider
    {
        string Prefix
        {
            get;
        }

        Stream GetFileContentsAsync();
    }
}
