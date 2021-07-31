using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackMD_ImgDownloader
{
    [Serializable]
    public class ImageUrlData
    {
        //----------------------------------------------------------------------------//
        // ※以下は、PropetyToolによって自動生成されました。手動による変更禁止！！    //
        //----------------------------------------------------------------------------//
        private string m_strImageUrl;
        private string m_strMarkDownPath;
        /// <summary>
        ///
        /// </summary>
        /// <param name="strImageUrl     "></param>
        /// <param name="strMarkDownPath "></param>
        public ImageUrlData(
        string strImageUrl,
        string strMarkDownPath
        )
         : base()
        {
            this.m_strImageUrl = strImageUrl;
            this.m_strMarkDownPath = strMarkDownPath;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="org"></param>
        public ImageUrlData(
        ImageUrlData org
        )
         : base()
        {
            this.m_strImageUrl = org.m_strImageUrl;
            this.m_strMarkDownPath = org.m_strMarkDownPath;
        }
        public string StrImageUrl { get { return this.m_strImageUrl; } protected set { this.m_strImageUrl = value; } }
        public string StrMarkDownPath { get { return this.m_strMarkDownPath; } protected set { this.m_strMarkDownPath = value; } }
        public override string ToString()
        {
            return new StringBuilder()
                .Append("strImageUrl：").Append(m_strImageUrl).AppendLine("、")
                .Append("strMarkDownPath：").Append(m_strMarkDownPath).AppendLine("、")
                .ToString();
        }
    }
}
