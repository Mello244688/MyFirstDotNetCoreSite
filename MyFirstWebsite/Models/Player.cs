using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstWebsite.Models
{
    public class Player
    {
        public int Id { get; set; }
        public int Rank { get; set; }
        public int PositionDrafted { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string PlayerUrl { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }
    }
}
