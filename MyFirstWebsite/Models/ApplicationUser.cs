﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Draft> Drafts { get; set; }
    }
}
