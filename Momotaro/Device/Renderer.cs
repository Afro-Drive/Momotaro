using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;//Assert用

namespace Momotaro.Device
{
    /// <summary>
    /// 描画管理クラス
    /// </summary>
    class Renderer
    {
        #region フィールド
        private ContentManager contentManager; //コンテンツ管理者
        private GraphicsDevice graphicsDevice; //グラフィック機器
        private SpriteBatch spriteBatch; //スプライト一括描画用オブジェクト
        private RenderTarget2D target2D; //2D用レンダーターゲット

        //複数画像管理用変数の宣言と生成
        private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        private SpriteFont font;
        #endregion フィールド

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="content">Game1クラスのコンテンツ管理者</param>
        /// <param name="graphics">Game1クラスのグラフィック機器</param>
        public Renderer(ContentManager content, GraphicsDevice graphics)
        {
            contentManager = content;
            graphicsDevice = graphics;
            spriteBatch = new SpriteBatch(graphicsDevice);
            // フォントのロード
            font = content.Load<SpriteFont>("Font/font");
        }

        #region 画像の読み込み
        /// <summary>
        /// 画像の読み込み
        /// </summary>
        /// <param name="assetName">アセット名（ファイルの名前）</param>
        /// <param name="filepath">画像へのファイルパス</param>
        public void LoadContent(string assetName, string filepath = "./")
        {
            //すでにキー（assetName：アセット名）が登録されているとき
            if (textures.ContainsKey(assetName))
            {
#if DEBUG //DEBUGモードの時のみ下記エラー分をコンソールへ表示
                Console.WriteLine(assetName + "はすでに読み込まれています。\n プログラムを確認してください。");
#endif

                //それ以上読み込まないのでここで終了
                return;
            }
            //画像の読み込みとDictionaryへアセット名と画像を登録
            textures.Add(assetName, contentManager.Load<Texture2D>(filepath + assetName));

        }

        /// <summary>
        /// 画像の読み込み（画像オブジェクト版）
        /// </summary>
        /// <param name="assetName">アセット名</param>
        /// <param name="texture">２D画像オブジェクト</param>
        public void LoadContent(string assetName, Texture2D texture)
        {
            //すでにキー（assetName：アセット名）が登録されているとき
            if (textures.ContainsKey(assetName))
            {
#if DEBUG //DEBUGモードの時のみ下記エラー分をコンソールへ表示
                Console.WriteLine(
                    assetName +
                    "はすでに読み込まれています。\n" +
                    " プログラムを確認してください。");
#endif

                //それ以上読み込まないのでここで終了
                return;
            }

            textures.Add(assetName, texture);
        }
        #endregion 画像の読み込み

        /// <summary>
        /// アンロード
        /// </summary>
        public void Unload()
        {
            textures.Clear();//Dictionaryの情報をクリア
        }

        #region 描画開始・終了関連
        /// <summary>
        /// 描画開始
        /// </summary>
        public void Begin()
        {
            spriteBatch.Begin();
        }

        /// <summary>
        /// 描画開始
        /// </summary>
        /// <param name="sortMode">ソートモード</param>
        /// <param name="blendState">合成状態</param>
        public void Begin(SpriteSortMode sortMode, BlendState blendState)
        {
            spriteBatch.Begin(sortMode, blendState);
        }

        /// <summary>
        /// 描画終了
        /// </summary>
        public void End()
        {
            spriteBatch.End();
        }
        #endregion  描画開始・終了関連


        #region 画像の描画関連
        /// <summary>
        /// 画像の描画（画像サイズはそのまま）
        /// </summary>
        /// <param name="assetName">アセット名</param>
        /// <param name="position">位置</param>
        /// <param name="alpha">透明値（1.0f：不透明 0.0f：透明）</param>
        public void DrawTexture(string assetName, Vector2 position, float alpha = 1.0f)
        {
            //デバッグモードの時のみ、画像描画前のアセット名チェック
            Debug.Assert(
                textures.ContainsKey(assetName),
                "描画時にアセット名の指定を間違えたか、" +
                "画像の読み込み自体できていません");

            spriteBatch.Draw(textures[assetName], position, Color.White * alpha);
        }

