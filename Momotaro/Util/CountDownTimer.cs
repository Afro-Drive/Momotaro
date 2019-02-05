using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Momotaro.Util
{
    /// <summary>
    /// カウントダウン型タイマー
    /// </summary>
    class CountDownTimer : Timer
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CountDownTimer()
            : base()
        {
            //自分の初期化メソッドで初期化
            Initialize();
        }

        public CountDownTimer(float second)
            : base(second)
        {
            Initialize();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public override void Initialize()
        {
            currentTime = limitTime;
        }

        /// <summary>
        /// 制限時間になったか？
        /// </summary>
        /// <returns></returns>
        public override bool IsTime()
        {
            return currentTime <= 0.0f;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            //現在の時間を減らす。ただし最小値は0.0
            currentTime = Math.Max(currentTime - 1f, 0.0f);
        }

        /// <summary>
        /// 制限時間と開始の時間の割合
        /// </summary>
        /// <returns>はじめ0、制限時間で１</returns>
        public override float Rate()
        {
            return 1.0f - currentTime / limitTime;
        }
    }
}
