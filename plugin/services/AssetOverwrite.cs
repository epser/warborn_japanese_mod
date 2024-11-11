using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using System.IO;
using JapaneseMod.structs;
using Warborn;

namespace JapaneseMod.services
{
    internal class AssetOverwrite
    {
        internal Dictionary<string, FontsStruct> StoredLanguageFonts = [];

        private ModConfig config { get; set; }

        internal AssetOverwrite(ModConfig config)
        {
            this.config = config;
        }

        internal void LoadFontAssets() {
            Plugin.Logger.LogInfo("Loading font assets...");
            var path = $"{BepInEx.Paths.ConfigPath}/{MyPluginInfo.PLUGIN_GUID}/{config.assetBundleName.Value}";
            if(!System.IO.File.Exists(path))
            {
                Plugin.Logger.LogError($"AssetBundle not found: {path}");
                return;
            }

            var bundle = AssetBundle.LoadFromFile(path);
            if (bundle == null)
            {
                Plugin.Logger.LogError($"Failed to load AssetBundle: {path}");
                return;
            }
            Plugin.Logger.LogInfo($"Loaded AssetBundle: {path}");

            StoredLanguageFonts[Plugin.LANGUAGE_EN_GB] = new FontsStruct
            (
                Plugin.BootAssetsReference.DefaultFont,
                Plugin.BootAssetsReference.DefaultOutlineFont,
                Plugin.BootAssetsReference.AlternateFont,
                Plugin.BootAssetsReference.AlternateOutlineFont,
                Plugin.BootAssetsReference.ActionOutlineFont
            );
            StoredLanguageFonts[Plugin.LANGUAGE_JA_JP] = new FontsStruct
            (
                Plugin.BootAssetsReference.JPDefaultFont,
                Plugin.BootAssetsReference.JPDefaultOutlineFont,
                Plugin.BootAssetsReference.JPDefaultFont,
                Plugin.BootAssetsReference.JPDefaultOutlineFont,
                Plugin.BootAssetsReference.JPDefaultOutlineFont
            );
            StoredLanguageFonts[Plugin.LANGUAGE_JA_JP_MOD] = new FontsStruct
            (
                LoadFromAsset(bundle, config.jpDefaultFontName.Value) ?? Plugin.BootAssetsReference.JPDefaultFont,
                LoadFromAsset(bundle, config.jpDefaultOutlineFontName.Value) ?? Plugin.BootAssetsReference.JPDefaultOutlineFont,
                LoadFromAsset(bundle, config.jpAlternateFontName.Value) ?? Plugin.BootAssetsReference.JPDefaultFont,
                LoadFromAsset(bundle, config.jpAlternateOutlineFontName.Value) ?? Plugin.BootAssetsReference.JPDefaultOutlineFont,
                LoadFromAsset(bundle, config.jpActionOutlineFontName.Value) ?? Plugin.BootAssetsReference.JPDefaultOutlineFont
            );
        }

        private TMP_FontAsset LoadFromAsset(AssetBundle bundle, string fontName)
        {
            // contains
            if (!bundle.Contains(fontName))
            {
                Plugin.Logger.LogError($"Font asset not found: {fontName}");
                return null;
            }

            var font = bundle.LoadAsset<TMP_FontAsset>(fontName);
            if (font == null)
            {
                Plugin.Logger.LogError($"Failed to load font asset: {fontName}");
            }

            Plugin.Logger.LogInfo($"Loaded font asset: {fontName}");
            return font;
        }

    }
}
