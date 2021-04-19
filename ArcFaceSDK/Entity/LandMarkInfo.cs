
using ArcFaceSDK.SDKModels;

namespace ArcFaceSDK.Entity
{
    /// <summary>
    /// 额头区域检测结果
    /// </summary>
    public class LandMarkInfo
    {
        /// <summary>
        /// 额头点位
        /// </summary>
        public ASF_FaceLandmark[][] pointAyy { get; set; }

        /// <summary>
        /// 人脸数量
        /// </summary>
        public int num { get; set; }
    }
}
