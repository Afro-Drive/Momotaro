using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Device;
using Microsoft.Xna.Framework.Input;
using Momotaro.Util;

namespace Momotaro.Scene
{
    /// <summary>
    /// エンディングクラス
    /// </summary>
    class Ending : IScene
    {
        private bool isEndFlag;//終了フラグ
        private Timer timer;
        private Sound sound;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Ending(Timer timer)
        {
            isEndFlag = false;

            this.timer = timer;
            sound = GameDevice.Instance().GetSound();
        }

        /// <summary>
        /// 描画
        /// </summary>
        /// <param name="renderer"></param>
        public void Draw(Renderer renderer)
        {
            renderer.Begin();
            renderer.DrawTexture("clear", Vector2.Zero);
            renderer.DrawNumber("number", new Vector2(580,600), timer.Now());
            renderer.End();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize(Scene lastSceneName)
        {
            isEndFlag = false;
        }

        /// <summary>
        /// シーン終了か？
        /// </summary>
        /// <returns>シーン終了してたらtrue</returns>
        public bool IsEnd()
        {
            return isEndFlag;
        }

        /// <summary>
        /// 次のシーンへ
        /// </summary>
        /// <returns>次のシーン名</returns>
        public Scene Next()
        {
            GameData.stageNum += 1;
            //GameData.stageNum = 0;
            return Scene.GamePlay;
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        public void Shutdown()
        {
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="gameTime">ゲーム時間</param>
        public void Update(GameTime gameTime)
        {
            sound.PlayBGM("bgm_clear");

            if(Input.GetKeyTrigger(Keys.Space) || 
               Input.GetKeyTrigger(PlayerIndex.One, Buttons.Start) ||
               Input.GetKeyTrigger(PlayerIndex.One, Buttons.B))
            {
                isEndFlag = true;
            }
        }
    }
}
