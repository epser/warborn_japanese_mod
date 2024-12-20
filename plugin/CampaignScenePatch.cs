﻿using HarmonyLib;
using InControl;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using Warborn;

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
                        (Game.Input.ControllerWasLastInput && Game.Input.ActiveDevice.LeftTrigger.WasPressed)
                        || (!Game.Input.ControllerWasLastInput && SpaceKey.IsPressed)
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
        public static bool LoadMissionsToMapFromCurrentSaveDataPrefix(ref CampaignScene __instance)
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

#if DEBUG
            if (missionInfo.MissionScript != null && missionInfo.MissionID == "LUELLA-INTRO")
            {
                return HandleEpilogueButtonPressed(ref __instance, true);
            }
#endif

            if (missionInfo.MissionScript != null && missionInfo.MissionID != "LUELLA-INTRO")
            {
                var instance = __instance;
                int selectedCellIndex = instance.CampaignView.CampaignMissionMap.MissionListPanel.SelectedMissionListCell.Index;
                instance.CampaignView.CampaignMissionMap.MissionListPanel.SetButtonsEnabled(false);
                instance.CampaignView.CampaignMissionMap.MapMissionInfo.StartButton.ChildButton.SetButtonEnabled(false, true);
                __instance.CampaignView.CampaignMissionMap.MissionListPanel.MissionCollectionView.AllowInput = false;
                instance.CampaignView.IsTransitioning = true;
                CampaignMissionMapViewPatch.ControllerActionsStackingView?.ShowOrHideViewOffscreenInDirection(RectTransform.Edge.Right, false, 0f, 0.6f, true, null);
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
            BaseGame.Audio.PlayCancelSFX();
            return false;
        }


        public static bool HandleEpilogueButtonPressed(ref CampaignScene __instance, bool debugFlg = false)
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
                CampaignMissionMapViewPatch.ControllerActionsStackingView?.ShowOrHideViewOffscreenInDirection(RectTransform.Edge.Right, false, 0f, 0.6f, true, null);
                missionInfo.MissionScript.LoadDialogueCharacters();
                Traverse.Create(__instance).Field("isRunningCompletionDialogue").SetValue(true);
                var instance = __instance;
                __instance.CampaignView.CampaignMissionMap.TransitionOut(false, null);

                var dialogueFunc = missionInfo.MissionScript.RunCampaignMapCompletionDialogue;
#if DEBUG
                if (missionInfo.MissionID == "LUELLA-INTRO" && debugFlg)
                {
                    dialogueFunc = RunAllCampaignDialogue;
                }
#endif
                Game.Transition.FadeOutBlack(0.6f, delegate
                {
                    CampaignScene.PlayBGMAudio();
                    Game.Transition.FadeInBlack(0.6f, null);
                    dialogueFunc(instance.CampaignView, delegate
                    {
                        Traverse.Create(instance).Field("isRunningCompletionDialogue").SetValue(false);
                        missionInfo.MissionScript.CleanUp();
                        AnimatedBannerView missionSelectBanner = instance.CampaignView.MissionSelectBanner;
                        Action complete2 = delegate ()
                        {
                            instance.CampaignView.CampaignMissionMap.TransitionIn(false, delegate
                            {
                                instance.CampaignView.CampaignMissionMap.TransitionOut(animated: false, null);
                                CampaignMissionMapViewPatch.ControllerActionsStackingView?.ShowOrHideViewOffscreenInDirection(RectTransform.Edge.Right, true, 0f, 0.6f, true, null);

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

#if DEBUG
        public static void RunAllCampaignDialogue(CampaignView campaignView, Action complete)
        {
            var luellaCharacterInfo = CharacterConfig.GetCharacterInfo(CharacterConfig.Characters.LuellaAugstein, true);
            var samCharacterInfo = CharacterConfig.GetCharacterInfo(CharacterConfig.Characters.SamMatthews, true);

            Queue<DialogueViewAction> queue = new Queue<DialogueViewAction>();
            var scenarioRegex = new Regex(@"^MISSION_(LUELLA|VINCENT|AURIELLE|IZOL)_.+$");
            foreach (FieldInfo field in typeof(LocaleKeys).GetFields()) {
                var symbol = (string)field.GetValue(null);

                if (scenarioRegex.IsMatch(symbol))
                {
                    queue.Enqueue(new DialogueViewAction(luellaCharacterInfo, samCharacterInfo, luellaCharacterInfo, CharacterConfig.Moods.Smiling, null, false, Game.Locale.GetLocalizedString(symbol, Array.Empty<object>())));
                }

            }
            CutsceneView cutsceneView = CutsceneView.CreateDefaultCutscene();
            cutsceneView.ConfigureDefaultLocationWithShip(CutsceneView.CutsceneLocations.CeruliaSpace, CommanderConfig.Factions.NOMAD, false);
            CutsceneView.RunCutscene(cutsceneView, queue, campaignView, complete);
        }
#endif

        [HarmonyPatch(nameof(CampaignScene.HandleCommanderInfoButtonPressed))]
        [HarmonyPostfix]
        public static void HandleCommanderInfoButtonPressedPostfix()
        {
            CampaignMissionMapViewPatch.ControllerActionsStackingView?.ShowOrHideViewOffscreenInDirection(RectTransform.Edge.Right, false, 0f, 0.6f, true, null);
        }

        [HarmonyPatch(nameof(CampaignScene.HandleOptionsButtonPressed))]
        [HarmonyPostfix]
        public static void HandleOptionsButtonPressedPostfix()
        {
            CampaignMissionMapViewPatch.ControllerActionsStackingView?.ShowOrHideViewOffscreenInDirection(RectTransform.Edge.Right, false, 0f, 0.6f, true, null);
        }

        [HarmonyPatch(nameof(CampaignScene.HandleOptionsScreenConfirmButtonPressed))]
        [HarmonyPostfix]
        public static void HandleOptionsScreenConfirmButtonPressedPostFix()
        {
            CampaignMissionMapViewPatch.ControllerActionsStackingView?.ShowOrHideViewOffscreenInDirection(RectTransform.Edge.Right, true, 0f, 0.6f, true, null);
        }

        [HarmonyPatch(nameof(CampaignScene.HandleCommanderViewCloseButtonPressed))]
        [HarmonyPostfix]
        public static void HandleCommanderViewCloseButtonPressedPostfix()
        {
            CampaignMissionMapViewPatch.ControllerActionsStackingView?.ShowOrHideViewOffscreenInDirection(RectTransform.Edge.Right, true, 0f, 0.6f, true, null);
        }

        [HarmonyPatch(nameof(CampaignScene.LaunchMission))]
        [HarmonyPostfix]
        public static void LaunchMissionPostfix()
        {
            CampaignMissionMapViewPatch.ControllerActionsStackingView?.ShowOrHideViewOffscreenInDirection(RectTransform.Edge.Right, false, 0f, 0.6f, true, null);
        }

        [HarmonyPatch(nameof(CampaignScene.DisplayTraitUnlock))]
        [HarmonyPostfix]
        public static void DisplayTraitUnlockPostfix(ref CampaignScene __instance)
        {
            CampaignMissionMapViewPatch.ControllerActionsStackingView?.ShowOrHideViewOffscreenInDirection(RectTransform.Edge.Right, true, 0f, 0.6f, true, null);
        }


    }
}
