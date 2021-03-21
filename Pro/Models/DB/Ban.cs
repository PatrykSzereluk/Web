using System;
using System.Collections.Generic;

#nullable disable

namespace Pro.Models.DB
{
    public partial class Ban
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public byte BanReason { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }

        public virtual User User { get; set; }
    }
}
