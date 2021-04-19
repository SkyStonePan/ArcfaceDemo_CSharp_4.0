using System.Text;

namespace ArcFaceSDK.Entity
{
    /// <summary>
    /// 激活文件信息
    /// </summary>
    public class ActiveFileInfo
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public string startTime { get; set; }

        /// <summary>
        /// 截止时间
        /// </summary>
        public string endTime { get; set; }

        /// <summary>
        /// 激活码
        /// </summary>
        public string activeKey { get; set; }

        /// <summary>
        /// 平台
        /// </summary>
        public string platform { get; set; }

        /// <summary>
        /// sdk类型
        /// </summary>
        public string sdkType { get; set; }

        /// <summary>
        /// APPID
        /// </summary>
        public string appId { get; set; }

        /// <summary>
        /// SDKKEY
        /// </summary>
        public string sdkKey { get; set; }

        /// <summary>
        /// SDK版本号
        /// </summary>
        public string sdkVersion { get; set; }

        /// <summary>
        /// 激活文件版本号
        /// </summary>
        public string fileVersion { get; set; }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            builder.Append("激活文件信息如下:\r\n");
            builder.AppendFormat("{0}:{1}\r\n", "开始时间",startTime);
            builder.AppendFormat("{0}:{1}\r\n", "截止时间", endTime);
            builder.AppendFormat("{0}:{1}\r\n", "激活码", activeKey);
            builder.AppendFormat("{0}:{1}\r\n", "平台", platform);
            builder.AppendFormat("{0}:{1}\r\n", "sdk类型", sdkType);
            builder.AppendFormat("{0}:{1}\r\n", "APPID", appId);
            builder.AppendFormat("{0}:{1}\r\n", "SDKKEY", sdkKey);
            builder.AppendFormat("{0}:{1}\r\n", "SDK版本号", sdkVersion);
            builder.AppendFormat("{0}:{1}\r\n", "激活文件版本号", fileVersion);
            return builder.ToString();
        }
    }
}
