using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Momotaro.Actor;
using Momotaro.Device;
using Momotaro.Scene;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Momotaro.Util
{
    /// <summary>
    /// ゲームオブジェクトがまとめられたCSVデータの解析
    /// </summary>
    class GameObjectCSVParser
    {
        private CSVReader csvReader; //CSV読み込み用オブジェクト
        private List<GameObject> gameObjects; //ゲームオブジェクトのリスト

        //デリゲート宣言（メソッドを変数に保存するための型宣言）
        //戻り値の型がGameObject、引数はList<string>のメソッドを
        //保存できるiFunction型を宣言
        private delegate GameObject iFunction(List<string> data);

        //文字列とiFunction型をディクショナリで保存
        private Dictionary<string, iFunction> functionTable;

        private IGameObjectMediator mediator; //仲介者

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mediator"></param>
        public GameObjectCSVParser(IGameObjectMediator mediator)
        {
            this.mediator = mediator;

            csvReader = new CSVReader();
            gameObjects = new List<GameObject>();
            functionTable = new Dictionary<string, iFunction>();

            //ディクショナリにデータを追加
            //文字列はクラス名、それと実行用メソッド
            //functionTable.Add("Block", newBlock);
            //functionTable.Add("Trap", newTrap);
        }


        public List<GameObject> Parse(string filename, string path = "./")
        {
            gameObjects.Clear();

            csvReader.Read(filename, path);
            var data = csvReader.GetData();

            foreach(var line in data)
            {
                if(line[0] == "#")
                {
                    continue;
                }
                if(line[0] == "")
                {
                    continue;
                }

                var temp = line.ToList();
                temp.RemoveAll(s => s == "");

                gameObjects.Add(functionTable[line[0]](temp));
            }

            return gameObjects;
        }


        //以下に追加オブジェクトの解析と生成の処理を追加
        ///// <summary>
        ///// 通常ブロックの解析と生成
        ///// </summary>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //private Block newBlock(List<string> data)
        //{
        //    Debug.Assert(
        //        (data.Count == 3),
        //        "CSVデータを確認してください。");

        //    return new Block(
        //        new Vector2(float.Parse(data[1]), float.Parse(data[2])) * 64,
        //        GameDevice.Instance());
        //}

        ///// <summary>
        ///// 罠の解析と生成
        ///// </summary>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //private Trap newTrap(List<string> data)
        //{
        //    Debug.Assert(
        //        (data.Count == 6),
        //        "CSVデータを確認してください。");

        //    return new Trap(
        //        new Vector2(float.Parse(data[1]), float.Parse(data[2])) * 64,
        //        GameDevice.Instance(),
        //        mediator,
        //        float.Parse(data[3]),
        //        float.Parse(data[4]),
        //        int.Parse(data[5]));
        //}
    }
}
