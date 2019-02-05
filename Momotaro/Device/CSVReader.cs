using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Momotaro.Device
{
    /// <summary>
    /// CSVファイル読み込みクラス
    /// </summary>
    class CSVReader
    {
        private List<string[]> stringData;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CSVReader()
        {
            stringData = new List<string[]>();
        }

        /// <summary>
        /// CSVファイルの読み込み
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="path"></param>
        public void Read(string filename,string path="./")
        {
            //リストのクリア
            Clear();

            //例外処理
            try
            {
                //CSVファイルを開く
                using (var sr = new System.IO.StreamReader(@"Content/" + path + filename))
                {
                    //ストリームの末尾まで繰り返す
                    while(!sr.EndOfStream)
                    {
                        //１行読み込む
                        var line = sr.ReadLine();
                        //カンマごとに分けて配列に格納する
                        var values = line.Split(',');

                        //リストに読み込んだ１行を追加
                        stringData.Add(values);

#if DEBUG
                    //出力する
                    foreach(var v in values)
                        {
                            System.Console.Write("{0}", v);
                        }
                        System.Console.WriteLine();
#endif
                    }
                }
            }
            catch(System.Exception e)
            {
                //ファイルオープンが失敗したとき
                System.Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// リスト内をクリア
        /// </summary>
        public void Clear()
        {
            stringData.Clear();
        }

        /// <summary>
        /// string配列のListを取得
        /// </summary>
        /// <returns></returns>
        public List<string[]> GetData()
        {
             return stringData;
        }

        /// <summary>
        /// stringのジャグ配列で取得
        /// </summary>
        /// <returns>stringのジャグ配列</returns>
        public string[][] GetArrayData()
        {
            return stringData.ToArray();
        }

        /// <summary>
        /// int型のジャグ配列で取得
        /// </summary>
        /// <returns>int型のジャグ配列</returns>
        public int[][] GetIntData()
        {
            //string型のジャグ配列で取得
            var data = GetArrayData();
            //縦の長さ取得
            int row = data.Count();

            //int型のジャグ配列生成（縦の長さ指定）
            int[][] intData = new int[row][];
            //それぞれの列の配列生成（横の長さ指定）
            for (int i = 0; i < row; i++)
            {
                //string配列データの横の長さ取得
                int col = data[i].Count();
                //その長さでint型配列生成
                intData[i] = new int[col];
            }
            //縦のループ
            for (int y = 0; y < row; y++)
            {
                //横のループ
                for (int x = 0; x < intData[y].Count(); x++)
                {
                    //対応するstringデータをintに変換して代入
                    intData[y][x] = int.Parse(data[y][x]);
                }
            }
            //変換が終わったデータを戻す
            return intData;
        }

        /// <summary>
        /// stringの配列取得（Matrix型）
        /// </summary>
        /// <returns>stringの配列</returns>
        public string[,] GetStringMatrix()
        {
            //string型のジャグ配列で取得
            var data = GetArrayData();
            //今回はMatrix型なので、縦横の長さ取得
            int row = data.Count();
            int col = data[0].Count();

            //縦横の長さがそろった配列を生成
            string[,] result = new string[row, col];
            //縦のループ
            for (int y = 0; y < row; y++)
            {
                //横のループ
                for (int x = 0; x < col; x++)
                {
                    //対応位置のデータ代入
                    result[y, x] = data[y][x];
                }
            }
            //結果を戻す
            return result;
        }

        /// <summary>
        /// int型の２次元配列の取得（Matrix型）
        /// </summary>
        /// <returns>int型の２次元配列</returns>
        public int[,] GetIntMatrix()
        {
            //int型のジャグ配列データの取得
            var data = GetIntData();
            //今回はMatrix型と考えて縦横の長さを取得
            int row = data.Count();
            int col = data[0].Count();

            //縦横の長さがそろった二次元配列を生成
            int[,] result = new int[row, col];
            //縦のループ
            for (int y = 0; y < row; y++)
            {
                //横のループ
                for (int x = 0; x < col; x++)
                {
                    //対応位置のデータ代入
                    result[y, x] = data[y][x];
                }
            }
            //結果を戻す
            return result;
        }
    }
}
