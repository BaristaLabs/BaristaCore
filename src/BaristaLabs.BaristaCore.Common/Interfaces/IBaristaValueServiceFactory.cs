namespace BaristaLabs.BaristaCore
{
    public interface IBaristaValueServiceFactory
    {
        /// <summary>
        /// Creates a new IBaristaValueService associated with the specified context.
        /// </summary>
        /// <param name="context"></param>
        IBaristaValueService CreateValueService(BaristaContext context);
    }
}
