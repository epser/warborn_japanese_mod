using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using System.IO;

namespace JapaneseMod.@struct
{
    internal class AssetOverwrite
    {
        internal TMP_FontAsset jpDefaultFont;
        internal TMP_FontAsset jpDefaultOutlineFont;
        internal TMP_FontAsset jpAlternateFont;
        internal TMP_FontAsset jpAlternateOutlineFont;
        internal TMP_FontAsset jpActionOutlineFont;

        internal AssetOverwrite(ModConfig config)
        {
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

            jpDefaultFont = LoadFromAsset(bundle, config.jpDefaultFontName.Value);
            jpDefaultOutlineFont = LoadFromAsset(bundle, config.jpDefaultOutlineFontName.Value);
            jpAlternateFont = LoadFromAsset(bundle, config.jpAlternateFontName.Value);
            jpAlternateOutlineFont = LoadFromAsset(bundle, config.jpAlternateOutlineFontName.Value);
            jpActionOutlineFont = LoadFromAsset(bundle, config.jpActionOutlineFontName.Value);
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
