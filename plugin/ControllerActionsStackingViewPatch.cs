using HarmonyLib;
using InControl;
using System;
using System.Collections.Generic;
using Warborn;

namespace JapaneseMod
{

    [HarmonyPatch(typeof(ControllerActionsStackingView), "UpdateActionPrompts")]
    public class UpdateActionPromptsPatch
    {
        //// 元引数: 		public void UpdateActionPrompts(Dictionary<InputControlType, string> actionPrompts, float width, float height, float spacing, bool vertical, bool forceControllerForPC = false)
        //[HarmonyPrefix]
        //[HarmonyPatch(new Type[] { typeof(Dictionary<InputControlType, string>), typeof(float), typeof(float), typeof(float), typeof(bool), typeof(bool) })]
        //public static void PrefixA(ref ControllerActionsStackingView __instance, ref Dictionary<InputControlType, string> actionPrompts, ref float width, ref float height, ref float spacing, ref bool vertical, ref bool forceControllerForPC)
        //{
        //    Plugin.EventPool.RemoveLanguageChangedHandler(__instance); // 自分を削除
        //}


        //[HarmonyPostfix]
        //[HarmonyPatch(new Type[] { typeof(Dictionary<InputControlType, string>), typeof(float), typeof(float), typeof(float), typeof(bool), typeof(bool) })]
        //public static void PostfixA(ref ControllerActionsStackingView __instance, ref Dictionary<InputControlType, string> actionPrompts, ref float width, ref float height, ref float spacing, ref bool vertical, ref bool forceControllerForPC)
        //{
        //    // 自分をそのまま呼び出すクロージャActionをLanguageChangedに登録
        //    var myself = __instance;
        //    var _actionPrompts = actionPrompts;
        //    var _width = width;
        //    var _height = height;
        //    var _spacing = spacing;
        //    var _vertical = vertical;
        //    var _forceControllerForPC = forceControllerForPC;
        //    Action action = () =>
        //    {
        //        myself.UpdateActionPrompts(_actionPrompts, _width, _height, _spacing, _vertical, _forceControllerForPC);
        //    };
        //    Plugin.EventPool.AddLanguageChangedHandler(__instance, new LocalizationManager.LanguageChangedHandler(action));
        //}

        //// 元引数:		public void UpdateActionPrompts(Dictionary<InputControlType, string> actionPrompts, bool vertical, bool forceControllerForPC = false, float maxWidth = 0f)
        //[HarmonyPrefix]
        //[HarmonyPatch(new Type[] { typeof(Dictionary<InputControlType, string>), typeof(bool), typeof(bool), typeof(float) })]
        //public static void PrefixB(ref ControllerActionsStackingView __instance, ref Dictionary<InputControlType, string> actionPrompts, ref bool vertical, ref bool forceControllerForPC, ref float maxWidth)
        //{
        //    Plugin.EventPool.RemoveLanguageChangedHandler(__instance); // 自分を削除
        //}

        //[HarmonyPostfix]
        //[HarmonyPatch(new Type[] { typeof(Dictionary<InputControlType, string>), typeof(bool), typeof(bool), typeof(float) })]
        //public static void PostfixB(ref ControllerActionsStackingView __instance, ref Dictionary<InputControlType, string> actionPrompts, ref bool vertical, ref bool forceControllerForPC, ref float maxWidth)
        //{
        //    // 自分をそのまま呼び出すクロージャActionをLanguageChangedに登録
        //    var myself = __instance;
        //    var _actionPrompts = actionPrompts;
        //    var _vertical = vertical;
        //    var _forceControllerForPC = forceControllerForPC;
        //    var _maxWidth = maxWidth;
        //    Action action = () =>
        //    {
        //        myself.UpdateActionPrompts(_actionPrompts, _vertical, _forceControllerForPC, _maxWidth);
        //    };
        //    Plugin.EventPool.AddLanguageChangedHandler(__instance, new LocalizationManager.LanguageChangedHandler(action));
        //}
    }
}
