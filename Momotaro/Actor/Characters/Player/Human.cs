using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Momotaro.Actor.Characters.BossObj;
using Momotaro.Actor.GameObjects;
using Momotaro.Actor.GameObjects.DamageObj;
using Momotaro.Def;
using Momotaro.Device;
using Momotaro.Scene;
using Momotaro.Util;

namespace Momotaro.Actor.Characters.Player
{
    class Human : Character, IPlayable
    {
        private Direction dir;
        private IGameObjectMediator mediator;

        private bool godMod;

        private bool rightDamage;
        private bool leftDamage;
        private Timer damageShow;

        private Sound sound;
        private Timer soundTimer;

        private Timer actionTimer; //【追加】アクションモーションを魅せるためのタイマー

        public Human(Vector2 position, GameDevice gameDevice, IGameObjectMediator mediator)
            : base("momotaro_R", position, 64, 64, 40, 60, gameDevice)
        {
            velocity = Vector2.Zero;
            dir = Direction.Right;
            isJump = true;
            isClearFlag = false;
            this.mediator = mediator;
            map = mediator.GetMap();
            damageShow = new CountDownTimer(0.2f);

            godMod = false;

            sound = GameDevice.Instance().GetSound();
            soundTimer = new CountDownTimer(0.5f);

            #region　モーションの生成→ディクショナリによる管理に変更
            //movemotion = new Motion();
            //for (int i = 0; i < 7; i++)
            //{
            //    movemotion.Add(i, new Rectangle(i * 64, 0, 64, 64));
            //}
            //movemotion.Initialize(new Range(0, 6), new CountDownTimer(0.15f));

            //jumpmotion = new Motion();
            //for (int i = 0; i < 5; i++)
            //{
            //    jumpmotion.Add(i, new Rectangle(i * 64, 0, 64, 64));
            //}
            //jumpmotion.Initialize(new Range(0, 4), new CountDownTimer(0.15f));
            #endregion　モーションの生成→ディクショナリによる管理に変更

            //【追加】モーションの生成・登録
            motionDict = new Dictionary<MotionName, Motion>()
            {
                { MotionName.idling, new Motion(new Range(0, 4), new CountDownTimer(0.10f)) }, //普通のアニメーション
                { MotionName.attack, new Motion(new Range(0, 5), new CountDownTimer(0.07f)) }, //攻撃モーション
            };

            //モーション切り取り位置の追加
            for (int i = 0; i <= 4; i++) //アクション以外
            {
                motionDict[MotionName.idling].Add(i, new Rectangle(new Point(64 * i, 0), new Point(64)));
            }
            for (int i = 0; i <= 5; i++) //アクション
            {
                //切り取り範囲が異なるので描画時に注意！！
                motionDict[MotionName.attack].Add(i, new Rectangle(new Point(96 * i, 0), new Point(96, 64)));
            }

            //現在モーションはジャンプモーションに設定
            currentMotion = motionDict[MotionName.idling];

            //【追加】アクションを魅せるためのタイマーを初期化
            actionTimer = new CountDownTimer(0.35f); //アクションモーション1周分の制限時間で

        }

        public override void HitChara(Character other)
        {
            if (invincible == false)
            {

                if (other is Enemy || other is ShotEnemyBullet || other is Boss || other is BossBullet || other is BossBulletZ)
                {
                    Direction dir = this.CheckDirection(other);

                    sound.PlaySE("dmg");
                    mediator.ChangeHp(-1);
                    damageShow = new CountDownTimer(0.2f);
                    invincibleTime = new CountDownTimer(2f);
                    invincible = true;

                    if (dir == Direction.Right)
                    {
                        rightDamage = true;
                    }
                    else if (dir == Direction.Left)
                    {

                        leftDamage = true;
                    }
                    else if (dir == Direction.Top)
                    {
                        velocity.Y -= 8f;
                        velocity.Y = (velocity.X < -8f) ? (velocity.Y) : (-8f);

                    }
                    else if (dir == Direction.Bottom)
                    {
                        velocity.Y += 8f;
                        velocity.Y = (velocity.X < 8f) ? (velocity.Y) : (8f);
                        Console.WriteLine("bottom");

                    }
                }
            }
        }

        public override void HitObj(GameObject obj)
        {
            if (obj is ClearBlock)
            {
                isClearFlag = true;
            }

            if (obj is DeathBlock)
            {
                isDeadFlag = true;
            }

            if (obj is Flame || obj is Thorn)
            {
                if (invincible == false)
                {
                    sound.PlaySE("dmg");
                    mediator.ChangeHp(-1);
                    damageShow = new CountDownTimer(0.2f);
                    invincibleTime = new CountDownTimer(2f);
                    invincible = true;
                }
            }
        }

        public void GodMove()
        {
            velocity = Input.Velocity() * 10;
            position += velocity;
        }

