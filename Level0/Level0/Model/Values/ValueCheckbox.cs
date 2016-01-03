using LevelZero.Model.Enuns;

namespace LevelZero.Model.Values
{
    class ValueCheckbox : ValueAbstract
    {
        public bool InitialValue { get; set; }

        public ValueCheckbox(bool initialValue, string identifier, string displayName)
        {
            Identifier = identifier;
            DisplayName = displayName;
            InitialValue = initialValue;
            EnumMenuStyle = EnumMenuStyle.Checkbox;
        }
    }
}
