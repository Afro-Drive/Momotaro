using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Momotaro.Actor.AI;
using Momotaro.Actor.GameObjects;
using Momotaro.Actor.GameObjects.Effects;
using Momotaro.Device;
using Momotaro.Scene;
using Momotaro.Util;

namespace Momotaro.Actor.Characters
{
    class Enemy : Character
    {
        private EnemyAI ai;
        private AIName aiName;

        private IGameObjectMediator mediator;

        private CharaState state;
        private Direction direction;

        private Timer damageTimer;

        private float gravity;
        private float damageVelocityY;

        public Enemy(string name, Vector2 position,AIName aiName, GameDevice gameDevice, IGameObjectMediator mediator)
            : base(name, position, 64, 64, 55, 60, gameDevice)
        {
            this.Position = position;
            this.aiName = aiName;
            this.mediator = mediator;
            map = mediator.GetMap();

            damageTimer = new CountDownTimer(0.5f);
            gravity = 0.5f;
            damageVelocityY = -5;

            state = CharaState.Normal;

            SetAI();

            //【追加】モーションの生成・追加
            motionDict = new Dictionary<MotionName, Util.Motion>()
            {
                { MotionName.attack, new Motion(new Range(0, 4), new CountDownTimer(0.25f)) },
                { MotionName.move, new Motion (new Range(0, 4), new CountDownTimer(0.15f)) },
                { MotionName.idling, new Motion (new Range(0, 4), new CountDownTimer(0.25f)) },
            };

            for (int i = 0; i <= 4; i++)
            {
                motionDict[MotionName.attack].Add(i, new Rectangle(new Point(64 * i, 0), new Point(64)));
            }
            for (int i = 0; i <= 4; i++)
            {
                motionDict[MotionName.move].Add(i, new Rectangle(new Point(64 * i, 0), new Point(64)));
            }
            for (int i = 0; i <= 4; i++)
            {
                motionDict[MotionName.idling].Add(i, new Rectangle(new Point(64 * i, 0), new Point(64)));
            }
            //最初のモーションはムーブに設定
            currentMotion = motionDict[MotionName.move];

            //【追加】移動方向の設定
            myDirectionX = Direction.Right;
            myDirectionY = Direction.Top;

        }

        public override void HitChara(Character character)
        {
            if(character is Attack)
            {
                if(CheckDirection(character) == Direction.Right)
                {
                    direction = Direction.Right;
                }
                if (CheckDirection(character) == Direction.Left)
                {
                    direction = Direction.Left;
                }

                state = CharaState.Damage;
                //isDeadFlag = true;
            }
        }

        public override void HitObj(GameObject obj)
        {
            if (obj is DeathBlock)
            {
                isDeadFlag = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            switch(state)
            {
                case CharaState.Normal:
                    ai.Attack();
                    Position = ai.Move();
                    break;
                case CharaState.Damage:
                    DamageUpdate(gameTime);
                    break;
                case CharaState.Delete:
                    isDeadFlag = true;
                    mediator.AddGameObject(new SmokeEffect(Position, gameDevice));
                    break;
            }
            //ai.Attack();
            //Position = ai.Move();

            //【追加】
            velocity = ai.Velocity();
            UpdateMyDirection();
            if (aiName == AIName.Shot)
                UpdateShotMyDir();
            currentMotion.Update(gameTime);
        }

        private void SetAI()
        {
            switch (aiName)
            {
                case AIName.Normal:
                    ai = new NormalEnemyAI(this, mediator);
                    break;
                case AIName.Turn:
                    ai = new TurnEnemyAI(this, mediator);
                    break;
                case AIName.Shot:
                    ai = new ShotEnemyAI(this, mediator);
                    break;
            }
        }

        public override void Draw(Renderer renderer)
        {
            switch (state)
            {
                case CharaState.Normal:
                    //base.Draw(renderer);
                    DrawAnimation(renderer); //【変更】
                    break;
                case CharaState.Damage:
                    renderer.DrawTexture("kooni_damage", Position + gameDevice.GetDisplayModify());
                    break;
                case CharaState.Delete:
                    break;
            }
        }

        /// <summary>
        /// 【追加】搭載しているAIと、方向に応じたアニメーションの描画
        /// </summary>
        /// <param name="renderer"></param>
        public void DrawAnimation(Renderer renderer)
        {
            if (aiName == AIName.Shot) //シュートAIの場合
            {
                currentMotion = motionDict[MotionName.attack];
                DrawingAttack(renderer, currentMotion);
            }
            else　//それ以外の場合
            {

                //ジャンプ中
                if (ai.IsJump())
                {
                    currentMotion = motionDict[MotionName.move];
                    DrawJumping(renderer, "kooni_moveL", currentMotion);
                }
                //非ジャンプ中
                else
                {
                    //移動中
                    if (velocity != Vector2.Zero)
                    {
                        currentMotion = motionDict[MotionName.move];
                        DrawDirMotion(renderer, "kooni_moveL", currentMotion);
                    }
                    //止まっている（アイドリング状態）
                    else
                    {
                        currentMotion = motionDict[MotionName.move];
                        DrawIdling(renderer, "kooni_moveL", currentMotion);
                    }
                }

            }

        }

        /// <summary>
        /// 【追加】攻撃時のアニメーション描画
        /// </summary>
        /// <param name="attackMotion">使用する攻撃モーション</param>
        public void DrawingAttack(Renderer renderer, Motion attackMotion)
        {
            if (myDirectionX == Direction.Left)
            {
                renderer.DrawTexture(
                    "Oni_Attack",
                    Position + gameDevice.GetDisplayModify(),
                    currentMotion.DrawingRange(),
                    color);
            }
            if (myDirectionX == Direction.Right)
            {
                //左向きの画像を反転させて描画する
                renderer.DrawAntiAxisTexture(
                    "Oni_Attack",
                    Position + gameDevice.GetDisplayModify(),
                    currentMotion.DrawingRange(),
                    Vector2.Zero,
                    1f,
                    SpriteEffects.FlipHorizontally,
                    color);
            }
        }


        private void DamageUpdate(GameTime gameTime)
        {
            damageTimer.Update(gameTime);

            if(direction == Direction.Right)
            {
                Position += new Vector2(10, damageVelocityY);
            }
            else if (direction == Direction.Left)
            {
                Position += new Vector2(-10, damageVelocityY);
            }

            damageVelocityY += gravity;

            if (damageTimer.IsTime())
            {
                damageTimer.Initialize();
                state = CharaState.Delete;
                damageVelocityY = -5;
            }
        }

        /// <summary>
        /// ShotAI搭載時の向き変更
        /// </summary>
        private void UpdateShotMyDir()
        {
            Character target = mediator.GetPlayer();
            if (target == null) return;

            float targetPosX = target.Position.X;

            myDirectionX = Direction.Right;
            if (targetPosX < this.Position.X)
                myDirectionX = Direction.Left;
        }
    }
}
