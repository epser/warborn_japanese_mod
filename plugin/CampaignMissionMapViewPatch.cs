﻿using HarmonyLib;
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
        public static bool displayingControllerActions;
        public static Dictionary<InputControlType, string> actionPromptsController = new Dictionary<InputControlType, string>
        {
            {
                InputControlType.Action3,
                "Prologue"
            },
            {
                InputControlType.LeftTrigger,
                "Epilogue"
            }
        };
        public static Dictionary<InputControlType, string> actionPromptsPC = new Dictionary<InputControlType, string>
        {
            {
                InputControlType.Action3, // Tab
                "Prologue"
            },
            {
                InputControlType.Action4, // Space
                "Epilogue"
            }
        };

        public static void Postfix(ref CampaignMissionMapView __instance)
        {
            ControllerActionsStackingView = __instance.AddNewChildOfType<ControllerActionsStackingView>("ControllerActionsStackingView");
            ControllerActionsStackingView.RectTransform.anchorMin = new Vector2(0, 0);
            ControllerActionsStackingView.RectTransform.anchorMax = new Vector2(0, 1);

            ControllerActionsStackingView.SetEdgeAlignedFrame(new Frame(RectTransform.Edge.Right, RectTransform.Edge.Top, 32f, 32f, 200f, 0f));
            ControllerActionsStackingView.transform.localScale = new Vector3(1f, 1f, 1f);

            ControllerActionsStackingView.UpdateActionPrompts(actionPromptsController, true, true, 0f);
            ControllerActionsStackingView.ShowOrHideViewOffscreenInDirection(RectTransform.Edge.Right, false, 0f, 0f, false, null, false);
            displayingControllerActions = true;
        }
    }
}
