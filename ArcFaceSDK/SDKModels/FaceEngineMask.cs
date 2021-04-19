namespace ArcFaceSDK.SDKModels
{
    /// <summary>
    /// 引擎方法类型结构体，在初始化时将用到的类型用|连接传入，如 ASF_NONE|ASF_FACE_DETECT|ASF_FACERECOGNITION
    /// </summary>
    public struct FaceEngineMask
    {
        /// <summary>
        /// 不做方法初始化方法类型
        /// </summary>
        public const int ASF_NONE = 0x00000000;

        /// <summary>
        /// 此处detect可以是tracking或者detection两个引擎之一，具体的选择由detect mode 确定
        /// </summary>
        public const int ASF_FACE_DETECT = 0x00000001;

        /// <summary>
        /// 人脸特征
        /// </summary>
        public const int ASF_FACERECOGNITION = 0x00000004;

        /// <summary>
        /// 年龄
        /// </summary>
        public const int ASF_AGE = 0x00000008;

        /// <summary>
        /// 性别
        /// </summary>
        public const int ASF_GENDER = 0x00000010;

        /// <summary>
        /// 3D角度
        /// </summary>
        public const int ASF_FACE3DANGLE = 0x00000020;

        /// <summary>
        /// 额头区域检测
        /// </summary>
        public const int ASF_FACELANDMARK = 0x00000040;

        /// <summary>
        /// RGB活体
        /// </summary>
        public const int ASF_LIVENESS = 0x00000080;

        /// <summary>
        /// 图像质量检测
        /// </summary>
        public const int ASF_IMAGEQUALITY = 0x00000200;

        /// <summary>
        /// 红外活体
        /// </summary>
        public const int ASF_IR_LIVENESS = 0x00000400;

        /// <summary>
        /// 人脸遮挡
        /// </summary>
        public const int ASF_FACESHELTER = 0x00000800;

        /// <summary>
        /// 口罩检测
        /// </summary>
        public const int ASF_MASKDETECT = 0x00001000;

        /// <summary>
        /// 人脸信息
        /// </summary>
        public const int ASF_UPDATE_FACEDATA = 0x00002000;
    }
}
