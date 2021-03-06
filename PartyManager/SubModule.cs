﻿using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library.EventSystem;
using HarmonyLib;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Library;
using System.Linq;
using PartyManager.ViewModel;
using PartyManager.ViewModels;

namespace PartyManager
{
    public class SubModule : MBSubModuleBase
    {
        bool enableHotkey = false;
        bool enableRecruitUpgradeSort = false;
        bool enableAutoSort = false;
        bool enableSortTypeCycleHotkey = false;
        
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            try
            {
                GenericHelpers.LogMessage("Loaded PartyManager. Press CTRL+SHIFT+S in Party Screen to sort, CTRL+SHIFT+R to upgrade/recruit sort, CTRL+SHIFT+(Minus) to cycle sort types", "#ffa136FF");
                if (!string.IsNullOrEmpty(PartyManagerSettings.Settings.NewFileMessage))
                {
                    GenericHelpers.LogMessage(PartyManagerSettings.Settings.NewFileMessage);
                }

            }
            catch (Exception ex)
            {
                GenericHelpers.LogException("OnBeforeInitialModuleScreenSetAsRoot", ex);
            }
        }

        bool showFileCreateMessage = false;
        protected override void OnSubModuleLoad()
        {

            base.OnSubModuleLoad();
            try
            {
                enableHotkey = PartyManagerSettings.Settings.EnableHotkey;
                enableAutoSort = PartyManagerSettings.Settings.EnableAutoSort;
                enableRecruitUpgradeSort = PartyManagerSettings.Settings.EnableRecruitUpgradeSortHotkey;
                enableSortTypeCycleHotkey = PartyManagerSettings.Settings.EnableSortTypeCycleHotkey;
                new Harmony("mod.partymanager").PatchAll();
            }
            catch (Exception ex)
            {
                GenericHelpers.LogException("Patch Failed", ex);
            }
        }

        private GauntletLayer settingsLayer;
        long lastHotkeyExecute = 0;
        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);
            if (enableHotkey || enableRecruitUpgradeSort || enableSortTypeCycleHotkey)
            {
                string key = "";
                try
                {
                    if (Campaign.Current == null || !Campaign.Current.GameStarted || (!(ScreenManager.TopScreen is GauntletPartyScreen) || (!InputKey.LeftShift.IsDown()) && !InputKey.LeftControl.IsDown() && !InputKey.Minus.IsDown()))
                    {
                        return;
                    }


                    if ((enableHotkey && InputKey.S.IsDown()) || (enableRecruitUpgradeSort && InputKey.R.IsDown()) || (enableSortTypeCycleHotkey && InputKey.Minus.IsDown()))
                    {
                        var diff = (DateTime.Now.Ticks - lastHotkeyExecute) / TimeSpan.TicksPerMillisecond;

                        //Prevent the key from triggering more than once per tenth of a second
                        if (diff < 100)
                        {
                            return;
                        }

                        if (InputKey.Minus.IsDown() && enableSortTypeCycleHotkey)
                        {
                            key = "-";
                            PartyManagerSettings.Settings.CycleSortType();
                        }

                        //SortHotkey
                        if (InputKey.S.IsDown() && enableHotkey)
                        {
                            key = "S";
                            PartyController.CurrentInstance.SortPartyScreen();
                        }//RecruitSort
                        else if (InputKey.R.IsDown() && enableRecruitUpgradeSort)
                        {
                            key = "R";
                            PartyController.CurrentInstance.SortPartyScreen(SortType.RecruitUpgrade);
                        }
                        lastHotkeyExecute = DateTime.Now.Ticks;
                    }
                }
                catch (Exception ex)
                {
                    lastHotkeyExecute = DateTime.Now.Ticks;
                    GenericHelpers.LogException($"Tick('{key}')", ex);
                }
            }
        }
    }
}