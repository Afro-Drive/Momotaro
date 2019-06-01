using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Actor.Characters;
using Momotaro.Actor.Characters.Player;
using Momotaro.Device;
using Momotaro.Scene;

namespace Momotaro.Actor.GameObjects.ItemObj
{
    class Item : GameObject
    {
        private IGameObjectMediator mediator;
        private int score;
        private Sound sound;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="position"></param>
        /// <param name="gameDevice"></param>
        /// <param name="mediator"></param>
        public Item(Vector2 position, GameDevice gameDevice, IGameObjectMediator mediator)
            : base ("dango" , position , 64 , 64, gameDevice ) 
        {
            this.mediator = mediator;
            score = 100;
            isDeadFlag = false;

            sound = GameDevice.Instance().GetSound();
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="other"></param>
        public Item(Item other)
            :this (other.Position,other.gameDevice,other.mediator)
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
            return new Item(this); 
        }

        /// <summary>
        /// オブジェクトとのヒット通知
        /// </summary>
        /// <param name="gameObject"></param>
        public override void Hit(GameObject gameObject)
        {
            return;
        }

        /// <summary>
        /// プレイヤーとのヒット通知
        /// </summary>
        /// <param name="chara"></param>
        public override void HitChara(Character chara)
        {
            if ((chara is Human ||
                chara is Dog ||
                chara is Bird ||
                chara is Monkey) &&
                isDeadFlag == false)
            {
                sound.PlaySE("dango");
                isDeadFlag = true;
                mediator.AddScore(score);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) 
        {
        }

        /// <summary>
        /// 描画
        /// </summary>
        /// <param name="renderer"></param>
        public override void Draw(Renderer renderer)
        {
            if(!isDeadFlag)
            {
                base.Draw(renderer);
            }
            
        }
    }
}
