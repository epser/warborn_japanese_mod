using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using JapaneseMod.@struct;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Warborn;

namespace JapaneseMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("WARBORN.exe")]
public class Plugin : BaseUnityPlugin
{
    // static
    internal static new ManualLogSource Logger;
    internal static ModConfig ConfigStruct;
    internal static AssetOverwrite Assets;
    internal static Dictionary<string, Dictionary<string, string>> StoredLanguageStrings;
    internal static Dictionary<string, string> StoredLocalizedStrings;
    internal static bool IsPatchEnabled = true;

    // private
    private IInputSystem Input;
    private KeyCode MainKeyForLanguageSwitch;
    private IEnumerable<KeyCode> ModifierKeysForLanguageSwitch;
    private KeyCode MainKeyForPatchSwitch;
    private IEnumerable<KeyCode> ModifierKeysForPatchSwitch;

    // original classes
    internal static LocalizationManager LanguageManagerReference;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();

        // Load config
        ConfigStruct = new ModConfig(Config);

        // フォントの読み込み
        Assets = new AssetOverwrite(ConfigStruct);

        // Input system
        Input = UnityInput.Current;

        StoredLanguageStrings = new Dictionary<string, Dictionary<string, string>>();
        StoredLocalizedStrings = new Dictionary<string, string>();
    }

    /**
     * 毎フレーム呼ばれる処理
     */
    private void Update()
    {
        if(IsPressedKeys(ConfigStruct.languageSwitchKey.Value.MainKey, ConfigStruct.languageSwitchKey.Value.Modifiers))
        {
            SwitchLanguage();
        }

        if(IsPressedKeys(ConfigStruct.modSwitchKey.Value.MainKey, ConfigStruct.modSwitchKey.Value.Modifiers))
        {
            SwitchModMode();
        }
    }

    private bool IsPressedKeys(KeyCode mainKey, IEnumerable<KeyCode> modifiers)
    {
        if (Input.GetKeyDown(mainKey) && modifiers.All(Input.GetKey))
        {
            Input.GetKeyDown(mainKey);
            return true;
        }
        return false;
    }

    private void SwitchModMode()
    {
        IsPatchEnabled = !IsPatchEnabled;
        LanguageManagerReference.LoadStringsForLocale(LanguageManagerReference.CurrentLanguageKey, false);
        Logger.LogInfo($"Patch is {(IsPatchEnabled ? "enabled" : "disabled")}");
    }

    private void SwitchLanguage()
    {
        if (LanguageManagerReference.CurrentLanguageKey == null)
        {
            Logger.LogInfo("CurrentLanguageKey is null");
            return;
        }

        // ja-JP と en-GB の間をトグル
        if (LanguageManagerReference.CurrentLanguageKey == "ja-JP")
        {
            Logger.LogInfo("Switching to en-GB");
            LanguageManagerReference.LoadStringsForLocale("en-GB", false);
        }
        else
        {
            Logger.LogInfo("Switching to ja-JP");
            LanguageManagerReference.LoadStringsForLocale("ja-JP", false);
        }
    }
}
