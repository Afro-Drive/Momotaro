using Microsoft.Xna.Framework;
using Momotaro.Def;
using Momotaro.Device;
using Momotaro.Scene;
using Momotaro.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Momotaro.Actor
{
    class MonkeyAddBlock : GameObject
    {
        private bool isActive;

        public MonkeyAddBlock(Vector2 position, GameDevice gameDevice)
            : base("player", position, 64, 64, gameDevice)
        {
            isActive = true;

            //【追加】モーションを生成・登録
            motionDict = new Dictionary<string, Motion>()
            {
                {"idling", new Motion(new Range(0, 4), new CountDownTimer(0.25f)) },
            };
            for(int i = 0; i <= 4; i++)
            {
                motionDict["idling"].Add(i, new Rectangle(new Point(64 * i, 0), new Point(64)));
            }
            currentMotion = motionDict["idling"];
        }

        public MonkeyAddBlock(MonkeyAddBlock other)
            : this(other.position, other.gameDevice) { }

        public override void Change()
        {

        }

        public override object Clone()
        {
            return new MonkeyAddBlock(this);
        }

        public override void Draw(Renderer renderer)
        {
            if (!isDeadFlag)
                renderer.DrawTexture(
                    "saru_idlingL",
                    position + gameDevice.GetDisplayModify(),
                    currentMotion.DrawingRange(),
                    Color.White);
        }

        public override void Hit(GameObject gameObject)
        {

        }

        public override void HitChara(Character chara)
        {
            //当たったキャラがプレイヤーでなければ何もしない
            if (chara is Human == false && chara is Dog == false &&
                chara is Bird == false && chara is Monkey == false)
            {
                return;
            }
            //プレイアブルキャラクターに衝突したら消滅
            if (chara is Human == true || chara is Dog == true ||
                chara is Bird == true || chara is Monkey == true)
            {
                isDeadFlag = true;
            }

            //Active状態だったらpCountを３にする(Monkeyに切り替えられるようになる)
            if (isActive)
            {
                GameData.pCount = 4;
                isActive = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            currentMotion.Update(gameTime);
        }
    }
}
