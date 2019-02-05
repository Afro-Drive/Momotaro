using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Momotaro.Device;
using Momotaro.Util;

namespace Momotaro.Scene
{
    class GameOver : IScene
    {
        private bool isEndFlag; //終了フラグ
        private Score score; //スコア
        private Timer timer; //タイマー
        private Sound sound;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="score"></param>
        /// <param name="timer"></param>
        public GameOver(Score score, Timer timer)
        {
            isEndFlag = false;
            this.score = score;
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
            renderer.DrawTexture("gameover", Vector2.Zero);
            renderer.DrawNumber("number", new Vector2(580,500), score.GetScore());
            renderer.DrawNumber("number", new Vector2(580, 600), timer.Now());
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
            GameData.stageNum = 0;
            score.Initialize();
            return Scene.Title;
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
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            sound.PlayBGM("bgm_over");
            if(Input.GetKeyTrigger(Keys.Space) || 
               Input.GetKeyTrigger(PlayerIndex.One, Buttons.Start) ||
               Input.GetKeyTrigger(PlayerIndex.One, Buttons.B))
            {
                isEndFlag = true;
            }
        }
    }
}
