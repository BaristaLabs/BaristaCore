namespace BaristaLabs.BaristaCore.JavaScript
{
    /// <summary>
    ///     Stepping types.
    /// </summary>
    public enum JavaScriptDiagStepType
    {
        /// <summary>
        ///     Perform a step operation to next statement.
        /// </summary>
        StepIn = 0,
        /// <summary>
        ///     Perform a step out from the current function.
        /// </summary>
        StepOut = 1,
        /// <summary>
        ///     Perform a single step over after a debug break if the next statement is a function call, else behaves as a stepin.
        /// </summary>
        StepOver = 2,
        /// <summary>
        ///     Perform a single step back to the previous statement (only applicable in TTD mode).
        /// </summary>
        StepBack = 3,
        /// <summary>
        ///     Perform a reverse continue operation (only applicable in TTD mode).
        /// </summary>
        ReverseContinue = 4
    }
}
