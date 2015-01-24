using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RS.Core;

namespace RS.DAL
{
    public static class Mapper 
    {
        public static void setTokens(long userID, TwitterCore.Tokens tokens)
        {
            TwitterCore.User userData = new TwitterCore.User();
            userData.ID = userID;
            userData.AccessToken = tokens.AccessToken;
            userData.AccessTokenSecret = tokens.AccessTokenSecret;
            DataStorage.setTokens(userData);
        }

        public static TwitterCore.Tokens getTokens(long userID)
        {
            TwitterCore.Tokens tokens = new TwitterCore.Tokens();
            TwitterCore.User user = DataStorage.getTokens(userID);

            tokens.AccessToken = user.AccessToken;
            tokens.AccessTokenSecret = user.AccessTokenSecret;

            return tokens;
        }

        public static UserCore.userData getUserData(int userID)
        {
            UserCore.userData userData = new UserCore.userData();

            var data = DataStorage.getUserData(userID);

            userData.id = data.id;
            userData.firstName = data.firstName;
            userData.lastName = data.lastName;
            userData.facebookID = data.facebookID;
            userData.likes = data.likes;
            userData.friends = data.friends;

            return userData;
        }

        public static void storeUserData(int id, string firstName, string lastName, string facebookID, List<UserCore.LikeData> like, List<UserCore.FriendData> friends)
        {

            UserCore.DBuserData userData = new UserCore.DBuserData();
            userData.firstName = firstName;
            userData.lastName = lastName;
            userData.facebookID = facebookID;
            userData.likes = like;
            userData.friends = friends;
            userData.id = id;

            DataStorage.storeUserData(userData);
        }

        public static int getUserID(string facebookID)
        {
            return DataStorage.getUserID(facebookID);
        }

        public static long getTwitterID(int userID)
        {
            return DataStorage.getTwitterID(userID);
        }

        public static void updateFriends(UserCore.userData userToFriend)
        {
            DataStorage.UpdateFriends(userToFriend);
        }


        public static void updateUsers( UserCore.userData noviUser)
        {
               DataStorage.UpdateUser(noviUser);        
        }
    }
}
