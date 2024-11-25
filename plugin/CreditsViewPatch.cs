using HarmonyLib;
using JapaneseMod.structs;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using Warborn;

namespace JapaneseMod
{
    internal static class CreditsViewPatchVariables
    {
        internal static TextView EnglishLyrics;
        internal static TextView JapaneseLyrics;
        internal static ImageView TextBg;
        internal static TextView CommonLyrics;
    }

    [HarmonyPatch(typeof(CreditsView), "Awake")]
    public static class CreditsViewAwakePatch
    {
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

        public static void Postfix(ref CreditsView __instance)
        {
            CreditsViewPatchVariables.TextBg = __instance.AddNewChildImageView("Logo", CreateSolidColorSprite(new Color(0f, 0f, 0f, 0.9f)));
            CreditsViewPatchVariables.EnglishLyrics = __instance.AddNewChildTextView("Lyrics", "", Plugin.BootAssetsReference.AlternateFont, 32, Color.white, TextAlignmentOptions.Center);
            CreditsViewPatchVariables.CommonLyrics = __instance.AddNewChildTextView("Lyrics", "", Plugin.BootAssetsReference.AlternateFont, 32, Color.white, TextAlignmentOptions.Center);
            CreditsViewPatchVariables.JapaneseLyrics = __instance.AddNewChildTextView("Lyrics", "", Plugin.Assets.StoredLanguageFonts.Where(font => font.LanguageCode == Plugin.LANGUAGE_JA_JP && font.Type == FontType.AlternateOutline && font.PatchMode).FirstOrDefault().Font, 32, Color.white, TextAlignmentOptions.Center);

            CreditsViewPatchVariables.TextBg.SetSize(1920f, 1080 / 100 * 12);
            CreditsViewPatchVariables.TextBg.SetBottomPosition(0);
            CreditsViewPatchVariables.TextBg.RectTransform.anchorMin = new Vector2(0, 0);
            CreditsViewPatchVariables.TextBg.RectTransform.anchorMax = new Vector2(1, 0);

            CreditsViewPatchVariables.EnglishLyrics.SetBottomPosition(1080 / 100 * 6);
            CreditsViewPatchVariables.EnglishLyrics.SetWidth(1920f);
            Game.Common.ApplyDefaultBlueGradient(CreditsViewPatchVariables.EnglishLyrics);

            CreditsViewPatchVariables.CommonLyrics.SetBottomPosition(1080 / 100 * 3.5f);
            CreditsViewPatchVariables.CommonLyrics.SetWidth(1920f);
            Game.Common.ApplyDefaultBlueGradient(CreditsViewPatchVariables.CommonLyrics);

            CreditsViewPatchVariables.JapaneseLyrics.SetBottomPosition(1080 / 100 * 1);
            CreditsViewPatchVariables.JapaneseLyrics.SetWidth(1920f);
            Game.Common.ApplyDefaultBlueGradient(CreditsViewPatchVariables.JapaneseLyrics);

            __instance.ControllerActionsStackingView.RectTransform.SetAsLastSibling();
        }

        [HarmonyPatch(typeof(CreditsView), "RunCredits")]
        public static class RunCreditsPatch
        {
            public static void Prefix(ref CreditsView __instance)
            {
                __instance.StartCoroutine(UpdateLyrics(__instance));
            }

            private static IEnumerator UpdateLyrics(CreditsView instance)
            {
                yield return new WaitUntil(() => BaseGame.Audio.CurrentBGMAudioSource.clip.name == "Warborn - Music - Rise Up");

                yield return new WaitForSeconds(20.6f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("I know the ways that you have made me stronger    Even if it's not the way you'd hoped it to be");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("あなたが望んでいなくても　あなたのくれた強さで");
                yield return new WaitForSeconds(11.5f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("Walking on coals    The fire in your eyes");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("灼熱の道　燃える瞳");
                yield return new WaitForSeconds(5.5f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("A step out of line    Is the only way of making our wrongs right");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("踏み出す者が　あやまちを正せるなら");
                yield return new WaitForSeconds(6.5f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("");
                CreditsViewPatchVariables.CommonLyrics.ConfigureText("Rise up");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("");
                yield return new WaitForSeconds(1.5f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("'cos I've had enough of these lies");
                CreditsViewPatchVariables.CommonLyrics.ConfigureText("");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("もう噓はいらない");
                yield return new WaitForSeconds(5.5f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("");
                CreditsViewPatchVariables.CommonLyrics.ConfigureText("(it's a sign, it's a sign of the times)");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("");
                yield return new WaitForSeconds(4.2f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("");
                CreditsViewPatchVariables.CommonLyrics.ConfigureText("Rise up");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("");
                yield return new WaitForSeconds(1.5f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("With the rage and fear of a fight");
                CreditsViewPatchVariables.CommonLyrics.ConfigureText("");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("怖れと怒りを抱いて");
                yield return new WaitForSeconds(5.5f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("");
                CreditsViewPatchVariables.CommonLyrics.ConfigureText("(it's a sign, it's a sign of the times)");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("");
                yield return new WaitForSeconds(6f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("Uphold our name");
                CreditsViewPatchVariables.CommonLyrics.ConfigureText("");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("誰もが");
                yield return new WaitForSeconds(5.8f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("From dawn to dusk");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("黄昏");
                yield return new WaitForSeconds(5.5f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("We're both the same");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("誇りで");
                yield return new WaitForSeconds(5.5f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("We're credulous");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("染めてく");
                yield return new WaitForSeconds(3.5f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("Walking on coals    The fire in your eyes");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("灼熱の道　燃える瞳");
                yield return new WaitForSeconds(5.5f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("A step out of line    Is the only way of making our wrongs right");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("踏み出す者が　あやまちを正せるなら");
                yield return new WaitForSeconds(6.5f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("");
                CreditsViewPatchVariables.CommonLyrics.ConfigureText("Rise up");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("");
                yield return new WaitForSeconds(1.5f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("'cos I've had enough of these lies");
                CreditsViewPatchVariables.CommonLyrics.ConfigureText("");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("もう噓はいらない");
                yield return new WaitForSeconds(5.5f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("");
                CreditsViewPatchVariables.CommonLyrics.ConfigureText("(it's a sign, it's a sign of the times)");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("");
                yield return new WaitForSeconds(4.2f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("");
                CreditsViewPatchVariables.CommonLyrics.ConfigureText("Rise up");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("");
                yield return new WaitForSeconds(1.5f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("With the rage and fear of a fight");
                CreditsViewPatchVariables.CommonLyrics.ConfigureText("");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("怖れと怒りを抱いて");
                yield return new WaitForSeconds(5.5f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("");
                CreditsViewPatchVariables.CommonLyrics.ConfigureText("(it's a sign, it's a sign of the times)");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("");
                yield return new WaitForSeconds(5.5f);

                CreditsViewPatchVariables.EnglishLyrics.ConfigureText("");
                CreditsViewPatchVariables.CommonLyrics.ConfigureText("");
                CreditsViewPatchVariables.JapaneseLyrics.ConfigureText("");
                yield return new WaitForSeconds(5.5f);

                CreditsViewPatchVariables.TextBg.ShowOrHideViewOffscreenInDirection(RectTransform.Edge.Bottom, false, 0, 5.5f, true, null);
                yield return new WaitForSeconds(6f);

                yield break;

            }
        }
    }
}
