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

namespace JapaneseMod
{

    // TextView.Awakeのパッチ
    [HarmonyPatch(typeof(TextView), "Awake")]
    public static class TextViewAwakePatch
    {
        //public static void Postfix(ref TextView __instance)
        //{
        //    TMP_FontAsset currentFont = __instance.Text.font;

        //    // 現在のフォント種別を比較で確定させる
        //    FontType type = Plugin.Assets.StoredLanguageFonts.Where(font => font.LanguageCode == Game.Locale.CurrentLanguageKey && font.Font == currentFont).FirstOrDefault().Type;

        //    // actionクロージャを変数に入れる
        //    var instance = __instance;
        //    Action action = () =>
        //    {
        //        // フォントを取得設定
        //        TMP_FontAsset font = Plugin.Assets.StoredLanguageFonts.Where(font => font.LanguageCode == Game.Locale.CurrentLanguageKey && font.Type == type).FirstOrDefault().Font;
        //        if (font == null)
        //        {
        //            return;
        //        }
        //        instance.Text.font = font;
        //    };
        //    Plugin.EventPool.AddLanguageChangedHandler(__instance, new LocalizationManager.LanguageChangedHandler(action));
        //}
    }

    // TextView.ConfigureTextのパッチ
    [HarmonyPatch(typeof(TextView), "ConfigureText")]
    // Prefixで引数textを変更する
    public static class TextViewPatch
    {
        //public static void Prefix(ref TextView __instance, ref string txt)
        //{
        //    Plugin.Logger.LogInfo("TextView.ConfigureText is called!(pre) " + txt);
        //    txt = Plugin.DesilializeLocalizedSymbolJson(txt);
        //    Plugin.Logger.LogInfo(txt);
        //}
    }
}
