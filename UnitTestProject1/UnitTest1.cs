using System.Net.Http;
using System.Threading.Tasks;
using CommonOldProgram;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod, Description("正常系")]
        public async Task APIからのレスポンスが正常なときDBへ値が保存されていること()
        {
            var api = new RequestAPIWrapperMockNormalResponse(23);
            var setting = new CorporateSetting(api)
            {
                Item1 = "設定情報1",
                Item2 = "設定情報2"
            };
            await setting.Save();

            var actual = new CorporateSetting(api);
            await actual.ReadFromDB();

            Assert.AreEqual(setting.CooperatedId, actual.CooperatedId);
            Assert.AreEqual(setting.Item1, actual.Item1);
            Assert.AreEqual(setting.Item2, actual.Item2);
        }

        [TestMethod, Description("異常系1")]
        public async Task APIからのレスポンスが正常でなかったときにHTTPRequestExceptionが発生すること()
        {
            var api = new RequestAPIWrapperMockAbNormalResponse();
            var setting = new CorporateSetting(api)
            {
                Item1 = "設定情報1",
                Item2 = "設定情報2"
            };

            try
            {
                await setting.Save();
                Assert.Fail("例外が吐かれていません");
            }
            catch (HttpRequestException e)
            {
                Assert.AreEqual("API連携に失敗しました", e.Message);
            }
            catch
            {
                Assert.Fail("異なる例外が発生しました");
            }
        }

        [TestMethod, Description("異常系2")]
        public async Task APIからのレスポンスが正常でなかったときDBへ値が保存されていないこと()
        {
            var api = new RequestAPIWrapperMockAbNormalResponse();
            var setting = new CorporateSetting(api)
            {
                Item1 = "設定情報1",
                Item2 = "設定情報2"
            };

            try
            {
                await setting.Save();
                Assert.Fail("例外が吐かれていません");
            }
            catch (HttpRequestException e)
            {
                Assert.AreEqual("API連携に失敗しました", e.Message);
            }
            catch
            {
                Assert.Fail("異なる例外が発生しました");
            }
        }
    }

    public class RequestAPIWrapperMockNormalResponse : IRequestAPIWrapper
    {
        private readonly int cooperatedId;
        public RequestAPIWrapperMockNormalResponse(int testCooperatedId)
        {
            cooperatedId = testCooperatedId;
        }

        public async Task<(bool isSucess, int cooperatedId)> PostCorporateSetting()
        {
            return await Task.Run(() => (true, cooperatedId));
        }
    }

    public class RequestAPIWrapperMockAbNormalResponse : IRequestAPIWrapper
    {
        public async Task<(bool isSucess, int cooperatedId)> PostCorporateSetting()
        {
            return await Task.Run(() => (false, -1));
        }
    }
}
