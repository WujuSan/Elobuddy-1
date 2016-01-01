using System;
using LevelZero.Model.Enuns;

namespace LevelZero.Model.Values
{
    class ValueSlider : ValueAbstract
    {
        public int InitialValue { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }

        public ValueSlider(int maxValue, int minValue, int initialValue, string indentifier, string displayName)
        {
            DisplayName = displayName;
            Identifier = indentifier;
            InitialValue = initialValue;
            MaxValue = maxValue;
            MinValue = minValue;
            EnumMenuStyle = EnumMenuStyle.Slider;
        }
    }
}
