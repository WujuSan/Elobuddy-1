using System;
using System.Collections.Generic;
using Ass_Zed.Controller.Modes;
using Ass_Zed.Model;
using Ass_Zed.Model.Enum;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Utils;

namespace Ass_Zed.Controller
{
    public static class ModeManager
    {
        private static List<ModeBase> Modes { get; set; }

        static ModeManager()
        {
            Modes = new List<ModeBase>();

            Modes.AddRange(new ModeBase[]
            {
                new PermaActive(),
                new Combo(),
                new Harass(),
                new LaneClear(),
                new LastHit(),
                //new Flee()
            });

            Game.OnTick += OnTick;
        }

        public static void Initialize()
        {

        }

        private static void OnTick(EventArgs args)
        {
            Orbwalker.ForcedTarget = null;
            PluginModel.ActiveMode = EnumModeManager.None;
            Modes.ForEach(mode =>
            {
                try
                {
                    if (mode.ShouldBeExecuted())
                    {
                        mode.Execute();
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Error, "Error executing mode '{0}'\n{1}", mode.GetType().Name, e);
                }
            });
        }
    }
}
