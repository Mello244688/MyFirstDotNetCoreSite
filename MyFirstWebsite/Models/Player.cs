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
        public string Name { get; set; }
        public string Position { get; set; }

        public ICollection<DraftPlayer> DraftPlayer { get; set; }
        public ICollection<TeamPlayer> TeamPlayer { get; set; }
    }
}