        /// <summary>
        /// 画像の描画（画像を指定範囲内だけ描画）
        /// </summary>
        /// <param name="assetName">アセット名</param>
        /// <param name="position">位置</param>
        /// <param name="rect">指定範囲</param>
        /// <param name="alpha">透明値</param>
        public void DrawTexture(string assetName, Vector2 position, Rectangle rect, Color color, float alpha = 1.0f)
        {
            //デバッグモードの時のみ、画像描画前のアセット名チェック
            Debug.Assert(
                textures.ContainsKey(assetName),
                "描画時にアセット名の指定を間違えたか、" +
                "画像の読み込み自体できていません");

            spriteBatch.Draw(
                textures[assetName], //テクスチャ
                position,            //位置
                rect,                //指定範囲（矩形で指定：左上の座標、幅、高さ）
                color * alpha);//透明値
        }


        /// <summary>
        /// 画像の描画
        /// </summary>
        /// <param name="assetName">アセット名</param>
        /// <param name="positoin">位置</param>
        /// <param name="rect">切り出し範囲</param>
        /// <param name="rotate">回転角度</param>
        /// <param name="rotatePosition">回転軸位置</param>
        /// <param name="scale">拡大縮小</param>
        /// <param name="effects">表示反転効果</param>
        /// <param name="depth">スプライト深度</param>
        /// <param name="alpha">透明値</param>
        public void DrawTexture(
            string assetName,
            Vector2 positoin,
            Rectangle? rect, //nullを受け入れられるよう「？」で
            float rotate,
            Vector2 rotatePosition,
            Vector2 scale,
            SpriteEffects effects = SpriteEffects.None,
            float depth = 0.0f,
            float alpha = 1.0f)
        {
            spriteBatch.Draw(
                textures[assetName],//テクスチャ
                positoin,           //位置
                rect,               //切り取り範囲
                Color.White * alpha,//透明値
                rotate,             //回転角度
                rotatePosition,     //回転軸
                scale,              //拡大縮小
                effects,            //表示反転効果
                depth               //スプライト深度
                );
        }


        /// <summary>
        /// 画像の描画
        /// </summary>
        /// <param name="assetName">アセット名</param>
        /// <param name="position">位置</param>
        /// <param name="color">色（通常はColor.White）</param>
        /// <param name="alpha">透明値</param>
        public void DrawTexture(string assetName, Vector2 position, Color color, float alpha = 1.0f)
        {
            //デバッグモードの時のみ、画像描画前のアセット名チェック
            Debug.Assert(
                textures.ContainsKey(assetName),
                "描画時にアセット名の指定を間違えたか、" +
                "画像の読み込み自体できていません");

            spriteBatch.Draw(textures[assetName], position, color * alpha);
        }

        /// <summary>
        /// 画像を指定方向に反転させて描画
        /// </summary>
        /// <param name="assetName">アセット名</param>
        /// <param name="position">描画位置</param>
        /// <param name="flipAxis">反転させる際の対称軸(X,Y軸)</param>
        /// <param name="alpha">透明度（デフォルト：不透明）</param>
        /// <param name="depth">スプライト深度（デフォルト：１）</param>
        public void DrawAntiAxisTexture(string assetName, Vector2 position, SpriteEffects flipAxis, float alpha = 1.0f, float depth = 1.0f)
        {
            //デバッグモードの時のみ、画像描画前のアセット名チェック
            Debug.Assert(
                textures.ContainsKey(assetName),
                "描画時にアセット名の指定を間違えたか、" +
                "画像の読み込み自体できていません");

            spriteBatch.Draw(
                textures[assetName],
                position,
                null,
                Color.White * alpha,
                0f,
                new Vector2(position.X + 32, position.Y + 32), //反転時の基準点（画像の中心）
                1.0f,
                flipAxis, //引数で指定された方向に反転させる
                depth);
        }

        /// <summary>
        /// 画像の切り取り範囲&反転を指定した描画
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="position"></param>
        /// <param name="sourceRect"></param>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="flipAxis"></param>
        /// <param name="alpha"></param>
        /// <param name="rotation"></param>
        /// <param name="depth"></param>
        public void DrawAntiAxisTexture(string assetName, Vector2 position, Rectangle? sourceRect, Vector2 origin, float scale, SpriteEffects flipAxis, Color color, float alpha = 1.0f, float rotation = 0f, float depth = 1.0f)
        {
            //デバッグモードの時のみ、画像描画前のアセット名チェック
            Debug.Assert(
                textures.ContainsKey(assetName),
                "描画時にアセット名の指定を間違えたか、" +
                "画像の読み込み自体できていません");

             spriteBatch.Draw(
                 textures[assetName],
                 position, 
                 sourceRect, 
                 color * alpha, 
                 rotation, 
                 origin, 
                 scale,
                 flipAxis, 
                 depth);
        }
        #endregion  画像の描画


