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

    public interface IRequestAPIWrapper
    {
        Task<(bool isSucess, int cooperatedId)> PostCorporateSetting();
    }
    
    // 外部へ依存している部分を切り出す
    public class RequestAPIWrapper : IRequestAPIWrapper
    {
        private readonly HttpClient client = new HttpClient();
        private readonly string apiUrl = "https://example.com/2";

        public async Task<(bool isSucess, int cooperatedId)> PostCorporateSetting()
        {
            var json = JsonConvert.SerializeObject(this);
            var content = new StringContent(json, Encoding.UTF8, @"application/json");
            var response = await client.PostAsync(apiUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var cooperatedId = int.Parse(await response.Content.ReadAsStringAsync());
                return (true, cooperatedId);
            }

            return (false, -1);
        }
    }

    public class CorporateSetting
    {
        private static string item1 = "";
        private static string item2 = "";
        private static int cooperatedId = -1;

        public string Item1 { get; set; }
        public string Item2 { get; set; }
        /// <summary>
        /// 連携ID 外部APIにて発行される
        /// </summary>
        public int CooperatedId { get; private set; }

        private readonly IRequestAPIWrapper api;

        // 切り出した部分をここへ注入
        // 単体テスト時はここへモックを注入したいがRequestAPIWrapper型なのでそれ以外の型を注入できない
        // なのでインタフェースへ変更する
        public CorporateSetting(IRequestAPIWrapper api)
        {
            this.api = api;
        }

        // APIへ設定情報を送り、その後に同様の情報をDBへ保存する
        public async Task Save()
        {
            var (isSucess, cooperatedId) = await api.PostCorporateSetting();
            if (!isSucess)
            {
                throw new HttpRequestException("API連携に失敗しました");
            }

            CooperatedId = cooperatedId;

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
