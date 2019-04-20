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

namespace Momotaro.Actor.Characters
{
    class Attack : Character
    {
        private Timer time;
        private Direction dir;
        private Human human;

        public Attack(Vector2 position ,  Direction dir , GameDevice  gameDevice  , Human human)
            : base ( "" , position, 32, 32, 32, 32, gameDevice)
        {
            this.human = human;
            this.dir = dir; 
            this.position = human.SetPlayerPosition(ref position) + new Vector2(human.GetWidth() / 2 - GetWidth() / 2, 0); ;
            time = new CountDownTimer(0.1f);
        }


        public override void HitChara(Character chara)
        {
            
        }

        public override void HitObj(GameObject obj)
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (dir == Direction.Right)
                position += new Vector2(20, 0);
            else if (dir == Direction.Left)
                position += new Vector2(-20, 0);

            time.Update(gameTime);
                
            if (time.IsTime())
            {
                isDeadFlag = true;
            }           
        }

        /// <summary>
        /// 【追加】描画処理（何もしない）
        /// </summary>
        /// <param name="renderer"></param>
        public override void Draw(Renderer renderer)
        {
            
        }

        public override void Change()
        {

        }
    }
}
