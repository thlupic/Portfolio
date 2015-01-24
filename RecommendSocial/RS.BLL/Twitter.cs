using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Tweetinvi;
using System.Net.Http;
using System.Net.Http.Formatting;
using RS.Core;
using RS.DAL;

namespace RS.BLL
{
    public class Twitter
    {
        public static void Authenticate()
        {
            string oAuthConsumerKey = "CpGmHGNjVIIfxJ3rEB5nX4zXH";
            string oAuthConsumerSecret = "tdc55jotGTiNbFkZRymLLltqQpCZDv0BBHY4zq8xOeLsY1UWbV";
            //var oAuthUrl = "https://api.twitter.com/oauth2/token";

            var applicationCredentials = CredentialsCreator.GenerateApplicationCredentials(oAuthConsumerKey, oAuthConsumerSecret);
            var url = CredentialsCreator.GetAuthorizationURL(applicationCredentials);

            //authenticate on Twitter

            var authKey = applicationCredentials.AuthorizationKey;
            var authSecret = applicationCredentials.AuthorizationSecret;

            var callbackURL = HttpContext.Current.Request.Url.AbsoluteUri;

            var newCallBack = CredentialsCreator.GetAuthorizationURLForCallback(applicationCredentials, callbackURL);

            try
            {
                var newCredentials = CredentialsCreator.GetCredentialsFromCallbackURL(callbackURL, applicationCredentials);
                var accessToken = newCredentials.AccessToken;
                var accessTokenSecret = newCredentials.AccessTokenSecret;
            }
            catch
            {
                var exceptionStatusCode = ExceptionHandler.GetLastException().StatusCode;
                var exceptionDescription = ExceptionHandler.GetLastException().TwitterDescription;
                var exceptionDetails = ExceptionHandler.GetLastException().TwitterExceptionInfos.First().Message;
            }           
        }

        public static TwitterCore.TwitterAuth Authenticate1()
        {
            TwitterCore.TwitterAuth twitterAuth = new TwitterCore.TwitterAuth();
            string oAuthConsumerKey = "CpGmHGNjVIIfxJ3rEB5nX4zXH";
            string oAuthConsumerSecret = "tdc55jotGTiNbFkZRymLLltqQpCZDv0BBHY4zq8xOeLsY1UWbV";

            var applicationCredentials = CredentialsCreator.GenerateApplicationCredentials(oAuthConsumerKey, oAuthConsumerSecret);
            var url = CredentialsCreator.GetAuthorizationURL(applicationCredentials);

            var AuthKey = applicationCredentials.AuthorizationKey;
            var AuthSecret = applicationCredentials.AuthorizationSecret;

            twitterAuth.AuthKey = AuthKey;
            twitterAuth.AuthSecret = AuthSecret;
            twitterAuth.url = url;

            return twitterAuth;
        }

        public static TwitterCore.Tokens Authenticate2(TwitterCore.TwitterAuth twitterAuth)
        {
            TwitterCore.Tokens token = new TwitterCore.Tokens();
            string oAuthConsumerKey = "CpGmHGNjVIIfxJ3rEB5nX4zXH";
            string oAuthConsumerSecret = "tdc55jotGTiNbFkZRymLLltqQpCZDv0BBHY4zq8xOeLsY1UWbV";
            var applicationCredentials = CredentialsCreator.GenerateApplicationCredentials(oAuthConsumerKey, oAuthConsumerSecret);

            applicationCredentials.AuthorizationKey = twitterAuth.AuthKey;
            applicationCredentials.AuthorizationSecret = twitterAuth.AuthSecret;
            applicationCredentials.VerifierCode = twitterAuth.captcha;

            var newCredentials = CredentialsCreator.GetCredentialsFromVerifierCode(twitterAuth.captcha, applicationCredentials);
            //var newCredentials2 = CredentialsCreator.GetCredentialsFromVerifierCode(captcha, 
            token.AccessToken = newCredentials.AccessToken;
            token.AccessTokenSecret = newCredentials.AccessTokenSecret;
            
            return token;
        }

        public static void setTokens(long userID, TwitterCore.Tokens tokens)
        {
            Mapper.setTokens(userID, tokens);
        }

        public static TwitterCore.Tokens getTokens(long userID)
        {
            return Mapper.getTokens(userID);
        }

        public static UserCore.userData getUserData(long userID)
        {
            return Mapper.getUserData((int)userID);
        }

    }
}
