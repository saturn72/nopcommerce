using System.Diagnostics.Metrics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nop.Core;
using Nop.Plugin.ExternalAuth.Firebase.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Pipelines.Sockets.Unofficial.Arenas;

namespace Nop.Plugin.ExternalAuth.Firebase.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class FirebaseAuthenticationController : BasePluginController
    {
#warning remove this from here
        public const string FirebaseScheme = "firebase schema name";
        #region Fields

        protected readonly FirebaseExternalAuthSettings _firebaseExternalAuthSettings;
        protected readonly IAuthenticationPluginManager _authenticationPluginManager;
        protected readonly IExternalAuthenticationService _externalAuthenticationService;
        protected readonly ILocalizationService _localizationService;
        protected readonly INotificationService _notificationService;
        protected readonly IOptionsMonitorCache<object> _optionsCache;
        protected readonly IPermissionService _permissionService;
        protected readonly ISettingService _settingService;
        protected readonly IStoreContext _storeContext;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public FirebaseAuthenticationController(FirebaseExternalAuthSettings firebaseExternalAuthSettings,
            IAuthenticationPluginManager authenticationPluginManager,
            IExternalAuthenticationService externalAuthenticationService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IOptionsMonitorCache<object> optionsCache,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _firebaseExternalAuthSettings = firebaseExternalAuthSettings;
            _authenticationPluginManager = authenticationPluginManager;
            _externalAuthenticationService = externalAuthenticationService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _optionsCache = optionsCache;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            var model = new ConfigurationModel
            {
                ApiKey = _firebaseExternalAuthSettings.ApiKey,
                AuthDomain = _firebaseExternalAuthSettings.AuthDomain,
                ProjectId = _firebaseExternalAuthSettings.ProjectId,
                AppId = _firebaseExternalAuthSettings.AppId,
            };

            return View("~/Plugins/ExternalAuth.Firebase/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //save settings
            _firebaseExternalAuthSettings.ApiKey = model.ApiKey;
            _firebaseExternalAuthSettings.AuthDomain = model.AuthDomain;
            _firebaseExternalAuthSettings.ProjectId = model.ProjectId;
            _firebaseExternalAuthSettings.AppId = model.AppId;

            await _settingService.SaveSettingAsync(_firebaseExternalAuthSettings);

            //clear Firebase authentication options cache
            _optionsCache.TryRemove(FirebaseScheme);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        public async Task<IActionResult> Login(string returnUrl)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var methodIsAvailable = await _authenticationPluginManager
                .IsPluginActiveAsync(FirebaseAuthenticationDefaults.SystemName, await _workContext.GetCurrentCustomerAsync(), store.Id);
            if (!methodIsAvailable)
                throw new NopException("Firebase authentication module cannot be loaded");

            if (string.IsNullOrEmpty(_firebaseExternalAuthSettings.AppId) ||
                string.IsNullOrEmpty(_firebaseExternalAuthSettings.AuthDomain))
            {
                throw new NopException("Firebase authentication module not configured");
            }

            //configure login callback action
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("LoginCallback", "FirebaseAuthentication", new { returnUrl = returnUrl })
            };
            authenticationProperties.SetString(FirebaseAuthenticationDefaults.ErrorCallback, Url.RouteUrl("Login", new { returnUrl }));

            return Challenge(authenticationProperties, FirebaseScheme);
        }

        public async Task<IActionResult> LoginCallback(string returnUrl)
        {
            //authenticate Firebase user
            var authenticateResult = await HttpContext.AuthenticateAsync(FirebaseScheme);
            if (!authenticateResult.Succeeded || !authenticateResult.Principal.Claims.Any())
                return RedirectToRoute("Login");

            //create external authentication parameters
            var authenticationParameters = new ExternalAuthenticationParameters
            {
                ProviderSystemName = FirebaseAuthenticationDefaults.SystemName,
                AccessToken = await HttpContext.GetTokenAsync(FirebaseScheme, "access_token"),
                Email = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email)?.Value,
                ExternalIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
                ExternalDisplayIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name)?.Value,
                Claims = authenticateResult.Principal.Claims.Select(claim => new ExternalAuthenticationClaim(claim.Type, claim.Value)).ToList()
            };

            //authenticate Nop user
            return await _externalAuthenticationService.AuthenticateAsync(authenticationParameters, returnUrl);
        }

        public async Task<IActionResult> DataDeletionStatusCheck(int earId)
        {
            var externalAuthenticationRecord = await _externalAuthenticationService.GetExternalAuthenticationRecordByIdAsync(earId);
            if (externalAuthenticationRecord is not null)
                _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Plugins.ExternalAuth.Firebase.AuthenticationDataExist"));
            else
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.ExternalAuth.Firebase.AuthenticationDataDeletedSuccessfully"));

            return RedirectToRoute("CustomerInfo");
        }

        #endregion
    }
}