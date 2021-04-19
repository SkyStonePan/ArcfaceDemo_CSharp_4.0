using ArcFaceSDK.SDKModels;
using System;
using System.Runtime.InteropServices;

namespace ArcFaceSDK.Utils
{
    /// <summary>
    /// SDK接口
    /// </summary>
    public static class ASFFunctions
    {
        /// <summary>
        /// SDK动态链接库路径
        /// </summary>
        public const string Dll_PATH = "libarcsoft_face_engine.dll";

        ///<summary>
        ///获取激活文件信息接口
        ///</summary>
        ///<param name="deviceInfo">[out] 激活文件信息</param>
        ///<returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFGetActiveFileInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFGetActiveFileInfo(IntPtr activeFileInfo);

        /// <summary>
        /// 在线激活接口
        /// </summary>
        /// <param name="appId">[in]  APPID	官网下载</param>
        /// <param name="sdkKey">[in]  SDKKEY	官网下载</param>
        /// <param name="activeKey">[in]  ActiveKey	官网下载</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFOnlineActivation", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFOnlineActivation(string appId, string sdkKey, string activeKey);

        ///<summary>
        ///获取设备信息接口
        ///</summary>
        ///<param name="deviceInfo">[out] 采集的设备信息，用于到开发者中心做离线激活，生成离线授权文件</param>
        ///<returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFGetActiveDeviceInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFGetActiveDeviceInfo(ref IntPtr deviceInfo);

        /// <summary>
        /// 离线激活接口
        /// </summary>
        /// <param name="filePath"> [in] 许可文件路径(离线授权文件)，需要读写权限</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFOfflineActivation", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFOfflineActivation(string filePath);

        /// <summary>
        /// 初始化引擎
        /// </summary>
        /// <param name="detectMode">
        /// [in] AF_DETECT_MODE_VIDEO  视频模式：适用于摄像头预览，视频文件识别
        /// AF_DETECT_MODE_IMAGE 图片模式：适用于静态图片的识别
        /// </param>
        /// <param name="detectFaceOrientPriority">[in]	检测脸部的角度优先值，参考 ArcFaceCompare_OrientPriority</param>
        /// <param name="detectFaceMaxNum">[in] 最大需要检测的人脸个数</param>
        /// <param name="combinedMask">[in] 用户选择需要检测的功能组合，可单个或多个</param>
        /// <param name="pEngine">[out] 初始化返回的引擎handle</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFInitEngine", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFInitEngine(DetectionMode detectMode, ASF_OrientPriority detectFaceOrientPriority, int detectFaceMaxNum, int combinedMask, ref IntPtr pEngine);

        /// <summary>
        /// 设置遮挡阈值，取值范围[0-1]， 默认阈值:0.8， 用户可以根据实际需求设置
        /// </summary>
        /// <param name="pEngine">[in] 引擎handle</param>
        /// <param name="shelterThreshhold">[in] 遮挡阈值</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFSetFaceShelterParam", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFSetFaceShelterParam(IntPtr pEngine, float shelterThreshhold);

        /// <summary>
        /// VIDEO模式:人脸追踪 IMAGE模式:人脸检测
        /// </summary>
        /// <param name="pEngine"> [in] 引擎handle</param>
        /// <param name="width">[in] 图片宽度</param>
        /// <param name="height">[in] 图片高度</param>
        /// <param name="format">[in] 颜色空间格式</param>
        /// <param name="imgData">[in] 图片数据</param>
        /// <param name="detectedFaces">[out]检测到的人脸信息 </param>
        /// <param name="detectModel">[in] 预留字段，当前版本使用默认参数即可</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFDetectFaces", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFDetectFaces(IntPtr pEngine, int width, int height, int format, IntPtr imgData, IntPtr detectedFaces, ASF_DetectModel detectModel = ASF_DetectModel.ASF_DETECT_MODEL_RGB);

        /// <summary>
        /// VIDEO模式:人脸追踪 IMAGE模式:人脸检测
	    /// 图像数据以结构体形式传入，对采用更高字节对齐方式的图像兼容性更好
        /// </summary>
        /// <param name="hEngine">[in] 引擎handle</param>
        /// <param name="imgData">[in] 图片数据</param>
        /// <param name="detectedFaces">[out] 检测到的人脸信息</param>
        /// <param name="detectModel">[in]	预留字段，当前版本使用默认参数即可</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFDetectFacesEx", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFDetectFacesEx(IntPtr hEngine, IntPtr imgData, IntPtr detectedFaces, ASF_DetectModel detectModel = ASF_DetectModel.ASF_DETECT_MODEL_RGB);

