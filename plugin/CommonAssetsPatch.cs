using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Warborn;

namespace JapaneseMod
{
    [HarmonyPatch(typeof(CommonAssets))]
    [HarmonyPatch("DefaultFont", MethodType.Getter)]
    public static class DefaultFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            Plugin.Logger.LogInfo("DefaultFont:");
            if (Game.Locale != null && Game.Locale.CurrentLanguageKey == "ja-JP" && AssetOverwrite.DefaultFont != null)
            {
                Plugin.Logger.LogInfo("Overwrite DefaultFont");
                __result = AssetOverwrite.DefaultFont;
            }
        }
    }

    [HarmonyPatch(typeof(CommonAssets), "DefaultOutlineFont", MethodType.Getter)]
    public static class DefaultOutlineFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            Plugin.Logger.LogInfo("OutlineFont:");
            if (Game.Locale != null && Game.Locale.CurrentLanguageKey == "ja-JP" && AssetOverwrite.OutlineFont != null)
            {
                Plugin.Logger.LogInfo("Overwrite OutlineFont");
                __result = AssetOverwrite.OutlineFont;
            }
        }
    }

    [HarmonyPatch(typeof(CommonAssets), "AlternateFont", MethodType.Getter)]
    public static class AlternateFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            Plugin.Logger.LogInfo("AlternateFont:");
            if (Game.Locale != null && Game.Locale.CurrentLanguageKey == "ja-JP" && AssetOverwrite.AlternateFont != null)
            {
                Plugin.Logger.LogInfo("Overwrite AlternateFont");
                __result = AssetOverwrite.AlternateFont;
            }
        }
    }
}
