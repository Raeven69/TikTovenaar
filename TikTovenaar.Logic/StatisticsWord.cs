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

        public int totaalGespeelt { get; set; }
        public int totaalFout { get; set; }


        public int totaalGoed
        {
            get
            {
                return totaalGespeelt-totaalFout; 
            } 
        }

        public StatisticsWord(string woord, int totaalGespeelt, int totaalFout) 
        { 
            this.woord = woord;
            this.totaalGespeelt = totaalGespeelt;
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
