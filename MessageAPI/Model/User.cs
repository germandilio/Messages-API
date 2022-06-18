using System;

namespace MessageAPI.Model
{
    [Serializable]
    public class User
    {
        public User()
        {
        }
        
        public User(string userName, string email, int id)
        {
            UserName = userName;
            Email = email;
            Id = id;
        }

        /// <summary>
        /// User's name.
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// User's email.
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// User's id.
        /// </summary>
        public int Id { get; private set; }
    }
}
