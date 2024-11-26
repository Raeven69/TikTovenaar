namespace TikTovenaar.Logic
{
    public class Word
    {
        public List<Letter> Letters { get; } = [];
        public bool IsCompleted { get; private set; } = false;
        public bool IsWrong = false;
        public int Index { get; private set; } = 0;

        public Word(string word)
        {
            foreach (char c in word)
            {
                Letters.Add(new(c));
            }
        }

        public void EnterChar(char c)
        {
            if (c == ' ' && Index == Letters.Count)
            {
                IsCompleted = true;
            }
            else if (!IsCompleted)
            {
                if (Index > Letters.Count - 1)
                {
                    Letter newLetter = new(null)
                    {
                        Received = c
                    };
                    Letters.Add(newLetter);
                }
                else
                {
                    Letters[Index].Received = c;
                }
                Index++;
            }
        }
    }
}