using System.Text;

namespace ArcFaceSDK.Entity
{
    /// <summary>
    /// SDK版本信息
    /// </summary>
    public class SDKVersion
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// 构建日期
        /// </summary>
        public string buildDate { get; set; }

        /// <summary>
        /// Copyright
        /// </summary>
        public string copyRight { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SDK版本信息如下:\r\n");
            stringBuilder.AppendFormat("{0}:{1}\r\n", "version", version);
            stringBuilder.AppendFormat("{0}:{1}\r\n", "buildDate", buildDate);
            stringBuilder.AppendFormat("{0}:{1}\r\n", "copyRight", copyRight);
            return stringBuilder.ToString();

        }
    }
}
