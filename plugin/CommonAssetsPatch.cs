using HarmonyLib;
using JapaneseMod.structs;
using System.Linq;
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
            if (Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_EN_GB && Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_JA_JP)
            {
                return;
            }

            var font = Plugin.Assets.FindFontStructs(
                null,
                Game.Locale.CurrentLanguageKey,
                FontType.Default,
                (Game.Locale.CurrentLanguageKey == Plugin.LANGUAGE_JA_JP && Plugin.IsPatchEnabled)
            ).FirstOrDefault()?.Font ?? null;
            if (font == null)
            {
                return;
            }
            __result = font;
        }
    }

    [HarmonyPatch(typeof(CommonAssets), "DefaultOutlineFont", MethodType.Getter)]
    public static class DefaultOutlineFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            if (Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_EN_GB && Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_JA_JP)
            {
                return;
            }

            var font = Plugin.Assets.FindFontStructs(
                null,
                Game.Locale.CurrentLanguageKey,
                FontType.DefaultOutline,
                (Game.Locale.CurrentLanguageKey == Plugin.LANGUAGE_JA_JP && Plugin.IsPatchEnabled)
            ).FirstOrDefault()?.Font ?? null;
            if (font == null)
            {
                return;
            }
            __result = font;
        }
    }

    [HarmonyPatch(typeof(CommonAssets), "AlternateFont", MethodType.Getter)]
    public static class AlternateFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            if (Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_EN_GB && Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_JA_JP)
            {
                return;
            }

            var font = Plugin.Assets.FindFontStructs(
                null,
                Game.Locale.CurrentLanguageKey,
                FontType.Alternate,
                (Game.Locale.CurrentLanguageKey == Plugin.LANGUAGE_JA_JP && Plugin.IsPatchEnabled)
            ).FirstOrDefault()?.Font ?? null;
            if (font == null)
            {
                return;
            }
            __result = font;
        }
    }

    [HarmonyPatch(typeof(CommonAssets), "AlternateOutlineFont", MethodType.Getter)]
    public static class AlternateOutlineFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            if (Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_EN_GB && Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_JA_JP)
            {
                return;
            }

            var font = Plugin.Assets.FindFontStructs(
                null,
                Game.Locale.CurrentLanguageKey,
                FontType.AlternateOutline,
                (Game.Locale.CurrentLanguageKey == Plugin.LANGUAGE_JA_JP && Plugin.IsPatchEnabled)
            ).FirstOrDefault()?.Font ?? null;
            if (font == null)
            {
                return;
            }
            __result = font;
        }
    }

    [HarmonyPatch(typeof(CommonAssets), "ActionOutlineFont", MethodType.Getter)]
    public static class ActionOutlineFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            if (Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_EN_GB && Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_JA_JP)
            {
                return;
            }

            var font = Plugin.Assets.FindFontStructs(
                null,
                Game.Locale.CurrentLanguageKey,
                FontType.ActionOutline,
                (Game.Locale.CurrentLanguageKey == Plugin.LANGUAGE_JA_JP && Plugin.IsPatchEnabled)
            ).FirstOrDefault()?.Font ?? null;
            if (font == null)
            {
                return;
            }
            __result = font;
        }
    }
}
