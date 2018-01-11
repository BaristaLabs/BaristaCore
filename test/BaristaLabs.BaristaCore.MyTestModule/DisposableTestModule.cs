namespace BaristaLabs.BaristaCore.MyTestModule
{
    using System;

    [BaristaModule("MyDisposableTestModule", "This is a test, this is only a test.")]
    public class DisposableTestModule : IBaristaModule, IDisposable
    {
        public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            return context.CreateString("The maze still isn't meant for you.");
        }

        #region IDisposable Support
        public bool IsDisposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                IsDisposed = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DisposableTestModule() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
