using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Momotaro.Def;
using Momotaro.Device;

namespace Momotaro.Scene
{
    class Pause : IScene
    {
        private IScene lastScene;
        private bool isEndFlag;
        private Scene nextScene;
        private float alpha;
        private float alphaCnt;

        public Pause()
        {
            alpha = 0;
            alphaCnt = 0.01f;
        }

        public void Initialize(Scene lastSceneName)
        {
            isEndFlag = false;
            nextScene = lastSceneName;
        }

        public void Draw(Renderer renderer)
        {
            lastScene.Draw(renderer);

            renderer.Begin();
            //renderer.DrawString("Pause", new Vector2(64, 64), Color.Blue, alpha);
            renderer.DrawTexture(
                "pausebg", 
                new Vector2((Screen.WIDTH - 1024)/2, (Screen.HEIGHT - 576)/2));
            renderer.End();
        }

        public bool IsEnd()
        {
            return isEndFlag;
        }

        public Scene Next()
        {
            return nextScene;
        }

        public void Shutdown()
        {

        }

        public void Update(GameTime gameTime)
        {
            alpha += alphaCnt;
            if(alpha >= 1 || alpha <= 0)
            {
                alphaCnt = -alphaCnt;
            }

            if (Input.GetKeyTrigger(Keys.P) || Input.GetKeyTrigger(PlayerIndex.One,Buttons.Start))
            {
                isEndFlag = true;
            }
            if (Input.GetKeyTrigger(Keys.Delete) || Input.GetKeyTrigger(PlayerIndex.One,Buttons.X))
            {
                nextScene = Scene.SelectScene;
                isEndFlag = true;
            }
        }

        public void SetLastScene(IScene lastScene)
        {
            this.lastScene = lastScene;
        }
    }
}
