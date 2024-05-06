using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.Firebase.Infrastructure
{
    /// <summary>
    /// Represents registrar of Firebase authentication service
    /// </summary>
    public class FirebaseAuthenticationRegistrar : IExternalAuthenticationRegistrar
    {
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="builder">Authentication builder</param>
        public void Configure(AuthenticationBuilder builder)
        {
            //builder.AddFirebase(FirebaseDefaults.AuthenticationScheme, options =>
            //{
            //    //set credentials
            //    var settings = EngineContext.Current.Resolve<FirebaseExternalAuthSettings>();

            //    options.AppId = string.IsNullOrEmpty(settings?.ClientKeyIdentifier) ? nameof(options.AppId) : settings.ClientKeyIdentifier;
            //    options.AppSecret = string.IsNullOrEmpty(settings?.ClientSecret) ? nameof(options.AppSecret) : settings.ClientSecret;

            //    //store access and refresh tokens for the further usage
            //    options.SaveTokens = true;

            //    //set custom events handlers
            //    options.Events = new OAuthEvents
            //    {
            //        //in case of error, redirect the user to the specified URL
            //        OnRemoteFailure = context =>
            //        {
            //            context.HandleResponse();

            //            var errorUrl = context.Properties.GetString(FirebaseAuthenticationDefaults.ErrorCallback);
            //            context.Response.Redirect(errorUrl);

            //            return Task.FromResult(0);
            //        }
            //    };
            //});
        }
    }
}