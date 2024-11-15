using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JapaneseMod.structs;
using System.Linq;

namespace JapaneseMod.services
{
    internal class AssetOverwrite
    {
        internal List<FontStruct> StoredLanguageFonts = [];

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


            StoredLanguageFonts.Add(new FontStruct(
                Plugin.BootAssetsReference.DefaultFont,
                FontType.Default,
                Plugin.LANGUAGE_EN_GB
            ));
            StoredLanguageFonts.Add(new FontStruct(
                Plugin.BootAssetsReference.DefaultOutlineFont,
                FontType.DefaultOutline,
                Plugin.LANGUAGE_EN_GB
            ));
            StoredLanguageFonts.Add(new FontStruct(
                Plugin.BootAssetsReference.AlternateFont,
                FontType.Alternate,
                Plugin.LANGUAGE_EN_GB
            ));
            StoredLanguageFonts.Add(new FontStruct(
                Plugin.BootAssetsReference.AlternateOutlineFont,
                FontType.AlternateOutline,
                Plugin.LANGUAGE_EN_GB
            ));
            StoredLanguageFonts.Add(new FontStruct(
                Plugin.BootAssetsReference.ActionOutlineFont,
                FontType.ActionOutline,
                Plugin.LANGUAGE_EN_GB
            ));

            StoredLanguageFonts.Add(new FontStruct(
                Plugin.BootAssetsReference.JPDefaultFont,
                FontType.Default,
                Plugin.LANGUAGE_JA_JP
            ));
            StoredLanguageFonts.Add(new FontStruct(
                Plugin.BootAssetsReference.JPDefaultOutlineFont,
                FontType.DefaultOutline,
                Plugin.LANGUAGE_JA_JP
            ));
            StoredLanguageFonts.Add(new FontStruct(
                Plugin.BootAssetsReference.JPDefaultFont,
                FontType.Alternate,
                Plugin.LANGUAGE_JA_JP
            ));
            StoredLanguageFonts.Add(new FontStruct(
                Plugin.BootAssetsReference.JPDefaultOutlineFont,
                FontType.AlternateOutline,
                Plugin.LANGUAGE_JA_JP
            ));
            StoredLanguageFonts.Add(new FontStruct(
                Plugin.BootAssetsReference.JPDefaultOutlineFont,
                FontType.ActionOutline,
                Plugin.LANGUAGE_JA_JP
            ));

            StoredLanguageFonts.Add(new FontStruct(
                LoadFromAsset(bundle, config.jpDefaultFontName.Value) ?? Plugin.BootAssetsReference.JPDefaultFont,
                FontType.Default,
                Plugin.LANGUAGE_JA_JP,
                true
            ));
            StoredLanguageFonts.Add(new FontStruct(
                LoadFromAsset(bundle, config.jpDefaultOutlineFontName.Value) ?? Plugin.BootAssetsReference.JPDefaultOutlineFont,
                FontType.DefaultOutline,
                Plugin.LANGUAGE_JA_JP,
                true
            ));
            StoredLanguageFonts.Add(new FontStruct(
                LoadFromAsset(bundle, config.jpAlternateFontName.Value) ?? Plugin.BootAssetsReference.JPDefaultFont,
                FontType.Alternate,
                Plugin.LANGUAGE_JA_JP,
                true
            ));
            StoredLanguageFonts.Add(new FontStruct(
                LoadFromAsset(bundle, config.jpAlternateOutlineFontName.Value) ?? Plugin.BootAssetsReference.JPDefaultOutlineFont,
                FontType.AlternateOutline,
                Plugin.LANGUAGE_JA_JP,
                true
            ));
            StoredLanguageFonts.Add(new FontStruct(
                LoadFromAsset(bundle, config.jpActionOutlineFontName.Value) ?? Plugin.BootAssetsReference.JPDefaultOutlineFont,
                FontType.ActionOutline,
                Plugin.LANGUAGE_JA_JP,
                true
            ));
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

        // fontstructの各要素を引数にした検索
        internal List<FontStruct> FindFontStructs(TMP_FontAsset font = null, string languageCode = null, FontType? type = null, bool? patchMode = false)
        {
            return StoredLanguageFonts
                .Where(fontStruct => font == null || (fontStruct.Font.hashCode == font.hashCode && fontStruct.Font.materialHashCode == font.materialHashCode))
                .Where(fontStruct => fontStruct.LanguageCode == languageCode || languageCode == null)
                .Where(fontStruct => fontStruct.Type == type || type == null)
                .Where(fontStruct => fontStruct.PatchMode == patchMode || patchMode == null)
                .ToList();
        }


    }
}
