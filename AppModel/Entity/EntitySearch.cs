using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModel
{
    public class EntitySearch
    {
        [Display(Name = "Ref")]
        public string EntityRef { get; set; }

        [Display(Name = "Entity Type")]
        public int? EntityTypeID { get; set; }

        [Display(Name = "Date From")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "Date To")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateTo { get; set; }

    }
}
