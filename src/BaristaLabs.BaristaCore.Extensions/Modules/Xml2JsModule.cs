namespace BaristaLabs.BaristaCore.Modules
{
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Xml;

    [BaristaModule("barista-xml2js", "Simple XML to JavaScript object converter.")]
    public class Xml2JsModule : IBaristaModule
    {
        public JsValue ExportDefault(BaristaContext context, BaristaModuleRecord referencingModule)
        {
            var toXml = context.CreateFunction(new Func<JsObject, JsValue, JsValue>((thisObj, json) =>
            {
                if (json == context.Undefined || json == context.Null || String.IsNullOrWhiteSpace(json.ToString()))
                {
                    var error = context.CreateError($"A Json string must be specified as the first argument.");
                    context.Engine.JsSetException(error.Handle);
                    return context.Undefined;
                }

                var jsonData = json.ToString();
                var xmlDoc = JsonConvert.DeserializeXmlNode(jsonData);
                using (var stringWriter = new StringWriter())
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    xmlDoc.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                    return context.CreateString(stringWriter.GetStringBuilder().ToString());
                }
            }));

            var toJson = context.CreateFunction(new Func<JsObject, JsValue, JsValue, JsValue>((thisObj, xml, options) =>
            {
                if (xml == context.Undefined || xml == context.Null || String.IsNullOrWhiteSpace(xml.ToString()))
                {
                    var error = context.CreateError($"An xml string must be specified as the first argument.");
                    context.Engine.JsSetException(error.Handle);
                    return context.Undefined;
                }

                var xmlData = xml.ToString();
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlData);

                bool toObject = false;
                bool omitRootObject = false;
                Newtonsoft.Json.Formatting formatting = Newtonsoft.Json.Formatting.None;
                if (options is JsObject jsOptions && jsOptions.Type == JsValueType.Object)
                {
                    if (jsOptions.HasProperty("object") && jsOptions["object"].ToBoolean() == true)
                        toObject = true;

                    if (jsOptions.HasProperty("omitRootObject") && jsOptions["omitRootObject"].ToBoolean() == true)
                        omitRootObject = true;

                    if (jsOptions.HasProperty("formatting") && Enum.TryParse(jsOptions["formatting"].ToString(), out Newtonsoft.Json.Formatting requestedFormatting))
                        formatting = requestedFormatting;

                }

                var json = JsonConvert.SerializeXmlNode(xmlDoc, formatting, omitRootObject);

                if (toObject)
                    return context.JSON.Parse(context.CreateString(json));
                return context.CreateString(json);
            }));

            var resultObj = context.CreateObject();
            context.Object.DefineProperty(resultObj, "toXml", new JsPropertyDescriptor
            {
                Enumerable = true,
                Value = toXml
            });

            context.Object.DefineProperty(resultObj, "toJson", new JsPropertyDescriptor
            {
                Enumerable = true,
                Value = toJson
            });

            return resultObj;
        }
    }
}
