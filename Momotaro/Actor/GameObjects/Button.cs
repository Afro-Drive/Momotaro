using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Actor.Characters;
using Momotaro.Actor.Characters.Player;
using Momotaro.Device;
using Momotaro.Scene;
using Momotaro.Util;

namespace Momotaro.Actor.GameObjects
{
    class Button : GameObject
    {
        private bool isTouch;
        private bool isHit;

        private GameObjectID linkedGameOjectID = GameObjectID.Door;
        private IGameObjectMediator mediator;
        private enum Direction
        { CanPut, TimeDown}
        private Dictionary<Direction, string> buttonState;
        private Direction correntState;
        private Timer timer; 

       // private string name;

        public Button( Vector2 position, GameDevice gameDevice,
            IGameObjectMediator mediator)
            : base("switch_TimeDown", position, 64, 64, gameDevice)
        {
            this.mediator = mediator;
            isTouch = false;
            isHit = false;
            id = GameObjectID.Button;
            
            buttonState = new Dictionary<Direction, string>()
            {
                {Direction.CanPut, "switch_CanPut" },
                {Direction.TimeDown, "switch_TimeDown" }
            };

            timer = new CountDownTimer(5f); 
            correntState = Direction.CanPut; 
        }

        public Button(Button other) 
            : this(other.position, other.gameDevice, other.mediator)
        {

        }

        public override object Clone()
        {
            return new Button(this);
        }

        public override void Hit(GameObject gameObject)
        {
            
        }


        public override void HitChara(Character chara)
        {
            if (isHit == true)
            {
                return;
            }
            if (chara is Human == false && chara is Dog == false &&
                chara is Bird == false && chara is Monkey == false)
            {
                return;
            }
            if (isTouch == false)
            {
                //複数のドアに対応
                List<GameObject> doorList =
                    mediator.GetGameObjectList(GetLinkedGameObjectID());
                foreach (var d in doorList)
                {
                    ((Door)d).Flip();
                }
            }
            isHit = true;
        }

        public  void  ChangeButtonState(GameTime gameTime)
        {

            if (isHit)
            {
                correntState = Direction.TimeDown; 
                name = buttonState[correntState];
                timer.Update(gameTime); 
            }
            if(timer.IsTime())
            {   
                isHit = false;
                timer = new CountDownTimer(5f);
                correntState = Direction.CanPut;
                name = buttonState[correntState];
                List<GameObject> doorList =
                     mediator.GetGameObjectList(GetLinkedGameObjectID());
                foreach (var d in doorList)
                {
                    ((Door)d).Flip();
                }
            }
        }
        public override void Update(GameTime gameTime)
        { 
            isTouch = (isHit) ? (true) : (false);
            ChangeButtonState(gameTime);            
        }

        public void SetLinkedGameObjectID(GameObjectID id)
        {
            linkedGameOjectID = id;
        }

        public GameObjectID GetLinkedGameObjectID()
        {
            return linkedGameOjectID;
        }

        public override void Change()
        {
        }
    }
}
