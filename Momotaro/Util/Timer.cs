using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Momotaro.Util
{
    /// <summary>
    /// 抽象タイマークラス
    /// </summary>
    abstract class Timer
    {
        protected float limitTime; //制限時間
        protected float currentTime;//現在の時間

        /// <summary>
        /// /コンストラクタ
        /// </summary>
        /// <param name="second">制限時間</param>
        public Timer(float second)
        {
            limitTime = 60 * second;//60fps×秒
        }

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public Timer()
            : this(1) //1秒
        { }

        //抽象メソッド
        public abstract void Initialize();//初期化

        public abstract void Update(GameTime gameTime);//更新

        public abstract bool IsTime();//指定時間になったか？

        /// <summary>
        /// 制限時間を設定
        /// </summary>
        /// <param name="second"></param>
        public void SetTime(float second)
        {
            limitTime = 60 * second;
        }

        /// <summary>
        /// 現在時間の取得
        /// </summary>
        /// <returns>秒</returns>
        public float Now()
        {
            return currentTime / 60f;//60fps想定なので60で割る
        }

        /// <summary>
        /// 制限時間と開始の時間の割合
        /// </summary>
        /// <returns></returns>
        public abstract float Rate();
    }
}
