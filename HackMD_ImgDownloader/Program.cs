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

            string hackMD_Path = Path.Combine(fileAssembly.DirectoryName, @"markdown");
            string img_Path = Path.Combine(fileAssembly.DirectoryName, @"img");

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

            List<string> lstImgUrl = new List<string>();

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

                        List<string> lst = RegexUtil.RegexMatches(markdown, RegexUtil.RegPattern_Tag(@"\(\s*", @"\s*\)"));
                        // 先頭がhttpsで始まるものを抽出する。
                        lst = lst
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
                        lstImgUrl.AddRange(lst);
                    }
                }
            }

            using (HttpClientManager http = new HttpClientManager())
            {
                HttpClient client = http.GetClient();

                foreach (string url in lstImgUrl)
                {
                    string name = HTTPUtil.GetNameFromURL(url);
                    string path = HTTPUtil.GetUrlExcludeName(url);

                    path = path.Replace("/", "^");
                    path = path.Replace(":", "-");

                    Console.WriteLine("[" + url + "][" + path + "][" + name + "]");

                    List<FileInfo> files = HTTPUtil.Download(client, url, name);
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

            Console.WriteLine(count_md + " 個のMarkdown");
            Console.WriteLine(lstImgUrl.Count + " 個の画像");
        }
    }
}
