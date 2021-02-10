using SagecomRouterAPI.Model;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SagecomRouterAPI
{
    public class Router
    {
        private int _sessionId = 0;
        private int _requestId = -1;
        private long _nonce = 0;
        private string _serverNonce = "";
        private string _authKey = "";
        private string _passwordHash = "";

        private void GenerateNonce()
        {
            Random rnd = new Random();

            _nonce = (long)Math.Floor(rnd.NextDouble() * 10000000000.0);
        }

        private string GenerateHash(string val)
        {
            var mD5 = new MD5CryptoServiceProvider();
            var hash = mD5.ComputeHash(Encoding.UTF8.GetBytes(val));
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
        }

        private string GenerateAuthKey(string username)
        {
            var credHash = GenerateHash($"{username}:{_serverNonce}:{_passwordHash}");

            return GenerateHash($"{credHash}:{_requestId}:{_nonce}:JSON:/cgi/json-req");
        }

        private string PrepareRequest(string req, string actions)
        {
            req = req.Replace("%%SESSIONID%%", _sessionId.ToString());
            req = req.Replace("%%ACTIONS%%", actions);
            req = req.Replace("%%AUTHKEY%%", _authKey);
            req = req.Replace("%%NONCE%%", _nonce.ToString());
            req = req.Replace("%%REQUESTID%%", _requestId.ToString());

            return req;
        }

        private async Task<T> SendRequest<T>(string requestDat, string host)
        {
            var apiHost = $"http://{host}/cgi/json-req";
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiHost);

            var req = File.ReadAllText(@".\Requests\request.json");

            req = PrepareRequest(req, requestDat);

            request.Content = new StringContent($"req={req}", Encoding.UTF8, "application/json");

            var k = await client.SendAsync(request);

            var result = await k.Content.ReadAsStringAsync();
            Console.WriteLine(result);
            var opt = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<T>(result, opt);
        }

        public async Task<bool> Login(string username, string password, string host)
        {
            _authKey = GenerateAuthKey(username);

            var loginReq = File.ReadAllText(@".\Requests\login-actions.json");

            loginReq = loginReq.Replace("%%USERNAME%%", username);

            var reply = await SendRequest<Response>(loginReq, host);

            if (reply.Reply.Error.Code == reply.Reply.Success_Code)
            {
                // extract session ID and server nonce 
                _serverNonce = ((JsonElement)reply?.Reply?.Actions?.First()?.Callbacks?.First()?.Parameters["nonce"]).GetString();
                _sessionId = ((JsonElement)reply?.Reply?.Actions?.First()?.Callbacks?.First()?.Parameters["id"]).GetInt32();

                return true;
            }

            return false;
        }

        public async Task<object> HandleRequest(Request request, string username, string password, string host)
        {
            _requestId++;
            GenerateNonce();

            _passwordHash = GenerateHash(password);

            var reqAttr = GetAttribute<RequestDefAttribute>(request);

            if (reqAttr != null)
            {
                return await Request(username, password, host, reqAttr.ConfigFile);
            }

            return null;
        }
        private async Task<object> Request(string username, string password, string host, string reqFile)
        {
            await Login(username, password, host);

            _requestId++;

            _authKey = GenerateAuthKey(username);

            var loginReq = File.ReadAllText(reqFile);

            return await SendRequest<Response>(loginReq, host);
        }

        private static T GetAttribute<T>(object value) where T : class
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value);
            return enumType.GetField(name).GetCustomAttributes(false).OfType<T>().SingleOrDefault();
        }
    }
}
