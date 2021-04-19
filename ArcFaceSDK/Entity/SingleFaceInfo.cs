using ArcFaceSDK.SDKModels;

namespace ArcFaceSDK.Entity
{
    /// <summary>
    ///  单人脸检测结果
    /// </summary>
    public class SingleFaceInfo
    {
        /// <summary>
        /// 人脸坐标Rect结果
        /// </summary>
        public MRECT faceRect { get; set; }

        /// <summary>
        /// 人脸角度
        /// </summary>
        public int faceOrient { get; set; }

        /// <summary>
        /// 单张人脸信息
        /// </summary>
        public FaceDataInfo faceDataInfo { get; set; }
    }
}
