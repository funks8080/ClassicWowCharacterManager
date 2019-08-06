using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicWowCharacterManager.Models
{
    public class Character
    {
        public string Name { get; set; }
        public FactionRaceClass Characteristics { get; set; }
        public int Level { get; set; }
    }
}
