using System;
using System.Collections.Generic;
namespace PMS.Models
{
    public partial class Admin
    {
        public int Id { get; set; }
        public string Admin_Name { get; set; }
        public string Password { get; set; }
    }
}
