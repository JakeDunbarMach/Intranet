using AppModel.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModel
{
    public class GenericList
    {

        public int ID { get; set; }

        public string Name { get; set; }

        public Boolean Selected { get; set; }

        public int? ParentID { get; set; }

        public string Parent { get; set; }

        public GenericList() { }

        public GenericList(int id, string name)
        {
            this.ID = id;
            this.Name = name;
        }

        public GenericList(int id, string name, Boolean selected)
        {
            this.ID = id;
            this.Name = name;
            this.Selected = selected;
        }

    }
}
