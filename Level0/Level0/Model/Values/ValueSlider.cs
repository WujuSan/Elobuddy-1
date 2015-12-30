namespace LevelZero.Model.Values
{
    class ValueSlider : ValueAbstract
    {
        public int InitialValue { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }

        public ValueSlider(int maxValue, int minValue, int initialValue = 0)
        {
            InitialValue = initialValue;
            MaxValue = maxValue;
            MinValue = minValue;
        }
    }
}
