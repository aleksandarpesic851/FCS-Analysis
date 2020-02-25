using Online_FCS_Analysis.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Online_FCS_Analysis.Models.Entities
{
    public class FCSModel : BaseEntityModel
    {
        [Required]
        public string fcs_name { get; set; }
        [Required]
        public string fcs_path { get; set; }
        [Required]
        public int user_id { get; set; }
        [Required]
        public bool is_shared { get; set; } = false;
        [NotMapped]
        public WBC3Cell wBC3Cell
        {
            get
            {
                return Global.FromByteArray<WBC3Cell>(wbc_3_cells);
            }
            set
            {
                wbc_3_cells = Global.ToByteArray<WBC3Cell>(wBC3Cell);
            }
        }
        [Required]
        public string fcs_type { get; set; } = Constants.FCS_TYPE_WBC;
        [Required]
        public Byte[] wbc_3_cells { get; set; }
    }
}
