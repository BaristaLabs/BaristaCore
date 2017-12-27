namespace BaristaLabs.BaristaCore
{
    delegate JsValue BaristaFunctionDelegate(JsObject calleeObj, bool isConstructCall, JsObject thisObj, object[] nativeArgs);
}
