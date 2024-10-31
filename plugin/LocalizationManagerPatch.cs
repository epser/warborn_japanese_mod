using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using Warborn;
using static System.Net.Mime.MediaTypeNames;
using BepInEx;
using Newtonsoft.Json;
using System.Collections;
using TMPro;


namespace JapaneseMod
{
    [HarmonyPatch(typeof(LocalizationManager), "LoadStringsForLocale")]
    public static class LoadStringsForLocalePatch
    {
        // public void LoadStringsForLocale(string locale, bool silent = false)
        public static bool Prefix(LocalizationManager __instance, ref Dictionary<string, string> ___LocalizedStrings, ref LocalizationManager.LanguageChangedHandler ___LanguageChanged, object[] __args)
        {
            string text = (string)__args[0];
            bool silent = (bool)__args[1];
            if (!LocalizationManager.AvailableLanguageKeys.Contains(text))
            {
                text = "en-GB";
            }
            if (text != __instance.CurrentLanguageKey)
            {
                Plugin.Logger.LogInfo($"Loading strings for locale {text}");
                // プラグインPATH/{MyPluginInfo.PLUGIN_GUID}/{text}.json が存在するか確認。存在するならTextAssetとしてロード
                string path = $"{BepInEx.Paths.ConfigPath}/{MyPluginInfo.PLUGIN_GUID}/{text}.json";
                if (System.IO.File.Exists(path))
                {
                    Plugin.Logger.LogInfo($"Found {path}");
                    TextAsset loadedAsset = new TextAsset(System.IO.File.ReadAllText(path));
                    if (loadedAsset != null)
                    {
                        ___LocalizedStrings = JsonConvert.DeserializeObject<Dictionary<string, string>>(loadedAsset.text);
                        Traverse.Create(__instance).Property("CurrentLanguageKey").SetValue(text);
                        if (!silent && ___LanguageChanged != null)
                        {
                            ___LanguageChanged();
                        }
                    }
                    return false; // 元の関数を実行しない
                }
            }
            return true; // 元の関数を実行
        }
    }
}
