using System;

namespace Unity.GraphToolkit.CSO
{
    static class StateObserverHelper
    {
        /// <summary>
        /// The currently active observer.
        /// </summary>
        public static IStateObserver CurrentObserver { get; set; }
    }
}
