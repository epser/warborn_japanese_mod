﻿using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using JapaneseMod.services;
using JapaneseMod.structs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Warborn;

namespace JapaneseMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("WARBORN.exe")]
public class Plugin : BaseUnityPlugin
{
    // define
    internal static readonly string LANGUAGE_JA_JP = "ja-JP";
    internal static readonly string LANGUAGE_EN_GB = "en-GB";
    internal static readonly string LANGUAGE_JA_JP_MOD = "ja-JP-MOD";

    // static
    internal static new ManualLogSource Logger;
    internal static ModConfig ConfigStruct;
    internal static AssetOverwrite Assets;
    internal static Dictionary<string, Dictionary<string, string>> StoredLanguageStrings = [];
    internal static Dictionary<string, string> StoredLocalizedStrings = [];
    internal static bool IsPatchEnabled = true;
    internal static EventPool EventPool = new();

    // private
    private IInputSystem Input;
    private KeyCode MainKeyForLanguageSwitch;
    private IEnumerable<KeyCode> ModifierKeysForLanguageSwitch;
    private KeyCode MainKeyForPatchSwitch;
    private IEnumerable<KeyCode> ModifierKeysForPatchSwitch;

    // original classes
    internal static LocalizationManager LocalizationManagerReference;
    internal static BootAssets BootAssetsReference;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();

        ConfigStruct = new ModConfig(Config);
        Assets = new AssetOverwrite(ConfigStruct);

        // Input system
        Input = UnityInput.Current;
    }

    /**
     * 毎フレーム呼ばれる処理
     */
    private void Update()
    {
        if (IsPressedKeys(ConfigStruct.languageSwitchKey.Value.MainKey, ConfigStruct.languageSwitchKey.Value.Modifiers))
        {
            SwitchLanguage();
        }

        if (IsPressedKeys(ConfigStruct.modSwitchKey.Value.MainKey, ConfigStruct.modSwitchKey.Value.Modifiers))
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
        LocalizationManagerReference.LoadStringsForLocale(LocalizationManagerReference.CurrentLanguageKey, false);
        Logger.LogInfo($"Patch is {(IsPatchEnabled ? "enabled" : "disabled")}");
    }

    private void SwitchLanguage()
    {
        if (LocalizationManagerReference.CurrentLanguageKey == null)
        {
            Logger.LogInfo("CurrentLanguageKey is null");
            return;
        }

        // ja-JP と en-GB の間をトグル
        if (LocalizationManagerReference.CurrentLanguageKey == "ja-JP")
        {
            Logger.LogInfo("Switching to en-GB");
            LocalizationManagerReference.LoadStringsForLocale("en-GB", false);
        }
        else
        {
            Logger.LogInfo("Switching to ja-JP");
            LocalizationManagerReference.LoadStringsForLocale("ja-JP", false);
        }
    }

    internal static string DesilializeLocalizedSymbolJson(string value)
    {
        Plugin.Logger.LogInfo("DesilializeLocalizedSymbolJson is called! " + value);

        // まず1文字目を見て簡易判定したほうが安いか？　と思って入れる
        if (value == null || value.Length < 1 || value[0] != '{')
        {
            return value;
        }

        // JSONでパースしてみて、違うならそのまま返却
        var parsed = new InternalJsonReaderStruct();
        try
        {
            parsed = JsonConvert.DeserializeObject<InternalJsonReaderStruct>(value);
            if (parsed == null)
            {
                Plugin.Logger.LogInfo("failed parse");
                return value;
            }
        }
        catch (Exception)
        {
            Plugin.Logger.LogInfo("failed parse");
            return value;
        }

        var localeKey = parsed.text;
        var args = parsed.args;
        var capitalized = false;
        if (localeKey == null && parsed.TEXT != null && parsed.ARGS != null)
        {
            localeKey = parsed.TEXT;
            args = parsed.ARGS;
            capitalized = true;
        }
        if (localeKey == null)
        {
            return value;
        }
        Plugin.Logger.LogInfo("arg:" + args);

        var passArgs = new List<object>();
        // args(フォーマット文字列)が1個以上あればデコードする
        if (args != null)
        {
            foreach (var arg in args)
            {
                passArgs.Add(arg);
                Plugin.Logger.LogInfo("parseArgs:" + arg.ToString());
            }
        }

        Plugin.Logger.LogInfo("set localize string");
        if (capitalized)
        {
            return GetLocalizedStringOriginal(localeKey, passArgs.ToArray()).ToUpper();
        }
        return GetLocalizedStringOriginal(localeKey, passArgs.ToArray());
    }

    /**
     * LocalizationManager.GetLocalizedStringの元動作 一旦ここに置く
     */
    internal static string GetLocalizedStringOriginal(string key, params object[] formatArgs)
    {
        //Plugin.Logger.LogInfo("LocalizationManager.GetLocalizedStringOriginal is called! " + Game.Locale.CurrentLanguageKey);

        Dictionary<string, string> localizedStrings = null;

        // en-GBでもja-JPでもない場合は現在の言語を参照する
        if (Game.Locale.CurrentLanguageKey != LANGUAGE_EN_GB && Game.Locale.CurrentLanguageKey != LANGUAGE_JA_JP)
        {
            localizedStrings = (Dictionary<string, string>)Traverse.Create(Game.Locale).Field("LocalizedStrings").GetValue();
        }
        else if (Game.Locale.CurrentLanguageKey == LANGUAGE_EN_GB)
        {
            localizedStrings = Plugin.StoredLanguageStrings[LANGUAGE_EN_GB];
        }
        else if (Game.Locale.CurrentLanguageKey == LANGUAGE_JA_JP)
        {
            // patched分岐
            if (Plugin.IsPatchEnabled)
            {
                localizedStrings = Plugin.StoredLanguageStrings[LANGUAGE_JA_JP_MOD];
            }
            else
            {
                localizedStrings = Plugin.StoredLanguageStrings[LANGUAGE_JA_JP];
            }
        }

        if (localizedStrings == null)
        {
            LocalizationManagerReference.LoadStringsForLocale(LANGUAGE_EN_GB, false);
        }
        if (localizedStrings.ContainsKey(key))
        {
            Plugin.Logger.LogInfo("LocalizedStrings contains key: " + key);
            string text = localizedStrings[key];
            if (formatArgs != null && formatArgs.Length != 0)
            {
                text = string.Format(text, formatArgs);
            }
            return text;
        }
        return key;
    }
}
