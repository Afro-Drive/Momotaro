using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Actor;
using Momotaro.Actor.Characters;
using Momotaro.Actor.Characters.BossObj;
using Momotaro.Actor.Characters.Player;
using Momotaro.Actor.GameObjects;
using Momotaro.Device;
using Momotaro.Util;

namespace Momotaro.Scene
{
    /// <summary>
    /// ゲームオブジェクトマネージャという名のオブジェ＆キャラマネージャ
    /// </summary>
    class GameObjectManager : IGameObjectMediator
    {
        private List<GameObject> gameObjectList;
        private List<GameObject> addGameObject;
        private List<Character> characterList;
        private List<Character> addCharacterList;

        private Map map;

        private Score score;

        private Hp hp;

        public GameObjectManager(Score score)
        {
            this.score = score;
            Initialize();
        }

        public void Initialize()
        {

            if (gameObjectList != null)
            {
                gameObjectList.Clear();
            }
            else
            {
                gameObjectList = new List<GameObject>();
            }

            if (addGameObject != null)
            {
                addGameObject.Clear();
            }
            else
            {
                addGameObject = new List<GameObject>();
            }

            if (characterList != null)
            {
                characterList.Clear();
            }
            else
            {
                characterList = new List<Character>();
            }

            if (addCharacterList != null)
            {
                addCharacterList.Clear();
            }
            else
            {
                addCharacterList = new List<Character>();
            }

            hp = new Hp();
        }

        public void Add(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }
            addGameObject.Add(gameObject);
        }

        public void Add(Character character)
        {
            if (character == null)
            {
                return;
            }
            addCharacterList.Add(character);
        }

        public int GetScore()
        {
            return score.GetScore();
        }

        
        public void Add(Map map)
        {
            if (map == null)
            {
                return;
            }
            this.map = map;
        }

        //キャラクターとキャラクターの当たり判定
        private void hitToCharacter()
        {
            foreach (var c1 in characterList)
            {
                foreach (var c2 in characterList)
                {
                    if (c1.Equals(c2) || c1.IsDead() || c2.IsDead())
                    {
                        continue;
                    }

                    if (c1.IsCollision(c2))
                    {
                        c1.HitChara(c2);
                        c2.HitChara(c1);
                    }
                }
            }
        }

        //キャラクターとオブジェクトの当たり判定
        private void hitToObject()
        {
            foreach (var chara in characterList)
            {
                foreach (var obj in gameObjectList)
                {
                    if (chara.IsDead() || obj.IsDead())
                    {
                        continue;
                    }

                    if (chara.GetRectangle().Intersects(obj.GetRectangle()))
                    {
                        chara.HitObj(obj);
                        obj.HitChara(chara);
                    }
                }
            }
        }

        private void removeDeadCharacters()
        {
            gameObjectList.RemoveAll(c => c.IsDead());
            gameObjectList.RemoveAll(c => c.IsChange());
            characterList.RemoveAll(c => c.IsDead());
            characterList.RemoveAll(c => c.IsChange());
        }

        public void Update(GameTime gameTime)
        {
            foreach (var c in gameObjectList)
            {
                c.Update(gameTime);
            }
            foreach (var c in addGameObject)
            {
                gameObjectList.Add(c);
            }
            addGameObject.Clear();

            foreach (var c in characterList)
            {
                c.Update(gameTime);
            }
            foreach (var c in addCharacterList)
            {
                characterList.Add(c);
            }
            addCharacterList.Clear();

            hitToCharacter();
            hitToObject();
            score.Update(gameTime);

            removeDeadCharacters();
        }

        public void Draw(Renderer renderer)
        {
            foreach (var c in gameObjectList)
            {
                c.Draw(renderer);
            }
            foreach (var c in characterList)
            {
                c.Draw(renderer);
            }

            score.Draw(renderer);
            hp.Draw(renderer);
        }

        public void AddGameObject(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }
            addGameObject.Add(gameObject);
        }

        public void AddCharacter(Character character)
        {
            if (character == null)
            {
                return;
            }
            addCharacterList.Add(character);
        }

        public Character GetPlayer()
        {
            Character find = characterList.Find(c => c is Human || c is Dog || c is Monkey || c is Bird);
            if (find != null && !find.IsDead())
            {
                return find;
            }
            return null;
        }

        public bool IsPlayerDead()
        {
            Character find = characterList.Find(c => c is Human || c is Dog || c is Monkey || c is Bird);

            return (find == null || find.IsDead());
        }

        public Map GetMap()
        {
            return map;
        }

        public GameObject GetGameObject(GameObjectID id)
        {
            GameObject find = gameObjectList.Find(c => c.GetID() == id);

            if (find != null && !find.IsDead())
            {
                return find;
            }
            return null;
        }

        public List<GameObject> GetGameObjectList(GameObjectID id)
        {
            List<GameObject> list = new List<GameObject>();

            foreach(var l in map.GetMapList())
            {
                foreach(var obj in l)
                {
                    list.Add(obj);
                }
            }

            list = list.FindAll(c => c.GetID() == id);

            List<GameObject> aliveList = new List<GameObject>();

            foreach (var c in list)
            {
                if (!c.IsDead())
                {
                    aliveList.Add(c);
                }
            }
            return aliveList;
        }

        public void AddScore(int s)
        {
            score.Add(s);
        }

        public List<Character> GetCharacterList()
        {
            return characterList;
        }

        public Vector2 MapSize()
        {
            return new Vector2(map.GetWidth(), map.GetHeight());
        }

        public void ChangeHp(int num)
        {
            hp.Add(num);
        }

        public int GetHp()
        {
            return hp.GetHp();
        }

        public Character GetBoss()
        {
            Character find = characterList.Find(c => c is Boss);
            if (find != null && !find.IsDead())
            {
                return find;
            }
            return null;
        }
    }
}
