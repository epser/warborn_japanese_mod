using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace JapaneseMod.structs
{
    /**
     * フォントの種別を表現するEnum
     */
    internal enum FontType
    {
        Default,
        DefaultOutline,
        Alternate,
        AlternateOutline,
        ActionOutline
    }

    /**
     * フォントに属性をつける構造体
     */
    internal class FontStruct(TMP_FontAsset font, FontType type, string languageCode, bool patchMode = false)
    {
        public TMP_FontAsset Font { get; } = font;
        public FontType Type { get; } = type;
        public string LanguageCode { get; } = languageCode;
        public bool PatchMode { get; } = patchMode;
    }
}
