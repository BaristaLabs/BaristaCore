namespace BaristaLabs.BaristaCore
{
    public interface IBaristaModuleRecordFactory
    {
        BaristaModuleRecord CreateBaristaModuleRecord(BaristaContext context, string moduleName, BaristaModuleRecord parentModule = null, bool setAsHost = false);
    }
}
