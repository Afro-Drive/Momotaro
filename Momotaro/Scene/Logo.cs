using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Momotaro.Def;
using Momotaro.Device;

namespace Momotaro.Scene
{
    class Logo : IScene
    {
        private bool isEndFlag;
        private enum State
        {
            In,
            Stay,
            Out,
        }
        private State state;
        private float alpha;

        public Logo()
        {
            isEndFlag = false;
        }

        public void Draw(Renderer renderer)
        {
            renderer.Begin();
            renderer.DrawTexture("logo", Vector2.Zero, alpha);
            renderer.End();
        }

        public void Initialize(Scene lastSceneName)
        {
            isEndFlag = false;
            state = State.In;
            alpha = 0;
        }

        public bool IsEnd()
        {
            return isEndFlag;
        }

        public Scene Next()
        {
            return Scene.Title;
        }

        public void Shutdown()
        {

        }

        public void Update(GameTime gameTime)
        {
            switch (state)
            {
                case State.In:
                    alpha += 0.01f;
                    if (alpha >= 1)
                    {
                        state = State.Stay;
                    }
                    break;
                case State.Stay:
                    if (Input.GetKeyTrigger(Keys.Space) || 
                        Input.GetKeyTrigger(PlayerIndex.One,Buttons.Start) ||
                        Input.GetKeyTrigger(PlayerIndex.One, Buttons.B))
                    {
                        state = State.Out;
                    }
                    break;
                case State.Out:
                    alpha -= 0.01f;
                    if(alpha <= 0)
                    {
                        isEndFlag = true;
                    }
                    break;
            }
        }
    }
}
