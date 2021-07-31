using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using HackMD_ImgDownloader.http;

namespace HackMD_ImgDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Assembly assembly = Assembly.GetEntryAssembly();
            string assemblyPath = assembly.Location;

            FileInfo fileAssembly = new FileInfo(assemblyPath);

            string dirAssembly = fileAssembly.DirectoryName;

            dirAssembly = @"D:\Samples\hackmd";

            string hackMD_Path = Path.Combine(dirAssembly, @"markdown");
            string img_Path = Path.Combine(dirAssembly, @"img");

            DirectoryInfo dirinfo = new DirectoryInfo(hackMD_Path);
            if (dirinfo.Exists == false)
            {
                throw new Exception("don't exists hackDM directory.");
            }

            DirectoryInfo dirImg = new DirectoryInfo(img_Path);
            if (dirImg.Exists == false)
            {
                throw new Exception("don't exists hackDM images directory.");
            }

            List<ImageUrlData> lstImgUrl = new List<ImageUrlData>();

            int count_md = 0;
            foreach (FileInfo fileInfo in dirinfo.EnumerateFiles())
            {
                //Console.WriteLine(fileInfo.FullName);

                if (fileInfo.Name.ToLower().EndsWith(".md"))
                {
                    count_md++;
                    using (StreamReader stream = fileInfo.OpenText())
                    {
                        stream.BaseStream.Seek(0, SeekOrigin.Begin);
                        string markdown = stream.ReadToEnd();
                        //Console.WriteLine("========================================================");
                        //Console.WriteLine(markdown);
                        //Console.WriteLine("========================================================");

                        // ![image alt](https://i.imgur.com/hWZxhNc.png "タイトル" =100x100)
                        List<string> lst = RegexUtil.RegexMatches(
                            markdown, 
                            RegexUtil.RegPattern_Tag(
                                @"\(\s*",
                                @"\s*\)"
                                )
                            );

                        // 先頭がhttpsで始まるものを抽出する。
                        lst = lst
                            .Select(n => RegexUtil.Replace(n, @"\s*=[0-9]+x[0-9]+\s*", ""))
                            .Select(n => RegexUtil.Replace(n, @"\s*""[^""]*""\s*", ""))
                            .Where(n =>
                                   n.ToLower().StartsWith(@"http://")
                                || n.ToLower().StartsWith(@"https://")
                                )
                            .Where(n => 
                                (
                                       n.ToLower().EndsWith(".png" )
                                    || n.ToLower().EndsWith(".jpg" )
                                    || n.ToLower().EndsWith(".jpeg")
                                    || n.ToLower().EndsWith(".gif" )
                                    || n.ToLower().EndsWith(".svg" )
                                ) == true
                                )
                            .ToList();
                        lstImgUrl.AddRange(lst.Select(n => new ImageUrlData(n, fileInfo.FullName)));
                    }
                }
            }

            List<ImageUrlData> errorImageUrl = new List<ImageUrlData>();
            using (HttpClientManager http = new HttpClientManager())
            {
                HttpClient client = http.GetClient();

                int intCount = 0;
                foreach (ImageUrlData datUrl in lstImgUrl)
                {
                    intCount++;

                    string url = datUrl.StrImageUrl;

                    string name = HTTPUtil.GetNameFromURL(url);
                    string path = HTTPUtil.GetUrlExcludeName(url);

                    path = path.Replace("/", "^");
                    path = path.Replace(":", "-");

                    Console.WriteLine("[" + url + "][" + path + "][" + name + "] " + intCount + "/" + lstImgUrl.Count);

                    List<FileInfo> files = new List<FileInfo>();
                    try
                    {
                        files = HTTPUtil.Download(client, url, name);
                    }
                    catch
                    {
                        errorImageUrl.Add(datUrl);
                    }

                    if (files != null)
                    {
                        DirectoryInfo dir = new DirectoryInfo(Path.Combine(dirImg.FullName, path));
                        if (dir.Exists == false)
                        {
                            dir.Create();
                        }

                        foreach (var file in files)
                        {
                            FileInfo newFile = new FileInfo(Path.Combine(dir.FullName, file.Name));
                            if (newFile.Exists == true)
                            {
                                newFile.Delete();
                            }
                            file.MoveTo(newFile.FullName);
                        }
                    }
                }
            }

            Console.WriteLine("===================================================");
            Console.WriteLine(count_md + " 個のMarkdown");
            Console.WriteLine(lstImgUrl.Count + " 個の画像");
            Console.WriteLine(errorImageUrl.Count + " 件のエラー");
            Console.WriteLine("===================================================");
            Console.WriteLine("ダウンロード出来なかった画像");
            foreach (ImageUrlData datUrl in errorImageUrl)
            {
                Console.WriteLine("[" + datUrl.StrMarkDownPath + "][" + datUrl.StrImageUrl + "]");
            }

        }
    }
}
