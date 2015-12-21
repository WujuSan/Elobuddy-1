using System;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK.Events;
using OneForWeek.Draw.Notifications;
using OneForWeek.Model.Notification;
using OneForWeek.Plugin.Hero;
using OneForWeek.Util.Misc;
using OneForWeek.Plugin;

namespace OneForWeek
{
    class Program
    {
        public static PluginModel Champion;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadCompleted;
        }

        private static void OnLoadCompleted(EventArgs args)
        {
            try
            {
                var handle = Activator.CreateInstance(null, "OneForWeek.Plugin.Hero." + ObjectManager.Player.ChampionName);
                Champion = (PluginModel)handle.Unwrap();
                Notification.DrawNotification(new NotificationModel(Game.Time, 20f, 1f, ObjectManager.Player.ChampionName +  " injected !", Color.DeepSkyBlue));
                Notification.DrawNotification(new NotificationModel(Game.Time, 20f, 1f, "Addon by: MrArticuno", Color.Purple));
                Notification.DrawNotification(new NotificationModel(Game.Time, 20f, 1f, "Jinx is in BETA !", Color.Purple));

                Igniter.Init();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Notification.DrawNotification(new NotificationModel(Game.Time, 20f, 1f, ObjectManager.Player.ChampionName + " is Not Supported", Color.Red));
            }
        }
    }
}
