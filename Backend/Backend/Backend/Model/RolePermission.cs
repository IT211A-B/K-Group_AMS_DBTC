using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Backend.Model
{
    public class RolePermission
    {
        [Required]
        public required string Role_ID { get; set; } // Composite Key

        [Required]
        public required int Permission_ID { get; set; } // Composite Key Relation to Permission ID Superkey
        public Permission? Permission_Entity { get; set; } // por reperens  

        public IdentityRole? Role { get; set; } // porr reperens
    }
}