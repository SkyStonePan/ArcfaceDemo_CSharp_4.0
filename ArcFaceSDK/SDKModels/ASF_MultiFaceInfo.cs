using System;

namespace ArcFaceSDK.SDKModels
{
    /// <summary>
    /// 多人脸检测结构体
    /// </summary>
    public struct ASF_MultiFaceInfo
    {
        /// <summary>
        /// 人脸Rect结果集
        /// </summary>
        public IntPtr faceRect;

        /// <summary>
        /// 人脸角度结果集，与faceRect一一对应  对应ASF_OrientCode
        /// </summary>
        public IntPtr faceOrient;

        /// <summary>
        /// 结果集大小
        /// </summary>
        public int faceNum;

        /// <summary>
        /// face ID，IMAGE模式下不返回FaceID
        /// </summary>
        public IntPtr faceID;

        /// <summary>
        /// 戴眼镜置信度[0-1],推荐阈值0.5
        /// </summary>
        public IntPtr wearGlasses;

        /// <summary>
        /// 左眼状态 0 未闭眼；1 闭眼
        /// </summary>
        public IntPtr leftEyeClosed;

        /// <summary>
        /// 右眼状态 0 未闭眼；1 闭眼
        /// </summary>
        public IntPtr rightEyeClosed;

        /// <summary>
        /// "1" 表示 遮挡, "0" 表示  未遮挡, "-1" 表示不确定
        /// </summary>
        public IntPtr faceShelter;

        /// <summary>
        /// 多张人脸信息
        /// </summary>
        public IntPtr faceDataInfoList;
    }
}
