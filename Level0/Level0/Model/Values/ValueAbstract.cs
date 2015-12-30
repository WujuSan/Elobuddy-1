using LevelZero.Model.Enuns;

namespace LevelZero.Model.Values
{
    internal abstract class ValueAbstract
    {
        public string Identifier { get; set; }
        public string DisplayName { get; set; }
        public object CurrentValue { get; set; }
        public EnumMenuStyle EnumMenuStyle { get; set; }
    }
}
