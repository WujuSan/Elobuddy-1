using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using OneForWeek.Util.Misc;
using SharpDX;
using System.Collections.Generic;
using Color = System.Drawing.Color;

namespace OneForWeek.Plugin.Hero
{
    class Vayne : PluginModel, IChampion
    {
        public static Spell.Active Q;
        public static Spell.Targeted E;
        public static Spell.Active R;
        private static List<Spell.SpellBase> spells;

        public Vayne()
        {
            Init();
        }

        public void Init()
        {
            spells = new List<Spell.SpellBase>();
            InitVariables();
        }

        public void InitVariables()
        {
            Q = new Spell.Active(SpellSlot.Q, 325);
            spells.Add(Q);
            E = new Spell.Targeted(SpellSlot.E, 550);
            spells.Add(E);
            R = new Spell.Active(SpellSlot.R);
            spells.Add(R);

            InitMenu();

            Orbwalker.OnPostAttack += OnAfterAttack;
            Gapcloser.OnGapcloser += OnGapCloser;
            Interrupter.OnInterruptableSpell += OnPossibleToInterrupt;

            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
        }

        public void InitMenu()
        {
            Menu = MainMenu.AddMenu(GCharname, GCharname);

            Menu.AddLabel("Version: " + GVersion);
            Menu.AddSeparator();
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Menu.AddLabel("By MrArticuno");

            DrawMenu = Menu.AddSubMenu("Draw - " + GCharname, GCharname + "Draw");
            DrawMenu.AddGroupLabel("Draw");
            DrawMenu.Add("drawDisable", new CheckBox("Turn off all drawings", false));
            DrawMenu.Add("drawAARange", new CheckBox("Draw Auto Attack Range", true));
            DrawMenu.Add("drawQ", new CheckBox("Draw Q Range", true));
            DrawMenu.Add("drawE", new CheckBox("Draw E Range", true));
            DrawMenu.Add("drawTumblePos", new CheckBox("Draw Tumble Pos", true));
            DrawMenu.Add("wallTumble", new KeyBind("Wall Tumble", false, KeyBind.BindTypes.HoldActive, 'W'));
            DrawMenu.Add("drawCondemnPos", new CheckBox("Draw Condemn Position", true));

            ComboMenu = Menu.AddSubMenu("Combo - " + GCharname, GCharname + "Combo");
            ComboMenu.AddGroupLabel("Combo");
            ComboMenu.Add("comboQ", new CheckBox("Use Q", true));
            ComboMenu.Add("comboE", new CheckBox("Use E", true));
            ComboMenu.Add("comboFE", new CheckBox("Finisher E", false));
            ComboMenu.Add("comboR", new CheckBox("Use R", true));
            ComboMenu.Add("minEnemiesInRange", new Slider("Min enemies in range for R: ", 2, 1, 5));

            HarassMenu = Menu.AddSubMenu("Harass - " + GCharname, GCharname + "Harass");
            HarassMenu.AddGroupLabel("Harass");
            HarassMenu.Add("hsQ", new CheckBox("Use Q", true));
            HarassMenu.Add("hsE", new CheckBox("Use E", true));
            HarassMenu.Add("minManaPercent", new Slider("Min Mana Percent to use Skills: ", 50, 0, 100));

            LaneClearMenu = Menu.AddSubMenu("Lane Clear - " + GCharname, GCharname + "LaneClear");
            LaneClearMenu.AddGroupLabel("Lane Clear");
            LaneClearMenu.Add("lcQ", new CheckBox("Use Q", true));
            LaneClearMenu.Add("minManaPercent", new Slider("Min Mana Percent to use Skills: ", 50, 0, 100));

            LastHitMenu = Menu.AddSubMenu("Last Hit - " + GCharname, GCharname + "LastHit");
            LastHitMenu.AddGroupLabel("Last Hit");
            LastHitMenu.Add("lhQ", new CheckBox("Use Q", true));
            LastHitMenu.Add("minManaPercent", new Slider("Min Mana Percent to use Skills: ", 50, 0, 100));

            MiscMenu = Menu.AddSubMenu("Misc - " + GCharname, GCharname + "Misc");
            MiscMenu.Add("miscAntiGapQ", new CheckBox("Anti Gap Closer Q", true));
            MiscMenu.Add("miscAntiGapE", new CheckBox("Anti Gap Closer E", true));
            MiscMenu.Add("miscInterruptDangerous", new CheckBox("Try Interrupt Dangerous Spells", true));

            MiscMenu.AddGroupLabel("Condemn Options");
            MiscMenu.Add("fastCondemn",
                new KeyBind("Fast Condemn HotKey", false, KeyBind.BindTypes.PressToggle, 'T'));
            MiscMenu.AddGroupLabel("Auto Condemn");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(a => a.IsEnemy))
            {
                MiscMenu.Add("dnCondemn" + enemy.ChampionName.ToLower(), new CheckBox("Don't Condemn " + enemy.ChampionName, false));
            }
            MiscMenu.AddGroupLabel("Priority Condemn");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(a => a.IsEnemy))
            {
                MiscMenu.Add("priorityCondemn" + enemy.ChampionName.ToLower(), new Slider(enemy.ChampionName + " Priority", 1, 1, 5));
            }
            MiscMenu.Add("condenmErrorMargin", new Slider("Subtract Condemn Push by: ", 20, 0, 100));
            MiscMenu.Add("QMinDist", new Slider("Min Distance for Q: ", 375, 325, 525));

        }

        public void OnCombo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);

            if (target == null || !target.IsValidTarget(E.Range)) return;
            
            if (Misc.IsChecked(ComboMenu, "comboR") && R.IsReady())
            {
                if (_Player.CountEnemiesInRange(_Player.GetAutoAttackRange() + Q.Range) >= Misc.GetSliderValue(ComboMenu, "minEnemiesInRange"))
                {
                    R.Cast();
                }
            }

            if (Misc.IsChecked(ComboMenu, "comboE") && E.IsReady())
            {
                AIHeroClient priorityTarget = null;

                if (MiscMenu["fastCondemn"].Cast<KeyBind>().CurrentValue)
                {
                    E.Cast(target);
                    return;
                }

                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(a => a.IsEnemy).Where(a => !a.IsDead).Where(a => E.IsInRange(a)))
                {
                    if (priorityTarget == null)
                    {
                        priorityTarget = enemy;
                    }
                    else
                    {
                        if (Misc.GetSliderValue(MiscMenu, "priorityCondemn" + enemy.ChampionName.ToLower()) > Misc.GetSliderValue(MiscMenu, "priorityCondemn" + priorityTarget.ChampionName.ToLower()))
                        {
                            priorityTarget = enemy;
                        }
                    }

                    if (IsCondemnable(priorityTarget))
                    {
                        E.Cast(priorityTarget);
                    }

                    if (priorityTarget.VayneWStacks() == 2 && PossibleDamage(priorityTarget) > priorityTarget.Health && Misc.IsChecked(ComboMenu, "comboFE"))
                    {
                        E.Cast(priorityTarget);
                    }
                }
            }

            MiscMenu["fastCondemn"].Cast<KeyBind>().CurrentValue = false;
        }

        public void OnLastHit(){}

        public void OnHarass()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);

            if (target == null || !target.IsValidTarget(E.Range) || Player.Instance.ManaPercent < Misc.GetSliderValue(HarassMenu, "minManaPercent")) return;

            if (Misc.IsChecked(HarassMenu, "hsE") && E.IsReady())
            {
                AIHeroClient priorityTarget = null;

                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(a => a.IsEnemy).Where(a => !a.IsDead).Where(a => E.IsInRange(a)))
                {
                    if (priorityTarget == null)
                    {
                        priorityTarget = enemy;
                    }
                    else
                    {
                        if (Misc.GetSliderValue(MiscMenu, "priorityCondemn" + enemy.ChampionName.ToLower()) > Misc.GetSliderValue(MiscMenu, "priorityCondemn" + priorityTarget.ChampionName.ToLower()))
                        {
                            priorityTarget = enemy;
                        }
                    }

                    if (IsCondemnable(priorityTarget))
                    {
                        E.Cast(priorityTarget);
                    }

                    if (MiscMenu["fastCondemn"].Cast<KeyBind>().CurrentValue)
                    {
                        E.Cast(priorityTarget);
                        MiscMenu["fastCondemn"].Cast<KeyBind>().CurrentValue = false;
                    }
                }
            }

        }

        public void OnLaneClear()
        {
            if(Player.Instance.ManaPercent < Misc.GetSliderValue(LaneClearMenu, "minManaPercent")) return;

            if (Misc.IsChecked(LaneClearMenu, "lcQ") && Q.IsReady())
            {
                
            }
        }

        public void OnFlee()
        {

        }

        public void OnGameUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                OnCombo();
                return;
            }
                
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnHarass();
                return;
            }
                
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                OnFlee();
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                OnLastHit();
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                OnLaneClear();
            }

            if (Misc.IsKeyBindOn(DrawMenu, "wallTumble"))
            {
                WallTumble();
            }
            else
            {
                Orbwalker.DisableMovement = false;
            }

        }

        public void OnDraw(EventArgs args)
        {
            if (Misc.IsChecked(DrawMenu, "drawDisable"))
                return;

            if (Misc.IsChecked(DrawMenu, "drawQ") && Q.IsReady())
            {
                new Circle() { Color = Color.White, Radius = Q.Range, BorderWidth = 2f }.Draw(_Player.Position);
            }

            if (Misc.IsChecked(DrawMenu, "drawE") && E.IsReady())
            {

                new Circle() { Color = Color.White, Radius = E.Range, BorderWidth = 2f }.Draw(_Player.Position);
            }

            if (Misc.IsChecked(DrawMenu, "drawCondemnPos") && E.IsReady())
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(a => a.IsEnemy).Where(a => !a.IsDead).Where(a => _Player.Distance(a) <= E.Range))
                {
                    if (Misc.IsChecked(MiscMenu, "dnCondemn" + enemy.ChampionName.ToLower()))
                        return;

                    var condemnPos = _Player.Position.Extend(enemy.Position, _Player.Distance(enemy) + 470 - Misc.GetSliderValue(MiscMenu, "condenmErrorMargin"));

                    var realStart = Drawing.WorldToScreen(enemy.Position);
                    var realEnd = Drawing.WorldToScreen(condemnPos.To3D());

                    Drawing.DrawLine(realStart, realEnd, 2f, Color.Red);
                    new Circle() { Color = Color.Red, Radius = 60, BorderWidth = 2f }.Draw(condemnPos.To3D());
                }
            }

            if (Misc.IsChecked(DrawMenu, "drawTumblePos"))
            {
                if (Game.MapId != GameMapId.SummonersRift) return;
                var drakeWallQPos = new Vector2(12050, 4827);
                var midWallQPos = new Vector2(6962, 8952);
                if (drakeWallQPos.Distance(_Player) < 3000)
                    new Circle() { Color = _Player.Distance(drakeWallQPos) <= 100 ? Color.DodgerBlue : Color.White, Radius = 100 }.Draw(drakeWallQPos.To3D());
                if (midWallQPos.Distance(_Player) < 3000)
                    new Circle() { Color = _Player.Distance(midWallQPos) <= 100 ? Color.DodgerBlue : Color.White, Radius = 100 }.Draw(midWallQPos.To3D());
            }

        }

        public void OnAfterAttack(AttackableUnit tg, EventArgs args)
        {
            var target = TargetSelector.GetTarget(Player.Instance.GetAutoAttackRange() + Q.Range, DamageType.Physical);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && Misc.IsChecked(ComboMenu, "comboQ") && Q.IsReady())
            {
                if (target == null || !target.IsValidTarget() || target.IsDead) return;
                Player.CastSpell(SpellSlot.Q, target.GetTumblePos());
                Core.DelayAction(Orbwalker.ResetAutoAttack, 250);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && Misc.IsChecked(HarassMenu, "hsQ") && Q.IsReady() && Player.Instance.ManaPercent > Misc.GetSliderValue(HarassMenu, "minManaPercent"))
            {
                if (target == null || !target.IsValidTarget() || target.IsDead) return;
                Player.CastSpell(SpellSlot.Q, target.GetTumblePos());
                Core.DelayAction(Orbwalker.ResetAutoAttack, 250);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) && Misc.IsChecked(LastHitMenu, "lhQ") && Q.IsReady() && Player.Instance.ManaPercent > Misc.GetSliderValue(LastHitMenu, "minManaPercent"))
            {
                foreach (var minion in EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.Distance(Player.Instance) < Player.Instance.GetAutoAttackRange() + Q.Range && Player.Instance.GetAutoAttackDamage(m) > m.Health && !target.IsDead))
                {
                    Player.CastSpell(SpellSlot.Q, minion.Position);
                    Core.DelayAction(Orbwalker.ResetAutoAttack, 250);
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && Misc.IsChecked(LaneClearMenu, "lcQ") && Q.IsReady() && Player.Instance.ManaPercent > Misc.GetSliderValue(LaneClearMenu, "minManaPercent") && !target.IsDead)
            {
                Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                Core.DelayAction(Orbwalker.ResetAutoAttack, 250);
            }
        }

        public void OnPossibleToInterrupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs interruptableSpellEventArgs)
        {
            if (!sender.IsEnemy || !Misc.IsChecked(MiscMenu, "miscInterruptDangerous") || !E.IsReady()) return;

            if (E.IsInRange(sender))
            {
                E.Cast(sender);
            }
        }

        public void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsEnemy || e.End.Distance(Player.Instance) > 50) return;

            if (Q.IsReady() && Misc.IsChecked(MiscMenu, "miscAntiGapQ"))
            {
                Player.CastSpell(SpellSlot.Q, e.End.Extend(Player.Instance.Position, e.End.Distance(Player.Instance) + 325).To3D());
            }

            if (E.IsReady() && Misc.IsChecked(MiscMenu, "miscAntiGapE"))
            {
                E.Cast(sender);
            }

        }

        public void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args){}

        public void GameObjectOnCreate(GameObject sender, EventArgs args){}

        public void GameObjectOnDelete(GameObject sender, EventArgs args){}

        public static double PossibleDamage(AIHeroClient target)
        {
            var damage = 0d;
            var targetMaxHealth = target.MaxHealth;

            var silverBoltDmg = (new[] { 0, targetMaxHealth / 4, targetMaxHealth / 5, targetMaxHealth / 6, targetMaxHealth / 7, targetMaxHealth / 8 }[Player.Instance.Spellbook.GetSpell(SpellSlot.W).Level]);

            if (target.GetBuffCount("vaynesilvereddebuff") == 2) damage += silverBoltDmg;

            return damage + 150;
        }

        public static bool IsCondemnable(AIHeroClient target)
        {
            if (Misc.IsChecked(MiscMenu, "dnCondemn" + target.ChampionName.ToLower()))
            {
                return false;
            }

            if (target.HasBuffOfType(BuffType.SpellImmunity) || target.HasBuffOfType(BuffType.SpellShield) || _Player.IsDashing()) return false;

            var predPos = Prediction.Position.PredictUnitPosition(target, 500);

            var position = _Player.Position.Extend(target.Position, _Player.Distance(target) - Misc.GetSliderValue(MiscMenu, "condenmErrorMargin")).To3D();
            var predictPos = _Player.Position.Extend(predPos, _Player.Distance(predPos) - Misc.GetSliderValue(MiscMenu, "condenmErrorMargin")).To3D();
            for (var i = 0; i < 470 - Misc.GetSliderValue(MiscMenu, "condenmErrorMargin"); i += 10)
            {
                var cPos = _Player.Position.Extend(position, _Player.Distance(position) + i).To3D();
                var cPredPos = _Player.Position.Extend(predictPos, _Player.Distance(predictPos) + i).To3D();

                if ((cPredPos.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Wall) || cPredPos.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Building)) && (cPos.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Wall) || cPos.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Building)))
                {
                    return true;
                }
            }
            return false;
        }

        public static void WallTumble()
        {
            if (Game.MapId != GameMapId.SummonersRift) return;
            if (!Q.IsReady())
            {
                Orbwalker.DisableMovement = false;
                return;
            }
            Orbwalker.DisableMovement = true;

            var drakeWallQPos = new Vector2(11514, 4462);
            var midWallQPos = new Vector2(6667, 8794);

            var selectedPos = drakeWallQPos.Distance(_Player) < midWallQPos.Distance(_Player) ? drakeWallQPos : midWallQPos;
            var walkPos = drakeWallQPos.Distance(_Player) < midWallQPos.Distance(_Player)
                ? new Vector2(12050, 4827)
                : new Vector2(6962, 8952);
            if (_Player.Distance(walkPos) < 200 && _Player.Distance(walkPos) > 60)
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, walkPos.To3D());
            }
            else if (_Player.Distance(walkPos) <= 50)
            {
                Player.CastSpell(SpellSlot.Q, selectedPos.To3D());
            }
        }
    }
}
