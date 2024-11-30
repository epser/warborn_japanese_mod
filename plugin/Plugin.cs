using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using JapaneseMod.services;
using JapaneseMod.structs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        if (LocalizationManagerReference?.CurrentLanguageKey == LANGUAGE_JA_JP && IsPressedKeys(ConfigStruct.modSwitchKey.Value.MainKey, ConfigStruct.modSwitchKey.Value.Modifiers))
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

    private static readonly Regex JsonPattern = new Regex(
        @"\{(?:[^{}]|(?<Open>\{)|(?<Close-Open>\}))+(?(Open)(?!))\}",
        RegexOptions.Compiled
    );


    /**
     * JSON混じりの文字列をデシリアライズしてローカライズされた文字列を返す
     * 文字列にはLocalizeSymbolJsonStructのJSONが複数個含まれていることを想定
     */
    public static string DesilializeLocalizedSymbolJson(string value)
    {
        if (value == null)
        {
            return value;
        }

        var matches = JsonPattern.Matches(value);
        if (matches.Count == 0)
        {
            return value;
        }

        var result = value;
        foreach (Match match in matches)
        {
            var localized = DesilializeLocalizedSymbolJsonInternal(match.Value);
            result = result.Replace(match.Value, localized);
        }

        return result;
    }

    public static LocalizeSymbolJsonStruct DesilializeSingleLocalizedSymbolJson(string value)
    {
        try
        {
            var parsed = JsonConvert.DeserializeObject<LocalizeSymbolJsonStruct>(value);
            if (parsed == null)
            {
                return null;
            }
            return parsed;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static string DesilializeLocalizedSymbolJsonInternal(string value)
    {
        // まず1文字目を見て簡易判定したほうが安いか？　と思って入れる
        if (value == null || value.Length < 1 || value[0] != '{')
        {
            return value;
        }

        // JSONでパースしてみて、違うならそのまま返却
        var parsed = new LocalizeSymbolJsonStruct();
        try
        {
            parsed = JsonConvert.DeserializeObject<LocalizeSymbolJsonStruct>(value);
            if (parsed == null)
            {
                Plugin.Logger.LogWarning("failed parse: " + value);
                return value;
            }
        }
        catch (Exception)
        {
            return value;
        }

        var localeKey = parsed.text;
        var args = parsed.args;
        var capitalized = false;
        var split = parsed.split;
        if (localeKey == null && parsed.TEXT != null && parsed.ARGS != null)
        {
            localeKey = parsed.TEXT;
            args = parsed.ARGS;
            capitalized = true;
            split = parsed.SPLIT;
        }
        if (localeKey == null)
        {
            return value;
        }

        var passArgs = new List<object>();
        // args(フォーマット文字列)が1個以上あればデコードする
        if (args != null)
        {
            foreach (var arg in args)
            {
                passArgs.Add(DesilializeLocalizedSymbolJson(arg));
            }
        }

        var returnString = GetLocalizedStringOriginal(localeKey, passArgs.ToArray());
        if (capitalized)
        {
            returnString = returnString.ToUpper();
        }
        if (split != null)
        {
            var splitted = returnString.Split(new string[]
            {
                " ",
                "・"
            }, StringSplitOptions.RemoveEmptyEntries);
            split = Math.Min((int)split, splitted.Length - 1);
            returnString = splitted[(int)split];
        }
        return returnString;
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
            string text = localizedStrings[key];
            if (formatArgs != null && formatArgs.Length != 0)
            {
                text = string.Format(text, formatArgs);
            }
            return text;
        }
        return key;
    }

    public static Sprite CreateSolidColorSprite(Color color, int width = 32, int height = 32)
    {
        // テクスチャを作成
        Texture2D texture = new Texture2D(width, height);

        // すべてのピクセルを指定した色で塗りつぶす
        Color[] colors = new Color[width * height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = color;
        }
        texture.SetPixels(colors);
        texture.Apply();

        // SpriteをTextureから作成
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f), // ピボットを中心に設定
            100f  // pixelsPerUnit
        );

        return sprite;
    }
}
