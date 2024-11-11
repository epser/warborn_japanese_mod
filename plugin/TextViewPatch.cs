using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Warborn;
using TMPro;

namespace JapaneseMod
{

    // TextView.Awakeのパッチ
    [HarmonyPatch(typeof(TextView), "Awake")]
    public static class TextViewAwakePatch
    {
        public static void Postfix(ref TextView __instance)
        {
            TMP_FontAsset font = __instance.Text.font;
            

        }
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
