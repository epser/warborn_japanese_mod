using HarmonyLib;
using JapaneseMod.structs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Warborn;

namespace JapaneseMod
{
    [HarmonyPatch(typeof(CommonAssets))]
    [HarmonyPatch("DefaultFont", MethodType.Getter)]
    public static class DefaultFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            if (Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_EN_GB && Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_JA_JP)
            {
                return;
            }

            var font = Plugin.Assets.FindFontStructs(
                null,
                Game.Locale.CurrentLanguageKey,
                FontType.Default,
                (Game.Locale.CurrentLanguageKey == Plugin.LANGUAGE_JA_JP && Plugin.IsPatchEnabled)
            ).FirstOrDefault()?.Font ?? null;
            if (font == null)
            {
                return;
            }
            __result = font;
        }
    }

    [HarmonyPatch(typeof(CommonAssets), "DefaultOutlineFont", MethodType.Getter)]
    public static class DefaultOutlineFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            if (Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_EN_GB && Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_JA_JP)
            {
                return;
            }

            var font = Plugin.Assets.FindFontStructs(
                null,
                Game.Locale.CurrentLanguageKey,
                FontType.DefaultOutline,
                (Game.Locale.CurrentLanguageKey == Plugin.LANGUAGE_JA_JP && Plugin.IsPatchEnabled)
            ).FirstOrDefault()?.Font ?? null;
            if (font == null)
            {
                return;
            }
            __result = font;
        }
    }

    [HarmonyPatch(typeof(CommonAssets), "AlternateFont", MethodType.Getter)]
    public static class AlternateFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            if (Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_EN_GB && Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_JA_JP)
            {
                return;
            }

            var font = Plugin.Assets.FindFontStructs(
                null,
                Game.Locale.CurrentLanguageKey,
                FontType.Alternate,
                (Game.Locale.CurrentLanguageKey == Plugin.LANGUAGE_JA_JP && Plugin.IsPatchEnabled)
            ).FirstOrDefault()?.Font ?? null;
            if (font == null)
            {
                return;
            }
            __result = font;
        }
    }

    [HarmonyPatch(typeof(CommonAssets), "AlternateOutlineFont", MethodType.Getter)]
    public static class AlternateOutlineFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            if (Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_EN_GB && Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_JA_JP)
            {
                return;
            }

            var font = Plugin.Assets.FindFontStructs(
                null,
                Game.Locale.CurrentLanguageKey,
                FontType.AlternateOutline,
                (Game.Locale.CurrentLanguageKey == Plugin.LANGUAGE_JA_JP && Plugin.IsPatchEnabled)
            ).FirstOrDefault()?.Font ?? null;
            if (font == null)
            {
                return;
            }
            __result = font;
        }
    }

    [HarmonyPatch(typeof(CommonAssets), "ActionOutlineFont", MethodType.Getter)]
    public static class ActionOutlineFontPatch
    {
        public static void Postfix(ref TMP_FontAsset __result)
        {
            if (Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_EN_GB && Game.Locale.CurrentLanguageKey != Plugin.LANGUAGE_JA_JP)
            {
                return;
            }

            var font = Plugin.Assets.FindFontStructs(
                null,
                Game.Locale.CurrentLanguageKey,
                FontType.ActionOutline,
                (Game.Locale.CurrentLanguageKey == Plugin.LANGUAGE_JA_JP && Plugin.IsPatchEnabled)
            ).FirstOrDefault()?.Font ?? null;
            if (font == null)
            {
                return;
            }
            __result = font;
        }
    }

    // 元定義 public string[] GetLocalisedNameArrayForCommander(CommanderConfig.Commanders commander)
    [HarmonyPatch(typeof(CommonAssets), "GetLocalisedNameArrayForCommander")]
    public static class GetLocalisedNameArrayForCommanderPatch
    {
        // CommanderConfigとLocaleKeys.*の対応Dictionary
        private static readonly Dictionary<CommanderConfig.Commanders, string> LocaleKeyTable = new Dictionary<CommanderConfig.Commanders, string>
        {
            { CommanderConfig.Commanders.LuellaAugstein, LocaleKeys.LUELLA_AUGSTEIN },
            { CommanderConfig.Commanders.VincentUviir, LocaleKeys.VINCENT_UVIIR },
            { CommanderConfig.Commanders.IzolLokman, LocaleKeys.IZOL_LOKMAN },
            { CommanderConfig.Commanders.AurielleKrukov, LocaleKeys.AURIELLE_KRUKOV },
            { CommanderConfig.Commanders.LysanderOswell, LocaleKeys.LYSANDER_OSWELL },
            { CommanderConfig.Commanders.CervantesMoray, LocaleKeys.CERVANTES_MORAY },
            { CommanderConfig.Commanders.GenericNOMAD, LocaleKeys.NOMAD_FORCES },
            { CommanderConfig.Commanders.GenericNethalis, LocaleKeys.NETHALIS_FORCES },
            { CommanderConfig.Commanders.GenericCerulia, LocaleKeys.CERULIA_FORCES },
            { CommanderConfig.Commanders.GenericKrukov, LocaleKeys.KRUKOV_FORCES },
            { CommanderConfig.Commanders.GenericShadowWolves, LocaleKeys.PIRATES },
            { CommanderConfig.Commanders.GenericLysandersLoyalists, LocaleKeys.LOYALISTS }
        };

        public static bool Prefix(ref string[] __result, CommanderConfig.Commanders commander)
        {
            if (!LocaleKeyTable.ContainsKey(commander))
            {
                return true;
            }

            int splittedCount = Plugin.DesilializeLocalizedSymbolJson(
                Game.Locale.GetLocalizedString(LocaleKeyTable[commander], Array.Empty<object>())
            ).Split(new string[]
            {
                " ",
                "・"
            }, StringSplitOptions.RemoveEmptyEntries).Length;

            __result = new string[splittedCount];
            for (int i = 0; i < splittedCount; i++)
            {
                __result[i] = JsonConvert.SerializeObject(new Dictionary<string, object>
                {
                    { "text", LocaleKeyTable[commander] },
                    { "args", Array.Empty<object>() },
                    { "split", i }
                });
            }
            return false;
        }
    }
}