        /// <summary>
        /// 传入修改后的人脸框，更新人脸信息，用于做双目对齐或其他策略
        /// 注意：LPASF_MultiFaceInfo在该接口中既是入参也是出参
        /// </summary>
        /// <param name="pEngine">[in] 引擎handle</param>
        /// <param name="width">[in] 图片宽度</param>
        /// <param name="height">[in] 图片高度</param>
        /// <param name="format">[in] 颜色空间格式</param>
        /// <param name="imgData">[in] 图片数据</param>
        /// <param name="detectedFaces">[in/out]检测到的人脸信息 </param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH,EntryPoint = "ASFUpdateFaceData", CallingConvention =CallingConvention.Cdecl)]
        public static extern int ASFUpdateFaceData(IntPtr pEngine,int width,int height,int format,IntPtr imgData,IntPtr detectedFaces);

        /// <summary>
        /// 传入修改后的人脸框，更新人脸信息，用于做双目对齐或其他策略
        /// 注意：LPASF_MultiFaceInfo在该接口中既是入参也是出参
        /// </summary>
        /// 图像数据以结构体形式传入，对采用更高字节对齐方式的图像兼容性更好
        /// <param name="pEngine">[in] 引擎handle</param>
        /// <param name="imgData">[in] 图像数据</param>
        /// <param name="detectedFaces">[in/out] 检测到的人脸信息</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFUpdateFaceDataEx", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFUpdateFaceDataEx(IntPtr pEngine, IntPtr imgData, IntPtr detectedFaces);

        /// <summary>
        /// 图像质量检测，（RGB模式： 识别阈值：0.49 注册阈值：0.63  口罩模式：识别阈值：0.29）
        /// </summary>
        /// <param name="pEngine">[in] 引擎handle</param>
        /// <param name="width">[in] 图片宽度</param>
        /// <param name="height">[in] 图片高度</param>
        /// <param name="format">[in] 颜色空间格式</param>
        /// <param name="imgData">[in] 图片数据</param>
        /// <param name="faceInfo">[in] 人脸位置信息</param>
        /// <param name="isMask">[in] 仅支持传入1、0、-1，戴口罩 1，否则认为未戴口罩</param>
        /// <param name="confidenceLevel">[out] 图像质量检测结果</param>
        /// <param name="detectModel">[in]	预留字段，当前版本使用默认参数即可</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFImageQualityDetect", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFImageQualityDetect(IntPtr pEngine, int width, int height, int format, IntPtr imgData, IntPtr faceInfo, int isMask, ref float confidenceLevel, ASF_DetectModel detectModel = ASF_DetectModel.ASF_DETECT_MODEL_RGB);

        /// <summary>
        /// 图像质量检测，（RGB模式： 识别阈值：0.49 注册阈值：0.63  口罩模式：识别阈值：0.29）
        /// 图像数据以结构体形式传入，对采用更高字节对齐方式的图像兼容性更好
        /// </summary>
        /// <param name="hEngine">[in] 引擎handle</param>
        /// <param name="imgData">[in] 图片数据</param>
        /// <param name="faceInfo">[in] 人脸位置信息 </param>
        /// <param name="isMask">[in] 仅支持传入1、0、-1，戴口罩 1，否则认为未戴口罩</param>
        /// <param name="confidenceLevel">[out] 图像质量检测结果</param>
        /// <param name="detectModel">[in]	预留字段，当前版本使用默认参数即可</param>
        /// <returns></returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFImageQualityDetectEx", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFImageQualityDetectEx(IntPtr hEngine, IntPtr imgData, IntPtr faceInfo,int isMask, ref float confidenceLevel, ASF_DetectModel detectModel = ASF_DetectModel.ASF_DETECT_MODEL_RGB);


        /// <summary>
        /// 年龄/性别/人脸3D角度/口罩/遮挡/额头区域（该接口仅支持RGB图像），最多支持4张人脸信息检测，超过部分返回未知
        /// RGB活体仅支持单人脸检测，该接口不支持检测IR活体
        /// </summary>
        /// <param name="pEngine">[in] 引擎handle</param>
        /// <param name="width">[in] 图片宽度</param>
        /// <param name="height">[in] 图片高度</param>
        /// <param name="format">[in] 颜色空间格式</param>
        /// <param name="imgData">[in] 图片数据</param>
        /// <param name="detectedFaces">[in] 人脸信息，用户根据待检测的功能选择需要使用的人脸。</param>
        /// <param name="combinedMask">
        /// 只支持初始化时候指定需要检测的功能，在process时进一步在这个已经指定的功能集中继续筛选
        /// 例如初始化的时候指定检测年龄和性别，在process的时候可以只检测年龄，但是不能检测除年龄和性别之外的功能
        /// </param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFProcess", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFProcess(IntPtr pEngine, int width, int height, int format, IntPtr imgData, IntPtr detectedFaces, int combinedMask);

