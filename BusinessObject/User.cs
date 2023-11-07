
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class User : IdentityUser
    {
        public string? FirstName { set; get; }
        public string? LastName { set; get; }

        public IList<Order>? Orders { set; get; }
    }
}
