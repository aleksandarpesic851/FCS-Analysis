using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Online_FCS_Analysis.Utilities
{
    public class Constants
    {
        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_CUSTOMER = "Customer";
        public const string ROLE_PREMIUM_CUSTOMER = "Premium Customer";

        public const string ERROR_IN_LOGIN = "login";
        public const string ERROR_IN_REGISTER = "register";

        public const string CLAIM_TYPE_USER_NAME = "user_name";
        public const string CLAIM_TYPE_USER_ID = "id";
        public const string CLAIM_TYPE_USER_AVATAR = "user_avatar";

        public const string FCS_TYPE_WBC = "wbc";
        public const string FCS_TYPE_RBC = "rbc";

        public static string[] WBC_NOMENCLATURES = new string[] { "old_names", "middleaged_names", "new_names" };

        /* -----------File paths ----------*/
        public static string wwwroot_abs_path = Path.GetFullPath("./wwwroot");

        public static string wbc_fcs_path = "/uploads/fcs/wbc/fcs/";
        public static string wbc_fcs_full_path = Path.Combine(wwwroot_abs_path, "uploads/fcs/wbc/fcs");
        public static string wbc_3cell_path = "/uploads/fcs/wbc/3cell/";
        public static string wbc_3cell_full_path = Path.Combine(wwwroot_abs_path, "uploads/fcs/wbc/3cell");
        public static string wbc_heatmap_path = "/uploads/fcs/wbc/heatmap/";
        public static string wbc_heatmap_full_path = Path.Combine(wwwroot_abs_path, "uploads/fcs/wbc/heatmap");

        public static string rbc_fcs_path = "/uploads/fcs/rbc/fcs/";
        public static string rbc_fcs_full_path = Path.Combine(wwwroot_abs_path, "uploads/fcs/rbc/fcs");

        public static string avatar_path = "/uploads/avatars/";
        public static string avatar_full_path = Path.Combine(wwwroot_abs_path, "uploads/avatars");

        /* ---------- Heat Color ----------------*/
        public static byte Alpha = 0xff;
        public static List<Color> ColorsOfMap = new List<Color>()
        {
            Color.FromArgb(Alpha, 0, 0, 0) ,//Black
            Color.FromArgb(Alpha, 0, 0, 0xFF) ,//Blue
            Color.FromArgb(Alpha, 0, 0xFF, 0xFF) ,//Cyan
            Color.FromArgb(Alpha, 0, 0xFF, 0) ,//Green
            Color.FromArgb(Alpha, 0xFF, 0xFF, 0) ,//Yellow
            Color.FromArgb(Alpha, 0xFF, 0, 0) ,//Red
            Color.FromArgb(Alpha, 0xFF, 0xFF, 0xFF) // White
        };
    }
}
