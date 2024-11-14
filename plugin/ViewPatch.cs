using HarmonyLib;
using System;
using System.Linq;
using Warborn;
using TMPro;
using UnityEngine;

namespace JapaneseMod
{
    public static class ViewPatchVariables
    {
        // ビュー名をみて固定フォントを決める
        public static string[] FixedFontViewNames = new string[]
        {
            "Lyrics"
        };
    }

    // TextView.Awakeのパッチ
    [HarmonyPatch(typeof(View), "AddNewChildTextView")]
    // 元実装: public TextView AddNewChildTextView(string name, string text, TMP_FontAsset font, int fontSize, Color textColour, TextAlignmentOptions textAlignment)
    public static class AddNewChildTextViewPatch
    {
        public static void Postfix(ref View __instance, ref TextView __result, string name, string text, TMP_FontAsset font, int fontSize, Color textColour, TextAlignmentOptions textAlignment)
        {
            if (font == null || Plugin.Assets.StoredLanguageFonts == null)
            {
                return;
            }

            // Linqで今保持しているフォントと等しいものを探し、Typeを確定させる
            var currentLanguageFontList = Plugin.Assets.StoredLanguageFonts
                .Where(fontStruct => fontStruct.LanguageCode == Game.Locale.CurrentLanguageKey);
            if(Game.Locale.CurrentLanguageKey == Plugin.LANGUAGE_JA_JP)
            {
                currentLanguageFontList = currentLanguageFontList?.Where(fontStruct => fontStruct.PatchMode == Plugin.IsPatchEnabled);
            }
            if (currentLanguageFontList == null)
            {
                return;
            }
            var type = currentLanguageFontList
                .Where(fontStruct => fontStruct.Font == font)
                .FirstOrDefault()?.Type ?? null;
            if (type == null) return;

            // actionクロージャを変数に入れる
            var textView = __result;
            Action action = () =>
            {
                var originalText = "";
                if (TMPTextOriginalStrings.OriginalStrings.TryGetValue(textView.Text, out originalText))
                {
                    textView.Text.text = originalText;
                }

                if (ViewPatchVariables.FixedFontViewNames.Contains(textView.name))
                {
                    return;
                }
                var fonts = Plugin.Assets.StoredLanguageFonts
                    .Where(fontStruct => fontStruct.LanguageCode == Game.Locale.CurrentLanguageKey && fontStruct.Type == type);
                if (Game.Locale.CurrentLanguageKey == Plugin.LANGUAGE_JA_JP)
                {
                    fonts = fonts.Where(fontStruct => fontStruct.PatchMode == Plugin.IsPatchEnabled);
                }
                var font = fonts?.FirstOrDefault()?.Font ?? null;
                if (font != null)
                {
                    textView.Text.font = font;
                }
            };
            Plugin.EventPool.AddLanguageChangedHandler(textView, new LocalizationManager.LanguageChangedHandler(action));
        }
    }

}
