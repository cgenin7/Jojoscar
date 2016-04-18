using System;


namespace JojoscarMVCBusinessLogic
{
    public class TPercent
    {
        public TPercent()
        {
            First = 36;
            Second = 24;
            Third = 17;
            Fourth = 13;
            Fifth = 10;
        }
        public int First { get; set; }
        public int Second { get; set; }
        public int Third { get; set; }
        public int Fourth { get; set; }
        public int Fifth { get; set; }
    }

    public class TPoint
    {
        public TPoint()
        {
            CategoryValue = new int[Calculation.NB_CATEGORIES];
        }
        public int[] CategoryValue { get; set; }
    }
}
