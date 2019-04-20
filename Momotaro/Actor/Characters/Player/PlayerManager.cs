using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Device;
using Momotaro.Scene;

namespace Momotaro.Actor.Characters.Player
{ 
    class PlayerManager
    {
        private List<Character> playerList; //プレイヤーリスト
        private int pNum; //リストからの取り出し用番号
        private int pCnt; //切り替え用番号

        private IGameObjectMediator mediator; //仲介者

        private Sound sound;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mediator"></param>
        public PlayerManager(IGameObjectMediator mediator)
        {
            this.mediator = mediator;
            sound = GameDevice.Instance().GetSound();

            //リストの生成
            if (playerList != null)
            {
                playerList.Clear();
            }
            else
            {
                playerList = new List<Character>();
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            pNum = 0;
            pCnt = 0;
        }

        /// <summary>
        /// 初期キャラの設定
        /// </summary>
        /// <param name="position"></param>
        public void SetStartPlayer(Vector2 position)
        {
            Character newChara = GetPlayer();
            newChara.SetPosition(position);
            mediator.AddCharacter(newChara);
        }

        /// <summary>
        /// リストにプレイヤー追加
        /// </summary>
        /// <param name="obj"></param>
        public void Add(Character obj)
        {
            if(obj==null)
            {
                return;
            }
            playerList.Add(obj);
        }

        /// <summary>
        /// キャラクター切り替え
        /// </summary>
        /// <param name="num">切り替えの方向</param>
        public void Change(int num)
        {
            if (GameData.pCount == 1)
            {
                return; //キャラが１体のとき、処理をしない
            }
            else
            {
                sound.PlaySE("p_change");
                //現在動いているキャラから位置を取得
                Vector2 position = playerList[pNum].GetPosition();

                //当たり判定の違いによって位置がずれて埋まってしまうのを修正(1)
                //キャラクターの右下の位置を取得する
                position = new Vector2(position.X, position.Y + playerList[pNum].GetHitH() + playerList[pNum].GetHeigthMargin());

                playerList[pNum].Change();

                //リスト番号の割り出し
                pCnt += num;
                if (pCnt < 0)
                {
                    pCnt = pCnt + GameData.pCount;
                }
                pNum = pCnt % GameData.pCount;

                //追加キャラをリストから取得
                Character addPlayer = GetPlayer();

                //当たり判定の違いによって位置がずれて埋まってしまうのを修正(2)
                //変更元のキャラの右下の位置をもとにポジションを取得
                position = new Vector2(position.X, position.Y - playerList[pNum].GetHitH() - playerList[pNum].GetHeigthMargin());

                //取得した位置を設定
                addPlayer.SetPosition(position);
                //切り替えフラグをfalseに設定
                addPlayer.ChangeFlagTurn();
                //次のキャラを追加
                mediator.AddCharacter(addPlayer);
            }
        }

        /// <summary>
        /// 現在のプレイヤーを返す
        /// </summary>
        /// <returns></returns>
        public Character GetPlayer()
        {
            return playerList[pNum];
        }

        /// <summary>
        /// クリア状態かどうか
        /// </summary>
        /// <returns></returns>
        public bool IsClear()
        {
            return playerList[pNum].IsClear();
        }
    }
}
