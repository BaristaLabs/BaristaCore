namespace BaristaLabs.BaristaCore.JavaScript
{
    using System;

    /// <summary>
    ///     A Promise Rejection Tracker callback.
    /// </summary>
    /// <remarks>
    ///     The host can specify a promise rejection tracker callback in <c>JsSetHostPromiseRejectionTracker</c>.
    ///     If a promise is rejected with no reactions or a reaction is added to a promise that was rejected
    ///     before it had reactions by default nothing is done.
    ///     A Promise Rejection Tracker callback may be set - which will then be called when this occurs.
    ///     Note - per draft ECMASpec 2018 25.4.1.9 this function should not set or return an exception
    ///     Note also the promise and reason parameters may be garbage collected after this function is called
    ///     if you wish to make further use of them you will need to use JsAddRef to preserve them
    ///     However if you use JsAddRef you must also call JsRelease and not hold unto them after 
    ///     a handled notification (both per spec and to avoid memory leaks)
    /// </remarks>
    /// <param name="promise">The promise object, represented as a JsValueRef.</param>
    /// <param name="reason">The value/cause of the rejection, represented as a JsValueRef.</param>
    /// <param name="handled">Boolean - false for promiseRejected: i.e. if the promise has just been rejected with no handler, 
    ///                         true for promiseHandled: i.e. if it was rejected before without a handler and is now being handled.</param>
    /// <param name="callbackState">The state passed to <c>JsSetHostPromiseRejectionTracker</c>.</param>
    public delegate void JavaScriptPromiseRejectionTrackerCallback(JavaScriptValueSafeHandle promise, JavaScriptValueSafeHandle reason, bool handled, IntPtr callbackState);
}
