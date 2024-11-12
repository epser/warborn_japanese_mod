using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace JapaneseMod.structs
{
    /**
     * 一つの役割に対して複数言語を格納する構造体
     * 言語コード文字列をキーに出し入れできる
     */
    internal class LanguageFontStruct
    {
        private Dictionary<string, TMP_FontAsset> fonts = [];

        // コンストラクタ AddFontsと同じ
        public LanguageFontStruct(Dictionary<string, TMP_FontAsset> fonts)
        {
            AddFonts(fonts);
        }

        // 言語コードとフォントのペアを受け取って格納するメソッド
        public void AddFont(string languageCode, TMP_FontAsset font)
        {
            fonts[languageCode] = font;
        }

        // AddFontの複数
        public void AddFonts(Dictionary<string, TMP_FontAsset> fonts)
        {
            foreach (var font in fonts)
            {
                AddFont(font.Key, font.Value);
            }
        }

        // 言語コードを受け取ってフォントを返すメソッド
        public TMP_FontAsset GetFont(string languageCode)
        {
            return fonts[languageCode];
        }

        public void RemoveFont(string languageCode) {
            fonts.Remove(languageCode);
        }

        // 

    }
}
