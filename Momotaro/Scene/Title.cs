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
    class Title : IScene
    {
        private bool isEndFlag;
        private Sound sound;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Title()
        {
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
            renderer.DrawTexture("title", Vector2.Zero);
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
        /// シーンが終了しているか？
        /// </summary>
        /// <returns>終了していたらtrue</returns>
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
            //GameData.stageNum = 5;
            return Scene.SelectScene;
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
            sound.PlayBGM("bgm_title");
            if(Input.GetKeyTrigger(Keys.Space) || Input.GetKeyTrigger(PlayerIndex.One,Buttons.Start))
            {
                isEndFlag = true;
            }
        }
    }
}