        public override void Update(GameTime gameTime)
        {
            soundTimer.Update(gameTime);
            //movemotion.Update(gameTime);
            //jumpmotion.Update(gameTime);

            if (Input.GetKeyTrigger(Keys.G))
            {
                godMod = !godMod;
            }

            if (!godMod || !rightDamage || !leftDamage)
            {
                Jump();
                Move();
            }
            if (godMod)
            {
#if DEBUG
                GodMove();
#endif
            }

            Action();
            ChangeDir();

            //【追加】
            UpdateMyDirection();
            currentMotion.Update(gameTime);
            if (Input.GetKeyTrigger(Keys.Z) || Input.GetKeyTrigger(PlayerIndex.One, Buttons.Y))
                //攻撃ボタンが入力されたらモーションを行う時間を確保
                actionTimer.Initialize();
            actionTimer.Update(gameTime);　//攻撃モーション用タイマーを起動

            Invincible(gameTime);
            DamageShow(gameTime);


            SetDisplayModify();
            PlayWalkSE();
        }

        /// <summary>
        /// 位置補正
        /// </summary>
        private void SetDisplayModify()
        {
            float setModifyX = -position.X + (Screen.Width / 2 - width / 2);
            float setModifyY = -position.Y + (Screen.Height / 2);

            //x方向の画面端の処理
            if (position.X < Screen.Width / 2 - width / 2 + 64)//+64を外すと一番左のブロックも見えます
            {
                setModifyX = -64;//-64を0にすると一番左のブロックも見えます
            }
            else if (position.X > mediator.MapSize().X - Screen.Width / 2 - width / 2 - 64)//-64を外すと一番右のブロックも見えます
            {
                setModifyX = -(mediator.MapSize().X - Screen.Width / 2 - width / 2) + (Screen.Width / 2 - width / 2 + 64);//+64を外すと一番右のブロックも見えます
            }

            //y方向　マップの一番下のブロック以下が見えないようにする
            if (position.Y > mediator.MapSize().Y - Screen.Height / 2 - 64)// -64を外すと一番下のブロックも見えます。
            {
                setModifyY = -(mediator.MapSize().Y - Screen.Height - 64);// -64を外すと一番下のブロックも見えます。
            }

            gameDevice.SetDisplayModify(new Vector2(setModifyX, setModifyY));
        }

        public override void Change()
        {
            isChangeFlag = true;
            invincible = false;
        }

        public void Move()
        {
            float speed = 6;

            velocity.X = (Input.Velocity().X + Input.Velocity(PlayerIndex.One).X) * speed;
            MoveX();
            MoveY();
        }

        public void MoveX()
        {
            List<Vector2> checkPos = checkRList;
            if (velocity.X < 0)
            {
                checkPos = checkLList;
            }

            float normalVX = Math.Sign(velocity.X);
            for (int x = 0; x < Math.Abs(velocity.X); x++)
            {
                foreach (var pos in checkPos)
                {
                    if (map.IsBlock(position + pos))
                    {
                        if (0 < velocity.X)
                        {
                        }
                        else
                        {
                        }

                        return;
                    }
                }
                position.X += normalVX;
            }
        }

        public void MoveY()
        {
            List<Vector2> checkPos = checkUList;
            if (velocity.Y > 0)
            {
                checkPos = checkDList;
            }

            float normalVY = Math.Sign(velocity.Y);
            for (int y = 0; y < Math.Abs(velocity.Y); y++)
            {
                foreach (var pos in checkPos)
                {
                    if (map.IsBlock(position + pos))
                    {
                        if (velocity.Y > 0)
                        {
                            Landing();
                        }
                        else
                        {
                            velocity.Y = 0;
                        }
                        return;
                    }
                }
                position.Y += normalVY;
            }
        }

        public void Jump()
        {
            JumpStart();
            JumpUpdate();
            FallStart();
        }

        public void JumpStart()
        {
            if (isJump == false)
            {
                if (Input.GetKeyState(Keys.Space) || Input.GetKeyTrigger(PlayerIndex.One, Buttons.B))
                {
                    sound.PlaySE("p_jump");
                    velocity.Y = -12;
                    isJump = true;
                }
            }
        }

        public void JumpUpdate()
        {
            if (isJump == true)
            {
                velocity.Y = velocity.Y + 0.4f;
                velocity.Y = (velocity.Y > 16.0f) ? (16) : (velocity.Y);
            }
        }

        public void FallStart()
        {
            // 下にブロックがあるかチェック
            foreach (var pos in checkDList)
            {
                // 下にブロックがあったら
                if (map.IsBlock(position + pos))
                {
                    // 処理終了
                    return;
                }
            }
            // 下にブロックがなかったらジャンプ中にする
            isJump = true;
        }

