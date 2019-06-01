using Microsoft.Xna.Framework;
using Momotaro.Actor.Characters;
using Momotaro.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Momotaro.Actor.GameObjects
{
    class DeathBlock : GameObject
    {
        public DeathBlock(Vector2 position, GameDevice gameDevice)
            : base("", position, 64, 64, gameDevice)
        {
        }

        public DeathBlock(DeathBlock other)
            : this(other.Position, other.gameDevice)
        {

        }

        public override object Clone()
        {
            return new DeathBlock(this);
        }

        /// <summary>
        /// 【追加】描画処理(何もしない)
        /// </summary>
        /// <param name="renderer"></param>
        public override void Draw(Renderer renderer)
        {
            
        }

        public override void Hit(GameObject gameObject)
        {

        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Change()
        {

        }

        public override void HitChara(Character chara)
        {
            
        }
    }
}
