using System;
using AspNetCore.Identity.Mongo;

namespace MyAwesomeWebApi.Models.Identity
{
    //Add any custom field for a user
    public class ApplicationUser : MongoIdentityUser
    {
        public string Name { get; set; }

        public string LastName { get; set; }

        public string City { get; set; }
    }
}