        /// <summary>
        /// 年龄/性别/人脸3D角度/口罩/遮挡/额头区域（该接口仅支持RGB图像），最多支持4张人脸信息检测，超过部分返回未知
        /// RGB活体仅支持单人脸检测，该接口不支持检测IR活体
        /// 图像数据以结构体形式传入，对采用更高字节对齐方式的图像兼容性更好
        /// </summary>
        /// <param name="hEngine">引擎handle</param>
        /// <param name="imgData">图片数据</param>
        /// <param name="detectedFaces">人脸信息，用户根据待检测的功能选择需要使用的人脸。</param>
        /// <param name="combinedMask">只支持初始化时候指定需要检测的功能，在process时进一步在这个已经指定的功能集中继续筛选</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFProcessEx", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFProcessEx(IntPtr hEngine, IntPtr imgData, IntPtr detectedFaces, int combinedMask);

        /// <summary>
        /// 该接口目前仅支持单人脸IR活体检测（不支持年龄、性别、3D角度的检测），默认取第一张人脸
        /// </summary>
        /// <param name="pEngine">[in] 引擎handle</param>
        /// <param name="width">[in] 图片宽度</param>
        /// <param name="height">[in] 图片高度</param>
        /// <param name="format">[in] 颜色空间格式</param>
        /// <param name="imgData">[in] 图片数据</param>
        /// <param name="detectedFaces">[in] 人脸信息，用户根据待检测的功能选择需要使用的人脸。</param>
        /// <param name="combinedMask">[in] 目前只支持传入ASF_IR_LIVENESS属性的传入，且初始化接口需要传入  </param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFProcess_IR", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFProcess_IR(IntPtr pEngine, int width, int height, int format, IntPtr imgData, IntPtr detectedFaces, int combinedMask);

        /// <summary>
        /// 该接口目前仅支持单人脸IR活体检测，默认取第一张人脸
        /// 图像数据以结构体形式传入，对采用更高字节对齐方式的图像兼容性更好
        /// </summary>
        /// <param name="hEngine">[in] 引擎handle</param>
        /// <param name="imgData">[in] 图片数据</param>
        /// <param name="detectedFaces">[in] 人脸信息，用户根据待检测的功能选择需要使用的人脸。</param>
        /// <param name="combinedMask">[in] 目前只支持传入ASF_IR_LIVENESS属性的传入，且初始化接口需要传入</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFProcessEx_IR", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFProcessEx_IR(IntPtr hEngine, IntPtr imgData, IntPtr detectedFaces, int combinedMask);

        /// <summary>
        /// 单人脸特征提取
        /// </summary>
        /// <param name="pEngine">[in]	引擎handle</param>
        /// <param name="width">[in] 图片宽度</param>
        /// <param name="height">[in] 图片高度</param>
        /// <param name="format">[in] 颜色空间格式</param>
        /// <param name="imgData">[in] 图片数据</param>
        /// <param name="faceInfo">[in] 单张人脸位置和角度信息</param>
        /// <param name="registerOrNot">[in] 注册 1 识别为 0</param>
        /// <param name="mask">[in] 带口罩 1，否则0</param>
        /// <param name="faceFeature">[out] 人脸特征</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFFaceFeatureExtract", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFFaceFeatureExtract(IntPtr pEngine, int width, int height, int format, IntPtr imgData, IntPtr faceInfo, ASF_RegisterOrNot registerOrNot, int mask, IntPtr faceFeature);

        /// <summary>
        /// 单人脸特征提取
        /// 图像数据以结构体形式传入，对采用更高字节对齐方式的图像兼容性更好
        /// </summary>
        /// <param name="hEngine">[in]	引擎handle</param>
        /// <param name="imgData">[in] 图像数据</param>
        /// <param name="faceInfo">[in] 单张人脸位置和角度信息</param>
        /// <param name="registerOrNot">[in] 注册 1 识别为 0</param>
        /// <param name="mask">[in] 带口罩 1，否则0</param>
        /// <param name="feature">[out] 人脸特征</param>
        /// <returns></returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFFaceFeatureExtractEx", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFFaceFeatureExtractEx(IntPtr hEngine, IntPtr imgData, IntPtr faceInfo, ASF_RegisterOrNot registerOrNot, int mask, IntPtr feature);

