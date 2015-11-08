using Ass_Zed.Model.Enum;

namespace Ass_Zed.Model.Languages
{
    class Deutsch : LanguageController
    {
        public Deutsch()
        {
            Dictionary.Add(EnumContext.Version, "Fassung: ");
            Dictionary.Add(EnumContext.Creator, "Von Vector");
            Dictionary.Add(EnumContext.Draw, "Zeichnen");
            Dictionary.Add(EnumContext.TurnOffDraws, "Schalten Sie alle Zeichnungen");
            Dictionary.Add(EnumContext.Range, "Angebot");
            Dictionary.Add(EnumContext.Combo, "Combo");
            Dictionary.Add(EnumContext.Use, "Benutzen");
            Dictionary.Add(EnumContext.Harass, "Belästigen");
            Dictionary.Add(EnumContext.MinimunMana, "Mindestbewertung % Mana zu aktivieren");
            Dictionary.Add(EnumContext.Settings, "Einstellungen");
            Dictionary.Add(EnumContext.ComboStyle, "Nur in Passivbereich");
            Dictionary.Add(EnumContext.LastHit, "Letzter Schlag");
            Dictionary.Add(EnumContext.LaneClear, "Lane Klar");
            Dictionary.Add(EnumContext.NotificationMessage, " Loaded.");
        }
    }
}
