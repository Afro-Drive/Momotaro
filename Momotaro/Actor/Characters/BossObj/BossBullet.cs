using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Actor.Characters.Player;
using Momotaro.Actor.GameObjects;
using Momotaro.Device;
using Momotaro.Scene;
using Momotaro.Util;

namespace Momotaro.Actor.Characters.BossObj
{
    /// <summary>
    /// ラスボス「鬼」が放つ気玉その２
    /// 作成者:任くん
    /// </summary>
    class BossBullet : Character 
    {
         //フィールド
        //private Vector2 velocity;
        private Vector2 playerPosition;　//ターゲットの位置情報
        private Timer time;
        private int dir = 0;
        private float Fdir;
        private int num = 0;

        private float angle;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="position"></param>
        /// <param name="playerPosition"></param>
        /// <param name="gameDevice"></param>
        public BossBullet ( Vector2  position , Vector2 playerPosition  , GameDevice gameDevice ) 
            : base ( "button2" ,  position , 32,32,32,32,gameDevice)
        {
            this.playerPosition = playerPosition;
            velocity = playerPosition - position;
            
            time = new CountDownTimer(2f);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="position">初期位置</param>
        /// <param name="gameDevice">ゲームデバイス</param>
        public BossBullet (Vector2 position , GameDevice gameDevice  )
            : base ("boss_particle" , position , 32 , 32,32, 32,gameDevice )
        {
            float angle; 
           
            //角度をランダムで決定
            angle = MathHelper.ToRadians(gameDevice.GetRandom().Next(-180, 180));
            //角度に応じた移動量を取得
            velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            //制限時間2秒のカウントダウンタイマーをセット
            time = new CountDownTimer(2f); 
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="position">初期位置</param>
        /// <param name="gameDevice"></param>
        /// <param name="dir">移動方法</param>
        /// <param name="Fdir">移動量の部分的な指定</param>
        public BossBullet (Vector2 position , GameDevice gameDevice , int  dir ,float Fdir )
            : base ("boss_particle", position , 32,32,32,32, gameDevice)
        {
            this.dir = dir;
            this.Fdir = Fdir;
            velocity = new Vector2(0, 8);
            time = new CountDownTimer(2f);
        }

        public BossBullet(Vector2 position, GameDevice gameDevice, float angle)
            : base("boss_particle", position, 32, 32, 32, 32, gameDevice)
        {
            dir = 3;
            time = new CountDownTimer(2f);

            this.angle = angle;
            this.position = position;
        }

        public override void Change()
        {
        }    

        public override void HitChara(Character character)
        {
            //プレイアブルキャラクターに衝突すると消滅
            if(character is Human || 
                character is Dog  || 
                character is Bird ||
                character is Monkey )
            {
                isDeadFlag = true; 
            }
         }

        public override void HitObj(GameObject obj)
        { 
            //ブロックにぶつかると消滅
             if(obj is  Block )
             {
                isDeadFlag = true; 
             }           
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            time.Update(gameTime);　//カウントダウンタイマーを起動

            if(time.IsTime())
            {
                //一定時間経過後消滅
                isDeadFlag = true;
            }
            if(dir ==0)
            {
                velocity.Normalize();
                position += velocity * 10;
            }
            else  if(dir ==1 )
            {
                velocity.X = Fdir;
                velocity.Y -= 0.5f; 
                velocity.Y = (velocity.Y < -16.0f) ? (-16) : (velocity.Y);
                position += velocity * 3;
            }
            else if(dir == 2 )
            { 
                velocity.X = Fdir;
                velocity.Y -= 0.5f;　//徐々に下方向への移動量を加速
                velocity.Y = (velocity.Y < -16.0f) ? (-16) : (velocity.Y);
                position += velocity * 3;
            }
            else if (dir == 3)
            {
                num += 5;
                float angle = MathHelper.ToRadians(num);
                velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                velocity.Normalize();
                position += velocity * 10;
            }
        }

    }
}
