using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace JapaneseMod
{
    internal class AssetOverwrite
    {
        internal static TMP_FontAsset DefaultFont;
        internal static TMP_FontAsset OutlineFont;
        internal static TMP_FontAsset AlternateFont;
        public static void LoadFontAsset(string assetBundlePath)
        {
            var assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
            if(assetBundle == null)
            {
                Plugin.Logger.LogError($"Failed to load AssetBundle from {assetBundlePath}");
                return;
            }
            DefaultFont = assetBundle.LoadAsset<TMP_FontAsset>("NotoSansJP-Regular SDF");
            OutlineFont = assetBundle.LoadAsset<TMP_FontAsset>("NotoSansJP-Regular SDF Outline");
            AlternateFont = assetBundle.LoadAsset<TMP_FontAsset>("NotoSansJP-Regular SDF");
        }
    }
}
