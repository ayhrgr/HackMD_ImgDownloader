using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HackMD_ImgDownloader.http
{
    public class HttpClientManager : IDisposable
    {
        private static object m_lok = new object();

        public void Dispose()
        {
            lock (m_lok)
            {
                internal_Dispose();
            }
        }

        private void internal_Dispose()
        {
            if (m_client != null)
            {
                m_client.Dispose();
                m_client = null;
            }
            if (m_handler != null)
            {
                m_handler.Dispose();
                m_handler = null;
            }
            this.m_username = null;
        }

        public HttpClient GetClient(
            string username = null,
            string strBaseAddress = null
            )
        {
            lock (m_lok)
            {
                if (username != this.m_username)
                {
                    // ユーザ名が異なる場合は、再ログインしてセッションを取得しなおす必要がある。
                    internal_Dispose();
                    this.m_username = username;
                }

                if (m_client == null)
                {
                    this.m_handler = new HttpClientHandler();

                    var cookieContainer = new CookieContainer();
                    this.m_handler.CookieContainer = cookieContainer;

                    this.m_handler.UseCookies = false; // trueにするとCookieがキャッシュされてしまうのでfalseにする
                    this.m_handler.UseCookies = true;

                    this.m_handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                    // Location ヘッダがある場合に自動的にリダイレクトするかどうか。
                    this.m_handler.AllowAutoRedirect = true;

                    this.m_client = HttpClientFactory.Create(
                        this.m_handler,
                        new HttpLoggingHandler()
                        );

                    if (   this.m_client.BaseAddress == null 
                        && strBaseAddress != null
                        )
                    {
                        var baseAddress = new Uri(strBaseAddress);
                        this.m_client.BaseAddress = baseAddress;
                    }

                }
                return this.m_client;
            }
        }

        public void ClearCookie(
            string[] aryUrl = null
            )
        {
            lock (m_lok)
            {
                if (   this.m_client != null 
                    && this.m_handler != null
                    )
                {
                    if (aryUrl == null && aryUrl.Any() == false)
                    {
                        aryUrl = new string[]
                        {
                            this.m_client.BaseAddress.OriginalString
                        };
                    }

                    foreach (var strUrl in aryUrl)
                    {
                        CookieCollection collectionCookie = m_handler
                            .CookieContainer
                            .GetCookies(new Uri(strUrl));
                        //string ASPNETSessionId = "";
                        foreach (Cookie cook in collectionCookie)
                        {
                            //Log.Logger.Information("GetCookies:" + cook.Name + "=" + cook.Value);
                            cook.Expired = true;

                            //if (cook.Name == "ASP.NET_SessionId")
                            //{
                            //    ASPNETSessionId = cook.Value;
                            //}
                        }
                    }
                }
            }
        }

        public HttpClientManager()
            : base()
        {
            this.m_client = null;
            this.m_handler = null;
            this.m_username = null;
        }


        //----------------------------------------------------------------------------//
        // ※以下は、PropetyToolによって自動生成されました。手動による変更禁止！！    //
        //----------------------------------------------------------------------------//
        private HttpClient m_client;
        private HttpClientHandler m_handler;
        private string m_username;
        /// <summary>
        ///
        /// </summary>
        /// <param name="client   "></param>
        /// <param name="handler  "></param>
        /// <param name="username "></param>
        public HttpClientManager(
        HttpClient client,
        HttpClientHandler handler,
        string username
        )
         : base()
        {
            this.m_client = client;
            this.m_handler = handler;
            this.m_username = username;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="org"></param>
        public HttpClientManager(
        HttpClientManager org
        )
         : base()
        {
            this.m_client = org.m_client;
            this.m_handler = org.m_handler;
            this.m_username = org.m_username;
        }
        public HttpClient Client { get { return this.m_client; } protected set { this.m_client = value; } }
        public HttpClientHandler Handler { get { return this.m_handler; } protected set { this.m_handler = value; } }
        public string Username { get { return this.m_username; } protected set { this.m_username = value; } }
        public override string ToString()
        {
            return new StringBuilder()
                .Append("client：").Append(m_client).AppendLine("、")
                .Append("handler：").Append(m_handler).AppendLine("、")
                .Append("username：").Append(m_username).AppendLine("、")
                .ToString();
        }
    }
}
