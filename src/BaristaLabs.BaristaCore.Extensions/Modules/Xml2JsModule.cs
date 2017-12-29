namespace BaristaLabs.BaristaCore.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    [BaristaModule("xml2js", "Simple XML to JavaScript object converter.")]
    public class Xml2JsModule : IBaristaModule
    {
        public Task<object> ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            throw new NotImplementedException();
        }
    }
}
