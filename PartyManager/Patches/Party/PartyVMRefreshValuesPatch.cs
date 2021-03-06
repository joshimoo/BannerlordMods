﻿using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace PartyManager.Patches
{
    [HarmonyPatch(typeof(PartyVM), "RefreshValues")]
    public class PartyVMRefreshValuesPatch
    {
        static bool Prefix(PartyVM __instance)
        {
            if (PartyManagerSettings.Settings.EnableAutoSort)
            {
                GenericHelpers.LogDebug("PartyVM RefreshValues Patch", "Pre Update called");
                PartyController.CurrentInstance.PartyVM = __instance;
                PartyController.CurrentInstance.SortPartyScreen(SortType.Default, true);
            }

            return true;
        }
    }
}
