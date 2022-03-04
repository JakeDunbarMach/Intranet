using AppModel.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModel
{
    public class Entity
    {

        [Display(Name = "Entity")]
        public int EntityID { get; set; }

        [Display(Name = "Ref")]
        public string EntityRef { get; set; }

        [Required]
        [Display(Name = "Entity Date")]
        public DateTime EntityDate { get; set; }

        [Required]
        [Display(Name = "Entity Type")]
        public int EntityTypeID { get; set; }

    }
}
