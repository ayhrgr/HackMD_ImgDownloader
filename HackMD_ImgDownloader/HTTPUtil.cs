using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HackMD_ImgDownloader
{
    public static class HTTPUtil
    {
        public static string GetNameFromURL(string url)
        {
            string name = "";
            string[] split = url.Split("/");
            if (split != null && split.Any())
            {
                name = split[split.Length - 1];
            }
            return name;
        }

        public static string GetUrlExcludeName(string url)
        {
            string urlExcludeName = "";
            string name = GetNameFromURL(url);
            if (name != null && name.Any())
            {
                urlExcludeName = url.Substring(0, url.Length - name.Length);
            }
            return urlExcludeName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static List<FileInfo> Download(
            HttpClient client,
            string url,
            string filename = null
            )
        {
            List<FileInfo> lstFileZip_Temp = new List<FileInfo>();
            {
                using (Task<HttpResponseMessage> tsk = client.GetAsync(url))
                {
                    tsk.Wait();
                    using (HttpResponseMessage response = tsk.Result)
                    {
                        if (response.IsSuccessStatusCode == false)
                        {
                            throw new Exception("ファイルのダウンロードに失敗しました。");
                        }

                        byte[] aryContent = response.Content.ReadAsByteArrayAsync().Result;

                        IEnumerable<string> iEnumContentDisposition = null;
                        string strContent_Disposition = "";
                        if (response.Content.Headers.TryGetValues("Content-Disposition", out iEnumContentDisposition))
                        {
                            strContent_Disposition = iEnumContentDisposition.FirstOrDefault();
                        }

                        string strFileName = RegexUtil.RegexMatch(strContent_Disposition, @"attachment; filename=""(?<filename>[^""]+)""", "filename");
                        if (string.IsNullOrWhiteSpace(strFileName))
                        {
                            if (filename != null)
                            {
                                strFileName = filename;
                            }
                            else
                            {
                                strFileName = "test.zip";
                            }
                        }

#if WINDOWS_UWP
                        var temp_path = ApplicationData.Current.LocalFolder.Path;
#else
                        var temp_path = Path.GetTempPath();
#endif

                        var fileZip_Temp = new FileInfo(Path.Combine(temp_path, Path.GetRandomFileName(), strFileName));
                        if (File.Exists(fileZip_Temp.FullName))
                        {
                            fileZip_Temp.Delete();
                        }
                        if (fileZip_Temp.Directory.Exists == false)
                        {
                            Directory.CreateDirectory(fileZip_Temp.Directory.FullName);
                        }
                        {
                            using (FileStream fStream = fileZip_Temp.OpenWrite())
                            {
                                fStream.Seek(0, SeekOrigin.Begin);
                                fStream.Write(aryContent, 0, aryContent.Length);
                            }
                        }

                        lstFileZip_Temp.Add(fileZip_Temp);

                        // zipファイルの場合は、解凍してその中身のCSVファイル情報のリストを返却する。
                        if (Path.GetExtension(fileZip_Temp.Name) == ".zip")
                        {
                            DirectoryInfo dirUnzip = new DirectoryInfo(
                                Path.Combine(
                                    fileZip_Temp.DirectoryName, 
                                    Path.GetFileNameWithoutExtension(fileZip_Temp.FullName)
                                    )
                                );
                            // zipファイルなので、解凍する。
                            //DotNetZipUtil.ExtractZip(
                            //    fileZip_Temp.FullName,
                            //    dirUnzip.FullName
                            //    );

                            lstFileZip_Temp = dirUnzip.GetFiles().ToList();
                        }

                        if (lstFileZip_Temp.Any() == false)
                        {
                            throw new Exception("CSVファイルのダウンロードまたは、ZIPファイルの解凍に失敗している可能性があります。");
                        }

                        //m_fileZip_Temp = fileZip_Temp;
                    }
                }
            }
            return lstFileZip_Temp;
        }

    }
}
