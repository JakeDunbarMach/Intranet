using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppModel
{
    public class EmployeePermission
    {
        public Int32 EmployeePermID { get; set; }

        public Int32 EmployeeID { get; set; }

        [Required]
        [DisplayName("Section")]
        public Int32 SectionID { get; set; }

        public string Section { get; set; }

        [Required]
        [DisplayName("Permission")]
        public Int32 PermissionID { get; set; }

        public string Permission { get; set; }

        [UIHint("YesNoBool")]
        [DisplayName("Active")]
        public Boolean IsActive { get; set; }

    }
}
