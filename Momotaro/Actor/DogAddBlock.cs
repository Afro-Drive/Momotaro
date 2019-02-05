using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Def;
using Momotaro.Device;
using Momotaro.Scene;
using Momotaro.Util;

namespace Momotaro.Actor
{
    class DogAddBlock : GameObject
    {
        private bool isActive;

        public DogAddBlock(Vector2 position, GameDevice gameDevice)
            : base("player", position, 64, 64, gameDevice)
        {
            isActive = true;

            //【追加】モーションの生成・登録
            motionDict = new Dictionary<string, Motion>()
            {
                {"idling", new Motion(new Range(0, 4), new CountDownTimer(0.25f)) },
            };
            for (int i = 0; i <= 4; i++)
            {
                motionDict["idling"].Add(i, new Rectangle(new Point(64 * i, 0), new Point(64)));
            }
            currentMotion = motionDict["idling"];

        }

        public DogAddBlock(DogAddBlock other)
            : this(other.position, other.gameDevice) { }

        public override void Change()
        {

        }

        public override object Clone()
        {
            return new DogAddBlock(this);
        }

        public override void Hit(GameObject gameObject)
        {

        }

        /// <summary>
        /// アニメーション付きの描画
        /// </summary>
        /// <param name="renderer"></param>
        public override void Draw(Renderer renderer)
        {
            //死亡状態でなければ描画する
            if (!isDeadFlag)
                renderer.DrawTexture(
                    "inu_idlingL",
                    position + gameDevice.GetDisplayModify(),
                    currentMotion.DrawingRange(),
                    Color.White);
        }

        public override void HitChara(Character chara)
        {
            //当たったキャラがプレイヤーでなければ何もしない
            if (chara is Human == false && chara is Dog == false &&
                chara is Bird == false && chara is Monkey == false)
            {
                return;
            }
            //当たったキャラがプレイアブルキャラならブロック消滅
            else if (chara is Human == true || chara is Dog == true ||
                chara is Bird == true || chara is Monkey == true)
            {
                isDeadFlag = true;
            }

            //Active状態だったらpCountを２にする(Dogに切り替えられるようになる)
            if (isActive)
            {
                GameData.pCount = 2;
                isActive = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            currentMotion.Update(gameTime);
        }
    }
}
