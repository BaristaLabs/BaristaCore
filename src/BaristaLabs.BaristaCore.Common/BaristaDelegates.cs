namespace BaristaLabs.BaristaCore
{
    public delegate JsValue BaristaFunctionDelegate(JsObject calleeObj, bool isConstructCall, JsObject thisObj, object[] nativeArgs);
}
