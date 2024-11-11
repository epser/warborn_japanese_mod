using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TMPro;

namespace JapaneseMod
{
    [HarmonyPatch(typeof(TMP_Text), "text", MethodType.Setter)]
    public static class TextMeshProTextPropertyPatch
    {
        // Prefixで引数textを変更する
        public static void Prefix(ref TMP_Text __instance, ref string value)
        {
            value = Plugin.DesilializeLocalizedSymbolJson(value);
        }
    }

    [HarmonyPatch(typeof(TMP_Text), "GetPreferredValues")]
    public static class TextMeshProGetPreferredValuesPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(new Type[] { typeof(string) })]
        public static void PrefixText(ref string text)
        {
            text = Plugin.DesilializeLocalizedSymbolJson(text);
        }

        [HarmonyPrefix]
        [HarmonyPatch(new Type[] { typeof(string), typeof(float), typeof(float) })]
        public static void PrefixTextAndFloat(ref string text)
        {
            text = Plugin.DesilializeLocalizedSymbolJson(text);
        }
    }
}
