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

    /**
         * UpdateDialogueText()
         **/
    [HarmonyPatch(typeof(DialogueView), "UpdateDialogueText")]
    public static class UpdateDialogueTextPatch
    {
        public static void Postfix(ref DialogueView __instance, ref string speakerName, ref bool isLeft, ref bool isAlly, ref string text, ref bool animated)
        {
            Plugin.Logger.LogInfo("UpdateDialogueText is called!");
            Plugin.EventPool.RemoveLanguageChangedHandler(__instance);

            // 自分をコールし直すクロージャ
            var instance = __instance;
            var name = speakerName;
            var left = isLeft;
            var ally = isAlly;
            var dialogueText = text;
            var isAnimated = animated;
            Action callMyself = () =>
            {
                Traverse.Create(instance).Method("UpdateDialogueText", new object[] { name, left, ally, dialogueText, isAnimated }).GetValue();
            };
            Plugin.EventPool.AddLanguageChangedHandler(__instance, new LocalizationManager.LanguageChangedHandler(callMyself));
        }
    }


}
