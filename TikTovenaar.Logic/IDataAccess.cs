using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTovenaar.Logic
{
    public interface IDataAccess
    {
        public void InsertScore(int userid, int wordsAmount, int timeInSeconds, int score, string[] incorrectWords, string[] incorrectLetters);
        public Dictionary<string, int> GetScores(); //int is the actual score, the string is the name of the player
    }
}
