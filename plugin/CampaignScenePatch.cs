using HarmonyLib;
using InControl;
using System.Collections.Generic;
using System;
using UnityEngine;
using Warborn;
using System.Numerics;

namespace JapaneseMod
{
    [HarmonyPatch(typeof(CampaignScene))]
    public class CampaignScenePatch
    {
        public static bool DisplayingControllerActions = false;
        public static int? PreservedTabIndex = null;
        public static int? PreservedMissionIndex = null;
        public static InControl.KeyCombo SpaceKey = new([Key.Space]);

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static bool UpdatePrefix(ref CampaignScene __instance)
        {
            // キー押下チェック
            if (__instance.CampaignView.CampaignMissionMap.MissionListPanel.MissionCollectionView.AllowInput && Game.Input.Controls.Intel)
            {
                return HandlePrologueButtonPressed(ref __instance);
            }
            else if (__instance.CampaignView.CampaignMissionMap.MissionListPanel.MissionCollectionView.AllowInput 
                && 
                    (
                        (Game.Input.IsOnlyUsingController() && Game.Input.ControllerWasLastInput && Game.Input.ActiveDevice.LeftTrigger.WasPressed)
                        || (!Game.Input.IsOnlyUsingController() && !Game.Input.ControllerWasLastInput && SpaceKey.IsPressed)
                    )
                )
            {
                return HandleEpilogueButtonPressed(ref __instance);
            }

            // ボタンラベル変更
            if (CampaignMissionMapViewPatch.displayingControllerActions && !Game.Input.IsOnlyUsingController() && !Game.Input.ControllerWasLastInput)
            {
                CampaignMissionMapViewPatch.ControllerActionsStackingView.UpdateActionPrompts(
                    CampaignMissionMapViewPatch.actionPromptsPC, true, false, 0f
                );
                CampaignMissionMapViewPatch.displayingControllerActions = false;
            }
            else if (!CampaignMissionMapViewPatch.displayingControllerActions && (Game.Input.IsOnlyUsingController() || Game.Input.ControllerWasLastInput))
            {
                CampaignMissionMapViewPatch.ControllerActionsStackingView.UpdateActionPrompts(
                    CampaignMissionMapViewPatch.actionPromptsController, true, true, 0f
                );
                CampaignMissionMapViewPatch.displayingControllerActions = true;
            }

            return true;
        }

        [HarmonyPatch("LoadMissionsToMapFromCurrentSaveData")]
        [HarmonyPrefix]
        public static bool LoadMissionsToMapFromCurrentSaveDataPatch(ref CampaignScene __instance)
        {
            var instance = __instance;
            instance.CampaignView.CampaignMissionMap.TransitionOut(false, null);
            instance.CampaignView.CampaignMissionMap.UpdateCommanderAndMissionLists();
            instance.DisplayTraitUnlock(delegate (bool didDisplay)
            {
                if (!didDisplay)
                {
                    instance.CampaignView.CampaignMissionMap.TransitionIn(true, delegate {
                        if (PreservedTabIndex != null)
                        {
                            instance.CampaignView.CampaignMissionMap.MissionListPanel.ChapterTabView.HandleTabSelected((int)PreservedTabIndex);
                            PreservedTabIndex = null;
                        }
                        if (PreservedMissionIndex != null)
                        {
                            // リストビューの選択位置復元
                            instance.CampaignView.CampaignMissionMap.MissionListPanel.MissionCollectionView.SelectCell((int)PreservedMissionIndex);

                            // スクロール調整
                            instance.CampaignView.CampaignMissionMap.MissionListPanel.MissionCollectionView.OptimiseCellVisibility();

                            PreservedMissionIndex = null;
                        }
                    });
                }
            });
            return false;
        }

        public static bool HandlePrologueButtonPressed(ref CampaignScene __instance)
        {
            CampaignSaveData activeSaveSlot = Game.Data.GetActiveSaveSlot();
            // 選択中ミッションの情報を取得
            var missionInfo = __instance.CampaignView.CampaignMissionMap.MissionListPanel.SelectedMissionListCell.LinkedMissionInfo;
            PreservedTabIndex = __instance.CampaignView.CampaignMissionMap.MissionListPanel.ChapterTabView.SelectedIndex;
            PreservedMissionIndex = __instance.CampaignView.CampaignMissionMap.MissionListPanel.SelectedMissionListCell.Index;

            if (missionInfo.MissionScript != null
                )
            {
                var instance = __instance;
                int selectedCellIndex = instance.CampaignView.CampaignMissionMap.MissionListPanel.SelectedMissionListCell.Index;
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
                        BaseGame.Scene.ChangeScene("CampaignScene", typeof(CampaignScene), true,
                        delegate (Action complete)
                        {
                            CampaignScene.PlayBGMAudio();
                            Game.Transition.FadeInBlack(0.6f, complete);
                        },
                        delegate (Action complete)
                        {
                            Game.Transition.FadeOutBlack(0.6f, complete);
                        },
                        null);
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
            var tabIndex = __instance.CampaignView.CampaignMissionMap.MissionListPanel.ChapterTabView.SelectedIndex;
            var missionIndex = __instance.CampaignView.CampaignMissionMap.MissionListPanel.SelectedMissionListCell.Index;

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
                    CampaignScene.PlayBGMAudio();
                    Game.Transition.FadeInBlack(0.6f, null);
                    missionInfo.MissionScript.RunCampaignMapCompletionDialogue(instance.CampaignView, delegate
                    {
                        Traverse.Create(instance).Field("isRunningCompletionDialogue").SetValue(false);
                        missionInfo.MissionScript.CleanUp();
                        AnimatedBannerView missionSelectBanner = instance.CampaignView.MissionSelectBanner;
                        Action complete2 = delegate ()
                        {
                            instance.CampaignView.CampaignMissionMap.TransitionIn(false, delegate
                            {
                                instance.CampaignView.CampaignMissionMap.TransitionOut(animated: false, null);
                                instance.CampaignView.CampaignMissionMap.UpdateCommanderAndMissionLists();
                                instance.DisplayTraitUnlock(delegate (bool didDisplay)
                                {
                                    if (!didDisplay)
                                    {
                                        instance.CampaignView.CampaignMissionMap.TransitionIn(animated: true, delegate
                                        {
                                            // タブの選択位置復元
                                            instance.CampaignView.CampaignMissionMap.MissionListPanel.ChapterTabView.HandleTabSelected(tabIndex);

                                            // リストビューの選択位置復元
                                            instance.CampaignView.CampaignMissionMap.MissionListPanel.MissionCollectionView.SelectCell(missionIndex);

                                            // スクロール調整
                                            instance.CampaignView.CampaignMissionMap.MissionListPanel.MissionCollectionView.OptimiseCellVisibility();
                                        });
                                    }
                                });
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