        #region 数字の描画
        /// <summary>
        /// 数字の描画（整数のみ）
        /// </summary>
        /// <param name="assetName">数字画像の名前</param>
        /// <param name="position">位置</param>
        /// <param name="number">表示したい整数値</param>
        /// <param name="alpha">透明値</param>
        public void DrawNumber(
            string assetName,
            Vector2 position,
            int number,
            float alpha = 1.0f)
        {
            //デバッグモードの時のみ、画像描画前のアセット名チェック
            Debug.Assert(
                textures.ContainsKey(assetName),
                "描画時にアセット名の指定を間違えたか、" +
                "画像の読み込み自体できていません");

            //マイナスの数は0
            if (number < 0)
            {
                number = 0;
            }

            int width = 32;//画像横幅

            //数字を文字列化し、1文字ずつ取り出す
            foreach (var n in number.ToString())
            {
                //数字のテクスチャが数字1つにつき幅32高さ64
                //文字と文字を引き算し、整数値を取得している
                spriteBatch.Draw(
                    textures[assetName],
                    position,
                    new Rectangle((n - '0') * width, 0, width, 64),
                    Color.White);

                //1文字描画したら1桁分右にずらす
                position.X += width;
            }
        }

        /// <summary>
        /// 数字の描画（実数、小数点以下は2桁表示）
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="position"></param>
        /// <param name="number"></param>
        /// <param name="alpha"></param>
        public void DrawNumber(
            string assetName,
            Vector2 position,
            float number,
            float alpha = 1.0f)
        {
            //マイナスは０へ
            if (number < 0.0f)
            {
                number = 0.0f;
            }

            int width = 32;
            //小数部は2桁まで、整数部が1桁の時は0で埋める
            foreach (var n in number.ToString("00.00"))
            {
                //小数の「.」か？
                if (n == '.')
                {
                    spriteBatch.Draw(
                        textures[assetName],
                        position,
                        new Rectangle(10 * width, 0, width, 64),//ピリオドは10番目
                        Color.White * alpha);
                }
                else
                {
                    //数字の描画
                    spriteBatch.Draw(
                        textures[assetName],
                        position,
                        new Rectangle((n - '0') * width, 0, width, 64),
                        Color.White * alpha);
                }

                //1文字描画したら1桁分右にずらす
                position.X += width;
            }
        }
        #endregion 数字の描画

        #region レンダーターゲット関連
        /// <summary>
        /// レンダーターゲットの初期化
        /// テクスチャサイズの設定
        /// </summary>
        /// <param name="width">テクスチャ横幅</param>
        /// <param name="height">テクスチャ縦幅</param>
        public void InitializeRenderTarget(int width, int height)
        {
            target2D = new RenderTarget2D(graphicsDevice, width, height);
        }

        /// <summary>
        /// レンダーターゲット先への描画開始
        /// </summary>
        public void BeginRenderTarget()
        {
            //描画するレンダーターゲットを設定
            graphicsDevice.SetRenderTarget(target2D);
            //描画先のレンダーターゲットの内容を消す
            graphicsDevice.Clear(Color.Transparent);
            //レンダーターゲットへの描画開始
            spriteBatch.Begin();
        }

        /// <summary>
        /// レンダーターゲットへの描画終了
        /// </summary>
        public void EndRenderTarget()
        {
            //レンダーターゲットへの描画終了
            spriteBatch.End();
            //描画を画面に変える
            graphicsDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// レンダーターゲットのテクスチャを描画
        /// </summary>
        /// <param name="positoin"></param>
        /// <param name="rect"></param>
        /// <param name="rotate"></param>
        /// <param name="rotatePosition"></param>
        /// <param name="scale"></param>
        /// <param name="color"></param>
        /// <param name="effects"></param>
        /// <param name="depth"></param>
        public void DrawRenderTargetTexture(
            Vector2 positoin,
            Rectangle? rect, //nullを受け入れられるよう「？」で
            float rotate,
            Vector2 rotatePosition,
            float scale,
            Color color,
            SpriteEffects effects = SpriteEffects.None,
            float depth = 0.0f
            )
        {
            spriteBatch.Draw(
                target2D,
                positoin,
                rect,
                color,
                rotate,
                rotatePosition,
                scale,
                effects,
                depth);
        }
        #endregion レンダーターゲット関連

        /// <summary>
        /// 文字描画
        /// </summary>
        /// <param name="str">表示する文字列</param>
        /// <param name="position">位置</param>
        /// <param name="color">色</param>
        public void DrawString(string str, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, str, position, color);
        }


        public void DrawString(string str, Vector2 position, Color color, float alpha = 1.0f)
        {
            spriteBatch.DrawString(font, str, position, color * alpha);
        }
    }
}
