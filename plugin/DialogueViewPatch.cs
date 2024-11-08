using HarmonyLib;
using JapaneseMod.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Warborn;

namespace JapaneseMod
{
    [HarmonyPatch(typeof(DialogueView), "Awake")]
    public static class DialogViewAwakePatch
    {

        public static void Postfix(ref DialogueView __instance)
        {
            Plugin.Logger.LogInfo("Awake is called in dialogue!");
            var handleLanguageChanged = new LocalizationManager.LanguageChangedHandler(DialogViewAwakePatch.CreateHandleLanguageChanged(__instance));
            Plugin.EventPool.AddLanguageChangedHandler(__instance, handleLanguageChanged);
        }

        public static Action CreateHandleLanguageChanged(DialogueView instance)
        {
            return () =>
            {
                Plugin.Logger.LogInfo("Language changed in dialogue:" + instance.name);
                // インスタンスごとの言語変更処理
                instance.HandleLanguageChanged();
            };
        }
    }
}
