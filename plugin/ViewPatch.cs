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
        public bool ignoreLanguageChangedEvent = false; // 直接フォントを指定して言語問わず完全固定にしたい場合のみtrue (例: クレジットの歌詞) 英字フォント指定と混ぜるとバグる
        public FontType? englishFontType = null; // パッチモードで英語フォントを使う場合のみtrue
    }

    internal static class TextViewFontManipulator
    {
        // TODO データは外に出すかも
        public static TextViewFontManipulationStruct[] TextViewFontManipulationStructs =
        [
            // クレジットに追加した歌詞パーツ
            new TextViewFontManipulationStruct()
            {
                parentViewName = "CreditsView",
                childViewName = "Lyrics",
                ignoreLanguageChangedEvent = true,
            },

            // タイトル右上のバージョン表示
            new TextViewFontManipulationStruct()
            {
                parentViewName = "BackgroundImageBottom",
                childViewName = "VersionText",
                englishFontType = FontType.Alternate,
            },

            // 設定画面の言語プルダウン
            new TextViewFontManipulationStruct()
            {
                languageSymbolRegex = new Regex(@"^LANG_.+$"),
                ignoreLanguageChangedEvent = true,
            },

            // タイトル画面の「PRESS ANY BUTTON」
            new TextViewFontManipulationStruct()
            {
                childViewName = "PressButtonToStartText",
                englishFontType = FontType.Default,
            },

            // コマンダーパワー タイトル
            new TextViewFontManipulationStruct()
            {
                languageSymbolString = "CP",
                englishFontType = FontType.Default,
            },

            // SP タイトル
            new TextViewFontManipulationStruct()
            {
                childViewName = "SPTitleText",
                englishFontType = FontType.Default,
            },

            // マップ左上のSP数値
            new TextViewFontManipulationStruct()
            {
                parentViewName = "LocalPlayerStatus",
                childViewName = "ValueText",
                englishFontType = FontType.Default,
            },

            // マップ右上の数値
            new TextViewFontManipulationStruct()
            {
                parentViewName = "RemotePlayerStatus",
                childViewName = "ValueText",
                englishFontType = FontType.Default,
            },

            // 左上のPOWER
            new TextViewFontManipulationStruct()
            {
                parentViewName = "LocalPlayerStatus",
                childViewName = "ActivePowerText",
                englishFontType = FontType.DefaultOutline,
            },

            // 右上のPOWER
            new TextViewFontManipulationStruct()
            {
                parentViewName = "RemotePlayerStatus",
                childViewName = "ActivePowerText",
                englishFontType = FontType.DefaultOutline,
            },


            // マップ左上のPLAYER_TURN
            new TextViewFontManipulationStruct()
            {
                childViewName = "PlayerTurnIndicatorText",
                englishFontType = FontType.Default,
            },

            // 移動やターゲット指定で画面中央上に出てくるPLAYER_TURN/ENEMY_TURN
            new TextViewFontManipulationStruct()
            {
                parentViewName = "HelpMessage",
                childViewName = "TitleText",
                englishFontType = FontType.Default,
            },

            // ブリーフィングとターン開始時の全面バナー
            new TextViewFontManipulationStruct()
            {
                childViewName = "BannerMainText",
                englishFontType = FontType.Default,
            },

            // ターン開始時のバナーの横の小さい字
            new TextViewFontManipulationStruct()
            {
                childViewName = "BannerSubText",
                englishFontType = FontType.Default,
            },

            // 配備画面のSP
            new TextViewFontManipulationStruct()
            {
                childViewName = "ResourceBalanceView",
                englishFontType = FontType.Default,
            },

            // 配備画面の移動力
            new TextViewFontManipulationStruct()
            {
                childViewName = "MovementValueText",
                englishFontType = FontType.Default,
            },

            // ターン毎のSP供給 +\dSP
            new TextViewFontManipulationStruct()
            {
                childViewName = "IncomeText",
                englishFontType = FontType.Default,
            },

            // 持っている精製所の数
            new TextViewFontManipulationStruct()
            {
                parentViewName = "OwnedRefineries",
                childViewName = "MainText",
                englishFontType = FontType.Default,
            },

            // 持っている生産施設の数
            new TextViewFontManipulationStruct()
            {
                parentViewName = "OwnedOutposts",
                childViewName = "MainText",
                englishFontType = FontType.Default,
            },

            // ミッション選択画面クリアランク
            new TextViewFontManipulationStruct()
            {
                parentViewName = "OverallRanking",
                childViewName = "TitleText",
                englishFontType = FontType.Alternate,
            },
            new TextViewFontManipulationStruct()
            {
                parentViewName = "OverallRanking",
                childViewName = "RankingText",
                englishFontType = FontType.Default,
            },

            // リザルト画面クリアランク
            new TextViewFontManipulationStruct()
            {
                parentViewName = "RankingView",
                childViewName = "TitleText",
                englishFontType = FontType.Alternate,
            },
            new TextViewFontManipulationStruct()
            {
                parentViewName = "RankingView",
                childViewName = "RankingText",
                englishFontType = FontType.Default,
            },

            // ミッション選択画面クリアランク(ミッション個別)
            new TextViewFontManipulationStruct()
            {
                parentViewName = "RankBackgroundImage",
                childViewName = "RankText",
                englishFontType = FontType.Default,
            },

            // HP
            new TextViewFontManipulationStruct()
            {
                parentViewName = "HPBarView",
                childViewName = "HitPointsValueText",
                englishFontType = FontType.Default,
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
            string languageSymbol = localizeSymbolJsonStruct?.text ?? localizeSymbolJsonStruct?.TEXT ?? "";

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
        public static void Prefix(ref View __instance, ref string name, ref string text, ref TMP_FontAsset font, out object[] __state)
        {
            // オリジナルのフォント
            __state = new object[] { font };

            // パッチ非有効時に英語フォントに固定すると文字化けするので早めに抜ける
            if (!Plugin.IsPatchEnabled || Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_JA_JP)
            {
                return;
            }

            // 英語に固定すべきフォントか、条件を検索する
            var fixEnglishFontCondition = TextViewFontManipulator.FindFontManipulationConditions(
                __instance.name,
                name,
                Plugin.DesilializeSingleLocalizedSymbolJson(text),
                null,
                true
            ).FirstOrDefault();

            if (fixEnglishFontCondition == null)
            {
                return;
            }
            else
            {
                Plugin.Logger.LogInfo($"FixedFont Detected: {__instance.name}->{name}, {text}, {fixEnglishFontCondition.childViewName ?? ""}/{fixEnglishFontCondition.languageSymbolString ?? ""}/{fixEnglishFontCondition.languageSymbolRegex?.ToString() ?? ""}");
            }

            // (StoredFontには日本語2種と英語しかいないので、ほかの言語では最後までいかないはず)
            var fixedFont = Plugin.Assets.FindFontStructs(
                null,
                Plugin.LANGUAGE_EN_GB,
                fixEnglishFontCondition.englishFontType,
                null
            ).FirstOrDefault()?.Font ?? null;
            if (fixedFont == null)
            {
                return;
            }

            font = fixedFont;
        }

        public static void Postfix(ref View __instance, ref TextView __result, string name, string text, ref TMP_FontAsset font, int fontSize, Color textColour, TextAlignmentOptions textAlignment, object[] __state)
        {
            var originalFont = __state[0] as TMP_FontAsset;

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
                null,
                true
            ).FirstOrDefault()?.englishFontType ?? null;

            // 本来その場所に貼られてるフォントを見る(未使用のsourceFontFileNameカラムの悪用)
            if (Enum.TryParse<FontType>(originalFont.creationSettings.sourceFontFileName, out FontType parsedFontType))
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
                if (Plugin.IsPatchEnabled && Game.Locale.CurrentLanguageKey == Plugin.LANGUAGE_JA_JP)
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
