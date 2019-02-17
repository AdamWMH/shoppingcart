using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WooliesXAPI.Models
{
    public struct User
    {
        public string name, token;

        public User(string name, string token)
        {
            this.name = name;
            this.token = token;
        }
    }
}
