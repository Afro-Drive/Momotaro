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
    class TrueEnding : IScene
    {
        private bool isEndFlag; //終了フラグ
        private Score score; //スコア
        private Timer timer; //タイマー
        private Sound sound;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TrueEnding(Score score,Timer timer)
        {
            this.score = score;            
            this.timer = timer;
            isEndFlag = false;

            sound = GameDevice.Instance().GetSound();
        }

        /// <summary>
        /// 描画
        /// </summary>
        /// <param name="renderer"></param>
        public void Draw(Renderer renderer)
        {
            renderer.Begin();
            renderer.DrawTexture("trueEnding", Vector2.Zero);
            //renderer.DrawTexture("player", Vector2.Zero);
            renderer.DrawNumber("number", new Vector2(580, 500), score.GetScore());
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
        /// シーン終了か？
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
            GameData.stageNum = 0; //ステージ番号の初期化
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
            sound.PlayBGM("bgm_clear");

            if(Input.GetKeyTrigger(Keys.Space) || Input.GetKeyTrigger(PlayerIndex.One, Buttons.Start))
            {
                isEndFlag = true;
            }
        }
    }
}
