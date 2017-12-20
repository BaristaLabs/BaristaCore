namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.JavaScript;
    using System;

    public interface IBaristaModuleRecordFactory : IDisposable
    {

        BaristaModuleRecord GetBaristaModuleRecord(JavaScriptModuleRecord moduleRecord);

        BaristaModuleRecord CreateBaristaModuleRecord(BaristaContext context, string moduleName, BaristaModuleRecord parentModule = null, bool setAsHost = false);
    }
}
