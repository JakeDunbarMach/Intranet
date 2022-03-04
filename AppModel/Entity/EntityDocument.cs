using AppModel.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModel
{
    public class EntityDocument
    {

        public int EntityDocumentID { get; set; }

        [Required]
        public int EntityID { get; set; }

        [Required]
        public string DocumentName { get; set; }

        [Required]
        public string DocumentPath { get; set; }

    }
}
