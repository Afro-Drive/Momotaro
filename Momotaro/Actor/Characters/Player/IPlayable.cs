using Microsoft.Xna.Framework;
using Momotaro.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Momotaro.Actor.Characters.Player
{
     interface  IPlayable
    {
        void Draw(Renderer renderer); 

        void Move();
        void Jump();
        void Action();
        void Change();
    }
}
