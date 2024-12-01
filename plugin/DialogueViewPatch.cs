using HarmonyLib;
using InControl;
using JapaneseMod.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Warborn;

namespace JapaneseMod
{

    public class DialogueText(string symbol, string speaker, string text)
    {
        public readonly string symbol = symbol;
        public readonly string speaker = speaker;
        public readonly string text = text;
    }



    public class DialogueBackLogViewComponent
    {
        public ImageView BackLogView; // DialogueViewの子
        public View BackLogScrollView; // ScrollRectをアタッチする枠
        public ImageView BackLogForegroundView; // ホイールイベントを捕まえる枠
        public TextView BackLogTextView; // テキスト

        public ScrollRect BackLogScrollRect;
        public RectMask2D BackLogRectMask;

        public bool IsBackLogActive;
        public List<DialogueText> DialogueList;

        public DialogueBackLogViewComponent(View parentView) {
            BackLogView = parentView.AddNewChildImageView("BackLogView", Plugin.CreateSolidColorSprite(new Color(0, 0, 0, 0.98f)));
            BackLogView.StretchAndFitToParent();
            BackLogView.SetFrame(0, 0, 1920, 1080);

            BackLogScrollView = BackLogView.AddNewChildOfType<View>("BackLogScrollView");
            BackLogScrollView.SetFrame(0, 0, 1920, 1080);

            BackLogScrollRect = BackLogScrollView.gameObject.AddComponent<ScrollRect>();
            BackLogScrollRect.horizontal = false;
            BackLogScrollRect.vertical = true;
            BackLogScrollRect.movementType = ScrollRect.MovementType.Clamped;
            BackLogScrollRect.scrollSensitivity = 60f;

            BackLogRectMask = BackLogScrollView.gameObject.AddComponent<RectMask2D>();

            BackLogForegroundView = BackLogScrollView.AddNewChildImageView("BackLogForeground", "");
            BackLogForegroundView.Image.color = new Color(1f, 1f, 1f, 0.001f);
            BackLogForegroundView.Image.raycastTarget = true;
            BackLogForegroundView.SetFrame((1920 - DialogueView.DIALOGUE_WINDOW_FRAME.Width) / 2, 0, DialogueView.DIALOGUE_WINDOW_FRAME.Width, 2160);

            BackLogTextView = BackLogForegroundView.AddNewChildTextView("BackLogText", "", Game.Common.AlternateFont, 32, new Color(1f, 1f, 0.9f, 1f), TextAlignmentOptions.TopLeft);
            BackLogTextView.SetFrame(0, 0, DialogueView.DIALOGUE_TEXT_FRAME.Width, 2160);

            BackLogScrollRect.content = BackLogForegroundView.RectTransform;
            BackLogView.Hide();

            IsBackLogActive = false;
            DialogueList = [];
        }
    }

    [HarmonyPatch(typeof(DialogueView))]
    public static class DialogueViewPatch
    {
        public static ConditionalWeakTable<DialogueView, DialogueBackLogViewComponent> BackLogViewTable = new();

