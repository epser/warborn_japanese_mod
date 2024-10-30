using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using TMPro;

namespace JapaneseMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("WARBORN.exe")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    internal static string OverwriteDataPath;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        OverwriteDataPath = $"{BepInEx.Paths.ConfigPath}/{MyPluginInfo.PLUGIN_GUID}";

        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();

        // フォントの読み込み
        Logger.LogInfo($"Load font AssetBundle on {OverwriteDataPath}/font");
        AssetOverwrite.LoadFontAsset($"{OverwriteDataPath}/font");

    }
}
