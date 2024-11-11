using TMPro;

namespace JapaneseMod.structs
{
    public class FontsStruct(TMP_FontAsset defaultFont, TMP_FontAsset defaultOutlineFont, TMP_FontAsset alternateFont, TMP_FontAsset alternateOutlineFont, TMP_FontAsset actionOutlineFont)
    {
        public TMP_FontAsset DefaultFont { get; set; } = defaultFont;
        public TMP_FontAsset DefaultOutlineFont { get; set; } = defaultOutlineFont;
        public TMP_FontAsset AlternateFont { get; set; } = alternateFont;
        public TMP_FontAsset AlternateOutlineFont { get; set; } = alternateOutlineFont;
        public TMP_FontAsset ActionOutlineFont { get; set; } = actionOutlineFont;
    }
}
