using LevelZero.Model;
using EloBuddy;
using EloBuddy.SDK.Events;
using LevelZero.Model.Values;
using System.Collections.Generic;
using static EloBuddy.SDK.Spell;
using EloBuddy.SDK.Enumerations;

namespace LevelZero.Core.Champions
{
    class Tristana : PluginModel
    {
        public override void Init()
        {
            Spells = new List<SpellBase>
            {
                new Active(SpellSlot.Q),
                new Skillshot(SpellSlot.W, 1100, SkillShotType.Circular),
                new Targeted(SpellSlot.E, 600),
                new Targeted(SpellSlot.R, 600)
            };
        }

        public override void InitMenu()
        {
            var feature = new Feature
            {
                NameFeature = "Draw",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(false, "disable", "Disable"),
                    new ValueCheckbox(false, "draw.q", "Draw Q"),
                    new ValueCheckbox(false, "draw.w", "Draw W"),
                    new ValueCheckbox(false, "draw.e", "Draw E"),
                    new ValueCheckbox(false, "draw.r", "Draw R")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Combo",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true,  "combo.q", "Combo Q"),
                    new ValueCheckbox(false, "combo.w", "Combo W"),
                    new ValueCheckbox(true,  "combo.e", "Combo E"),
                    new ValueCheckbox(true,  "combo.r", "Combo R")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Harass",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true,  "harass.q", "Harass Q"),
                    new ValueCheckbox(false, "harass.w", "Harass W"),
                    new ValueCheckbox(true,  "harass.e", "Harass E")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Lane Clear",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true,  "laneclear.q", "Lane Clear Q"),
                    new ValueCheckbox(false, "laneclear.w", "Lane Clear W"),
                    new ValueCheckbox(true,  "laneclear.e", "Lane Clear E")
                }
            };

            feature.ToMenu();
            Features.Add(feature);

            feature = new Feature
            {
                NameFeature = "Jungle Clear",
                MenuValueStyleList = new List<ValueAbstract>
                {
                    new ValueCheckbox(true,  "jungleclear.q", "Jungle Clear Q"),
                    new ValueCheckbox(false, "jungleclear.w", "Jungle Clear W"),
                    new ValueCheckbox(true,  "jungleclear.e", "Jungle Clear E")
                }
            };

            feature.ToMenu();
            Features.Add(feature);
        }
    }
}
