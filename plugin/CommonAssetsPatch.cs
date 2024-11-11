using HarmonyLib;
using JapaneseMod.services;
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
            if (Game.Locale == null)
            {
                return;
            }

            if (Game.Locale.CurrentLanguageKey == "ja-JP" && Plugin.IsPatchEnabled && Plugin.Assets.StoredLanguageFonts.ContainsKey(Plugin.LANGUAGE_JA_JP_MOD))
            {
                __result = Plugin.Assets.StoredLanguageFonts[Plugin.LANGUAGE_JA_JP_MOD].DefaultFont;
            }
        }
    }

    [HarmonyPatch(typeof(CommonAssets), "DefaultOutlineFont", MethodType.Getter)]
    public static class DefaultOutlineFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            if (Game.Locale == null)
            {
                return;
            }

            if (Game.Locale.CurrentLanguageKey == "ja-JP" && Plugin.IsPatchEnabled && Plugin.Assets.StoredLanguageFonts.ContainsKey(Plugin.LANGUAGE_JA_JP_MOD))
            {
                __result = Plugin.Assets.StoredLanguageFonts[Plugin.LANGUAGE_JA_JP_MOD].DefaultOutlineFont;
            }
        }
    }

    [HarmonyPatch(typeof(CommonAssets), "AlternateFont", MethodType.Getter)]
    public static class AlternateFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            if (Game.Locale == null)
            {
                return;
            }

            if (Game.Locale.CurrentLanguageKey == "ja-JP" && Plugin.IsPatchEnabled && Plugin.Assets.StoredLanguageFonts.ContainsKey(Plugin.LANGUAGE_JA_JP_MOD))
            {
                __result = Plugin.Assets.StoredLanguageFonts[Plugin.LANGUAGE_JA_JP_MOD].AlternateFont;
            }
        }
    }

    [HarmonyPatch(typeof(CommonAssets), "AlternateOutlineFont", MethodType.Getter)]
    public static class AlternateOutlineFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            if (Game.Locale == null)
            {
                return;
            }

            if (Game.Locale.CurrentLanguageKey == "ja-JP" && Plugin.IsPatchEnabled && Plugin.Assets.StoredLanguageFonts.ContainsKey(Plugin.LANGUAGE_JA_JP_MOD))
            {
                __result = Plugin.Assets.StoredLanguageFonts[Plugin.LANGUAGE_JA_JP_MOD].AlternateOutlineFont;
            }
        }
    }

    [HarmonyPatch(typeof(CommonAssets), "ActionOutlineFont", MethodType.Getter)]
    public static class ActionOutlineFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            if (Game.Locale == null)
            {
                return;
            }

            if (Game.Locale.CurrentLanguageKey == "ja-JP" && Plugin.IsPatchEnabled && Plugin.Assets.StoredLanguageFonts.ContainsKey(Plugin.LANGUAGE_JA_JP_MOD))
            {
                __result = Plugin.Assets.StoredLanguageFonts[Plugin.LANGUAGE_JA_JP_MOD].ActionOutlineFont;
            }
        }
    }
}
