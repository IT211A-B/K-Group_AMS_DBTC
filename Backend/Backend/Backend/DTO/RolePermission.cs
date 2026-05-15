using Backend.Backend.Model;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Backend.Backend.DTOs
{
    public class GetRolePermissionDTO
    {
        public string? Role_ID;

        public int Permission_ID;

        public IdentityRole? Role;

        public Permission? Permission;
    }

    public class AddRolePermissionDTO
    {
        public required string Role_ID;

        public int Permission_ID;

        public IdentityRole? Role;

        public Permission? Permission;
    }
}