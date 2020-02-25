using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Online_FCS_Analysis.Models.Entities;
using Online_FCS_Analysis.Utilities;
using Microsoft.AspNetCore.Http;

namespace Online_FCS_Analysis.Models.ViewModel
{
    public class UserViewModel
    {
        public int id { get; set; }
        [EmailAddress]
        [Required]
        public string user_email { get; set; }
        [StringLength(100, MinimumLength =5)]
        [Required]
        public string user_password { get; set; }
        [Required]
        public string user_name { get; set; }
        public string user_avatar { get; set; }
        public IFormFile user_avatar_image { get; set; }
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;
        public string user_role { get; set; } = Constants.ROLE_CUSTOMER;
        [Phone]
        public string user_phone { get; set; }
        public string user_address { get; set; }
        public bool user_activated { get; set; } = true;

        public static implicit operator UserViewModel(UserModel user)
        {
            return new UserViewModel
            {
                id = user.id,
                user_email = user.user_email,
                user_password = user.user_password,
                user_name = user.user_name,
                user_avatar = user.user_avatar,
                user_role = user.user_role,
                user_phone = user.user_phone,
                user_address = user.user_address,
                user_activated = user.user_activated,
            };
        }

        public static implicit operator UserModel(UserViewModel user)
        {
            return new UserModel
            {
                id = user.id,
                user_email = user.user_email,
                user_password = user.user_password,
                user_name = user.user_name,
                user_avatar = user.user_avatar,
                user_role = user.user_role,
                user_phone = user.user_phone,
                user_address = user.user_address,
                user_activated = user.user_activated,
            };
        }
    }
}
