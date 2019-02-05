using Microsoft.Xna.Framework;
using Momotaro.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Momotaro.Util
{
    class Hp
    {
        private int hp;
        private int maxHp;

        public Hp()
        {
            Initialize();
        }

        public void Initialize()
        {
            maxHp = 10;
            hp = maxHp;
        }

        public void Add(int num)
        {
            if (hp >= 10)
            {
                hp = 10;
            }
            
            hp += num;
       
        }

        public int GetHp()
        {
            return hp;
        }

        public void Draw(Renderer renderer)
        {
            //最大HP（黒で表示）
            for(int i = 1;i <= maxHp; i++)
            {
                renderer.DrawTexture("hp_null", new Vector2(32 * i, 32));
            }

            //現在のHP（普通に表示）
            for (int i = 1; i <= hp; i++)
            {
                renderer.DrawTexture("hp", new Vector2(32 * i, 32));
            }

        }
    }
}
