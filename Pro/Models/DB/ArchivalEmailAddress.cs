using System;
using System.Collections.Generic;

#nullable disable

namespace Pro.Models.DB
{
    public partial class ArchivalEmailAddress
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string EmailAddress { get; set; }

        public virtual User User { get; set; }
    }
}
