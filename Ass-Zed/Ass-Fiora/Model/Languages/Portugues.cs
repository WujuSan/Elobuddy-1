using Ass_Zed.Model.Enum;

namespace Ass_Zed.Model.Languages
{
    class Portugues : LanguageController
    {
        public Portugues()
        {
            Dictionary.Add(EnumContext.Version, "Versão: ");
            Dictionary.Add(EnumContext.Creator, "Por Vector");
            Dictionary.Add(EnumContext.Draw, "Mostrar");
            Dictionary.Add(EnumContext.TurnOffDraws, "Desligar Desenhos");
            Dictionary.Add(EnumContext.Range, "Alcance");
            Dictionary.Add(EnumContext.Combo, "Combo");
            Dictionary.Add(EnumContext.Use, "Usar");
            Dictionary.Add(EnumContext.Harass, "Harass");
            Dictionary.Add(EnumContext.MinimunMana, "% Minima de Mana Para Ativar");
            Dictionary.Add(EnumContext.Settings, "Opções");
            Dictionary.Add(EnumContext.ComboStyle, "Somente no Alcance da Passiva");
            Dictionary.Add(EnumContext.LastHit, "Ultimo Ataque");
            Dictionary.Add(EnumContext.LaneClear, "Lane Clear");
            Dictionary.Add(EnumContext.NotificationMessage, " Carregado.");
        }
    }
}
