using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Actor.Characters;
using Momotaro.Device;
using Momotaro.Util;

namespace Momotaro.Actor.GameObjects.DamageObj
{
    class Flame : GameObject
    {
        private Timer timer; //出現管理のタイマー
        private float time; //出現時間

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="position"></param>
        /// <param name="gameDevice"></param>
        public Flame(Vector2 position, GameDevice gameDevice, float time) 
            : base("fire", position, 64, 128, gameDevice)
        {
            this.time = time;
            timer = new CountDownTimer(this.time);
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="other"></param>
        public Flame(Flame other)
            :this(other.Position,other.gameDevice, other.time)
        {

        }

        /// <summary>
        /// 切り替え
        /// </summary>
        public override void Change()
        {
        }

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new Flame(this);
        }

        /// <summary>
        /// オブジェクトとのヒット通知
        /// </summary>
        /// <param name="gameObject"></param>
        public override void Hit(GameObject gameObject)
        {
        }

        /// <summary>
        /// プレイヤーとのヒット通知
        /// </summary>
        /// <param name="chara"></param>
        public override void HitChara(Character chara)
        {
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            timer.Update(gameTime);
            if(timer.IsTime())
            {
                isDeadFlag = true;
                timer.Initialize();
            }
        }
    }
}
