using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Device;
using Momotaro.Scene;
using Momotaro.Util;

namespace Momotaro.Actor
{
    class MessageBox : GameObject
    {
        private string fileName;
        private string message;
        private bool isMessage;
        private CountDownTimer timer;

        private Motion motion;

        public MessageBox(Vector2 position, GameDevice gameDevice, string fileName)
            : base("owl_motion", position, 64, 64, gameDevice)
        {
            this.fileName = fileName;
            isMessage = false;
            timer = new CountDownTimer(5);

            motion = new Motion();
            for (int i = 0; i < 5; i++)
            {
                motion.Add(i, new Rectangle(i * 64, 0, 64, 64));
            }
            motion.Initialize(new Range(0, 4), new CountDownTimer(0.20f));
        }

        public MessageBox(MessageBox other)
            : this(other.position, other.gameDevice, other.fileName)
        {

        }

        public override void Change()
        {

        }

        public override object Clone()
        {
            return new MessageBox(this);
        }

        public override void Hit(GameObject gameObject)
        {

        }

        public override void HitChara(Character chara)
        {
            if (chara is Human == false && chara is Dog == false &&
                chara is Bird == false && chara is Monkey == false)
            {
                return;
            }

            //if(CheckDirection(chara) == Direction.Bottom)
            //{
            message = File.ReadAllText(fileName + ".txt");
            isMessage = true;
            //}
        }

        public override void Update(GameTime gameTime)
        {
            motion.Update(gameTime);

            if (isMessage)
            {
                timer.Update(gameTime);
            }
            if (timer.IsTime())
            {
                isMessage = false;
                timer.Initialize();
            }
        }

        public override void Draw(Renderer renderer)
        {
            renderer.DrawTexture(name, position + gameDevice.GetDisplayModify(), motion.DrawingRange(), Color.White);

            if (isMessage)
            {
                //【変更】ステージ04は背景が黒いため、文字の色を変える
                Color color;
                if (GameData.stageNum != 4)
                    color = Color.Black;
                else
                    color = Color.White;

                renderer.DrawString(
                    message,
                    position + new Vector2(-96, -192) + gameDevice.GetDisplayModify(),
                    color);
            }
        }
    }
}
