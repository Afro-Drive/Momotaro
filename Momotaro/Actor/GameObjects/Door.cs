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
    class Door : GameObject
    {
        private bool status;

        public Door(Vector2 position, GameDevice gameDevice)
            :base("door", position, 64, 64, gameDevice)
        {
            status = false;
            id = GameObjectID.Door;
        }

        public Door(Door other)
            : this(other.Position, other.gameDevice)
        {

        }

        public override object Clone()
        {
            return new Door(this);
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

        //操作
        public void Operation(bool status)
        {
            this.status = status;
        }

        public bool GetStatus()
        {
            return status;
        }

        public void Flip()
        {
            status = !status;
        }

        public override void Draw(Renderer renderer)
        {
            if (status)
            {
                return;
            }

            base.Draw(renderer);
        }

        public override void Change()
        {
        }
    }
}
