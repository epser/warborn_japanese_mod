using BepInEx.Configuration;
using UnityEngine;

namespace JapaneseMod.services
{
    internal class ModConfig
    {
        internal ConfigEntry<string> assetBundleName; // Font bundle
        internal ConfigEntry<string> jpDefaultFontName; // Heading font with underlay
        internal ConfigEntry<string> jpDefaultOutlineFontName; // Heading font without underlay
        internal ConfigEntry<string> jpAlternateFontName; // Body font with underlay
        internal ConfigEntry<string> jpAlternateOutlineFontName; // Body font without underlay
        internal ConfigEntry<string> jpActionOutlineFontName; // Action font without underlay

        internal ConfigEntry<bool> useEnglishDesiginingFonts;
        internal ConfigEntry<KeyboardShortcut> modSwitchKey;
        internal ConfigEntry<KeyboardShortcut> languageSwitchKey;

        internal ModConfig(ConfigFile config)
        {
            assetBundleName = config.Bind("AssetBundle", "AssetBundleName", "font", "AssetBundle name");

            jpDefaultFontName = config.Bind(
                "AssetBundle",
                "JPDefaultFontName",
                "NotoSansJP-Bold SDF",
                "Heading font name(with underlay)"
            );

            jpDefaultOutlineFontName = config.Bind(
                "AssetBundle",
                "JPDefaultOutlineFontName",
                "NotoSansJP-Bold Outline SDF",
                "Heading font name(without underlay)"
            );
            jpAlternateFontName = config.Bind(
                "AssetBundle",
                "JPAlternateFontName",
                "NotoSansJP-Regular SDF",
                "Body font name(with underlay)"
            );
            jpAlternateOutlineFontName = config.Bind(
                "AssetBundle",
                "JPAlternateOutlineFontName",
                "NotoSansJP-Regular Outline SDF",
                "Body font name(without underlay)"
            );
            jpActionOutlineFontName = config.Bind(
                "AssetBundle",
                "JPActionOutlineFontName",
                "NotoSansJP-Light Outline SDF",
                "Action font name(without underlay)"
            );

            useEnglishDesiginingFonts = config.Bind("Design", "UseEnglishDesiginingFonts", false, "Use English designing fonts");
            modSwitchKey = config.Bind("Key", "ModSwitchKey", new KeyboardShortcut(KeyCode.F5), "Switch mod key");
            languageSwitchKey = config.Bind("Key", "LanguageSwitchKey", new KeyboardShortcut(KeyCode.F6), "Switch language key");
        }
    }
}
