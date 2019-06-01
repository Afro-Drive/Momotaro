using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Actor.Characters.Player;
using Momotaro.Actor.GameObjects;
using Momotaro.Def;
using Momotaro.Device;

namespace Momotaro.Actor.Characters
{
    class ShotEnemyBullet : Character
    {
        public ShotEnemyBullet(Vector2 position, Vector2 velocity,  GameDevice gameDevice)
            : base("shootObj_dot", position, 32, 32, 32, 32, gameDevice)
        {
            this.velocity = velocity;
        }

        public override void HitChara(Character character)
        {
            if(character is Human || character is Dog || character is Monkey || character is Bird)
            {
                isDeadFlag = true;
            }
        }

        public override void HitObj(GameObject obj)
        {
            if ((obj is Block) || ((obj is Door) && !((Door)obj).GetStatus()) || obj is DeathBlock)
            {
                isDeadFlag = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            Position += velocity;

            velocity.Y = velocity.Y + 0.4f;
            velocity.Y = (velocity.Y > 16.0f) ? (16) : (velocity.Y);
        }
    }
}
