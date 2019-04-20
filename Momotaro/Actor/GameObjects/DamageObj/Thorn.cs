using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Actor.Characters;
using Momotaro.Device;

namespace Momotaro.Actor.GameObjects.DamageObj
{
    class Thorn : GameObject
    {
        public Thorn(Vector2 position, GameDevice gameDevice)
            : base("thorn", position, 64, 64, gameDevice)
        {
        }

        public Thorn(Thorn other)
            :this(other.position,other.gameDevice)
        {

        }

        public override void Change()
        {
        }

        public override object Clone()
        {
            return new Thorn(this);
        }

        public override void Hit(GameObject gameObject)
        {
        }

        public override void HitChara(Character chara)
        {
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
