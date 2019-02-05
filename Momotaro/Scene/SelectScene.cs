using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Momotaro.Device;

namespace Momotaro.Scene
{
    class SelectScene : IScene
    {
        private bool isEndFlag; //終了フラグ
        private Sound sound; //サウンド　

        private List<Vector2> positions;　//画像位置
        private List<Rectangle> sourceRects; //切り取り位置
        private int cursor; //カーソル番号

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SelectScene()
        {
            isEndFlag = false;
            //サウンド取得
            var gameDevice = GameDevice.Instance();
            sound = gameDevice.GetSound();

            //画像位置の初期化
            positions = new List<Vector2>()
            {
                new Vector2(120,70),
                new Vector2(680,70),
                new Vector2(120,380),
                new Vector2(680,380),
            };
            //切り取り位置の初期化
            sourceRects = new List<Rectangle>()
            {
                new Rectangle(0,0,480,270),
                new Rectangle(480,0,480,270),
                new Rectangle(960,0,480,270),
                new Rectangle(1440,0,480,270),
            };
        }

        /// <summary>
        /// 描画
        /// </summary>
        /// <param name="renderer"></param>
        public void Draw(Renderer renderer)
        {
            renderer.Begin();
            for(int i=0; i<4;i++)
            {
                renderer.DrawTexture("stage", positions[i], sourceRects[i], Color.White);
            }
            renderer.DrawTexture("cursor", positions[cursor]);
            renderer.End();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize(Scene lastSceneName)
        {
            //シーン終了でないと設定
            isEndFlag = false;
            //カーソル位置の初期化
            cursor = 0;
        }

        /// <summary>
        /// 終了か？
        /// </summary>
        /// <returns></returns>
        public bool IsEnd()
        {
            return isEndFlag;
        }

        /// <summary>
        /// 次のシーンへ
        /// </summary>
        /// <returns></returns>
        public Scene Next()
        {
            return Scene.GamePlay;
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        public void Shutdown()
        {
            sound.StopBGM();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            //カーソル移動
            if (Input.GetKeyTrigger(Keys.Right) || Input.GetKeyTrigger(PlayerIndex.One,Buttons.DPadRight)
                || Input.GetKeyTrigger(PlayerIndex.One, Buttons.LeftThumbstickRight))
            {
                cursor += 1;
                if (cursor > 3)
                    cursor = 3;
            }
            if (Input.GetKeyTrigger(Keys.Left) || Input.GetKeyTrigger(PlayerIndex.One, Buttons.DPadLeft)
                || Input.GetKeyTrigger(PlayerIndex.One, Buttons.LeftThumbstickLeft))
            {
                cursor -= 1;
                if (cursor < 0)
                    cursor = 0;
            }

            //スペースキーが押されたら
            if (Input.GetKeyTrigger(Keys.Space) || Input.GetKeyTrigger(PlayerIndex.One,Buttons.B))
            {
                //シーン終了
                isEndFlag = true;
                //ステージ番号設定
                GameData.stageNum = cursor + 1;
            }
        }
    }
}
