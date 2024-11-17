using HarmonyLib;
using System;
using System.Linq;
using Warborn;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using JapaneseMod.structs;
using System.Collections.Generic;

namespace JapaneseMod
{
    internal class TextViewFontManipulationStruct
    {
        public string parentViewName = null;
        public string childViewName = null;
        public string languageSymbolString = null;
        public Regex languageSymbolRegex = null;
        public bool ignoreLanguageChangedEvent = false;
        public FontType? englishFontType = null;
    }

    internal static class TextViewFontManipulator
    {
        // TODO データは外に出す
        public static TextViewFontManipulationStruct[] TextViewFontManipulationStructs =
        [
            new TextViewFontManipulationStruct()
            {
                parentViewName = "CreditsView",
                childViewName = "Lyrics",
                ignoreLanguageChangedEvent = true, // 直接フォントを指定して完全固定にしたい場合のみtrue
            },

            new TextViewFontManipulationStruct()
            {
                parentViewName = "BackgroundImageBottom",
                childViewName = "VersionText",
                englishFontType = FontType.Alternate,
            },
            new TextViewFontManipulationStruct()
            {
                languageSymbolRegex = new Regex(@"^LANG_.+$"),
                ignoreLanguageChangedEvent = true,
            },
        ];

        public static IEnumerable<TextViewFontManipulationStruct> FindFontManipulationConditions(
            string parentViewName = null,
            string childViewName = null,
            LocalizeSymbolJsonStruct localizeSymbolJsonStruct = null,
            bool? ignoreLanguageChangedEvent = null,
            bool? fixEnglishFont = null
        )
        {
            string languageSymbol = localizeSymbolJsonStruct?.text ?? localizeSymbolJsonStruct?.TEXT ?? null;

            return TextViewFontManipulationStructs
                .Where(v => v.parentViewName == parentViewName || v.parentViewName == null || parentViewName == null)
                .Where(v => v.childViewName == childViewName || v.childViewName == null || childViewName == null)
                .Where(v => (v.languageSymbolRegex == null && v.languageSymbolString == null) || languageSymbol == null || v.languageSymbolRegex?.IsMatch(languageSymbol) == true || v.languageSymbolString == languageSymbol)
                .Where(v => v.ignoreLanguageChangedEvent == ignoreLanguageChangedEvent || ignoreLanguageChangedEvent == null)
                .Where(v => (v.englishFontType != null && fixEnglishFont == true) || fixEnglishFont == null);
        }
    }

    // TextView.Awakeのパッチ
    [HarmonyPatch(typeof(View), "AddNewChildTextView")]
    // 元実装: public TextView AddNewChildTextView(string name, string text, TMP_FontAsset font, int fontSize, Color textColour, TextAlignmentOptions textAlignment)
    public static class AddNewChildTextViewPatch
    {
        public static void Prefix(ref View __instance, ref string name, ref string text, ref TMP_FontAsset font, out TMP_FontAsset __state)
        {
            // オリジナルのフォント
            __state = font;

            // パッチ非有効時に英語フォントに固定すると文字化けするので早めに抜ける
            if (!Plugin.IsPatchEnabled || Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_JA_JP)
            {
                return;
            }

            // 英語に固定すべきフォントかを判定
            var fixEnglishFont = TextViewFontManipulator.FindFontManipulationConditions(__instance.name, name, Plugin.DesilializeSingleLocalizedSymbolJson(text), true, true).FirstOrDefault();
            if (fixEnglishFont == null)
            {
                return;
            }

            // (StoredFontには日本語2種と英語しかいないので、ほかの言語では最後までいかないはず)
            var fixedFont = Plugin.Assets.FindFontStructs(
                null,
                Plugin.LANGUAGE_EN_GB,
                fixEnglishFont.englishFontType,
                null
            ).FirstOrDefault()?.Font ?? null;
            if (fixedFont == null)
            {
                return;
            }

            font = fixedFont;
        }

        public static void Postfix(ref View __instance, ref TextView __result, string name, string text, ref TMP_FontAsset font, int fontSize, Color textColour, TextAlignmentOptions textAlignment, TMP_FontAsset __state)
        {
            if (font == null || Plugin.Assets.StoredLanguageFonts == null || (Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_JA_JP && Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_EN_GB))
            {
                return;
            }

            // 今保持しているフォントと等しいものを探し、Typeを確定させる
            // 英語固定モードの場合は先に英語フォントのTypeを取得
            var fontType = TextViewFontManipulator.FindFontManipulationConditions(
                __instance.name,
                name,
                Plugin.DesilializeSingleLocalizedSymbolJson(text),
                true,
                true
            ).FirstOrDefault()?.englishFontType ?? null;

            // 本来その場所に貼られてるフォントを見る
            if (Enum.TryParse<FontType>(__state.creationSettings.sourceFontFileName, out FontType parsedFontType))
            {
                fontType = parsedFontType;
            }
            else
            {
                // パースに失敗した場合はnullのままにする
                fontType = null;
            }

            // actionクロージャを変数に入れる
            var parent = __instance;
            var textView = __result;
            var textViewName = name;
            Action action = () =>
            {
                if(Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_JA_JP && Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_EN_GB)
                {
                    return;
                }

                string originalText = "";
                if (TMPTextOriginalStrings.OriginalStrings.TryGetValue(textView.Text, out originalText))
                {
                    textView.Text.text = originalText;
                }

                // フォントの変更
                // をしない条件を検索
                if (fontType == null)
                {
                    return;
                }
                if (TextViewFontManipulator.FindFontManipulationConditions(parent.name, textView.name, Plugin.DesilializeSingleLocalizedSymbolJson(originalText), true, null).Count() > 0)
                {
                    return;
                }

                // 書き換え先フォントの確定
                TMP_FontAsset rewriteFont = null;
                FontType? searchType = null;
                if (Plugin.IsPatchEnabled)
                {
                    searchType = TextViewFontManipulator.FindFontManipulationConditions(parent.name, textView.name, Plugin.DesilializeSingleLocalizedSymbolJson(originalText), null, true).FirstOrDefault()?.englishFontType;
                    if(searchType != null)
                    {
                        rewriteFont = Plugin.Assets.FindFontStructs(
                            null,
                            Plugin.LANGUAGE_EN_GB,
                            searchType,
                            null
                        ).FirstOrDefault()?.Font ?? null;
                    }
                }
                if(rewriteFont == null)
                {
                    rewriteFont = Plugin.Assets.FindFontStructs(
                        null,
                        Game.Locale.CurrentLanguageKey,
                        fontType,
                        Plugin.IsPatchEnabled && Game.Locale.CurrentLanguageKey == Plugin.LANGUAGE_JA_JP ? true : null
                    ).FirstOrDefault()?.Font ?? null;
                }

                if (rewriteFont != null)
                {
                    textView.Text.font = rewriteFont;
                }
            };
            Plugin.EventPool.AddLanguageChangedHandler(textView, new LocalizationManager.LanguageChangedHandler(action));
        }
    }

}
