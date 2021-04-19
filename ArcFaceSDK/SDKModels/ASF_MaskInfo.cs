using System;

namespace ArcFaceSDK.SDKModels
{
    /// <summary>
    /// 口罩检测结果
    /// </summary>
    public struct ASF_MaskInfo
    {
        /// <summary>
        /// "0" 代表没有带口罩，"1"代表带口罩 ,"-1"表不确定
        /// </summary>
        public IntPtr maskArray;

        /// <summary>
        /// 检测的人脸个数
        /// </summary>
        public int num;
    }
}
