using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTovenaar.Logic
{
    public class StatisticsWord
    {

        public string woord { get; set; }

        public int totaalGespeelt 
        {
            get { return totaalGoed + totaalFout; }
        }
        public int totaalFout { get; set; }


        public int totaalGoed { get; set; }


        public StatisticsWord(string woord, int totaalGoed, int totaalFout) 
        { 
            this.woord = woord;
            this.totaalGoed = totaalGoed;
            this.totaalFout = totaalFout;
        }

        public string ToStringWord()
        {
            return woord;
        }

        public string ToStringStats()
        {
            return $"totaal gespeelt: {totaalGespeelt}       ✅ {totaalGoed}       ❎ {totaalFout}";
        }

    }
}
