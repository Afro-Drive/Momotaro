﻿using System;
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
    class Dog : Character ,IPlayable
    {
        //private Vector2 velocity;

        //private bool isJump;

        private IGameObjectMediator mediator;
        private Map map;

        private bool rightDamage;
        private bool leftDamage;
        private Timer damageShow;

        private Sound sound;
        private Timer soundTimer;

        //private Motion movemotion;
        //private Motion jumpmotion;

        public Dog(Vector2 position, GameDevice gameDevice, IGameObjectMediator mediator)
            : base("inu", position, 64, 64, 64, 40, gameDevice)
        {
            velocity = Vector2.Zero;
            isJump = true;
            isClearFlag = false;
            this.mediator = mediator;
            map = mediator.GetMap();
            damageShow = new CountDownTimer(0.2f);

            sound = GameDevice.Instance().GetSound();
            soundTimer = new CountDownTimer(0.5f);

            //【追加】モーションの生成・登録
            motionDict = new Dictionary<string, Motion>()
            {
                { "idling", new Motion(new Range(0, 5 - 1), new CountDownTimer(0.25f) )},
                { "jump", new Motion(new Range(0, 5 - 1), new CountDownTimer(0.25f) )},
                { "move", new Motion(new Range(0, 5 - 1), new CountDownTimer(0.10f) )},
            };
            for (int i = 0; i <= 4; i++)
            {
                motionDict["idling"].Add(i, new Rectangle(new Point(64 * i, 0), new Point(64)));
                motionDict["jump"].Add(i, new Rectangle(new Point(64 * i, 0), new Point(64)));
                motionDict["move"].Add(i, new Rectangle(new Point(64 * i, 0), new Point(64)));
            }
            currentMotion = motionDict["idling"];

            #region　モーションの生成→ディクショナリでの管理に委託
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
            #endregion　モーションの生成→ディクショナリでの管理に委託
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

        public override void Update(GameTime gameTime)
        {
            soundTimer.Update(gameTime);
            //movemotion.Update(gameTime);
            //jumpmotion.Update(gameTime);

            SetDisplayModify();
            Jump();
            Move();

            //【追加】
            UpdateMyDirection();
            currentMotion.Update(gameTime);

            DamageShow(gameTime);
            Invincible(gameTime);


            PlayWalkSE();
        }

        /// <summary>
        /// 【追加】アニメーションに対応した描画処理
        /// </summary>
        /// <param name="renderer"></param>
        public override void Draw(Renderer renderer)
        {
            //ジャンプ中
            if (isJump)
            {
                currentMotion = motionDict["jump"];
                DrawJumping(renderer, "inu_jumpL", currentMotion);
            }
            //非ジャンプ中
            else
            {
                //移動中
                if (base.velocity != Vector2.Zero)
                {
                    currentMotion = motionDict["move"];
                    DrawDirMotion(renderer, "inu_moveL", currentMotion);
                }
                //止まっている（アイドリング状態）
                else
                {
                    currentMotion = motionDict["idling"];
                    DrawIdling(renderer, "inu_idlingL", currentMotion);
                }
            }

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
            float speed = 8;

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
                    velocity.Y = -9.5f;
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
            if(!isJump && (Input.GetKeyTrigger(Keys.Right) || Input.GetKeyTrigger(Keys.Left)
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

        //public override void Draw(Renderer renderer)
        //{
        //    if (isJump)
        //    {
        //        renderer.DrawTexture("kiji_jumpmotion", position + gameDevice.GetDisplayModify(), jumpmotion.DrawingRange());
        //    }
        //    else
        //    {
        //        renderer.DrawTexture(name, position + gameDevice.GetDisplayModify(), movemotion.DrawingRange());
        //    }
        //}
    }
}