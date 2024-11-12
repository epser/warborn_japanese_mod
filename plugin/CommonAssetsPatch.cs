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
            if (Game.Locale == null)
            {
                return;
            }

            if (Game.Locale.CurrentLanguageKey == "ja-JP" && Plugin.IsPatchEnabled && Plugin.Assets.StoredLanguageFonts.Count() > 0)
            {
                // Collection<FontStruct>なので、そのように絞る
                var font = Plugin.Assets.StoredLanguageFonts.Where(font => font.LanguageCode == Plugin.LANGUAGE_JA_JP && font.Type == FontType.Default && font.PatchMode).FirstOrDefault().Font;
                if (font == null)
                {
                    return;
                }
                __result = font;
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

            if (Game.Locale.CurrentLanguageKey == "ja-JP" && Plugin.IsPatchEnabled && Plugin.Assets.StoredLanguageFonts.Count() > 0)
            {
                var font = Plugin.Assets.StoredLanguageFonts.Where(font => font.LanguageCode == Plugin.LANGUAGE_JA_JP && font.Type == FontType.DefaultOutline && font.PatchMode).FirstOrDefault().Font;
                if (font == null)
                {
                    return;
                }
                __result = font;
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

            if (Game.Locale.CurrentLanguageKey == "ja-JP" && Plugin.IsPatchEnabled && Plugin.Assets.StoredLanguageFonts.Count() > 0)
            {
                var font = Plugin.Assets.StoredLanguageFonts.Where(font => font.LanguageCode == Plugin.LANGUAGE_JA_JP && font.Type == FontType.Alternate && font.PatchMode).FirstOrDefault().Font;
                if (font == null)
                {
                    return;
                }
                __result = font;
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

            if (Game.Locale.CurrentLanguageKey == "ja-JP" && Plugin.IsPatchEnabled && Plugin.Assets.StoredLanguageFonts.Count() > 0)
            {
                var font = Plugin.Assets.StoredLanguageFonts.Where(font => font.LanguageCode == Plugin.LANGUAGE_JA_JP && font.Type == FontType.AlternateOutline && font.PatchMode).FirstOrDefault().Font;
                if (font == null)
                {
                    return;
                }
                __result = font;
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

            if (Game.Locale.CurrentLanguageKey == "ja-JP" && Plugin.IsPatchEnabled && Plugin.Assets.StoredLanguageFonts.Count() > 0)
            {
                var font = Plugin.Assets.StoredLanguageFonts.Where(font => font.LanguageCode == Plugin.LANGUAGE_JA_JP && font.Type == FontType.ActionOutline && font.PatchMode).FirstOrDefault().Font;
                if (font == null)
                {
                    return;
                }
                __result = font;
            }
        }
    }
}
