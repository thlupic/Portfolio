using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using RS.Core;
using RS.DAL;
using System.Collections;


namespace RS.BLL
{
    public class Facebook
    {
      

        public static UserCore.userData CreateUser(dynamic me, List<UserCore.userData> users)
        {

            UserCore.userData user = new UserCore.userData();
            user.firstName = me.first_name;
            user.lastName = me.last_name;
            user.facebookID = me.id;
            var likes = me.likes;
            var friends = me.friends;


            //  napravi listu korisnikovih lajkova
            try
            {
                foreach (var like in likes.data)
                {
                    if (like.category.Equals("Movie"))
                    {
                        UserCore.LikeData movieLike = new UserCore.LikeData();
                        movieLike.likeID = like.id;
                        movieLike.name = like.name;
                        user.likes.Add(movieLike);
                    }
                }

            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

            //napuni listu prijatelja od korisnika

            // ako imas u bazi tog prijatelja onda i njemu nadopuni listu prijatelja s novim korisnikom

            foreach (var friend in friends.data)
            {

                if (users != null)
                {

                    UserCore.FriendData userFriend = new UserCore.FriendData();
                
                    UserCore.userData userToFriend = users.Where(n => n.facebookID == friend.id).FirstOrDefault();
                    if (userToFriend != null)
                    {
                        userFriend.friendID = userToFriend.facebookID;
                        userFriend.likes = userToFriend.likes;
                        user.friends.Add(userFriend);
                        UserCore.FriendData userFriendNew = new UserCore.FriendData();
                        userFriendNew.friendID = user.facebookID;
                        userFriendNew.likes = user.likes;
                        if (userToFriend.friends == null)
                        {                           
                            userToFriend.friends = new List<UserCore.FriendData>();
                        }
                        if (userToFriend.friends.Where(n => n.friendID == user.facebookID ).FirstOrDefault() == null)
                        {
                            userToFriend.friends.Add(userFriendNew);
                            // update postojeceg korisnika -- dodaj mu novog prijatelja
                            Mapper.updateUsers(userToFriend);
                        }
                     

                        
                    }
                }


            }
            // Set the auth cookie
            System.Web.Security.FormsAuthentication.SetAuthCookie(user.facebookID, false);


            //inace update korisnika ako je nesto promijenio
            user.id = RS.BLL.Facebook.getUserID(user.facebookID);
            if (user.id == 0)
            {
                RS.BLL.Facebook.saveUser(user.firstName, user.lastName, user.facebookID, user.likes, user.friends);
                users.Add(user);
            }
            else
            {
                
                UserCore.userData userToUpdate = RS.BLL.Facebook.getUser(user.id);

                var b = 0;
                foreach (var likeCompare in userToUpdate.likes)
                {
                    if(!user.likes.Any(p => p.name == likeCompare.name))
                    {
                       b = 1;
                    }
                    if (!user.likes.Any(p => p.likeID == likeCompare.likeID))
                    {
                       b = 1;
                    }
                }

                foreach (var likeCompare in user.likes)
                {
                    if (!userToUpdate.likes.Any(p => p.name == likeCompare.name))
                    {
                        b = 1;
                    }
                    if (!userToUpdate.likes.Any(p => p.likeID == likeCompare.likeID))
                    {
                        b = 1;
                    }
                }

                foreach (var friendCompare in userToUpdate.friends)
                {
                    if (!user.friends.Any(p => p.friendID == friendCompare.friendID))
                    {
                        b = 1;
                    }               
                }              

                foreach (var friendCompare in user.friends)
                {
                    if (!userToUpdate.friends.Any(p => p.friendID == friendCompare.friendID))
                    {
                        b = 1;
                    }
                }

                if (userToUpdate.lastName != user.lastName || userToUpdate.firstName != user.firstName || b==1)
                {
                    Mapper.updateUsers(user);
                }

            } 

            //RecommendMovies.recommend(user);
            return user;
        }


        private static UserCore.userData getUser(int id)
        {
            return Mapper.getUserData(id);
        }
        public static int getUserID(string facebookID)
        {
            return Mapper.getUserID(facebookID);
        }


        public static void saveUser(string firstName, string lastName, string facebookID, List<UserCore.LikeData> likes, List<UserCore.FriendData> friends)
        {
            int id = DataStorage.getLastID();
            Mapper.storeUserData(id +1, firstName, lastName, facebookID, likes,friends);
        }
        public static List<UserCore.userData> getUsers()
        {
            List<UserCore.DBuserData> List = DataStorage.getUsers();
            return mappFromCore(List);
        }

        public static List<UserCore.userData> mappFromCore(List<UserCore.DBuserData> listCore)
        {
            List<UserCore.userData> list = new List<UserCore.userData>();
            foreach (UserCore.DBuserData userCore in listCore)
            {
                UserCore.userData user = new UserCore.userData();
                user.id = userCore.id;
                user.firstName = userCore.firstName;
                user.lastName = userCore.lastName;
                user.facebookID = userCore.facebookID;
                user.likes = userCore.likes;
                user.friends = userCore.friends;
                list.Add(user);
            }
            return list;
        }



    }
}
