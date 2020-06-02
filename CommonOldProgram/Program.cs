using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CommonOldProgram
{
    class Program
    {
        public static void Main(params string[] args)
        {

        }
    }

    public class CorporateSetting
    {
        // DBを擬似的に表現するためのフィールド
        private static string item1 = "";
        private static string item2 = "";
        private static int cooperatedId = -1;
        // 

        private readonly string apiUrl = "https://example.com/2";
        public string Item1 { get; set; }
        public string Item2 { get; set; }
        /// <summary>
        /// 連携ID 外部APIにて発行される
        /// </summary>
        public int CooperatedId { get; private set; }

        // APIへ設定情報を送り、その後に同様の情報をDBへ保存する
        public async Task Save()
        {
            var client = new HttpClient();
            var json = JsonConvert.SerializeObject(this);
            var content = new StringContent(json, Encoding.UTF8, @"application/json");
            var response = await client.PostAsync(apiUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("API連携に失敗しました");
            }

            CooperatedId = int.Parse(await response.Content.ReadAsStringAsync());

            await SaveDB();
        }

        private async Task SaveDB()
        {
            await Task.Run(() =>
            {
                /*DBへ保存*/
                item1 = Item1;
                item2 = Item2;
                cooperatedId = CooperatedId;
            });
        }

        public async Task ReadFromDB()
        {
            await Task.Run(() =>
            {
                Item1 = item1;
                Item2 = item2;
                CooperatedId = cooperatedId;
            });
        }
    }
}
