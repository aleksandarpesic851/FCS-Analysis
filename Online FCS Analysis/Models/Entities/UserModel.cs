using Online_FCS_Analysis.Utilities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Online_FCS_Analysis.Models.Entities
{
    public class UserModel : BaseEntityModel
    {
        [EmailAddress]
        [Required]
        public string user_email { get; set; }
        [StringLength(100, MinimumLength = 5)]
        [Required]
        public string user_password { get; set; }
        [Required]
        public string user_name { get; set; }
        public string user_avatar { get; set; }
        public string user_role { get; set; } = Constants.ROLE_CUSTOMER;
        [Phone]
        public string user_phone { get; set; }
        public string user_address { get; set; }
    }
}
