using System;
using System.ComponentModel.DataAnnotations;

namespace MyAwesomeWebApi.Models.Requests
{
    public class LoginEntity
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
