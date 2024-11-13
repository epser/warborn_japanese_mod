using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;

namespace JapaneseMod
{
    public static class TMPTextOriginalStrings
    {
        public static ConditionalWeakTable<TMP_Text, string> OriginalStrings = new ConditionalWeakTable<TMP_Text, string>();
    }

    [HarmonyPatch(typeof(TMP_Text), "text", MethodType.Setter)]
    public static class TextMeshProTextPropertyPatch
    {
        public static void Prefix(ref TMP_Text __instance, ref string value)
        {
            TMPTextOriginalStrings.OriginalStrings.Remove(__instance);
            TMPTextOriginalStrings.OriginalStrings.Add(__instance, value);
            value = Plugin.DesilializeLocalizedSymbolJson(value);
        }
    }

    [HarmonyPatch(typeof(TMP_Text), "GetPreferredValues")]
    public static class TextMeshProGetPreferredValuesPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(new Type[] { typeof(string) })]
        public static void PrefixA(ref string text)
        {
            text = Plugin.DesilializeLocalizedSymbolJson(text);
        }

        [HarmonyPrefix]
        [HarmonyPatch(new Type[] { typeof(string), typeof(float), typeof(float) })]
        public static void PrefixB(ref string text)
        {
            text = Plugin.DesilializeLocalizedSymbolJson(text);
        }
    }
}