        /// <summary>
        /// 人脸特征比对
        /// </summary>
        /// <param name="pEngine">[in] 引擎handle</param>
        /// <param name="faceFeature1">[in] 待比较人脸特征1</param>
        /// <param name="faceFeature2"> [in] 待比较人脸特征2</param>
        /// <param name="confidenceLevel">[out] 比较结果，置信度数值</param>
        /// <param name="compareModel">
        /// [in] ASF_LIFE_PHOTO：用于生活照之间的特征比对
        /// ASF_ID_PHOTO：用于证件照或证件照和生活照之间的特征比对
        /// </param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFFaceFeatureCompare", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFFaceFeatureCompare(IntPtr pEngine, IntPtr faceFeature1, IntPtr faceFeature2, ref float confidenceLevel, ASF_CompareModel compareModel = ASF_CompareModel.ASF_LIFE_PHOTO);

        /// <summary>
        /// 获取年龄信息
        /// </summary>
        /// <param name="pEngine">[in] 引擎handle</param>
        /// <param name="ageInfo">[out] 检测到的年龄信息</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFGetAge", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFGetAge(IntPtr pEngine, IntPtr ageInfo);

        /// <summary>
        /// 获取性别信息
        /// </summary>
        /// <param name="pEngine"> [in] 引擎handle</param>
        /// <param name="genderInfo">[out] 检测到的性别信息</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFGetGender", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFGetGender(IntPtr pEngine, IntPtr genderInfo);

        /// <summary>
        /// 获取3D角度信息
        /// </summary>
        /// <param name="pEngine">[in] 引擎handle</param>
        /// <param name="p3DAngleInfo"> [out] 检测到脸部3D 角度信息</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFGetFace3DAngle", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFGetFace3DAngle(IntPtr pEngine, IntPtr p3DAngleInfo);

        //******************************** 活体信息 ***********************************

        /// <summary>
        /// 取值范围[0-1]，默认值 BGR:0.5 IR:0.7， 用户可以根据实际需求，设置不同的阈值
        /// </summary>
        /// <param name="hEngine">[in] 引擎handle</param>
        /// <param name="threshold"> [in] 活体置信度</param>
        /// <returns></returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFSetLivenessParam", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFSetLivenessParam(IntPtr hEngine, IntPtr threshold);

        /// <summary>
        /// 获取RGB活体结果
        /// </summary>
        /// <param name="hEngine">[in] 引擎handle</param>
        /// <param name="livenessInfo">[out] 检测RGB活体结果</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFGetLivenessScore", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFGetLivenessScore(IntPtr hEngine, IntPtr livenessInfo);

        /// <summary>
        /// 获取IR活体结果
        /// </summary>
        /// <param name="pEngine">[in] 引擎handle</param>
        /// <param name="irLivenessInfo">[out] 检测到IR活体结果</param>
        /// <returns></returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFGetLivenessScore_IR", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFGetLivenessScore_IR(IntPtr pEngine, IntPtr irLivenessInfo);

        //******************************** 口罩检测相关 **********************************************

        /// <summary>
        /// 获取口罩检测的结果
        /// </summary>
        /// <param name="pEngine">[in] 引擎handle</param>
        /// <param name="maskInfo">[out] 检测到的口罩检测相关</param>
        /// <returns></returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFGetMask", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFGetMask(IntPtr pEngine, IntPtr maskInfo);

        /// <summary>
        /// 获取额头区域检测结果（当前只支持0, 90, 180, 270度角检测）
        /// </summary>
        /// <param name="pEngine">[in] 引擎handle</param>
        /// <param name="maskInfo">[out] 人脸额头点数组，每张人脸额头区域通过四个点表示</param>
        /// <returns></returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFGetFaceLandMark", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFGetFaceLandMark(IntPtr pEngine, IntPtr LandMarkInfo);

        /// <summary>
        /// 销毁引擎
        /// </summary>
        /// <param name="pEngine">引擎handle</param>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFUninitEngine", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ASFUninitEngine(IntPtr pEngine);

        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <returns>调用结果</returns>
        [DllImport(Dll_PATH, EntryPoint = "ASFGetVersion", CallingConvention = CallingConvention.Cdecl)]
        public static extern ASF_VERSION ASFGetVersion();        
    }
}
