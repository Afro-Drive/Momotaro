﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Actor.Characters;
using Momotaro.Actor.Characters.Player;
using Momotaro.Device;
using Momotaro.Scene;
using Momotaro.Util;

namespace Momotaro.Actor.GameObjects
{
    class MessageBox : GameObject
    {
        //フィールド
        private string fileName;//出力するテキストファイル
        private string message;//出力メッセージ内容
        private bool isMessage;//メッセージ出力状態
        private CountDownTimer timer;//出力時間

        private Motion motion;//モーション

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="position">初期位置</param>
        /// <param name="gameDevice">ゲームデバイス</param>
        /// <param name="fileName">出力したいテキストファイル名</param>
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
            : this(other.Position, other.gameDevice, other.fileName)
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

            message = File.ReadAllText("./Content/Text/" + fileName + ".txt");
            isMessage = true;
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
            renderer.DrawTexture(
                name, 
                Position + gameDevice.GetDisplayModify(), 
                motion.DrawingRange(), 
                Color.White);

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
                    Position + new Vector2(-96, -192) + gameDevice.GetDisplayModify(),
                    color);
            }
        }
    }
}
