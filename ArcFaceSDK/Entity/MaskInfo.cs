
namespace ArcFaceSDK.Entity
{
    /// <summary>
    /// 口罩检测结果
    /// </summary>
    public class MaskInfo
    {
        /// <summary>
        /// "0" 代表没有带口罩，"1"代表带口罩 ,"-1"表不确定
        /// </summary>
        public int[] maskArray { get; set; }

        /// <summary>
        /// 检测的人脸个数
        /// </summary>
        public int num { get; set; }
    }
}
