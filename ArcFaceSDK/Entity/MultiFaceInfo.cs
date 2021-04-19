using ArcFaceSDK.SDKModels;

namespace ArcFaceSDK.Entity
{
    /// <summary>
    /// 多人脸检测
    /// </summary>
    public class MultiFaceInfo
    {
        /// <summary>
        /// 人脸Rect结果集
        /// </summary>
        public MRECT[] faceRects { get; set; }

        /// <summary>
        /// 人脸角度结果集，与faceRects一一对应  对应ASF_OrientCode
        /// </summary>
        public int[] faceOrients { get; set; }

        /// <summary>
        /// 结果集大小
        /// </summary>
        public int faceNum { get; set; }

        /// <summary>
        /// face ID，IMAGE模式下不返回FaceID
        /// </summary>
        public int[] faceID { get; set; }

        /// <summary>
        /// 戴眼镜置信度[0-1],推荐阈值0.5
        /// </summary>
        public float[] wearGlasses { get; set; }

        /// <summary>
        /// 左眼状态 0 未闭眼；1 闭眼
        /// </summary>
        public int[] leftEyeClosed { get; set; }

        /// <summary>
        /// 右眼状态 0 未闭眼；1 闭眼
        /// </summary>
        public int[] rightEyeClosed { get; set; }

        /// <summary>
        ///  "1" 表示 遮挡, "0" 表示  未遮挡, "-1" 表示不确定
        /// </summary>
        public int[] faceShelter { get; set; }

        /// <summary>
        /// 多张人脸信息
        /// </summary>
        public FaceDataInfo[] faceDataInfoList { get; set; }

        /// <summary>
        /// 额头点位
        /// </summary>
        public ASF_FaceLandmark[][] pointAyy { get; set; }
    }
}
