using HarmonyLib;
using JapaneseMod.structs;
using System;
using System.Linq;
using TMPro;
using Warborn;

namespace JapaneseMod
{
    [HarmonyPatch(typeof(DeploymentOptionsView), "Awake")]
    public static class DeploymentOptionsViewPatch
    {

        public static void Postfix(ref DeploymentOptionsView __instance)
        {
            var handleLanguageChanged = new LocalizationManager.LanguageChangedHandler(CreateHandleLanguageChanged(__instance));
            handleLanguageChanged();
            Plugin.EventPool.AddLanguageChangedHandler(__instance, handleLanguageChanged);

        }

        public static Action CreateHandleLanguageChanged(DeploymentOptionsView instance)
        {
            return () =>
            {
                TMP_FontAsset font;
                if (Plugin.IsPatchEnabled && Game.Locale.CurrentLanguageKey == Plugin.LANGUAGE_JA_JP)
                {
                    font = Plugin.Assets.FindFontStructs(
                        null,
                        Plugin.LANGUAGE_EN_GB,
                        FontType.Default
                    ).FirstOrDefault()?.Font ?? null;
                    instance.ResourceBalanceView.Text.font = font;
                    return;
                }

                instance.ResourceBalanceView.Text.font = Game.Common.DefaultFont;
            };
        }
    }
}
