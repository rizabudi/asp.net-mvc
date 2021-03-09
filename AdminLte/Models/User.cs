using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AdminLte.Models
{
    public class User : IdentityUser
    {
        public virtual BackendUser BackendUser { get; set; }
        public virtual ParticipantUser ParticipantUser { get; set; }
    }

    public class BackendUser
    {
        [Key, ForeignKey("User")]
        public string UserId { get; set; }
        public String Name { get; set; }
        public Entity Entity { get; set; }
        public virtual User User { get; set; }
    }

    public class ParticipantUser
    {
        [Key, ForeignKey("User")]
        public string UserId { get; set; }
        public String Name { get; set; }
        public String Email { get; set; }
        public String Phone { get; set; }
        public String EmployeeNumber { get; set; }
        public Boolean Sex { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? WorkDuration { get; set; }
        public DateTime DeletedAt { get; set; }
        public Entity Entity { get; set; }
        public Position Position { get; set; }
        public CompanyFunction CompanyFunction { get; set; }
        public Divition Divition { get; set; }
        public Department Department { get; set; }
        public JobLevel JobLevel { get; set; }
        public virtual User User { get; set; }
    }
}
