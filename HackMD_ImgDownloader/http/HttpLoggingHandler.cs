using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HackMD_ImgDownloader.http
{
    public class HttpLoggingHandler : DelegatingHandler
    {
        public HttpLoggingHandler()
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken
            )
        {
            try
            {
                string content = await (request.Content?.ReadAsStringAsync() ?? Task.FromResult(string.Empty));
                // ログ残すよ！
                StringBuilder sbrReq = new StringBuilder();
                sbrReq.Append(@"-----------------------------------------").AppendLine();
                sbrReq.Append(@"【これが俺のリクエストだ！】").AppendLine();
                sbrReq.Append(@"【種類！】").Append(request.Method).AppendLine();
                sbrReq.Append(@"【場所！】").Append(request.RequestUri).AppendLine();
                sbrReq.Append(@"【リクエストヘッダー！】").AppendLine();
                sbrReq.Append(FormattingHeaders(request.Headers)).AppendLine();

                //sbrReq.Append(@"【request Options】").AppendLine();
                //if (request.Options != null)
                //{
                //    foreach (var item in request.Options)
                //    {
                //        sbrReq.Append("Key:").Append(item.Key).Append("Val:").Append(item.Value).AppendLine();
                //    }
                //}

                sbrReq.Append(@"【request Version】").AppendLine();
                if (request.Version != null)
                {
                    sbrReq.Append(request.Version).AppendLine();
                }

                //sbrReq.Append(@"【request VersionPolicy】").AppendLine();
                //{
                //    sbrReq.Append(request.VersionPolicy).AppendLine();
                //}

                sbrReq.Append(@"【コンテンツヘッダー！】").AppendLine();
                sbrReq.Append(FormattingHeaders(request.Content)).AppendLine();
                sbrReq.Append(@"【内容！】").AppendLine();
                sbrReq.Append(content).AppendLine();
                sbrReq.Append(@"-----------------------------------------").AppendLine();

                //log.Information(sbrReq.ToString());
            }
            catch //(Exception ex)
            {
                //log.Error(ex.ToString());
            }

            HttpResponseMessage response = null;
            try
            {
                // とばすよ！
                response = await base.SendAsync(request, cancellationToken);

                // ログ残すよ！
//                log.Information($@"
//-----------------------------------------
//【これがお前のレスポンスか！？】
//【ステータスコード！】{(int)response.StatusCode} {response.StatusCode}
//【理由！】{response.ReasonPhrase}
//【レスポンスヘッダー！】
//{FormattingHeaders(response.Headers)}
//【コンテンツヘッダー！】
//{FormattingHeaders(response.Content)}
//【内容！】
//{await ReadContentAsUtf8StringAsync(response.Content)}
//-----------------------------------------");
            }
            catch //(Exception ex)
            {
//                log.Error(ex.ToString());
            }

            return response;
        }

        private string FormattingHeaders(HttpHeaders headers)
        {
            if (headers != null)
            {
                return string.Join("\r\n", headers.SelectMany(x => x.Value.Select(y => $"{x.Key}:{y}")));
            }
            return "";
        }

        private string FormattingHeaders(HttpContent content)
        {
            if (content != null)
            {
                return FormattingHeaders(content.Headers);
            }
            return "";
        }

        private async Task<string> ReadContentAsUtf8StringAsync(HttpContent content)
        {
            string val = "";
            try
            {
                byte[] byts = await content.ReadAsByteArrayAsync();
                val = Regex.Unescape(Encoding.UTF8.GetString(byts));
            }
            catch
            {
//                log.Error("ReadContentAsUtf8StringAsync error occurred.");
            }
            return val;
        }
    }
}
