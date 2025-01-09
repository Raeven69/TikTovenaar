using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTovenaar.Logic
{
    public class StatisticsWord
    {

        public string Word { get; set; }

        public int TotalPlayed 
        {
            get { return TotalCorrect + TotalWrong; }
        }
        public int TotalWrong { get; set; }


        public int TotalCorrect { get; set; }


        public StatisticsWord(string word, int totaalGoed, int totaalFout) 
        { 
            this.Word = word;
            this.TotalCorrect = totaalGoed;
            this.TotalWrong = totaalFout;
        }

        public string ToStringWord()
        {
            return Word;
        }

        public string ToStringStats()
        {
            return $"totaal gespeelt: {TotalPlayed}       ✅ {TotalCorrect}       ❎ {TotalWrong}";
        }

    }
}
