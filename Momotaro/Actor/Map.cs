using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Device;
using Momotaro.Util;
using Momotaro.Scene;
using Momotaro.Actor.GameObjects;
using Momotaro.Actor.GameObjects.DamageObj;
using Momotaro.Actor.Characters;
using Momotaro.Actor.AI;
using Momotaro.Actor.GameObjects.PlayerAddObj;
using Momotaro.Actor.GameObjects.ItemObj;

namespace Momotaro.Actor
{
    /// <summary>
    /// マップクラス
    /// </summary>
    class Map
    {
        private List<List<GameObject>> mapList; //ListのListで縦横の２次元配列を表現
        private GameDevice gameDevice; //ゲームデバイス
        private IGameObjectMediator mediator;

        //【追加】ブロックの番号
        private List<int> blkNum;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="gameDevice"></param>
        public Map(GameDevice gameDevice, IGameObjectMediator mediator)
        {
            mapList = new List<List<GameObject>>(); //マップの実体生成
            this.gameDevice = gameDevice;
            this.mediator = mediator;

            //【追加】ブロック番号のリストを初期化（要素番号と要素の値がずれているのに注意）
            blkNum = new List<int>
               { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        }

        /// <summary>
        /// ブロックの追加
        /// </summary>
        /// <param name="lineCnt"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private List<GameObject> AddBlock(int lineCnt, string[] line)
        {
            //コピー元オブジェクト登録用でディクショナリ
            Dictionary<string, GameObject> objectDict = new Dictionary<string, GameObject>();
            #region 【０～９】ブロック・空白
            objectDict.Add("0", new Space(Vector2.Zero, gameDevice)); //スペース
            //通常ブロック（マップチップ切り取り箇所の指定あり）
            objectDict.Add("1", new Block(Vector2.Zero, gameDevice, blkNum[1 - 1]));
            objectDict.Add("2", new Block(Vector2.Zero, gameDevice, blkNum[2 - 1]));
            objectDict.Add("3", new Block(Vector2.Zero, gameDevice, blkNum[3 - 1]));
            objectDict.Add("4", new Block(Vector2.Zero, gameDevice, blkNum[4 - 1]));
            objectDict.Add("5", new Block(Vector2.Zero, gameDevice, blkNum[5 - 1]));
            objectDict.Add("6", new Block(Vector2.Zero, gameDevice, blkNum[6 - 1]));
            objectDict.Add("7", new Block(Vector2.Zero, gameDevice, blkNum[7 - 1]));
            objectDict.Add("8", new Block(Vector2.Zero, gameDevice, blkNum[8 - 1]));
            objectDict.Add("9", new Block(Vector2.Zero, gameDevice, blkNum[9 - 1]));
            #endregion 【０～９】ブロック・空白

            #region　【１０～２９】特殊ブロック・ギミック系
            objectDict.Add("14", new DeathBlock(Vector2.Zero, gameDevice)); //デスブロック
            objectDict.Add("16", new Button(Vector2.Zero, gameDevice, mediator));　//ボタン
            objectDict.Add("17", new Door(Vector2.Zero, gameDevice));　//ドア
            //【２０～２９】プレイアブルキャラクター
            objectDict.Add("20", new DogAddBlock(Vector2.Zero, gameDevice)); //イヌ追加ブロック
            objectDict.Add("22", new MonkeyAddBlock(Vector2.Zero, gameDevice)); //サル追加ブロック
            objectDict.Add("21", new BirdAddBlock(Vector2.Zero, gameDevice)); //キジ追加ブロック
            #endregion　【１０～２９】特殊ブロック・ギミック系

            #region　【３０～３９】アイテム
            objectDict.Add("31", new Peach(Vector2.Zero, gameDevice, mediator)); //桃
            objectDict.Add("32", new Item(Vector2.Zero, gameDevice, mediator)); //アイテム
            #endregion【３０～３９】アイテム

            #region【５０～５９】被ダメージオブジェクト
            objectDict.Add("51", new Trap(Vector2.Zero, gameDevice, mediator)); //炎
            objectDict.Add("52", new Thorn(Vector2.Zero, gameDevice)); //刺
            objectDict.Add("53", new Space(Vector2.Zero, gameDevice)); //落石、空白
            #endregion【５０～５９】被ダメージオブジェクト

            #region【９０～９９】スタート・ゴール
            objectDict.Add("98", new Space(Vector2.Zero, gameDevice)); //開始位置
            objectDict.Add("99", new ClearBlock(Vector2.Zero, gameDevice)); //クリアブロック
            #endregion【９０～９９】スタート・ゴール

            #region【１００～１９９】テキストファイルのメッセージボックス
            //ステージ01
            objectDict.Add("111", new MessageBox(Vector2.Zero, gameDevice, "momotaro_Introduct"));
            objectDict.Add("112", new MessageBox(Vector2.Zero, gameDevice, "attack_Tutorial"));
            objectDict.Add("113", new MessageBox(Vector2.Zero, gameDevice, "dog_Introduct"));

            //ステージ02
            objectDict.Add("121", new MessageBox(Vector2.Zero, gameDevice, "bird_Introduct"));

            //ステージ03
            objectDict.Add("131", new MessageBox(Vector2.Zero, gameDevice, "monkey_Introduct"));
        
            //ステージ04
            objectDict.Add("141", new MessageBox(Vector2.Zero, gameDevice, "owl_Cheer"));
            objectDict.Add("142", new MessageBox(Vector2.Zero, gameDevice, "bigJump_Tutorial"));
            #endregion【１００～１９９】テキストファイルのメッセージボックス

            //作業用リスト
            List<GameObject> workList = new List<GameObject>();

            int colCnt = 0; //列カウント用
            //渡された1行から1つずつ作業リストに登録
            foreach (var s in line)
            {
                try
                {
                    //ディクショナリから元データを取り出し、クローン機能で複製
                    GameObject work = (GameObject)objectDict[s].Clone();
                    work.Position = new Vector2(colCnt * work.Height,
                        lineCnt * work.Width);
                    //if(work is Button)
                    //{
                    //    ((Button)(work)).SetLinkedGameObjectID(GameObjectID.Door);
                    //}
                    workList.Add(work);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                //列カウンタを増やす
                colCnt += 1;
            }
            return workList;
        }

        /// <summary>
        /// エネミーを生成し、エネミー生成後のcsvデータを返すメソッド
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string[][] AddEnemy(string[][] data)
        {
            for (int y = 0; y < data.Length; y++)
            {
                for (int x = 0; x < data[y].Length; x++)
                {
                    if (int.Parse(data[y][x]) >= 70 && int.Parse(data[y][x]) <= 79)
                    {
                        Vector2 enemyPositon = new Vector2(x * 64, y * 64);//エネミーの場所

                        switch (data[y][x])
                        {
                            case "70":
                                mediator.AddCharacter(new Enemy("teki", enemyPositon, AIName.Turn, gameDevice, mediator));
                                break;
                            case "71":
                                mediator.AddCharacter(new Enemy("teki", enemyPositon, AIName.Normal, gameDevice, mediator));
                                break;
                            case "72":
                                mediator.AddCharacter(new Enemy("teki", enemyPositon, AIName.Shot, gameDevice, mediator));
                                break;
                        }

                        data[y][x] = "0"; //30以上40以下の場所は0になります。
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// CSVReaderを使ってMapの読み込み
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="path"></param>
        public void Load(string filename, string path = "./")
        {
            CSVReader csvReader = new CSVReader();
            csvReader.Read(filename, path);

            var data = csvReader.GetArrayData(); //string[][]型で取得

            data = AddEnemy(data);

            //1行ごとmapListに追加していく
            for (int lineCnt = 0; lineCnt < data.Count(); lineCnt++)
            {
                mapList.Add(AddBlock(lineCnt, data[lineCnt]));
            }
        }

        /// <summary>
        /// マップリストのクリア
        /// </summary>
        public void Unload()
        {
            mapList.Clear();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            HitObj();

            foreach (var list in mapList) //listはList<GameObject>型
            {
                foreach (var obj in list) //objはGameObject型
                {
                    //objがSpaceクラスのオブジェクトなら次へ
                    if (obj is Space)
                    {
                        continue;
                    }

                    //更新
                    obj.Update(gameTime);
                }
            }         
        }

        //調べる位置がブロックかどうか
        public bool IsBlock(Vector2 position)
        {
            Point arrayPos = new Point(
                (int)position.X / 64, (int)position.Y / 64);

            //配列位置の番号を取り出す
            GameObject obj = mapList[arrayPos.Y][arrayPos.X];

            //番号がブロック番号リストに含まれていれば
            if (obj is Block || 
                (obj is Door && !((Door)obj).GetStatus())||
                obj is Trap)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// マップのオブジェクトとプレイヤーの衝突判定
        /// </summary>
        private void HitObj()
        {
            foreach (var chara in mediator.GetCharacterList())
            {
                Vector2 pos = chara.Position;
                pos = new Vector2((int)pos.X / 64, (int)pos.Y / 64);

                int y = (int)pos.Y;
                int x = (int)pos.X;

                for (int i = y - 1; i <= y + 1; i++)
                {
                    for (int j = x - 1; j <= x + 1; j++)
                    {
                        GameObject obj = mapList[i][j];

                        if (chara.GetRectangle().Intersects(obj.GetRectangle()))
                        {
                            chara.HitObj(obj);
                            obj.HitChara(chara);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 描画
        /// </summary>
        /// <param name="renderer">描画オブジェクト</param>
        public void Draw(Renderer renderer)
        {
            foreach (var list in mapList)
            {
                foreach (var obj in list)
                {
                    if (obj is Space)
                    {
                        continue;
                    }

                    obj.Draw(renderer);
                }
            }
        }


        public List<List<GameObject>> GetMapList()
        {
            return mapList;
        }

        public int GetWidth()
        {
            int col = mapList[0].Count;
            int width = col * mapList[0][0].Width;
            return width;
        }

        public int GetHeight()
        {
            int row = mapList.Count;
            int height = row * mapList[0][0].Height;
            return height;
        }
    }
}
