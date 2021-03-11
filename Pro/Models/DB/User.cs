using System;
using System.Collections.Generic;

#nullable disable

namespace Pro.Models.DB
{
    public partial class User
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int UserTypeId { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? LastPasswordChanged { get; set; }
        public bool CheckPassword { get; set; }
        public string UserHash { get; set; }
        public string ControlHash { get; set; }
        public bool IsChangingPassword { get; set; }
    }
}
