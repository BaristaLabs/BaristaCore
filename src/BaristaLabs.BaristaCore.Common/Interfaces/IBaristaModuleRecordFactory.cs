namespace BaristaLabs.BaristaCore
{
    using System;

    public interface IBaristaModuleRecordFactory : IDisposable
    {
        BaristaModuleRecord CreateBaristaModuleRecord(BaristaContext context, string moduleName, BaristaModuleRecord parentModule = null, bool setAsHost = false);
    }
}
