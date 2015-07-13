using UnityEngine;
using System.Collections.Generic;


namespace GoodFish
{
    [System.Serializable]
    public class UserManager : MonoBehaviour
    {
    	public List<User> users;

        private int nextUser = 0;

        public User localUser;

        void Awake()
        {
            nextUser = users.Count;
        }

        public User AddUser()
        {
            GameObject addedUser = new GameObject("User"+nextUser);
            ++nextUser;
            return addedUser.AddComponent<User>();
        }

        public void StartGame()
        {
            foreach(User user in users)
            {
                user.StartGame();
                user.SetAI(true);
            }
            ActivateLocalUser(null);
        }


        public void ToggleLocalUser()
        {
            User nextUser = null;
            if( localUser != null )
            {
                nextUser = localUser;
                do
                {
                    int localUserIndex = users.IndexOf(nextUser) + 1;    
                    if( localUserIndex >= users.Count )
                    {
                        localUserIndex = 0;
                    }
                    nextUser = users[localUserIndex];
                }
                while(!nextUser.isControllable);
            }
            ActivateLocalUser(nextUser);
        }

        public void ActivateLocalUser(User nextUser)
        {
            if( nextUser == null )
            {
                nextUser = users.Find(x => x.isControllable);
            }
            if( localUser != null )
            {
                localUser.SetAI(true);
            }
            localUser = nextUser;
            localUser.SetAI(false);
            localUser.Activate();
        }
    }
}