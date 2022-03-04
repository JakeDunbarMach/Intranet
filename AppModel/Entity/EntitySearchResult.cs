using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModel
{
    public class EntitySearchResult
    {

        public int EntityID { get; set; }

        public string EntityRef { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EntityDate { get; set; }

        public string EntityTypeName { get; set; }

    }
}
