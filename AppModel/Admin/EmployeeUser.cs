using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppModel
{
    public class EmployeeUser
    {
        public Int32 EmployeeID { get; set; }

        [DisplayName("Full Name")]
        public string EmployeeFullName { get; set; }

        [UIHint("YesNoBoolActive")]
        [DisplayName("Active")]
        public Boolean IsActive { get; set; }

        public int UserEmployeeTypeID { get; set; }

        public int? ManagedByID { get; set; }

    }
}
