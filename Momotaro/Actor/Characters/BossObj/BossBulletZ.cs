using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Actor.Characters.Player;
using Momotaro.Actor.GameObjects;
using Momotaro.Device;
using Momotaro.Util;

namespace Momotaro.Actor.Characters.BossObj
{
    /// <summary>
    /// ラスボス「鬼」が放つ気玉その３
    /// 作成者:任くん
    /// </summary>
    class BossBulletZ : Character
    {
        //フィールド
        private Timer time;
        //private Vector2 velocity;
        private Vector2 playerPosition;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="position">初期位置</param>
        /// <param name="playerPosition">ターゲットの位置情報</param>
        /// <param name="gameDevice">ゲームデバイス</param>
        public BossBulletZ(Vector2 position, Vector2 playerPosition, GameDevice gameDevice)
            : base("boss_particle", position, 32, 32, 32, 32, gameDevice)
        {
            this.playerPosition = playerPosition;
            //ターゲットの位置情報と自分の位置との差を移動量にする
            velocity = playerPosition - position;

            //カウントダウンタイマーを2秒で初期化
            time = new CountDownTimer(2f);
            color = Color.Red;
        }

        public override void HitChara(Character character)
        {
            //プレイアブルキャラクターに衝突したら消滅
            if (character is Human || character is Dog ||
                character is Bird || character is Monkey)
            {
                isDeadFlag = true;
            }
        }

        public override void HitObj(GameObject obj)
        {
            //ブロックに衝突したら消滅
            if (obj is Block)
            {
                isDeadFlag = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            //カウントダウンタイマーを起動
            time.Update(gameTime);

            if (time.IsTime())　//時間切れになったら消滅
            {
                isDeadFlag = true;
            }
            //ターゲットへの方向のみを取得
            velocity.Normalize();
            //ターゲットに向かって進む
            position += velocity * 15;
        }
    }
}
