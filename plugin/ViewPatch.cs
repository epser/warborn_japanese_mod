using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Warborn;
using TMPro;
using JapaneseMod.structs;
using JapaneseMod.services;
using UnityEngine;

namespace JapaneseMod
{
    // TextView.Awakeのパッチ
    [HarmonyPatch(typeof(View), "AddNewChildTextView")]
    // 元実装: public TextView AddNewChildTextView(string name, string text, TMP_FontAsset font, int fontSize, Color textColour, TextAlignmentOptions textAlignment)
    public static class AddNewChildTextViewPatch
    {
        public static void Postfix(ref View __instance, ref TextView __result, string name, string text, TMP_FontAsset font, int fontSize, Color textColour, TextAlignmentOptions textAlignment)
        {
            Plugin.Logger.LogInfo("AddNewChildTextView is called!(post) " + text);
            if (font == null || Plugin.Assets.StoredLanguageFonts == null)
            {
                return;
            }
            Plugin.Logger.LogInfo("continue 1");
            Plugin.Logger.LogInfo(Game.Locale.CurrentLanguageKey);
            Plugin.Logger.LogInfo(Plugin.Assets.StoredLanguageFonts.Count);

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

            Plugin.Logger.LogInfo("continue 2 " + type);

            // actionクロージャを変数に入れる
            var textView = __result;
            Action action = () =>
            {
                var fonts = Plugin.Assets.StoredLanguageFonts
                    .Where(fontStruct => fontStruct.LanguageCode == Game.Locale.CurrentLanguageKey && fontStruct.Type == type);
                if (Game.Locale.CurrentLanguageKey == Plugin.LANGUAGE_JA_JP)
                {
                    fonts = fonts.Where(fontStruct => fontStruct.PatchMode == Plugin.IsPatchEnabled);
                }
                if (fonts == null)
                {
                    return;
                }
                var font = fonts.FirstOrDefault()?.Font ?? null;
                if (font == null) return;
                textView.Text.font = font;
            };
            Plugin.EventPool.AddLanguageChangedHandler(textView, new LocalizationManager.LanguageChangedHandler(action));
        }
    }
}
