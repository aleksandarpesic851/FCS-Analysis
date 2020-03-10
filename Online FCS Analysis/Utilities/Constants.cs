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

        public const string SHOW_LOGIN = "login";
        public const string SHOW_REGISTER = "register";

        public const string CLAIM_TYPE_USER_NAME = "user_name";
        public const string CLAIM_TYPE_USER_ID = "id";
        public const string CLAIM_TYPE_USER_AVATAR = "user_avatar";

        public const string FCS_TYPE_WBC = "wbc";
        public const string FCS_TYPE_WBC_EOS = "wbc_eos";
        public const string FCS_TYPE_RBC = "rbc";

        public static string[] WBC_NOMENCLATURES = new string[] { "old_names", "middleaged_names", "new_names" };

        /* -----------File paths ----------*/
        public static string wwwroot_abs_path = Path.GetFullPath("./wwwroot");

        public static string wbc_fcs_path = "/uploads/fcs/wbc/fcs/";
        public static string wbc_fcs_full_path = Path.Combine(wwwroot_abs_path, "uploads/fcs/wbc/fcs");
        public static string wbc_3cell_path = "/uploads/fcs/wbc/3cell/";
        public static string wbc_3cell_full_path = Path.Combine(wwwroot_abs_path, "uploads/fcs/wbc/3cell");
        public static string wbc_gate2_path = "/uploads/fcs/wbc/gate2/";
        public static string wbc_gate2_full_path = Path.Combine(wwwroot_abs_path, "uploads/fcs/wbc/gate2");
        public static string wbc_heatmap_path = "/uploads/fcs/wbc/heatmap/";
        public static string wbc_heatmap_full_path = Path.Combine(wwwroot_abs_path, "uploads/fcs/wbc/heatmap");
        public static string wbc_custom_full_gate = Path.Combine(wwwroot_abs_path, "uploads/fcs/wbc/custom_gate");
        public static string rbc_fcs_path = "/uploads/fcs/rbc/fcs/";
        public static string rbc_fcs_full_path = Path.Combine(wwwroot_abs_path, "uploads/fcs/rbc/fcs");

        public static string avatar_path = "/uploads/avatars/";
        public static string avatar_full_path = Path.Combine(wwwroot_abs_path, "uploads/avatars");

        public static string vhc_path = "record_table/result.txt";

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


        /* ---------- V, HC Param ----------------*/
        private const double MEAN_S1 = 0.30327732397673;
        private const double MEAN_S2 = 0.323369234247477;
        private const int MEAN_RBC_1 = 5710;
        private const int MEAN_RBC_2 = 4187;
        public  static double Kx = MEAN_S1 / MEAN_RBC_1;  // s1 = fcs1 * Kx
        public  static double Ky = MEAN_S2 / MEAN_RBC_2;  // s2 = fcs2 * Ky

    }
}
