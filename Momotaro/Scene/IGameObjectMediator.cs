using Microsoft.Xna.Framework;
using Momotaro.Actor;
using Momotaro.Actor.Characters;
using Momotaro.Actor.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Momotaro.Scene
{
    interface IGameObjectMediator
    {
        void AddCharacter(Character character); //キャラクターの追加
        void AddGameObject(GameObject gameObject); //オブジェクトの追加
        Character GetPlayer(); //プレイヤーの取得
        bool IsPlayerDead(); //プレイヤーが死んでいるか？
        Map GetMap(); //マップの取得
        Character GetBoss();
        GameObject GetGameObject(GameObjectID id); //オブジェクトIDの取得
        List<GameObject> GetGameObjectList(GameObjectID id); //指定したIDを持つオブジェクトの取得
        void AddScore(int score); //スコア追加
        List<Character> GetCharacterList();
        Vector2 MapSize();
        void ChangeHp(int hp);
    }
}
