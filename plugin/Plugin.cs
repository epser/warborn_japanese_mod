using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using JapaneseMod.@struct;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JapaneseMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("WARBORN.exe")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    internal static ModConfig ConfigStruct;
    internal static AssetOverwrite Assets;
    internal static IInputSystem Input;

    private KeyCode MainKeyForLanguageSwitch;
    private IEnumerable<KeyCode> ModifierKeysForLanguageSwitch;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();

        // Load config
        ConfigStruct = new ModConfig(Config);
        MainKeyForLanguageSwitch = ConfigStruct.languageSwitchKey.Value.MainKey;
        ModifierKeysForLanguageSwitch = ConfigStruct.languageSwitchKey.Value.Modifiers;

        // フォントの読み込み
        Assets = new AssetOverwrite(ConfigStruct);

        // Input system
        Input = UnityInput.Current;
    }

    private void Update()
    {
        // Plugin update logic
        IsPressedKeys(MainKeyForLanguageSwitch, ModifierKeysForLanguageSwitch);
    }

    private void IsPressedKeys(KeyCode mainKey, IEnumerable<KeyCode> modifiers)
    {
        if (modifiers.All(Input.GetKey))
        {
            if (Input.GetKeyDown(mainKey))
            {
                Logger.LogInfo("Language switch key is pressed!");
                // ここに言語切り替えの処理を書く
            }
        }
    }
}
