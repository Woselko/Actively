﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace PlayMakerDomain.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public virtual IEnumerable<Game> Games { get; set; }
        public virtual IEnumerable<Payment> Payments { get; set; }
        public ApplicationUser? User { get; set; } = null;
        public string? UserId { get; set; } = null;
       
    }
}
