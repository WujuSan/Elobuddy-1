using Ass_Zed.Model.Enum;

namespace Ass_Zed.Model.Languages
{
    class Francais : LanguageController
    {
        /*
            Translated by Ban <3
            */
        public Francais()
        {
            Dictionary.Add(EnumContext.Version, "Version: ");
            Dictionary.Add(EnumContext.Creator, "Par Vector");
            Dictionary.Add(EnumContext.Draw, "Tracés");
            Dictionary.Add(EnumContext.TurnOffDraws, "Desactiver les tracés");
            Dictionary.Add(EnumContext.Range, "Portée");
            Dictionary.Add(EnumContext.Combo, "Combo");
            Dictionary.Add(EnumContext.Use, "Utiliser");
            Dictionary.Add(EnumContext.Harass, "Harass");
            Dictionary.Add(EnumContext.MinimunMana, "% de Mana minimum pour activer");
            Dictionary.Add(EnumContext.Settings, "Réglages");
            Dictionary.Add(EnumContext.ComboStyle, "Seulement a porté du passif");
            Dictionary.Add(EnumContext.LastHit, "Last Hit");
            Dictionary.Add(EnumContext.LaneClear, "Lane Clear");
            Dictionary.Add(EnumContext.NotificationMessage, " Chargé.");
        }
    }
}
