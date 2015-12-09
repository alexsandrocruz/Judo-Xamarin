namespace JudoDotNetXamarin
{
    internal class CSRange
    {
        public int Location{ get; set; }

        public int Length{ get; set; }

        public CSRange (int location, int length)
        {
            Location = location;
            Length = length;
        }

        public CSRange ()
        {
        }
    }
}

