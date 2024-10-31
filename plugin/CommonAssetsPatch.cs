using HarmonyLib;
using JapaneseMod.@struct;
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
            if (Game.Locale != null && Game.Locale.CurrentLanguageKey == "ja-JP" && Plugin.Assets.jpDefaultFont != null)
            {
                __result = Plugin.Assets.jpDefaultFont;
            }
        }
    }

    [HarmonyPatch(typeof(CommonAssets), "DefaultOutlineFont", MethodType.Getter)]
    public static class DefaultOutlineFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            if (Game.Locale != null && Game.Locale.CurrentLanguageKey == "ja-JP" && Plugin.Assets.jpDefaultFont != null)
            {
                __result = Plugin.Assets.jpDefaultOutlineFont;
            }
        }
    }

    [HarmonyPatch(typeof(CommonAssets), "AlternateFont", MethodType.Getter)]
    public static class AlternateFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            if (Game.Locale != null && Game.Locale.CurrentLanguageKey == "ja-JP" && Plugin.Assets.jpAlternateFont != null)
            {
                __result = Plugin.Assets.jpAlternateFont;
            }
        }
    }

    [HarmonyPatch(typeof(CommonAssets), "AlternateOutlineFont", MethodType.Getter)]
    public static class AlternateOutlineFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            if (Game.Locale != null && Game.Locale.CurrentLanguageKey == "ja-JP" && Plugin.Assets.jpAlternateOutlineFont != null)
            {
                __result = Plugin.Assets.jpAlternateOutlineFont;
            }
        }
    }

    [HarmonyPatch(typeof(CommonAssets), "ActionOutlineFont", MethodType.Getter)]
    public static class ActionOutlineFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            if (Game.Locale != null && Game.Locale.CurrentLanguageKey == "ja-JP" && Plugin.Assets.jpActionOutlineFont != null)
            {
                __result = Plugin.Assets.jpActionOutlineFont;
            }
        }
    }
}
