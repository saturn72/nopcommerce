//using System.IdentityModel.Tokens.Jwt;
//using System.Net;
//using DocumentFormat.OpenXml.Bibliography;
//using Microsoft.AspNetCore.Http;
//using Microsoft.IdentityModel.Tokens;

//namespace KM.Api.Infrastructure;
//public sealed class FirebaseAuthenticationMiddleware
//{
//    public readonly RequestDelegate _next;
//    private readonly JwtSecurityTokenHandler _tokenHandler = new();
//    private readonly TokenValidationParameters _validationParameters;

//    public FirebaseAuthenticationMiddleware(RequestDelegate next, FirebaseAdapter adapter)
//    {
//        _next = next;

//        var auth = adapter.GetAuth();
//        _validationParameters = new
//        {
//            ValidateIssuer = true,
//            ValidIssuer = oidc.issuer,
//            ValidateAudience = true,
//            ValidAudience = audience
//          ValidateIssuerSigningKey = true,
//            IssuerSigningKeys = signingKeys,
//            ValidateLifetime = true
//        };
//    }
//    public async Task Invoke(HttpContext context)
//    {

//        if (!context.Request.Headers.TryGetValue("Authorization", out var token) ||
//        string.IsNullOrEmpty(token) ||
//        string.IsNullOrWhiteSpace(token))
//        {
//            context.Response.ContentType = "application/json";
//            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
//            await context.Response.StartAsync();
//            return;
//        }

//        _tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
//        jwt = (JwtSecurityToken)validatedToken;

//        throw new NotImplementedException("");
//    }
//}
