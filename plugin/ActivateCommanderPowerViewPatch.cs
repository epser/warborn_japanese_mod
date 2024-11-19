using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TMPro;
using Warborn;

namespace JapaneseMod
{
    [HarmonyPatch(typeof(ActivateCommanderPowerView), "ConfigureForPlayer")]
    public class ActivateCommanderPowerViewPatch
    {
        public static void Postfix(ref ActivateCommanderPowerView __instance, ref Player player)
        {
            // privateフィールド powerName を取得
            string powerName = Traverse.Create(__instance).Field("powerName").GetValue<string>();

            switch (player.SelectedCommander)
            {
                case CommanderConfig.Commanders.LuellaAugstein:
                    powerName = 
                        Plugin.DesilializeLocalizedSymbolJson(
                            Game.Locale.GetLocalizedString(LocaleKeys.LUELLA_COMMANDER_POWER_NAME, Array.Empty<object>())
                        ).ToUpper().Replace(" ", "\n");
                    break;
                case CommanderConfig.Commanders.VincentUviir:
                    powerName =
                        Plugin.DesilializeLocalizedSymbolJson(
                            Game.Locale.GetLocalizedString(LocaleKeys.VINCENT_COMMANDER_POWER_NAME, Array.Empty<object>())
                        ).ToUpper().Replace(" ", "\n");
                    break;
                case CommanderConfig.Commanders.IzolLokman:
                    powerName =
                        Plugin.DesilializeLocalizedSymbolJson(
                            Game.Locale.GetLocalizedString(LocaleKeys.IZOL_COMMANDER_POWER_NAME, Array.Empty<object>())
                        ).ToUpper().Replace(" ", "\n");
                    break;
                case CommanderConfig.Commanders.AurielleKrukov:
                    powerName =
                        Plugin.DesilializeLocalizedSymbolJson(
                            Game.Locale.GetLocalizedString(LocaleKeys.AURIELLE_COMMANDER_POWER_NAME, Array.Empty<object>())
                        ).ToUpper().Replace(" ", "\n");
                    break;
                case CommanderConfig.Commanders.LysanderOswell:
                    powerName =
                        Plugin.DesilializeLocalizedSymbolJson(
                            Game.Locale.GetLocalizedString(LocaleKeys.LYSANDER_COMMANDER_POWER_NAME, Array.Empty<object>())
                        ).ToUpper().Replace(" ", "\n");
                    break;
                case CommanderConfig.Commanders.CervantesMoray:
                    powerName =
                        Plugin.DesilializeLocalizedSymbolJson(
                            Game.Locale.GetLocalizedString(LocaleKeys.CERVANTES_COMMANDER_POWER_NAME, Array.Empty<object>())
                        ).ToUpper().Replace(" ", "\n");
                    break;
            }
            Traverse.Create(__instance).Field("powerName").SetValue(powerName);
            __instance.PowerNameText.Text.text = powerName; // ロケール即時書き換えの対象外になる
        }
    }
}
