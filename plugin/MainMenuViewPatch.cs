using HarmonyLib;
using JapaneseMod.structs;
using System.Linq;
using TMPro;
using Warborn;

namespace JapaneseMod
{
    [HarmonyPatch(typeof(MainMenuView))]
    [HarmonyPatch("HandleLanguageChanged")]
    public static class MainMenuViewHandleLanguageChangedPatch
    {
        public static bool Prefix()
        {
            return false;
        }
    }
}
