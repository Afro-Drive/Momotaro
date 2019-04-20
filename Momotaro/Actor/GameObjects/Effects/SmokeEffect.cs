using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Actor.Characters;
using Momotaro.Device;
using Momotaro.Util;

namespace Momotaro.Actor.GameObjects.Effects
{
    class SmokeEffect : GameObject
    {
        private Motion motion;
        private Timer timer;

        public SmokeEffect(Vector2 position, GameDevice gameDevice)
            : base("smoke_effect", position, 64, 64, gameDevice)
        {
            this.position = position;

            motion = new Motion();
            for(int i=0; i<3; i++)
            {
                motion.Add(i, new Rectangle(i * 64, 0, 64, 64));
            }
            motion.Initialize(new Range(0, 2), new CountDownTimer(0.15f));

            timer = new CountDownTimer(0.45f);
        }

        public override void Change()
        {
        }

        public override object Clone()
        {
            return this;
        }

        public override void Hit(GameObject gameObject)
        {
        }

        public override void HitChara(Character chara)
        {
        }

        public override void Update(GameTime gameTime)
        {
            motion.Update(gameTime);
            timer.Update(gameTime);

            if(timer.IsTime())
            {
                isDeadFlag = true;
                timer.Initialize();
            }
        }

        public override void Draw(Renderer renderer)
        {
            renderer.DrawTexture(name, position + gameDevice.GetDisplayModify(), motion.DrawingRange(), Color.White);
        }
    }
}
