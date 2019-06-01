using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Momotaro.Device;
using Momotaro.Scene;

namespace Momotaro.Actor.Characters.Player
{
    /// <summary>
    /// Playableキャラの列挙型
    /// 仲間になる順番で列挙されており、ステージ番号と対応する
    /// </summary>
    enum PlayerName
    {
        Momotaro,
        Inu,
        Kiji,
        Saru,
    }

    class PlayerManager
    {
        private Dictionary<PlayerName, IPlayable> playerDict; //プレイヤーディクショナリ
        private List<IPlayable> addPlayers;
        private Character entryPlayer; //現在使用中のキャラ

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
            if (playerDict != null)
            {
                playerDict.Clear();
                return;
            }
            playerDict = new Dictionary<PlayerName, IPlayable>();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            addPlayers = new List<IPlayable>()
            {
                new Human (Vector2.Zero, GameDevice.Instance(), mediator),
                new Dog   (Vector2.Zero, GameDevice.Instance(), mediator),
                new Bird  (Vector2.Zero, GameDevice.Instance(), mediator),
                new Monkey(Vector2.Zero, GameDevice.Instance(), mediator)
            };
            Add(PlayerName.Momotaro, addPlayers[GameData.pCount]);
            entryPlayer = (Character)playerDict[PlayerName.Momotaro];
        }

        /// <summary>
        /// 初期キャラの設定
        /// </summary>
        /// <param name="position"></param>
        public void SetStartPlayer(Vector2 position)
        {
            entryPlayer.SetPosition(position);
            mediator.AddCharacter(entryPlayer);
        }

        /// <summary>
        /// リストにプレイヤー追加
        /// </summary>
        /// <param name="playerValue"></param>
        public void Add(PlayerName playerKey, IPlayable playerValue)
        {
            if (playerValue == null ||
                playerDict.ContainsKey(playerKey))
            {
                return;
            }
            playerDict.Add(playerKey, playerValue);
        }

        /// <summary>
        /// キャラクター切り替え
        /// </summary>
        /// <param name="num">切り替えの方向</param>
        public void Change(PlayerName next)
        {
            if (!playerDict.ContainsKey(next) ||              
                playerDict[next] == entryPlayer)
                return; 

            sound.PlaySE("p_change");

            Character nextPlayer = (Character)playerDict[next];
            //現在動いているキャラから位置を取得
            Vector2 position = entryPlayer.GetPosition();

            //当たり判定の違いによって位置がずれて埋まってしまうのを修正(1)
            //キャラクターの右下の位置を取得する
            position = new Vector2(
                position.X,
                position.Y + entryPlayer.GetHitH() + entryPlayer.GetHeigthMargin());

            ((IPlayable)entryPlayer).Change();

            //当たり判定の違いによって位置がずれて埋まってしまうのを修正(2)
            //変更元のキャラの右下の位置をもとにポジションを取得
            position = new Vector2(
                position.X,
                position.Y - nextPlayer.GetHitH() - nextPlayer.GetHeigthMargin());

            //取得した位置を設定
            nextPlayer.SetPosition(position);
            //操作キャラを上書き
            entryPlayer = nextPlayer;
            //切り替えフラグをfalseに設定
            entryPlayer.ChangeFlagTurn();
            //次のキャラを追加
            mediator.AddCharacter(entryPlayer);
        }

        public void AcceptInput()
        {
            if (Input.GetKeyTrigger(PlayerIndex.One, Buttons.DPadRight) ||
                Input.GetKeyTrigger(Keys.D))
            {
                Change(PlayerName.Momotaro);
            }
            else if (Input.GetKeyTrigger(PlayerIndex.One, Buttons.DPadUp) ||
                Input.GetKeyTrigger(Keys.W))
            {
                Change(PlayerName.Inu);
            }
            else if (Input.GetKeyTrigger(PlayerIndex.One, Buttons.DPadLeft) ||
                Input.GetKeyTrigger(Keys.A))
            {
                Change(PlayerName.Kiji);
            }
            else if (Input.GetKeyTrigger(PlayerIndex.One, Buttons.DPadDown) ||
                Input.GetKeyTrigger(Keys.S))
            {
                Change(PlayerName.Saru);
            }
        }

        /// <summary>
        /// クリア状態かどうか
        /// </summary>
        /// <returns></returns>
        public bool IsClear()
        {
            return entryPlayer.IsClear();
        }

        public void Update(GameTime gameTime)
        {
            Add((PlayerName)GameData.pCount, addPlayers[GameData.pCount]);
        }
    }
}
