﻿namespace Nancy.OAuth
{
    using System;
    using ModelBinding;

    public class AccessTokenEndPoint : NancyModule
    {
        public AccessTokenEndPoint(IAccessTokenEndPointService service) : base("/oauth/access_token")
        {
            Post["/"] = parameters =>{

                var model =
                    this.Bind<AccessTokenRequest>();

                // Perhaps always validate that the grant type == "authorization_code" and
                // return an error with unsupported_grant_type message???

                // Needs to validate that the authorization code was issues to the logged in
                // user and nobody else. Also need to verify the redirect_uri. Possibly verify
                // that the code is still valid to use (time-to-live)
                var result =
                    service.ValidateRequest(model, this.Context);

                if (!result.IsValid)
                {
                    // TODO: Return an error based on result.ErrorType
                }

                var response =
                    service.CreateAccessTokenResponse(model, this.Context);

                // TODO: need to set "Cache-Control: no-store" and "Pragma: no-cache" headers on the response to comply with the specification
                return Response.AsJson(response);
            };
        }
    }

    public class AccessTokenResponse
    {
        /// <summary>
        /// The access token issued by the authorization server.
        /// </summary>
        /// <remarks>This parameter is REQUIRED.</remarks>
        public string Access_Token { get; set; }

        /// <summary>
        /// The type of the token issued. Value is case insensitive.
        /// </summary>
        /// <remarks>This parameter is REQUIRED.</remarks>
        public string Token_Type { get; set; }

        /// <summary>
        /// The lifetime in seconds of the access token.  For example, the value "3600" denotes 
        /// that the access token will expire in one hour from the time the response was generated.
        /// If omitted, the authorization server SHOULD provide the expiration time via other 
        /// means or document the default value.
        /// </summary>
        /// <remarks>This parameter is RECOMMENDED.</remarks>
        public string Expires_In { get; set; }

        /// <summary>
        /// The refresh token which can be used to obtain new access tokens using the same authorization grant
        /// </summary>
        /// <remarks>This parameter is OPTIONAL.</remarks>
        public string Refresh_Token { get; set; }

        /// <summary>
        /// The scope of the access token.
        /// </summary>
        /// <remarks>This parameter is OPTIONAL, if identical to the scope requested by the client, otherwise REQUIRED.</remarks>
        public string Scope { get; set; }
    }

    public interface IAccessTokenEndPointService
    {
        AccessTokenResponse CreateAccessTokenResponse(AccessTokenRequest tokenRequest, NancyContext context);

        AccessTokenRequestValidationResult ValidateRequest(AccessTokenRequest tokenRequest, NancyContext context);
    }

    public class DefaultAccessTokenEndPointService : IAccessTokenEndPointService
    {
        public AccessTokenResponse CreateAccessTokenResponse(AccessTokenRequest tokenRequest, NancyContext context)
        {
            return new AccessTokenResponse
            {
                Access_Token = string.Concat("access-token-", Guid.NewGuid().ToString("D"))
            };
        }

        public AccessTokenRequestValidationResult ValidateRequest(AccessTokenRequest tokenRequest, NancyContext context)
        {
            return new AccessTokenRequestValidationResult(AccessTokenErrorType.None);
        }
    }

    public enum AccessTokenErrorType
    {
        None,
        InvalidRequest,
        InvalidClient,
        InvalidGrant,
        UnauthorizedClient,
        UnsupportedGrantType,
        InvalidScope
    }

    public class AccessTokenRequestValidationResult
    {
        public AccessTokenRequestValidationResult(AccessTokenErrorType errorType)
        {
            this.ErrorType = errorType;
        }

        public AccessTokenErrorType ErrorType { get; set; }

        public bool IsValid
        {
            get { return this.ErrorType == AccessTokenErrorType.None; }
        }
    }
}