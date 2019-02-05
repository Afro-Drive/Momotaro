using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Device;
using Momotaro.Scene;

namespace Momotaro.Actor
{
    /// <summary>
    /// もも
    /// </summary>
    class Peach : GameObject
    {
        private IGameObjectMediator mediator;

        private Sound sound;

        public Peach( Vector2 position,GameDevice gameDevice, IGameObjectMediator mediator)
            : base("momo", position, 64, 64, gameDevice)
        {
            this.mediator = mediator;

            sound = GameDevice.Instance().GetSound();
        }

        public Peach(Peach other)
            : this(other.position, other.gameDevice, other.mediator) { }

        public override void Change()
        {
            
        }

        public override object Clone()
        {
            return new Peach(this);
        }

        public override void Hit(GameObject gameObject)
        {
            
        }

        public override void HitChara(Character chara)
        {
            if ((chara is Human ||
                chara is Dog ||
                chara is Bird ||
                chara is Monkey) &&
                isDeadFlag == false)
            {
                sound.PlaySE("momo");
                isDeadFlag = true;
                mediator.ChangeHp(1);
            }
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(Renderer renderer)
        {
            if (!isDeadFlag)
            {
                base.Draw(renderer);
            }

        }
    }
}
