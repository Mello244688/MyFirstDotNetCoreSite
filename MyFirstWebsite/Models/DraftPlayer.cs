using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Models
{
    public class DraftPlayer
    {
        public int DraftId { get; set; }
        public Draft Draft { get; set; }
        public int PlayerId { get; set; }
        public Player Player { get; set; }
    }
}
