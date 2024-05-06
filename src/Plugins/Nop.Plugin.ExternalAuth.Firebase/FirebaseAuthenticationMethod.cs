using Nop.Core;
using Nop.Plugin.ExternalAuth.Firebase.Components;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.ExternalAuth.Firebase
{
    /// <summary>
    /// Represents method for the authentication with Firebase account
    /// </summary>
    public class FirebaseAuthenticationMethod : BasePlugin, IExternalAuthenticationMethod
    {
        #region Fields

        protected readonly ILocalizationService _localizationService;
        protected readonly ISettingService _settingService;
        protected readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public FirebaseAuthenticationMethod(ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/FirebaseAuthentication/Configure";
        }

        /// <summary>
        /// Gets a type of a view component for displaying plugin in public store
        /// </summary>
        /// <returns>View component type</returns>
        public Type GetPublicViewComponent()
        {
            return typeof(FirebaseAuthenticationViewComponent);
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new FirebaseExternalAuthSettings());

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {

                ["Plugins.ExternalAuth.Firebase.ApiKey"] = "ApiKey",
                ["Plugins.ExternalAuth.Firebase.ApiKey.Hint"] = "ApiKey value from Firebase'a application settings",
                ["Plugins.ExternalAuth.Firebase.AuthDomain"] = "AuthDomain",
                ["Plugins.ExternalAuth.Firebase.AuthDomain.Hint"] = "AuthDomain value from Firebase'a application settings",
                ["Plugins.ExternalAuth.Firebase.ProjectId"] = "ProjectId",
                ["Plugins.ExternalAuth.Firebase.ProjectId.Hint"] = "ProjectId value from Firebase'a application settings",
                ["Plugins.ExternalAuth.Firebase.AppId"] = "AppId",
                ["Plugins.ExternalAuth.Firebase.AppId.Hint"] = "AppId value from Firebase'a application settings",
                ["Plugins.ExternalAuth.Firebase.Instructions"] = "<p>To configure authentication with Firebase, please create application and copy it's settings using firebase console",
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<FirebaseExternalAuthSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.ExternalAuth.Firebase");

            await base.UninstallAsync();
        }

        #endregion
    }
}