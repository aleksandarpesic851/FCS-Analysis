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
        public string fcs_type { get; set; } = Constants.FCS_TYPE_WBC;
        [Required]
        public string wbc_3cells { get; set; }
        [Required] 
        public string wbc_heatmap { get; set; }
        [Required]
        public int nomenclature { get; set; } = 0;
    }
}