        [HarmonyPatch("Awake")] // privateなのでnameof()に出ない
        [HarmonyPostfix]
        public static void AwakePostfix(ref DialogueView __instance)
        {
            Plugin.Logger.LogInfo("Awake is called in dialogue!");

            // 言語変更イベント
            var handleLanguageChanged = new LocalizationManager.LanguageChangedHandler(DialogueViewPatch.CreateHandleLanguageChanged(__instance));
            Plugin.EventPool.AddLanguageChangedHandler(__instance, handleLanguageChanged);

            // バックログスクリーン
            var backLogView = new DialogueBackLogViewComponent(__instance);
            BackLogViewTable.Remove(__instance);
            BackLogViewTable.Add(__instance, backLogView);
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

        public static void RenderBackLogBuffer(DialogueBackLogViewComponent backLogView)
        {
            // バックログ用のテキストを整形
            string backLogText = "";
            foreach (var dialogue in backLogView.DialogueList)
            {
                backLogText += $"\n[{dialogue.speaker}]\n{dialogue.text}\n";
            }

            backLogView.BackLogTextView.Text.text = backLogText + "\n";

            // コンテンツのサイズを確定
            var vector = backLogView.BackLogTextView.Text.GetPreferredValues(backLogView.BackLogTextView.Text.text, 1920, 0);

            backLogView.BackLogForegroundView.SetFrame((1920 - DialogueView.DIALOGUE_WINDOW_FRAME.Width) / 2, 0, DialogueView.DIALOGUE_WINDOW_FRAME.Width, Math.Max(vector.y, 1080));
            backLogView.BackLogTextView.SetFrame(0, 0, DialogueView.DIALOGUE_TEXT_FRAME.Width, Math.Max(vector.y, 1080));
        }

        public static void AddBackLogBufferIfNewer(DialogueView dialogueView, string speakerNameJson, string textJson)
        {
            BackLogViewTable.TryGetValue(dialogueView, out var backLogView);

            LocalizeSymbolJsonStruct addDialogueTextJsonStrct = Plugin.DesilializeSingleLocalizedSymbolJson(textJson);
            if (addDialogueTextJsonStrct == null || backLogView == null)
            {
                return;
            }
            DialogueText lastText = backLogView.DialogueList.LastOrDefault();

            if (lastText?.symbol != addDialogueTextJsonStrct.text) {
                backLogView.DialogueList.Add(new DialogueText(addDialogueTextJsonStrct.text, speakerNameJson, textJson));
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix(ref DialogueView __instance)
        {
            BackLogViewTable.TryGetValue(__instance, out var backLogView);

            if (__instance.AllowInput && !backLogView.IsBackLogActive)
            {
                // スクロール上下が右スティックで、ズームインアウトがホイールに相当。あまりInControlのバインディングに手を加えたくない
                if (
                    (
                        Game.Input.ControllerWasLastInput &&
                        (Game.Input.Controls.ScrollTextUp.IsPressed || Game.Input.Controls.ScrollTextDown.IsPressed)
                    )
                    ||
                    (
                        !Game.Input.ControllerWasLastInput &&
                        (Game.Input.Controls.ZoomIn.IsPressed || Game.Input.Controls.ZoomOut.IsPressed)
                    )
                )
                {
                    backLogView?.BackLogView?.Show();
                    backLogView.IsBackLogActive = true;
                    __instance.AllowInput = false;
                }
                return;
            }
            if (!__instance.AllowInput && backLogView.IsBackLogActive)
            {
                if (Game.Input.Controls.ScrollTextUp.IsPressed)
                {
                    float num = Mathf.Clamp(Mathf.Abs(Game.Input.ActiveDevice.RightStick.Value.magnitude), 0f, 1f);
                    backLogView.BackLogScrollRect.verticalNormalizedPosition = Mathf.Clamp(backLogView.BackLogScrollRect.verticalNormalizedPosition + num * 2f * Time.smoothDeltaTime, 0f, 1f);
                }
                else if (Game.Input.Controls.ScrollTextDown.IsPressed)
                {
                    float num = Mathf.Clamp(Mathf.Abs(Game.Input.ActiveDevice.RightStick.Value.magnitude), 0f, 1f);
                    backLogView.BackLogScrollRect.verticalNormalizedPosition = Mathf.Clamp(backLogView.BackLogScrollRect.verticalNormalizedPosition - num * 2f * Time.smoothDeltaTime, 0f, 1f);
                }
                else if (Game.Input.Controls.Confirm.IsPressed || Game.Input.Controls.Cancel.IsPressed)
                {
                    backLogView.BackLogView.Hide();
                    backLogView.IsBackLogActive = false;
                    __instance.AllowInput = true;
                    Game.Input.Controls.Confirm.ClearInputState();
                    Game.Input.Controls.Cancel.ClearInputState();
                }
                return;
            }
        }

        [HarmonyPatch("UpdateDialogueText")]
        [HarmonyPrefix]
        public static void UpdateDialogueTextPrefix(ref DialogueView __instance, ref string speakerName, ref bool isLeft, ref bool isAlly, ref string text, ref bool animated, out string __state)
        {
            // コルーチンで本文を1文字ずつ出してるので、本文のローカライズ変換はここでやる必要がある
            Plugin.Logger.LogInfo("UpdateDialogueText is called!");
            Plugin.EventPool.RemoveLanguageChangedHandler(__instance);

            // バックログ追記
            AddBackLogBufferIfNewer(__instance, speakerName, text);
            if (BackLogViewTable.TryGetValue(__instance, out var backLogView))
            {
                RenderBackLogBuffer(backLogView);
            }

            __state = text; // 本文を保存
            text = Plugin.DesilializeLocalizedSymbolJson(text);
        }

        [HarmonyPatch("UpdateDialogueText")]
        [HarmonyPostfix]
        public static void UpdateDialogueTextPostfix(ref DialogueView __instance, ref string speakerName, ref bool isLeft, ref bool isAlly, ref string text, ref bool animated, string __state)
        {
            // current language
            var currentLanguage = Plugin.LocalizationManagerReference.CurrentLanguageKey;
            // Mod mode
            var isModEnabled = Plugin.IsPatchEnabled;

            var instance = __instance;
            var name = speakerName;
            var left = isLeft;
            var ally = isAlly;
            var dialogueText = __state; // 保存しといた本文
            var isAnimated = animated;

            // 自分をコールし直すクロージャ
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
