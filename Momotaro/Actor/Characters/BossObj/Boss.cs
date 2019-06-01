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
    /// ラスボス「鬼」クラス
    /// 作成者:任くん
    /// </summary>
    class Boss : Character
    {
        //フィールド
        private IGameObjectMediator mediator;　//仲介者(自分とPlayerとの距離を計測する際に使用)
        //private Vector2 velocity;
        private int hp = 400;　//体力
        private Timer time;　//タイマー
        private BossState currentState;　//ボスの戦闘状態
        private int stateNum;　//同一攻撃内での挙動の段階（ある程度加算されると戦闘態勢が遷移する）
        private Vector2 playerPos;　//チェイスターゲットの現在座標
        private int attackNum = 0;　//攻撃回数
        private int bulletNum = 6; // 玉個数

        private Timer CD;
        private int vertigo;　//目まい用の整数型？
        private bool isVertigo;　//目まい状態かってこと？
        private float vertigoTime;　//目まいのタイマー
        private float angle;　//気玉を放つ角度

        private string motionAssetWord = "idling"; //鬼の状態に合わせた文字列(描画モーション名の決定に使用)
        private Motion effectMotion; //追加

        /// <summary>
        /// ボスの戦闘態勢の列挙型
        /// </summary>
        private enum BossState
        {
            RunAway,　//プレイヤーターゲットからの逃走
            SprintAttack, 　//気玉の急上昇放出
            JumpAttack, 　//高速突進
            ShotAttack,　//ターゲットに向けて気玉を1発放つ
            CircleAttack,　//気玉の弾幕放出
            Wait, 　//待機
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="position">初期座標</param>
        /// <param name="gameDevice">ゲームデバイス</param>
        /// <param name="mediator">仲介者</param>
        public Boss(Vector2 position, GameDevice gameDevice, IGameObjectMediator mediator) :
           base("teki", position, 100, 100, 100, 100, gameDevice)
        {
            //各種パラメータの初期化

            color = Color.White;
            this.mediator = mediator;　//仲介者を引数受け取り
            time = new CountUpTimer(10000); //カウントアップタイマーを1万フレーム(200秒弱)で初期化
            currentState = BossState.JumpAttack;　//戦闘態勢は「高速突進」で初期化
            stateNum = 0;
            playerPos = Vector2.Zero;
            isVertigo = false;
            isDeadFlag = false; //死亡フラグは偽で初期化


            //【追加】モーションの登録・生成
            motionDict = new Dictionary<MotionName, Motion>()
            {
                { MotionName.attack , new Motion(new Range(0, 4), new CountDownTimer(0.07f)) },
                { MotionName.damage , new Motion(new Range(0, 4), new CountDownTimer(0.10f)) },
                { MotionName.idling , new Motion(new Range(0, 7), new CountDownTimer(0.25f)) },
                { MotionName.smoke ,  new Motion(new Range(0,2) , new CountDownTimer(0.25f)) },
            };
            for (int i = 0; i < 3; i++)
            {
                motionDict[MotionName.smoke].Add(i, new Rectangle(new Point(i * 64, 0), new Point(64, 64)));
            };
            for (int i = 0; i <= 4; i++)
            {
                motionDict[MotionName.attack].Add(i, new Rectangle(new Point(i * 128, 0), new Point(128, 144)));
                motionDict[MotionName.damage].Add(i, new Rectangle(new Point(i * 128, 0), new Point(128, 144)));
            }
            for (int i = 0; i <= 7; i++)
            {
                motionDict[MotionName.idling].Add(i, new Rectangle(new Point(i * 128, 0), new Point(128, 144)));
            }
            //現在の使用中モーションはアイドリングに設定
            //currentMotion = motionDict["idling"];
            //motionAssetWord = "idling"; //現在モーションに合わせて単語を初期化
            setMotionAndAsset(MotionName.idling);

            //追加　エフェクト用のモーションを初期化
            effectMotion = motionDict[MotionName.smoke];
        }

        /// <summary>
        /// 使用モーションとそれに合わせた描画アセットの指定
        /// </summary>
        /// <param name="motionName">モーション名</param>
        private void setMotionAndAsset(MotionName motionName)
        {
            #region 列挙型→文字列への変換を削除
            //MotionName name = MotionName.attack;
            //if (Enum.TryParse<MotionName>(motionName, out MotionName name) &&
            //    Enum.IsDefined(typeof(MotionName), name))
            //{
            //    //if(name.ToString() == "tackle")
            //    //{
            //    //    name = (MotionName)(Enum.Parse(typeof(MotionName), "attack"));
            //    //}
            //    currentMotion = motionDict[name];
            //}
            #endregion 列挙型→文字列への変換を削除
            currentMotion = motionDict[motionName];
            motionAssetWord = motionName.ToString();
        }

        /// <summary>
        /// キャラクターオブジェクトとの衝突判定
        /// </summary>
        /// <param name="character"></param>
        public override void HitChara(Character character)
        {
            //攻撃オブジェクトに接触
            if (character is Attack)
            {

                //ダメージ状態にするための設定をセット
                invincibleTime = new CountDownTimer(0.05f);　//ダメージ状態のタイマーをセット　
                invincible = true;　//ダメージ状態を真にする
            }
            if (character is Human || character is Dog)
            {
                invincibleTime = new CountDownTimer(0.05f);
                invincible = true;
            }
        }

        /// <summary>
        /// ダメージ状態のセット
        /// </summary>
        /// <param name="gameTime"></param>
        public void Invincible(GameTime gameTime)
        {
            if (invincible)　//ダメージ状態が真なら
            {
                color = Color.Red;　//描画色を赤くする

                //ダメージ状態のタイマーを起動
                invincibleTime.Update(gameTime);
                if (invincibleTime.IsTime())　//ダメージ状態タイマーが終了
                {
                    //体力を2減らす
                    hp -= 5;
                    vertigo += 5;

                    if (!isVertigo)　//目まい状態でないなら
                    {
                        vertigo += 1;　//目まい値を加算
                    }

                    //ダメージ状態を偽にして元に戻す
                    invincible = false;
                }
            }
        }

        /// <summary>
        /// ゲームオブジェクトとの衝突判定
        /// </summary>
        /// <param name="obj"></param>
        public override void HitObj(GameObject obj)
        {
            //鬼はブロックなどモノともしない！！！
            if (obj is Block)
            {
            }
        }

        /// <summary>
        /// 戦闘態勢の変更
        /// </summary>
        /// <param name="bossState">変更させたい状態</param>
        private void ChangeState(BossState bossState)
        {
            float cd = 0;
            if (cd != time.Now())　//カウントアップタイマーの現在時間が0でなければ＝タイマーが起動状態なら
            {
                cd = time.Now();　//floatの変数に現在時間を代入
            }
            if (time.Now() - cd >= 0.5f)　//これはどういう意味があるの？
            {


            }
            //気玉を放出する（戦闘態勢が遷移する時は毎回放出する）
            // mediator.AddCharacter(new BossBall(position, gameDevice, 0, mediator));
            currentState = bossState;　//引数の態勢を現在の戦闘態勢にセット
            //攻撃段階を初期化する
            stateNum = 0;
            attackNum = 0;
            color = Color.White;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            //  Vector2.Clamp(position , Vector2.Zero, new Vector2(map.GetWidth(), map.GetHeight())) ;
            Console.WriteLine(hp);　//HPを出力（体力確認用）
            Invincible(gameTime);　//ダメージ状態を更新
            if (hp <= 0) isDeadFlag = true; //HPが0になったら死亡させる→TrueEndingへ・・・
            time.Update(gameTime);　//カウントアップタイマーを更新
            angle = MathHelper.ToRadians(time.Now());　//タイマーの現在時間に合わせて角度を変えていく（凄い発想だ！）

            IsVertigo();　//めまいの更新？
            if (vertigo % 5 == 0)
            {
                var rnd = gameDevice.GetRandom().Next(0, 4);　//ランダムを用意
                //乱数で戦闘態勢を決定
                if (rnd == 0)
                {
                    //逃走状態に変更
                    ChangeState(BossState.RunAway);
                    vertigo += 1;
                }
                else
                {
                    return;
                }
                vertigo += 1;
            }

            //戦闘態勢に合わせた攻撃挙動をする
            if (currentState == BossState.SprintAttack)
            {
                SprintAttack(gameTime);
            }
            else if (currentState == BossState.Wait)
            {
                //待機状態のため、何もしない

                //描画モーションをアイドリングに
                setMotionAndAsset(MotionName.idling);
            }
            else if (currentState == BossState.ShotAttack)
            {
                ShotAttack(gameTime);
            }
            else if (currentState == BossState.CircleAttack)
            {
                CircleAttack(gameTime);
            }
            else if (currentState == BossState.JumpAttack)
            {
                JumpAttack(gameTime);
            }
            else if (currentState == BossState.RunAway)
            {
                RunAway(gameTime);
            }
            //キー入力でボスの状態を変更（デバッグ用？）
            if (Input.GetKeyTrigger(Microsoft.Xna.Framework.Input.Keys.J))
            {
                currentState += 1;
                stateNum = 0;
                attackNum = 0;
            }
            else if (Input.GetKeyTrigger(Microsoft.Xna.Framework.Input.Keys.K))
            {
                currentState -= 1;
                stateNum = 0;
                attackNum = 0;
            }

            //現在のボスの状態を出力（確認用）
            Console.WriteLine(currentState);

            //モーションを更新
            currentMotion.Update(gameTime);
            //方向の更新
            UpdateMyDirection();
        }

        /// <summary>
        /// 【追加】攻撃態勢に応じたアニメーション描画
        /// 更新処理・攻撃処理によって動的に切り替わる使用モーションを描画する
        /// </summary>
        /// <param name="renderer"></param>
        public override void Draw(Renderer renderer)
        {
            if (invincible)　//被ダメージ状態か？
            {
                //現在の使用モーションに関係なくダメージモーションを描画
                DrawDirMotion(renderer, "oni_damage_face", motionDict[MotionName.damage]);
                //追加　ダメージのエフェクトを描画
                DrawDirMotion(gameDevice.GetRenderer(), "smoke_effect", effectMotion);

            }
            else　//ダメージを受けていない
            {
                //現在モーションに合わせたて描画
                //アセット名の指定は、現在モーションに合わせて変更する
                DrawDirMotion(renderer, "oni_" + motionAssetWord + "_face", currentMotion);
            }
        }

        /// <summary>
        /// ターゲットからの逃走
        /// （戦闘態勢が逃走状態の時の挙動）
        /// </summary>
        /// <param name="gameTime"></param>
        private void RunAway(GameTime gameTime)
        {
            //描画するモーションを被ダメージ時のモノにする
            setMotionAndAsset(MotionName.damage);

            if (stateNum == 0)
            {
                CD = new CountDownTimer(0.5f);　//カウントダウンタイマーを生成
                alpha = 0.5f;
                stateNum = 1;
            }
            else if (stateNum == 1)
            {
                CD.Update(gameTime);　//上記で生成したカウントダウンタイマーを起動

                //自分とプレイヤーとのX座標の差を計算
                float x = playerPos.X - position.X;
                if (x < 0)　//プレイヤーの方が左側にいる
                {
                    //自分は右側に移動する（逃走する）
                    position += new Vector2(15, 0);
                }
                else　//自分の方が左側にいる
                {
                    //自分は左側に移動する（逃走する）
                    position += new Vector2(-15, 0);
                }
                if (CD.IsTime())　//カウントダウンタイマーが時間切れ
                {
                    alpha = 1;
                    //ランダムで戦闘態勢を決定
                    int num = gameDevice.GetRandom().Next(1, 5);
                    BossState state = (BossState)num;
                    //戦闘態勢をセット
                    ChangeState(state);
                }
            }
        }

        /// <summary>
        /// めまい状態の更新？
        /// めまい→動けない→待機状態にする
        /// </summary>
        private void IsVertigo()
        {
            if (vertigo >= 50)
            {
                vertigoTime = time.Now();
                isVertigo = true;　//目まい状態にする
                vertigo = 0;//目まいのカウントを戻す
            }
            if (isVertigo)　//目まい状態なら
            {
                //ボスの状態を待機にする
                ChangeState(BossState.Wait);
                alpha = 0.5f;

                if ((time.Now() - vertigoTime) >= 3f) //ある程度時間が経過したら
                {
                    alpha = 1;
                    ChangeState(BossState.RunAway);　//逃走状態にする
                    isVertigo = false;　//めまい状態を偽にする
                }
            }
        }

        /// <summary>
        /// ターゲットに向かって高速突進
        /// </summary>
        /// <param name="gameTime"></param>
        private void JumpAttack(GameTime gameTime)
        {
            if (stateNum == 0)　//サーチモード（ターゲットの位置を確認する）
            {
                //描画モーションをアイドリングにする
                setMotionAndAsset(MotionName.idling);

                CD = new CountDownTimer(0.8f);
                float a = CD.Now();

                //プレイヤーの位置情報を取得
                playerPos =
                      mediator.GetPlayer().GetPosition();
                alpha = (1 - a);
                //プレイヤーの座標が変わらなければ
                if (playerPos == mediator.GetPlayer().GetPosition())
                {
                    stateNum += 1;　//攻撃アルゴリズムを変更
                }
            }
            else if (stateNum == 1)　//ひたすら突進攻撃
            {
                //描画モーションを突進にする
                setMotionAndAsset(MotionName.attack);

                velocity = (playerPos + new Vector2(0, -150)) - position;　//移動量を設定
                if (!invincible)　//ダメージ状態でなければ
                {
                    //紫のオーラをまとわせる
                    color = Color.Purple;
                }
                velocity.Normalize();　//移動の方向のみ取得
                //特定の条件に一致いるか確認
                if ((playerPos.Y - position.Y) < 165 && (playerPos.Y - position.Y) > 130)
                {
                    //攻撃アルゴリズムを変更
                    CD.Update(gameTime);
                    float a = CD.Now();

                    alpha += (1 - a);
                    if (CD.IsTime())　//一定時間たったら
                    {
                        stateNum = 2;
                    }
                }
                else
                {
                    //プレイヤーのいたおよその方向に向かって突進
                    position += velocity * 20;
                }
            }
            else if (stateNum == 2)　//突進＆仕上げの気玉同時放出
            {
                //ターゲットのY座標を取得
                playerPos.Y = mediator.GetPlayer().GetPosition().Y;

                //自分とターゲットとの縦方向の距離が10より大きければ
                if (Math.Abs(position.Y - playerPos.Y) > 10f)
                {
                    //ターゲットへの方向と距離を取得
                    velocity = playerPos - position;
                    //方向のみを取得
                    velocity.Normalize();
                    //ターゲットに向かって突進
                    position += velocity * 20;
                }
                else　//ある程度ターゲットに近づいた
                {
                    //気玉を多方向に放出
                    mediator.AddCharacter(new BossBullet(position, gameDevice, 1, 2));
                    mediator.AddCharacter(new BossBullet(position, gameDevice, 1, 4));
                    mediator.AddCharacter(new BossBullet(position, gameDevice, 1, -2));
                    mediator.AddCharacter(new BossBullet(position, gameDevice, 1, -4));

                    //気玉を放つ戦闘態勢に変更
                    ChangeState(BossState.ShotAttack);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void ShotAttack(GameTime gameTime)
        {
            float length;

            if (stateNum == 0)　//サーチモード
            {
                //描画モーションをアイドリングにする
                setMotionAndAsset(MotionName.idling);

                color = Color.Blue;　//青く光を放つ
                float time;

                //ターゲットの位置情報を取得
                playerPos =
                    mediator.GetPlayer().GetPosition();
                //ターゲットと自分との距離を取得
                length = (playerPos - position).Length();
                if (length >= 500)　//ある程度距離が空いている
                {
                    time = 1f;
                }
                else　//距離が近い
                {
                    time = 1.5f;
                }

                //ターゲットとの距離に応じてタイマーをセット
                CD = new CountDownTimer(time);
                //ターゲットが移動していない
                if (playerPos == mediator.GetPlayer().GetPosition())
                {
                    stateNum += 1;　//攻撃段階を進める
                }

            }
            else if (stateNum == 1)　//気玉を放出
            {
                //描画モーションを突進にする
                setMotionAndAsset(MotionName.attack);

                CD.Update(gameTime);　//前段階でセットしたタイマーを起動する
                if (CD.IsTime())　//タイマーが時間切れになったら
                {
                    //気玉を放出
                    mediator.AddCharacter(new BossBulletZ(position, playerPos, GameDevice.Instance()));
                    //攻撃段階を進める
                    stateNum += 1;
                }
            }
            else if (stateNum == 2)
            {
                if (attackNum < 4)
                {
                    attackNum += 1;
                    Console.WriteLine(attackNum);
                    stateNum = 0;
                }
                else if (attackNum >= 4)
                {
                    //攻撃態勢を切り替える
                    ChangeState(BossState.CircleAttack);
                }
            }
        }

        /// <summary>
        /// 円形放出攻撃
        /// </summary>
        /// <param name="gameTime"></param>
        public void CircleAttack(GameTime gameTime)
        {
            float length;
            if (stateNum == 0)　//準備段階
            {
                //描画モーションをアイドリングにする
                setMotionAndAsset(MotionName.idling);
                attackNum = 8;
                //タイマーを1秒でセット
                CD = new CountDownTimer(1.5f);
                stateNum = 1;
            }
            if (!invincible)　//ダメージ状態でなければ
            {
                //緑に発光する
                color = Color.Yellow;
            }
            if (stateNum == 1)
            {

                float times = time.Now(); //長いカウントアップタイマーの現在時間の取得
                if (times % 1 == 0)//一定間隔で特定処理
                {
                    //ターゲットの位置情報を取得
                    playerPos = mediator.GetPlayer().GetPosition();
                    //自分とターゲットとの距離を取得
                    length = (playerPos - position).Length();
                    bulletNum += 2;
                    attackNum += 1;

                    //ターゲットとの距離に応じて気玉を放出
                    for (int i = 0; i < bulletNum; i++)
                    {
                        mediator.AddCharacter(new BossBullet(position, gameDevice, i * 10));

                    }
                }
                if (attackNum >= 3)
                {
                    //カウントダウンタイマーを起動
                    CD.Update(gameTime);

                    //赤く閃光を放つ
                    color = Color.Red;
                    if (CD.IsTime()) //カウントダウンタイマーが時間切れ
                    {
                        //攻撃態勢を変更
                        ChangeState(BossState.SprintAttack);

                        //描画モーションをアイドリングにする
                        setMotionAndAsset(MotionName.idling);
                    }
                }

            }
        }

        /// <summary>
        /// 突進攻撃
        /// </summary>
        /// <param name="gameTime"></param>
        private void SprintAttack(GameTime gameTime)
        {
            var velocity = Vector2.Zero;
            float timeS = 0;
            if (stateNum == 0)　//捜索段階
            {
                setMotionAndAsset(MotionName.idling); //描画するモーションをアイドリングモーションにする

                color = Color.Red;　//赤く発色
                timeS = time.Now();　//長いカウントアップタイマーの現在時間を取得
                playerPos = mediator.GetPlayer().GetPosition();　//ターゲットの位置情報を取得
                //ターゲットが移動していないなら
                if (playerPos == mediator.GetPlayer().GetPosition())
                {
                    //攻撃段階を進める
                    stateNum += 1;
                }
            }
            else if (stateNum == 1)　//高速突進
            {
                setMotionAndAsset(MotionName.attack); //描画するモーションをタックルにする

                CD.Update(gameTime);　//カウントダウンタイマーを起動
                if (CD.IsTime())　//時間切れになったら
                {
                    //自分とターゲットとの距離を計算
                    velocity = playerPos - position;
                    //ターゲットへの方向のみ取得
                    velocity.Normalize();
                    //ターゲットとの距離がある程度離れている
                    if ((position - playerPos).Length() > 20)
                    {
                        //ターゲットに向かって突進
                        position += velocity * 15f;
                    }
                    //ターゲットとの距離がある程度近い
                    else if ((position - playerPos).Length() <= 20)
                    {
                        CD = new CountDownTimer(1.5f);

                        stateNum = 2;　//攻撃段階を進める
                    }
                }
            }
            else if (stateNum == 2)
            {
                //描画モーションをアイドリングにする
                setMotionAndAsset(MotionName.idling);

                //紫の気を発する
                color = Color.Purple;

                CD.Update(gameTime);　//カウントダウンタイマーを起動
                //時間切れになったら攻撃態勢を変更
                if (CD.IsTime())
                    ChangeState(BossState.JumpAttack);
            }
        }
    }
}
