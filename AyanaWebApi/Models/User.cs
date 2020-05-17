using System;

namespace AyanaWebApi.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string Token { get; set; }

        public DateTime Created { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }
    }
}
