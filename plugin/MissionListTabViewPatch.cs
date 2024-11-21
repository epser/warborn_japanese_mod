using HarmonyLib;
using JapaneseMod.structs;
using System;
using System.Linq;
using TMPro;
using Warborn;

namespace JapaneseMod
{
    [HarmonyPatch(typeof(MissionListTabView), "Awake")]
    public static class MissionListTabViewPatch
    {

        public static void Postfix(ref MissionListTabView __instance)
        {
            var handleLanguageChanged = new LocalizationManager.LanguageChangedHandler(CreateHandleLanguageChanged(__instance));
            handleLanguageChanged();
            Plugin.EventPool.AddLanguageChangedHandler(__instance, handleLanguageChanged);

        }

        public static Action CreateHandleLanguageChanged(MissionListTabView instance)
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
                    instance.NewText.Text.font = font;
                    instance.NewText.Text.text = "New";
                    return;
                }

                instance.NewText.Text.font = Game.Common.DefaultFont;
                instance.NewText.Text.text = Plugin.LocalizationManagerReference.GetLocalizedString(LocaleKeys.NEW, Array.Empty<object>());
            };
        }
    }
}
