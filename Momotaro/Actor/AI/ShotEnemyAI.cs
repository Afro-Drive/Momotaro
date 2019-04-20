using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Actor.Characters;
using Momotaro.Device;
using Momotaro.Scene;

namespace Momotaro.Actor.AI
{
    class ShotEnemyAI : EnemyAI
    {
        private int bulletInterval;
        private int bulletCnt;
        private Sound sound;

        public ShotEnemyAI(Character character, IGameObjectMediator mediator)
            : base(character, mediator)
        {
            velocity.X = 0;
            gravity = 0.4f;

            bulletCnt = 0;
            bulletInterval = 100;

            sound = GameDevice.Instance().GetSound();
        }

        public override void Attack()
        {
            bulletCnt++;

            if (bulletCnt > bulletInterval)
            {
                bulletCnt = 0;

                float targetPosX = mediator.GetPlayer().GetPosition().X;
                float distance = Math.Abs(this.position.X - targetPosX);

                if (distance < 1500f)
                {
                    sound.PlaySE("e_atk");

                    if (targetPosX > this.position.X)
                        mediator.AddCharacter(
                            new ShotEnemyBullet(
                                new Vector2(position.X + 16, position.Y),
                                new Vector2(8, -8),
                                GameDevice.Instance()));
                    else
                        mediator.AddCharacter(
                            new ShotEnemyBullet(
                                new Vector2(position.X + 16, position.Y), 
                                new Vector2(-8, -8), 
                                GameDevice.Instance()));
                }
            }
        }

        public override Vector2 Move()
        {
            MoveX();
            MoveY();
            FallStart();
            Fall();

            return position;
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

        public void Landing()
        {
            velocity.Y = 0;
            isJump = false;
        }

        public void Fall()
        {
            if (isJump == true)
            {
                velocity.Y = velocity.Y + gravity;
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
    }
}
