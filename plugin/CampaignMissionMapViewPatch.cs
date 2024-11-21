using HarmonyLib;
using InControl;
using System.Collections.Generic;
using System;
using UnityEngine;
using Warborn;

namespace JapaneseMod
{
    [HarmonyPatch(typeof(CampaignMissionMapView), "Awake")]
    public class CampaignMissionMapViewPatch
    {
        public static ControllerActionsStackingView ControllerActionsStackingView;

        public static void Postfix(ref CampaignMissionMapView __instance)
        {
            Plugin.Logger.LogInfo("CampaignMissionMapViewPatch Postfix");

            ControllerActionsStackingView = __instance.AddNewChildOfType<ControllerActionsStackingView>("ControllerActionsStackingView");
            ControllerActionsStackingView.SetEdgeAlignedFrame(new Frame(RectTransform.Edge.Right, RectTransform.Edge.Top, 32f, 32f, 200f, 0f));
            ControllerActionsStackingView.transform.localScale = new Vector3(1f, 1f, 1f);
            var actionPrompts = new Dictionary<InputControlType, string>
            {
                {
                    InputControlType.Action3,
                    "Epilogue"
                },
            };
            ControllerActionsStackingView.UpdateActionPrompts(actionPrompts, true, true, 0f);
            ControllerActionsStackingView.ShowOrHideViewOffscreenInDirection(RectTransform.Edge.Right, true, 0f, 0f, false, null, false);
        }

    }
}
