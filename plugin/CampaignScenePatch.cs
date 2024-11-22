using HarmonyLib;
using InControl;
using System.Collections.Generic;
using System;
using UnityEngine;
using Warborn;
using System.Numerics;

namespace JapaneseMod
{
    [HarmonyPatch(typeof(CampaignScene), "Update")]
    public class CampaignScenePatch
    {
        public static bool Prefix(ref CampaignScene __instance)
        {
            // キー押下チェック
            if (__instance.CampaignView.CampaignMissionMap.MissionListPanel.MissionCollectionView.AllowInput && Game.Input.Controls.Intel)
            {
                return HandleEpilogueButtonPressed(ref __instance);
            }
            else if (__instance.CampaignView.CampaignMissionMap.MissionListPanel.MissionCollectionView.AllowInput && Game.Input.Controls.ZoomOut)
            {
                return HandlePrologueButtonPressed(ref __instance);
            }

            // ボタンラベル変更
            if (CampaignMissionMapViewPatch.displayingControllerActions && !Game.Input.IsOnlyUsingController() && !Game.Input.ControllerWasLastInput)
            {
                CampaignMissionMapViewPatch.ControllerActionsStackingView.UpdateActionPrompts(CampaignMissionMapViewPatch.actionPrompts, true, false, 0f);
                CampaignMissionMapViewPatch.displayingControllerActions = false;
            }
            else if (!CampaignMissionMapViewPatch.displayingControllerActions && (Game.Input.IsOnlyUsingController() || Game.Input.ControllerWasLastInput))
            {
                CampaignMissionMapViewPatch.ControllerActionsStackingView.UpdateActionPrompts(CampaignMissionMapViewPatch.actionPrompts, true, true, 0f);
                CampaignMissionMapViewPatch.displayingControllerActions = true;
            }

            return true;
        }

        public static bool HandlePrologueButtonPressed(ref CampaignScene __instance)
        {
            CampaignSaveData activeSaveSlot = Game.Data.GetActiveSaveSlot();
            // 選択中ミッションの情報を取得
            var missionInfo = __instance.CampaignView.CampaignMissionMap.MissionListPanel.SelectedMissionListCell.LinkedMissionInfo;
            if (missionInfo.MissionScript != null
                && missionInfo.MissionScript.HasCampaignMapBriefingDialogue()
                )
            {
                var instance = __instance;
                instance.CampaignView.CampaignMissionMap.MissionListPanel.SetButtonsEnabled(false);
                instance.CampaignView.CampaignMissionMap.MapMissionInfo.StartButton.ChildButton.SetButtonEnabled(false, true);
                __instance.CampaignView.CampaignMissionMap.MissionListPanel.MissionCollectionView.AllowInput = false;
                instance.CampaignView.IsTransitioning = true;
                Game.Network.CurrentMatch = new MatchInfo(missionInfo.MissionScript);
                BaseGame.Audio.PlaySFX(AudioConfig.SFXKeys.UILobbyPlayerJoins, 0.5f);
                instance.CampaignView.CampaignMissionMap.TransitionOut(true, delegate
                {
                    instance.RunMissionBriefing(missionInfo, delegate
                    {
                        BaseGame.Scene.ChangeScene("CampaignScene", typeof(CampaignScene), true, delegate (Action complete)
                        {
                            Game.Transition.FadeInBlack(0.6f, complete);
                        }, delegate (Action complete)
                        {
                            Game.Transition.FadeOutBlack(0.6f, complete);
                        }, null);
                    });
                });
            }
            return false;
        }


        public static bool HandleEpilogueButtonPressed(ref CampaignScene __instance)
        {
            CampaignSaveData activeSaveSlot = Game.Data.GetActiveSaveSlot();
            // 選択中ミッションの情報を取得
            var missionInfo = __instance.CampaignView.CampaignMissionMap.MissionListPanel.SelectedMissionListCell.LinkedMissionInfo;

            if (missionInfo.MissionScript != null
                && missionInfo.MissionScript.HasCampaignMapCompletionDialogue()
                && activeSaveSlot.CheckIfAlreadyCompletedMission(missionInfo.MissionID)
                )
            {
                BaseGame.Audio.PlayConfirmSFX(-1f);
                __instance.CampaignView.CampaignMissionMap.MissionListPanel.MissionCollectionView.AllowInput = false;
                missionInfo.MissionScript.LoadDialogueCharacters();
                Traverse.Create(__instance).Field("isRunningCompletionDialogue").SetValue(true);
                var instance = __instance;
                __instance.CampaignView.CampaignMissionMap.TransitionOut(false, null);
                Game.Transition.FadeOutBlack(0.6f, delegate
                {
                    Game.Transition.FadeInBlack(0.6f, null);
                    missionInfo.MissionScript.RunCampaignMapCompletionDialogue(instance.CampaignView, delegate
                    {
                        Traverse.Create(instance).Field("isRunningCompletionDialogue").SetValue(false);
                        missionInfo.MissionScript.CleanUp();
                        AnimatedBannerView missionSelectBanner = instance.CampaignView.MissionSelectBanner;
                        Action complete2;
                        complete2 = delegate ()
                        {
                            instance.CampaignView.CampaignMissionMap.TransitionIn(false, delegate
                            {
                                instance.LoadMissionsToMapFromCurrentSaveData();
                            });
                        };
                        missionSelectBanner.RunBannerAnimation(complete2);
                    });
                });

                return false;
            }
            BaseGame.Audio.PlayCancelSFX();
            return false;
        }
    }
}