        public void Landing()
        {
            velocity.Y = 0;
            isJump = false;
        }

        public void Action()
        {
            if (Input.GetKeyTrigger(Keys.Z) || Input.GetKeyTrigger(PlayerIndex.One, Buttons.Y))
            {
                sound.PlaySE("momo_atk");
                mediator.AddCharacter(new Attack(position, dir, gameDevice, this));
            }
        }

        public void ChangeDir()
        {
            if ( //!isJump && 
                (Input.GetKeyState(Keys.Right) || Input.GetKeyTrigger(PlayerIndex.One, Buttons.DPadRight)
                || Input.GetKeyTrigger(PlayerIndex.One, Buttons.LeftThumbstickRight)))
            {
                dir = Direction.Right;
            }
            else if ( //!isJump && 
                (Input.GetKeyState(Keys.Left) || Input.GetKeyTrigger(PlayerIndex.One, Buttons.DPadLeft)
                || Input.GetKeyTrigger(PlayerIndex.One, Buttons.LeftThumbstickLeft)))
            {
                dir = Direction.Left;
            }
        }

        public Vector2 SetPlayerPosition(ref Vector2 OtherPosition)
        {
            OtherPosition = position;
            return OtherPosition;
        }

        public void Invincible(GameTime gameTime)
        {
            if (invincible)
            {
                float a = invincibleTime.Now();
                color = Color.Red;


                invincibleTime.Update(gameTime);
                if (invincibleTime.IsTime())
                {
                    invincible = false;
                }
            }
            else
            {
                color = Color.White;
            }
        }

        private void DamageShow(GameTime gameTime)
        {
            if (rightDamage)
            {
                damageShow.Update(gameTime);
                velocity.X = 10f;
                velocity.Y = -3f;
                //position += velocity;
                MoveX();
                MoveY();
            }
            if (leftDamage)
            {
                damageShow.Update(gameTime);
                velocity.X = -10f;
                velocity.Y = -3f;
                //position += velocity;
                MoveX();
                MoveY();

            }
            if (damageShow.IsTime())
            {
                rightDamage = false;
                leftDamage = false;
            }
        }

        private void PlayWalkSE()
        {
            if (!isJump && (Input.GetKeyTrigger(Keys.Right) || Input.GetKeyTrigger(Keys.Left)
                || Input.GetKeyTrigger(PlayerIndex.One, Buttons.LeftThumbstickRight)
                || Input.GetKeyTrigger(PlayerIndex.One, Buttons.LeftThumbstickLeft)
                || Input.GetKeyTrigger(PlayerIndex.One, Buttons.DPadRight)
                || Input.GetKeyTrigger(PlayerIndex.One, Buttons.DPadLeft)))
            {
                if (soundTimer.IsTime())
                {
                    sound.PlaySE("p_move");
                    soundTimer.Initialize();
                }
            }
        }

        /// <summary>
        /// 【追加】アニメーションに対応した描画処理
        /// </summary>
        /// <param name="renderer"></param>
        public override void Draw(Renderer renderer)
        {
            //攻撃状態
            if (!actionTimer.IsTime()) //アクション用タイマーが起動中はモーションを行う
            {
                //描画状態を変える
                currentMotion = motionDict[MotionName.attack];
                    DrawAction(renderer, currentMotion);
            }
            //非攻撃状態
            else
            {
                //ジャンプ中
                if (isJump)
                {
                    currentMotion = motionDict[MotionName.idling];
                    DrawJumping(renderer, "momotaro_jumpR", "momotaro_jumpL", currentMotion);
                }
                //非ジャンプ中
                else
                {
                    //移動中
                    if (velocity != Vector2.Zero)
                    {
                        currentMotion = motionDict[MotionName.idling];
                        DrawDirMotion(renderer, "momotaro_moveR", "momotaro_moveL", currentMotion);
                    }
                    //止まっている（アイドリング状態）
                    else
                    {
                        currentMotion = motionDict[MotionName.idling];
                        DrawIdling(renderer, "momotaro_idlingR", "momotaro_idlingL", currentMotion);
                    }
                }
            }
        }

        /// <summary>
        /// 【追加】現在方向に合わせた攻撃時のアニメーション
        /// </summary>
        /// <param name="renderer"></param>
        public void DrawAction(Renderer renderer, Motion attackMotion)
        {
            //右向き
            if (dir == Direction.Right)
            {
                renderer.DrawTexture(
                    "momotaro_attackR",
                    position + gameDevice.GetDisplayModify(),
                    attackMotion.DrawingRange(),
                    color);
            }
            //左向き
            else if (dir == Direction.Left)
            {
                renderer.DrawTexture(
                    "momotaro_attackL",
                    position + gameDevice.GetDisplayModify(),
                    attackMotion.DrawingRange(),
                    color);
            }
        }

    }
}
