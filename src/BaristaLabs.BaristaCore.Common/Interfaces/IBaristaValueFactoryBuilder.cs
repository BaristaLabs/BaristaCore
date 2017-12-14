namespace BaristaLabs.BaristaCore
{
    public interface IBaristaValueFactoryBuilder
    {
        /// <summary>
        /// Creates a new IBaristaValueFactory associated with the specified context.
        /// </summary>
        /// <param name="context"></param>
        IBaristaValueFactory CreateValueFactory(BaristaContext context);
    }
}
