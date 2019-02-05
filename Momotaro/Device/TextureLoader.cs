using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Momotaro.Device
{
    /// <summary>
    /// 画像ローダー
    /// </summary>
    class TextureLoader : Loader
    {
        private Renderer renderer; //描画オブジェクト

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="resources"></param>
        public TextureLoader(string[,] resources)
            : base(resources)
        {
            renderer = GameDevice.Instance().GetRenderer();
            base.Initialize();
        }

        /// <summary>
        /// 更新しながら読み込み
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            //まず終了フラグを有効にして
            isEndFlag = true;
            //カウンタが最大に達していないか？
            if (counter < maxNum)
            {
                //画像の読み込み
                renderer.LoadContent(resources[counter, 0], resources[counter, 1]);
                //カウンタを増やす
                counter += 1;
                //読み込むものがあったので終了フラグを継続に設定
                isEndFlag = false;
            }
        }
    }
}
