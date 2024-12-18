# Japanese Mecha Anime Style Dialogue Mod for WARBORN

ストラテジーゲーム『WARBORN』の日本語ローカライズの品質を調整し、ストーリーの可読性と雰囲気を向上します。また、幾つかのQOLに関する追加機能を提供します（ゲームバランスは変更しません）。

## 配布物

[Releases](https://github.com/epser/warborn_japanese_mod/releases)ページで配布中です。インストール方法もそちらを参照してください。

## 法的注意事項

**!THIS MOD IS UNOFFICIAL!**

本Modは非公式です。適用に関する保証は存在しません。万一あなたのプレイ環境やゲーム進行への予期せぬ障害、あるいはその他のいかなる損害が発生したとしても、責任はあなた自身で負ってください。

本Modと本Modの制作者はRaredrop GamesおよびPQube Gamesとは無関係であり、同社の著作権や知的財産権その他の権利を侵害する意図は一切ありません。

Mod側からは使用・配信等の制限や規約については設けていません。動画配信やプレイ内容の共有等は、原作側に迷惑の掛からない範囲で好きなようにしてください。

## 追加機能

- 詳細はblogにも書いています: [『WARBORN』非公式日本語ローカライズ改善Modを作った - eps_r](https://eps-r.hatenablog.com/entry/2024/12/02/warborn-japanese-mod)

### ストーリー翻訳のリライト

- 誤訳
  - "Proximity Mine": 「近接鉱山」 → 「近接信管地雷」
  - "All systems nominal.": 「すべてのシステムは名前のみ」 → 「全システム正常」
  - その他さまざまに
- ストーリーの雰囲気を破壊する文体や訳語の選択の修正
- 文字がウィンドウをはみ出す、文字化け、改行記号の入力ミスなど(see [Screenshots](https://eps-r.hatenablog.com/entry/2024/12/02/warborn-japanese-mod#%E5%95%8F%E9%A1%8C%E7%82%B9))

上記のような深刻な問題を抱えるストーリーテキストを全面的に再翻訳しました。

### UIフォント、翻訳の向上

英語などの非CJKバージョンと比べて劣化している見栄えを改善します。

- 日本語ローカライズ時に「UIフォント」「本文フォント」の区別が消失していたのを修正（Noto Sans JP-Bold, Slimの追加）
- 一部の「ローカライズしすぎ」の文字列やフォントを英語版のスタイルに合わせ、UIデザインの統一感を改善

#### Before

![20241201233554](https://github.com/user-attachments/assets/544f1b17-e5d7-4dc1-8d82-14e9864051d9)

![20241201232840](https://github.com/user-attachments/assets/65e7858b-84ad-4fd1-8f92-a9f481ebdf14)

![20241202151926](https://github.com/user-attachments/assets/ee900e9a-c52d-42c0-ad14-db64782ecf81)

#### After

![20241201233458](https://github.com/user-attachments/assets/f3128880-234b-465a-82db-533fa244c799)

![20241201233829](https://github.com/user-attachments/assets/292cdfa2-0a12-43d6-9395-7a7a1d050022)

![20241202151953](https://github.com/user-attachments/assets/d753e343-28d4-4897-aba5-6d0423f4bc85)

### 実験的機能

#### オンデマンドな言語切り替え

翻訳の比較に便利な言語切り替えホットキーを追加しました。ほとんどの画面で機能します。

- **F5**: 日本語 - 非公式日本語
- **F6**: 英語 - 日本語(現在のModモード)

#### 会話のバックログ

**右スティック上下**か**ホイール上下**で現在の画面の会話ログを参照できます。

![20241202165233_1](https://github.com/user-attachments/assets/4ec6d9a3-742c-46e1-86f2-8381f26720bb)

#### シナリオプロローグ、エピローグのリプレイ

同一のセーブデータでは一度しか見ることのできない各話エピローグを再生可能にします。

![20241202164500](https://github.com/user-attachments/assets/b7884212-48fd-4112-a8d3-f7abf9e375ca)

#### クレジット画面に英語と日本語の歌詞を掲載

![20241202164717_1](https://github.com/user-attachments/assets/33234d31-e82b-4c12-971a-85bc795cc3c4)

OST: https://www.youtube.com/watch?v=gNUzb8ZDukg

##### English:

> I know the ways that you have made me stronger<br>
> Even if it's not the way you'd hoped it to be<br>
> 
> Walking on coals<br>
> The fire in your eyes<br>
> A step out of line<br>
> Is the only way of making our wrongs right<br>
>
> Rise up<br>
> 'cos I've had enough of these lies<br>
> (it's a sign, it's a sign of the times)<br>
> Rise up<br>
> With the rage and fear of a fight<br>
> (it's a sign, it's a sign of the times)<br>
>
> Uphold our name<br>
> From dawn to dusk<br>
> We're both the same<br>
> We're credulous<br>
> 
> Walking on coals<br>
> The fire in your eyes<br>
> A step out of line<br>
> Is the only way of making our wrongs right<br>
> 
> Rise up<br>
> 'cos I've had enough of these lies<br>
> (it's a sign, it's a sign of the times)<br>
> Rise up<br>
> With the rage and fear of a fight<br>
> (it's a sign, it's a sign of the times)<br>
> 
> (it's a sign, it's a sign of the times)

##### Japanese:

> あなたが望んでいなくても<br>
> あなたのくれた強さで<br>
> 
> 灼熱の道<br>
> 燃える瞳<br>
> 踏み出す者が<br>
> あやまちを正せるなら<br>
> 
> Rise up<br>
> もう嘘はいらない<br>
> (it's a sign, it's a sign of the times)<br>
> Rise up<br>
> 怖れと怒りを抱いて<br>
> (it's a sign, it's a sign of the times)<br>
>
> 誰もが<br>
> 黄昏<br>
> 誇りで<br>
> 染めてく<br>
> 
> 灼熱の道<br>
> 燃える瞳<br>
> 踏み出す者が<br>
> あやまちを正せるなら<br>
> 
> Rise up<br>
> もう嘘はいらない<br>
> (it's a sign, it's a sign of the times)<br>
> Rise up<br>
> 怖れと怒りを抱いて<br>
> (it's a sign, it's a sign of the times)<br>
> 
> (it's a sign, it's a sign of the times)

## Contact

- 稲葉(Inaba):
  - https://eps-r.net
  - https://x.com/eps_r
  - https://eps-r.hatenablog.com
  - https://docs.google.com/forms/d/e/1FAIpQLSeWbLshXTV8bs8tnRk3Wj-QSU79OA1MKzvHd-IlaSd1cUFzLg/viewform
