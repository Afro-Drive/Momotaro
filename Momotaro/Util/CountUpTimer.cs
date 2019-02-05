using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Momotaro.Util
{
    /// <summary>
    /// カウントアップ型タイマー
    /// </summary>
    class CountUpTimer : Timer
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CountUpTimer()
            : base()
        {
            Initialize();
        }

        public CountUpTimer(float second)
            : base(second)
        {
            Initialize();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public override void Initialize()
        {
            currentTime = 0.0f;
        }

        /// <summary>
        /// 指定時間になったか？
        /// </summary>
        /// <returns></returns>
        public override bool IsTime()
        {
            //制限時間を超えたら時間になっている
            return currentTime >= limitTime;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            //現在の時間を増やす。最大値は制限時間
            currentTime = Math.Min(currentTime + 1, limitTime);
        }

        /// <summary>
        /// 制限時間と開始の時間の割合
        /// </summary>
        /// <returns>はじめ0、制限時間で１</returns>
        public override float Rate()
        {
            return currentTime / limitTime;
        }
    }
}
