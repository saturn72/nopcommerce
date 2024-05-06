namespace Nop.Plugin.ExternalAuth.Firebase
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public class FirebaseAuthenticationDefaults
    {
        /// <summary>
        /// Gets a plugin system name
        /// </summary>
        public static string SystemName => "ExternalAuth.Firebase";

        /// <summary>
        /// Gets a name of the route to the data deletion callback
        /// </summary>
        public static string DataDeletionCallbackRoute => "Plugin.ExternalAuth.Firebase.DataDeletionCallback";

        /// <summary>
        /// Gets a name of the route to the data deletion status check
        /// </summary>
        public static string DataDeletionStatusCheckRoute => "Plugin.ExternalAuth.Firebase.DataDeletionStatusCheck";

        /// <summary>
        /// Gets a name of error callback method
        /// </summary>
        public static string ErrorCallback => "ErrorCallback";
    }
}