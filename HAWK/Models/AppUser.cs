using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HAWK.Models
{
    public class AppUser: IdentityUser
    {
        public string FullName { get; set; }
    }
}
