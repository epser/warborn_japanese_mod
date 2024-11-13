using HarmonyLib;
using InControl;
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
        public static void Prefix(ref DialogueView __instance, ref string speakerName, ref bool isLeft, ref bool isAlly, ref string text, ref bool animated, out string __state)
        {
            // コルーチンで本文を1文字ずつ出してるので、本文のローカライズ変換はここでやる必要がある
            Plugin.Logger.LogInfo("UpdateDialogueText is called!(pre)");
            Plugin.EventPool.RemoveLanguageChangedHandler(__instance);

            __state = text; // 本文を保存
            text = Plugin.DesilializeLocalizedSymbolJson(text);
        }

        public static void Postfix(ref DialogueView __instance, ref string speakerName, ref bool isLeft, ref bool isAlly, ref string text, ref bool animated, string __state)
        {
            Plugin.Logger.LogInfo("UpdateDialogueText is called!(post)");

            // current language
            var currentLanguage = Plugin.LocalizationManagerReference.CurrentLanguageKey;
            // Mod mode
            var isModEnabled = Plugin.IsPatchEnabled;

            // 自分をコールし直すクロージャ
            var instance = __instance;
            var name = speakerName;
            var left = isLeft;
            var ally = isAlly;
            var dialogueText = __state; // 保存しといた本文
            var isAnimated = animated;
            Action callMyself = () =>
            {
                Traverse.Create(instance).Method("UpdateDialogueText", new object[] { name, left, ally, dialogueText, isAnimated }).GetValue();
                var displayingControllerActions = Traverse.Create(instance).Field("displayingControllerActions").GetValue<bool>();
                var actionPrompts = Traverse.Create(instance).Field("actionPrompts").GetValue<Dictionary<InputControlType, string>>();

                // TODO: UpdateActionPrompts自身が自分を呼ぶようになったので、ここが必要かどうかは確認する
                if (displayingControllerActions)
                {
                    instance.ControllerActionsStackingView.UpdateActionPrompts(actionPrompts, false, true, 0f);
                }
                else
                {
                    instance.ControllerActionsStackingView.UpdateActionPrompts(actionPrompts, false, false, 0f);
                }
            };
            Plugin.EventPool.AddLanguageChangedHandler(__instance, new LocalizationManager.LanguageChangedHandler(callMyself));
        }
    }


}
