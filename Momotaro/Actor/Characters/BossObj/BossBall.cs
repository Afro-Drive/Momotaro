using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Actor.GameObjects;
using Momotaro.Device;
using Momotaro.Scene;
using Momotaro.Util;

namespace Momotaro.Actor.Characters.BossObj
{
    /// <summary>
    /// ラスボス「鬼」の放つ気玉その１
    /// 作成者:任くん
    /// </summary>
    class BossBall : Character
    {
        //フィールド
        private int num = 0;　//気玉の射出角度
        private IGameObjectMediator mediator; //仲介者
        private Vector2 playerPos;　//ターゲットの位置情報
        private Timer time;
        private int state;
        private Vector2 playerV;　//ターゲットの位置情報

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="position">初期位置</param>
        /// <param name="gameDevice">ゲームデバイス</param>
        /// <param name="dir"></param>
        /// <param name="mediator">仲介者</param>
        public BossBall (Vector2 position , GameDevice gameDevice , int dir ,IGameObjectMediator mediator) :
            base ("boss_particle",  position , 32,32,32,32 ,gameDevice )
        {
            state = 1;
            //制限時間10秒のカウントダウンタイマーを生成
            time = new CountDownTimer(10f);

            this.mediator = mediator; 

            //方向に応じて色・角度を設定
            if(dir == 0 )
            {
                //赤色にする
                color = Color.Red;
                num = 0;
            }
            else if(dir ==1)
            {
                //青色にする
                color = Color.Blue; 
                num = 90;
            }
            else if(dir ==2 )
            {
                //緑色
                color = Color.Yellow;
                num = 180; 
            }
            else if (dir ==3 )
            {
                //紫色
                color = Color.Purple; 
                num = 270; 
            }
        }
        public override void Change()
        {
        }

        public override void HitChara(Character character)
        {
            //プレイアブルキャラクターに衝突しても消えない！
        }

        public override void HitObj(GameObject obj)
        {
            if (obj is Block)
            {
                //ブロックに当たったら消滅
                isDeadFlag = true;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            //カウントダウンタイマーを起動
            time.Update(gameTime);
            if (!time.IsTime())　//まだ時間が残っている
            {
                //ラスボス「鬼」の現在位置を特定し、それを自分の座標にする
                position = mediator.GetBoss().GetPosition();
                //ターゲットの現在位置を取得
                playerPos = mediator.GetPlayer().GetPosition();
                //自分とターゲットとの座標の差を計算
                playerV = playerPos - position;

                var velocity = Vector2.Zero;
                num += 3;　//角度を増やしていく
                float angle = MathHelper.ToRadians(num);　//角度を弧度法に変換
                //角度に応じた移動量を算出(三角関数を用いる)
                velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                velocity.Normalize();　//角度で指定された移動方向のみを取得
                position += velocity * 100;　//その方向に向かって一気に進む(角度が動的に変わるため軌道も変化していく)
            }
            else　//時間切れになった
            {
                playerV.Normalize();　//ターゲットへの方向のみ取得
                position += playerV * 8;　//ターゲットに向かって進む

                var velocity = Vector2.Zero;
                num += 5;　//角度を変えていく
                float angle = MathHelper.ToRadians(num);　//角度を弧度法に変換
                //角度に応じた移動量を算出（三角関数を用いる）
                velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                velocity.Normalize();　//角度で指定された移動方向のみ取得
                position += velocity * 6;　//角度に応じて進む
            }
        }
    }
}
