using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Actor.Characters;
using Momotaro.Device;
using Momotaro.Scene;
using Momotaro.Util;

namespace Momotaro.Actor.GameObjects.DamageObj
{
    class Trap : GameObject
    {
        private IGameObjectMediator mediator;
        private Timer timer; //間隔用タイマー
        //private float burnTime; //表示時間
        //private int burns; //高さ
        //private float burnInterval; //間隔

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="position"></param>
        /// <param name="gameDevice"></param>
        public Trap(Vector2 position, GameDevice gameDevice,IGameObjectMediator mediator)
            : base("tubo", position, 64, 64, gameDevice)
        {
            this.mediator = mediator;

            //this.burnInterval = burnInterval;
            timer = new CountDownTimer(4.0f);

            //this.burns = burns;
            //this.burnTime = burnTime;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="other"></param>
        public Trap(Trap other)
            :this (other.Position, other.gameDevice, other.mediator)
        {

        }


        public override void Change()
        {
        }

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new Trap(this);
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
                
                mediator.AddGameObject(new Flame(Position + new Vector2(0, -128), gameDevice, 1.0f));
                
                timer.Initialize();
            }
        }
    }
}
