using Microsoft.Xna.Framework;
using Momotaro.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Momotaro.Util
{
    class Score
    {
        private int poolScore;
        private int score;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Score()
        {
            Initialize();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            score = 0;
            poolScore = 0;
        }

        public void Add()
        {
            poolScore += 1;
        }

        public void Add(int num)
        {
            poolScore += num;
        }

        public void Draw(Renderer renderer)
        {
             //renderer.DrawNumber("number", new Vector2(580, 77), score);
        }

        public void Shutdown()
        {
            score += poolScore;
            if (score < 0)
            {

                score = 0;
            }
            poolScore = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (poolScore > 0)
            {
                score += 1;
                poolScore -= 1;
            }
            else if (poolScore < 0)
            {
                score -= 1;
                poolScore += 1;

            }
        }

        public int GetScore()
        {
            int currentScore = score + poolScore;
            if (currentScore < 0)
            {
                currentScore = 0;
            }

            return currentScore;
        }
    }
}
