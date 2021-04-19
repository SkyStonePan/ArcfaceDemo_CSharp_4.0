using ArcFaceSDK.Entity;
using ArcFaceSDK.SDKModels;
using ArcFaceSDK.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ArcFaceSDK
{
    /// <summary>
    /// ArcFace 接口封装类
    /// </summary>
    public class FaceEngine
    {
        /// <summary>
        /// 引擎 handle
        /// </summary>
        private IntPtr pEngine;

        /// <summary>
        /// 判断引擎状态
        /// </summary>
        /// <returns>true:引擎已初始化；false:引擎未初始化</returns>
        public bool GetEngineStatus()
        {
            return !(pEngine.Equals(IntPtr.Zero));
        }

        /// <summary>
        /// 获取激活文件信息
        /// </summary>
        /// <param name="activeFileInfo">激活文件信息</param>
        /// <returns>返回0表示正常；返回负数请根据ErrorCodeUtil类注释查看；其他值请在官网-帮助中心查询</returns>
        public int ASFGetActiveFileInfo(out ActiveFileInfo activeFileInfo)
        {
            activeFileInfo = new ActiveFileInfo();
            int retCode = -1;
            IntPtr pASFActiveFileInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_ActiveFileInfo>());
            //调用SDK接口
            retCode = ASFFunctions.ASFGetActiveFileInfo(pASFActiveFileInfo);
            if (retCode != 0)
            {
                MemoryUtil.Free(pASFActiveFileInfo);
                return retCode;
            }
            //转化结果
            ASF_ActiveFileInfo asfActiveFileInfo = MemoryUtil.PtrToStructure<ASF_ActiveFileInfo>(pASFActiveFileInfo);
            activeFileInfo.startTime = Marshal.PtrToStringAnsi(asfActiveFileInfo.startTime);
            activeFileInfo.endTime = Marshal.PtrToStringAnsi(asfActiveFileInfo.endTime);
            activeFileInfo.activeKey = Marshal.PtrToStringAnsi(asfActiveFileInfo.activeKey);
            activeFileInfo.platform = Marshal.PtrToStringAnsi(asfActiveFileInfo.platform);
            activeFileInfo.sdkType = Marshal.PtrToStringAnsi(asfActiveFileInfo.sdkType);
            activeFileInfo.appId = Marshal.PtrToStringAnsi(asfActiveFileInfo.appId);
            activeFileInfo.sdkKey = Marshal.PtrToStringAnsi(asfActiveFileInfo.sdkKey);
            activeFileInfo.sdkVersion = Marshal.PtrToStringAnsi(asfActiveFileInfo.sdkVersion);
            activeFileInfo.fileVersion = Marshal.PtrToStringAnsi(asfActiveFileInfo.fileVersion);
            MemoryUtil.Free(pASFActiveFileInfo);
            return retCode;
        }

        /// <summary>
        /// 激活接口
        /// </summary>
        /// <param name="appId">appId</param>
        /// <param name="appKey">appKey</param>
        /// <param name="activeKey">activeKey</param>
        /// <returns>返回0或90114表示正常；其他值请在官网-帮助中心查询</returns>
        public int ASFOnlineActivation(string appId, string appKey, string activeKey)
        {
            return ASFFunctions.ASFOnlineActivation(appId, appKey, activeKey);
        }

        ///<summary>
        ///获取设备信息
        ///</summary>
        ///<param name="deviceInfo">[out] 采集的设备信息，用于到开发者中心做离线激活，生成离线授权文件</param>
        ///<returns>返回0表示正常；其他值请在官网-帮助中心查询</returns>
        public int ASFGetActiveDeviceInfo(out string deviceInfo)
        {
            deviceInfo = string.Empty;
            int retCode = -1;
            IntPtr tempIntPtr = IntPtr.Zero;
            //调用SDK接口
            retCode = ASFFunctions.ASFGetActiveDeviceInfo(ref tempIntPtr);
            if (retCode == 0)
            {
                deviceInfo = Marshal.PtrToStringAnsi(tempIntPtr);
            }
            return retCode;
        }

        /// <summary>
        /// 离线激活接口
        /// </summary>
        /// <param name="filePath">[in]  许可文件路径(离线授权文件)，需要读写权限</param>
        /// <returns>返回0表示正常；其他值请在官网-帮助中心查询</returns>
        public int ASFOfflineActivation(string filePath)
        {
            return ASFFunctions.ASFOfflineActivation(filePath);
        }

        /// <summary>
        /// 初始化引擎
        /// </summary>
        /// <param name="detectMode">
        /// [in]	AF_DETECT_MODE_VIDEO 视频模式：适用于摄像头预览，视频文件识别
        ///         AF_DETECT_MODE_IMAGE 图片模式：适用于静态图片的识别   
        /// </param>
        /// <param name="detectFaceOrientPriority">[in]	检测脸部的角度优先值，参考 ArcFaceCompare_OrientPriority</param>
        /// <param name="detectFaceMaxNum">[in] 最大需要检测的人脸个数</param>
        /// <param name="combinedMask">[in] 用户选择需要检测的功能组合，可单个或多个</param>
        /// <returns>返回0表示正常；其他值请在官网-帮助中心查询</returns>
        public int ASFInitEngine(DetectionMode detectMode, ASF_OrientPriority detectFaceOrientPriority, int detectFaceMaxNum, int combinedMask)
        {
            pEngine = IntPtr.Zero;
            if (detectFaceMaxNum < 1 || detectFaceMaxNum > 10)
            {
                detectFaceMaxNum = 10;
            }
            return ASFFunctions.ASFInitEngine(detectMode, detectFaceOrientPriority, detectFaceMaxNum, combinedMask, ref pEngine);
        }

        /// <summary>
        /// 设置遮挡阈值，取值范围[0-1]， 默认阈值:0.8， 用户可以根据实际需求设置
        /// </summary>
        /// <param name="shelterThreshhold">遮挡阈值，取值范围[0-1]， 默认阈值:0.8</param>
        /// <returns>返回0表示正常；其他值请在官网-帮助中心查询</returns>
        public int ASFSetFaceShelterParam(float shelterThreshhold)
        {
            return ASFFunctions.ASFSetFaceShelterParam(pEngine,shelterThreshhold);
        }

        /// <summary>
        /// 人脸检测/人脸追踪
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="multiFaceInfo">多人脸对象</param>
        /// <param name="detectModel">检测模式</param>
        /// <param name="imageFormat">图片格式</param>
        /// <param name="isEx">是否调用Ex类接口</param>
        /// <returns>返回0表示正常；返回负数请根据ErrorCodeUtil类注释查看；其他值请在官网-帮助中心查询</returns>
        public int ASFDetectFaces(Image image, out MultiFaceInfo multiFaceInfo, ASF_ImagePixelFormat imageFormat = ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8, ASF_DetectModel detectModel = ASF_DetectModel.ASF_DETECT_MODEL_RGB, bool isEx = false)
        {
            int retCode = -1;
            multiFaceInfo = new MultiFaceInfo();
            //判断图像是否为空
            if (image == null)
            {
                return ErrorCodeUtil.IMAGE_IS_NULL;
            }
            ImageInfo imageInfo = ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8.Equals(imageFormat) ? ImageUtil.ReadBMP(image) : ImageUtil.ReadBMP_IR(image);

            if (imageInfo == null)
            {
                return ErrorCodeUtil.IMAGE_DATA_READ_FAIL;
            }

            IntPtr pMultiFaceInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_MultiFaceInfo>());
            //调用SDK接口
            if (isEx)
            {
                ASF_ImageData asfInfoData = CommonUtil.TransImageDataStructByImageInfo(imageInfo);
                IntPtr pImageInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_ImageData>());
                MemoryUtil.StructureToPtr(asfInfoData, pImageInfo);
                retCode = ASFFunctions.ASFDetectFacesEx(pEngine, pImageInfo, pMultiFaceInfo);
                MemoryUtil.Free(pImageInfo);
            }
            else
            {
                retCode = ASFFunctions.ASFDetectFaces(pEngine, imageInfo.width, imageInfo.height, imageInfo.format, imageInfo.imgData, pMultiFaceInfo);
            }

            if (retCode != 0)
            {
                MemoryUtil.FreeArray(imageInfo.imgData, pMultiFaceInfo);
                return retCode;
            }
            ASF_MultiFaceInfo multiFaceInfoStruct = MemoryUtil.PtrToStructure<ASF_MultiFaceInfo>(pMultiFaceInfo);
            MemoryUtil.FreeArray(imageInfo.imgData, pMultiFaceInfo);

            //转化非托管内存到托管内存
            multiFaceInfo.faceNum = multiFaceInfoStruct.faceNum;
            if (multiFaceInfo.faceNum > 0)
            {
                if (multiFaceInfoStruct.faceID != IntPtr.Zero)
                {
                    multiFaceInfo.faceID = new int[multiFaceInfo.faceNum];
                    Marshal.Copy(multiFaceInfoStruct.faceID, multiFaceInfo.faceID, 0, multiFaceInfo.faceNum);
                }
                multiFaceInfo.faceOrients = new int[multiFaceInfo.faceNum];
                Marshal.Copy(multiFaceInfoStruct.faceOrient, multiFaceInfo.faceOrients, 0, multiFaceInfo.faceNum);
                multiFaceInfo.faceRects = new MRECT[multiFaceInfo.faceNum];
                for (int i = 0; i < multiFaceInfo.faceNum; i++)
                {
                    multiFaceInfo.faceRects[i] = MemoryUtil.PtrToStructure<MRECT>(multiFaceInfoStruct.faceRect + MemoryUtil.SizeOf<MRECT>() * i);
                }
                multiFaceInfo.wearGlasses = new float[multiFaceInfo.faceNum];
                Marshal.Copy(multiFaceInfoStruct.wearGlasses, multiFaceInfo.wearGlasses, 0, multiFaceInfo.faceNum);
                multiFaceInfo.leftEyeClosed = new int[multiFaceInfo.faceNum];
                Marshal.Copy(multiFaceInfoStruct.leftEyeClosed, multiFaceInfo.leftEyeClosed, 0, multiFaceInfo.faceNum);
                multiFaceInfo.rightEyeClosed = new int[multiFaceInfo.faceNum];
                Marshal.Copy(multiFaceInfoStruct.rightEyeClosed, multiFaceInfo.rightEyeClosed, 0, multiFaceInfo.faceNum);
                multiFaceInfo.faceShelter = new int[multiFaceInfo.faceNum];
                Marshal.Copy(multiFaceInfoStruct.faceShelter, multiFaceInfo.faceShelter, 0, multiFaceInfo.faceNum);
                multiFaceInfo.faceDataInfoList = new FaceDataInfo[multiFaceInfo.faceNum];
                for (int i = 0; i < multiFaceInfo.faceNum; i++)
                {
                    ASF_FaceDataInfo faceDataInfo = MemoryUtil.PtrToStructure<ASF_FaceDataInfo>(multiFaceInfoStruct.faceDataInfoList + MemoryUtil.SizeOf<ASF_FaceDataInfo>() * i);
                    byte[] faceData = new byte[faceDataInfo.dataSize];
                    MemoryUtil.Copy(faceDataInfo.data, faceData, 0, faceDataInfo.dataSize);
                    multiFaceInfo.faceDataInfoList[i] = new FaceDataInfo();
                    multiFaceInfo.faceDataInfoList[i].data = faceData;
                    multiFaceInfo.faceDataInfoList[i].dataSize = faceDataInfo.dataSize;
                }
            }
            return retCode;
        }

        /// <summary>
        /// 人脸检测/人脸追踪
        /// 图像数据以结构体形式传入，对采用更高字节对齐方式的图像兼容性更好
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="multiFaceInfo">多人脸对象</param>
        /// <param name="imageFormat">图片格式</param>
        /// <param name="detectModel">检测模式</param>
        /// <returns>返回0表示正常；返回负数请根据ErrorCodeUtil类注释查看；其他值请在官网-帮助中心查询</returns>
        public int ASFDetectFacesEx(Image image, out MultiFaceInfo multiFaceInfo, ASF_ImagePixelFormat imageFormat = ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8, ASF_DetectModel detectModel = ASF_DetectModel.ASF_DETECT_MODEL_RGB)
        {
            return ASFDetectFaces(image, out multiFaceInfo, imageFormat, detectModel, true);
        }

        /// <summary>
        /// 传入修改后的人脸框，更新人脸信息，用于做双目对齐或其他策略
        /// 注意：LPASF_MultiFaceInfo在该接口中既是入参也是出参
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="multiFaceInfo">多人脸信息</param>
        /// <param name="imageFormat">图片格式</param>
        /// <param name="isEx">是否调用Ex类接口</param>
        /// <returns>返回0表示正常；返回负数请根据ErrorCodeUtil类注释查看；其他值请在官网-帮助中心查询</returns>
        public int ASFUpdateFaceData(Image image, ref MultiFaceInfo multiFaceInfo, ASF_ImagePixelFormat imageFormat = ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8, bool isEx = false)
        {
            if (image == null)
            {
                return ErrorCodeUtil.IMAGE_IS_NULL;
            }
            ImageInfo imageInfo = ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8.Equals(imageFormat) ? ImageUtil.ReadBMP(image) : ImageUtil.ReadBMP_IR(image);
            if (imageInfo == null)
            {
                return ErrorCodeUtil.IMAGE_DATA_READ_FAIL;
            }
            ASF_MultiFaceInfo multiFaceInfoStruct = new ASF_MultiFaceInfo();
            multiFaceInfoStruct.faceNum = multiFaceInfo.faceNum;
            List<IntPtr> faceDataInfoIntPtrList = new List<IntPtr>();
            if (multiFaceInfo.faceNum > 0)
            {
                if (multiFaceInfo.faceID != null)
                {
                    multiFaceInfoStruct.faceID = MemoryUtil.Malloc(multiFaceInfo.faceNum * MemoryUtil.SizeOf<int>());
                    Marshal.Copy(multiFaceInfo.faceID, 0, multiFaceInfoStruct.faceID, multiFaceInfo.faceNum);
                }
                multiFaceInfoStruct.faceOrient = MemoryUtil.Malloc(multiFaceInfo.faceNum * MemoryUtil.SizeOf<int>());
                Marshal.Copy(multiFaceInfo.faceOrients, 0, multiFaceInfoStruct.faceOrient, multiFaceInfo.faceNum);
                multiFaceInfoStruct.faceRect = MemoryUtil.Malloc(MemoryUtil.SizeOf<MRECT>() * multiFaceInfo.faceNum);
                byte[] allByte = new byte[MemoryUtil.SizeOf<MRECT>() * multiFaceInfo.faceNum];
                for (int i = 0; i < multiFaceInfo.faceNum; i++)
                {
                    byte[] tempBytes = MemoryUtil.StructToBytes(multiFaceInfo.faceRects[i]);
                    tempBytes.CopyTo(allByte, MemoryUtil.SizeOf<MRECT>() * i);
                }
                Marshal.Copy(allByte, 0, multiFaceInfoStruct.faceRect, allByte.Length);
                if (multiFaceInfo.wearGlasses != null)
                {
                    multiFaceInfoStruct.wearGlasses = MemoryUtil.Malloc(multiFaceInfo.faceNum * MemoryUtil.SizeOf<float>());
                    Marshal.Copy(multiFaceInfo.wearGlasses, 0, multiFaceInfoStruct.wearGlasses, multiFaceInfo.faceNum);
                }
                if (multiFaceInfo.leftEyeClosed != null)
                {
                    multiFaceInfoStruct.leftEyeClosed = MemoryUtil.Malloc(multiFaceInfo.faceNum * MemoryUtil.SizeOf<int>());
                    Marshal.Copy(multiFaceInfo.leftEyeClosed, 0, multiFaceInfoStruct.leftEyeClosed, multiFaceInfo.faceNum);
                }
                if (multiFaceInfo.rightEyeClosed != null)
                {
                    multiFaceInfoStruct.rightEyeClosed = MemoryUtil.Malloc(multiFaceInfo.faceNum * MemoryUtil.SizeOf<int>());
                    Marshal.Copy(multiFaceInfo.rightEyeClosed, 0, multiFaceInfoStruct.rightEyeClosed, multiFaceInfo.faceNum);
                }
                if (multiFaceInfo.faceShelter != null)
                {
                    multiFaceInfoStruct.faceShelter = MemoryUtil.Malloc(multiFaceInfo.faceNum * MemoryUtil.SizeOf<int>());
                    Marshal.Copy(multiFaceInfo.faceShelter, 0, multiFaceInfoStruct.faceShelter, multiFaceInfo.faceNum);
                }
                int len = MemoryUtil.SizeOf<ASF_FaceDataInfo>();
                byte[] allFaceDataByte = new byte[len * multiFaceInfo.faceNum];
                multiFaceInfoStruct.faceDataInfoList = MemoryUtil.Malloc(multiFaceInfo.faceNum * len);
                for (int i = 0; i < multiFaceInfo.faceNum; i++)
                {
                    ASF_FaceDataInfo faceDataInfo = new ASF_FaceDataInfo();
                    faceDataInfo.data = MemoryUtil.Malloc(multiFaceInfo.faceDataInfoList[i].dataSize);
                    Marshal.Copy(multiFaceInfo.faceDataInfoList[i].data, 0, faceDataInfo.data, multiFaceInfo.faceDataInfoList[i].dataSize);
                    faceDataInfo.dataSize = multiFaceInfo.faceDataInfoList[i].dataSize;
                    byte[] tempBytes = MemoryUtil.StructToBytes(faceDataInfo);
                    tempBytes.CopyTo(allFaceDataByte, len * i);
                    faceDataInfoIntPtrList.Add(faceDataInfo.data);
                }
                Marshal.Copy(allFaceDataByte, 0, multiFaceInfoStruct.faceDataInfoList, allFaceDataByte.Length);
            }
            IntPtr pMultiFaceInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_MultiFaceInfo>());
            MemoryUtil.StructureToPtr(multiFaceInfoStruct, pMultiFaceInfo);
            //调用接口           
            int retCode = -1;
            if (isEx)
            {
                ASF_ImageData asfInfoData = CommonUtil.TransImageDataStructByImageInfo(imageInfo);
                IntPtr pImageInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_ImageData>());
                MemoryUtil.StructureToPtr(asfInfoData, pImageInfo);
                retCode = ASFFunctions.ASFUpdateFaceDataEx(pEngine, pImageInfo, pMultiFaceInfo);
                MemoryUtil.Free(pImageInfo);
            }
            else
            {
                retCode = ASFFunctions.ASFUpdateFaceData(pEngine, imageInfo.width, imageInfo.height, imageInfo.format, imageInfo.imgData, pMultiFaceInfo);
            }
            if(retCode != 0)
            {
                MemoryUtil.FreeArray(pMultiFaceInfo, imageInfo.imgData, multiFaceInfoStruct.faceDataInfoList,multiFaceInfoStruct.faceID, multiFaceInfoStruct.faceOrient, multiFaceInfoStruct.faceRect, multiFaceInfoStruct.faceShelter, multiFaceInfoStruct.leftEyeClosed, multiFaceInfoStruct.rightEyeClosed, multiFaceInfoStruct.wearGlasses);
                MemoryUtil.FreeList(faceDataInfoIntPtrList);
            }

            ASF_MultiFaceInfo multiFaceInfoStructNew = MemoryUtil.PtrToStructure<ASF_MultiFaceInfo>(pMultiFaceInfo);
            //转化非托管内存到托管内存
            multiFaceInfo.faceNum = multiFaceInfoStructNew.faceNum;
            if (multiFaceInfo.faceNum > 0)
            {
                if (multiFaceInfoStructNew.faceID != IntPtr.Zero)
                {
                    multiFaceInfo.faceID = new int[multiFaceInfo.faceNum];
                    Marshal.Copy(multiFaceInfoStructNew.faceID, multiFaceInfo.faceID, 0, multiFaceInfo.faceNum);
                }
                multiFaceInfo.faceOrients = new int[multiFaceInfo.faceNum];
                Marshal.Copy(multiFaceInfoStructNew.faceOrient, multiFaceInfo.faceOrients, 0, multiFaceInfo.faceNum);
                multiFaceInfo.faceRects = new MRECT[multiFaceInfo.faceNum];
                for (int i = 0; i < multiFaceInfo.faceNum; i++)
                {
                    multiFaceInfo.faceRects[i] = MemoryUtil.PtrToStructure<MRECT>(multiFaceInfoStructNew.faceRect + MemoryUtil.SizeOf<MRECT>() * i);
                }
                multiFaceInfo.wearGlasses = new float[multiFaceInfo.faceNum];
                Marshal.Copy(multiFaceInfoStructNew.wearGlasses, multiFaceInfo.wearGlasses, 0, multiFaceInfo.faceNum);
                multiFaceInfo.leftEyeClosed = new int[multiFaceInfo.faceNum];
                Marshal.Copy(multiFaceInfoStructNew.leftEyeClosed, multiFaceInfo.leftEyeClosed, 0, multiFaceInfo.faceNum);
                multiFaceInfo.rightEyeClosed = new int[multiFaceInfo.faceNum];
                Marshal.Copy(multiFaceInfoStructNew.rightEyeClosed, multiFaceInfo.rightEyeClosed, 0, multiFaceInfo.faceNum);
                multiFaceInfo.faceShelter = new int[multiFaceInfo.faceNum];
                Marshal.Copy(multiFaceInfoStructNew.faceShelter, multiFaceInfo.faceShelter, 0, multiFaceInfo.faceNum);
                multiFaceInfo.faceDataInfoList = new FaceDataInfo[multiFaceInfo.faceNum];
                for (int i = 0; i < multiFaceInfo.faceNum; i++)
                {
                    ASF_FaceDataInfo faceDataInfo = MemoryUtil.PtrToStructure<ASF_FaceDataInfo>(multiFaceInfoStructNew.faceDataInfoList + MemoryUtil.SizeOf<ASF_FaceDataInfo>() * i);
                    byte[] faceData = new byte[faceDataInfo.dataSize];
                    MemoryUtil.Copy(faceDataInfo.data, faceData, 0, faceDataInfo.dataSize);
                    multiFaceInfo.faceDataInfoList[i] = new FaceDataInfo();
                    multiFaceInfo.faceDataInfoList[i].data = faceData;
                    multiFaceInfo.faceDataInfoList[i].dataSize = faceDataInfo.dataSize;
                }
            }
            MemoryUtil.FreeArray(pMultiFaceInfo, imageInfo.imgData, multiFaceInfoStruct.faceDataInfoList, multiFaceInfoStruct.faceID, multiFaceInfoStruct.faceOrient, multiFaceInfoStruct.faceRect, multiFaceInfoStruct.faceShelter, multiFaceInfoStruct.leftEyeClosed, multiFaceInfoStruct.rightEyeClosed, multiFaceInfoStruct.wearGlasses);
            MemoryUtil.FreeList(faceDataInfoIntPtrList);
            return retCode;
        }
        
        /// <summary>
        /// 传入修改后的人脸框，更新人脸信息，用于做双目对齐或其他策略
        /// 注意：LPASF_MultiFaceInfo在该接口中既是入参也是出参
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="multiFaceInfo">多人脸信息</param>
        /// <param name="imageFormat">图片格式</param>
        /// <returns>返回0表示正常；返回负数请根据ErrorCodeUtil类注释查看；其他值请在官网-帮助中心查询</returns>
        public int ASFUpdateFaceDataEx(Image image, ref MultiFaceInfo multiFaceInfo, ASF_ImagePixelFormat imageFormat = ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8)
        {
            return ASFUpdateFaceData(image, ref multiFaceInfo, imageFormat, true);
        }

        /// <summary>
        /// 图像质量检测，（RGB模式： 识别阈值：0.49 注册阈值：0.63  口罩模式：识别阈值：0.29）
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="multiFaceInfo">多人脸信息</param>
        /// <param name="confidenceLevel">图像质量检测结果</param>
        /// <param name="faceIndex">人脸索引</param>
        /// <param name="isMask">是否戴口罩：true->是;false->否</param>
        /// <param name="imageFormat">图片格式</param>
        /// <param name="detectModel">预留字段，当前版本使用默认参数即可</param>
        /// <param name="isEx">是否调用Ex类接口</param>
        /// <returns>返回0表示正常；返回负数请根据ErrorCodeUtil类注释查看；其他值请在官网-帮助中心查询</returns>
        public int ASFImageQualityDetect(Image image, MultiFaceInfo multiFaceInfo, out float confidenceLevel, int faceIndex = 0, bool isMask = false, ASF_ImagePixelFormat imageFormat = ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8, ASF_DetectModel detectModel = ASF_DetectModel.ASF_DETECT_MODEL_RGB, bool isEx = false)
        {
            int retCode = -1;
            confidenceLevel = 0f;
            if (multiFaceInfo == null)
            {
                return ErrorCodeUtil.MULPTIFACEINFO_IS_NULL;
            }
            if (image == null)
            {
                return ErrorCodeUtil.IMAGE_IS_NULL;
            }
            if (faceIndex >= multiFaceInfo.faceNum)
            {
                return ErrorCodeUtil.FACEINDEX_INVALID;
            }
            ImageInfo imageInfo = ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8.Equals(imageFormat) ? ImageUtil.ReadBMP(image) : ImageUtil.ReadBMP_IR(image);
            if (imageInfo == null)
            {
                return ErrorCodeUtil.IMAGE_DATA_READ_FAIL;
            }
            //获取某个单人脸信息
            ASF_SingleFaceInfo singleFaceInfo = new ASF_SingleFaceInfo();
            IntPtr pSingleFaceInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_SingleFaceInfo>());
            singleFaceInfo.faceRect = multiFaceInfo.faceRects[faceIndex];
            singleFaceInfo.faceOrient = multiFaceInfo.faceOrients[faceIndex];
            singleFaceInfo.faceDataInfo = new ASF_FaceDataInfo();
            singleFaceInfo.faceDataInfo.data = MemoryUtil.Malloc(multiFaceInfo.faceDataInfoList[faceIndex].dataSize);
            singleFaceInfo.faceDataInfo.dataSize = multiFaceInfo.faceDataInfoList[faceIndex].dataSize;
            MemoryUtil.Copy(multiFaceInfo.faceDataInfoList[faceIndex].data,0, singleFaceInfo.faceDataInfo.data, multiFaceInfo.faceDataInfoList[faceIndex].dataSize);
            MemoryUtil.StructureToPtr(singleFaceInfo, pSingleFaceInfo);            

            //调用SDK接口
            if (isEx)
            {
                ASF_ImageData asfInfoData = CommonUtil.TransImageDataStructByImageInfo(imageInfo);
                IntPtr pImageInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_ImageData>());
                MemoryUtil.StructureToPtr(asfInfoData, pImageInfo);
                retCode = ASFFunctions.ASFImageQualityDetectEx(pEngine, pImageInfo, pSingleFaceInfo, isMask ? ASF_MaskOrNot.ASF_MASK : ASF_MaskOrNot.ASF_NOT, ref confidenceLevel, detectModel);
                MemoryUtil.Free(pImageInfo);
            }
            else
            {
                retCode = ASFFunctions.ASFImageQualityDetect(pEngine, imageInfo.width, imageInfo.height, imageInfo.format, imageInfo.imgData, pSingleFaceInfo, isMask ? ASF_MaskOrNot.ASF_MASK : ASF_MaskOrNot.ASF_NOT, ref confidenceLevel, detectModel);
            }
            MemoryUtil.FreeArray(imageInfo.imgData, pSingleFaceInfo);
            return retCode;
        }

        /// <summary>
        /// 图像质量检测，（RGB模式： 识别阈值：0.49 注册阈值：0.63  口罩模式：识别阈值：0.29）
        /// 图像数据以结构体形式传入，对采用更高字节对齐方式的图像兼容性更好
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="multiFaceInfo">多人脸信息</param>
        /// <param name="confidenceLevel">图像质量检测结果</param>
        /// <param name="faceIndex">人脸索引</param>
        /// <param name="isMask">是否戴口罩：true->是;false->否</param>
        /// <param name="imageFormat">图片格式</param>
        /// <param name="detectModel">预留字段，当前版本使用默认参数即可</param>
        /// <returns>返回0表示正常；返回负数请根据ErrorCodeUtil类注释查看；其他值请在官网-帮助中心查询</returns>
        public int ASFImageQualityDetectEx(Image image, MultiFaceInfo multiFaceInfo, out float confidenceLevel, int faceIndex = 0, bool isMask = false, ASF_ImagePixelFormat imageFormat = ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8, ASF_DetectModel detectModel = ASF_DetectModel.ASF_DETECT_MODEL_RGB)
        {
            return ASFImageQualityDetect(image, multiFaceInfo, out confidenceLevel, faceIndex, isMask, imageFormat, detectModel, true);
        }

        /// <summary>
        /// 人脸信息检测（年龄/性别/人脸3D角度）
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="multiFaceInfo">多人脸对象</param>
        /// <param name="combinedMask">检测属性</param>
        /// <param name="imageFormat">图片格式</param>
        /// <param name="isEx">是否调用Ex类接口</param>
        /// <returns>返回0表示正常；返回负数请根据ErrorCodeUtil类注释查看；其他值请在官网-帮助中心查询</returns>
        public int ASFProcess(Image image, MultiFaceInfo multiFaceInfo, int combinedMask, ASF_ImagePixelFormat imageFormat = ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8, bool isEx = false)
        {
            int retCode = -1;
            if (multiFaceInfo == null)
            {
                return ErrorCodeUtil.MULPTIFACEINFO_IS_NULL;
            }
            if (image == null)
            {
                return ErrorCodeUtil.IMAGE_IS_NULL;
            }
            ImageInfo imageInfo = ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8.Equals(imageFormat) ? ImageUtil.ReadBMP(image) : ImageUtil.ReadBMP_IR(image);
            if (imageInfo == null)
            {
                return ErrorCodeUtil.IMAGE_DATA_READ_FAIL;
            }
            //将多人脸对象的信息转化到结构体中
            ASF_MultiFaceInfo multiFaceInfoStruct = new ASF_MultiFaceInfo();
            IntPtr pMultiFaceInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_MultiFaceInfo>());
            multiFaceInfoStruct.faceNum = multiFaceInfo.faceNum;
            List<IntPtr> faceDataInfoIntPtrList = new List<IntPtr>();
            if (multiFaceInfo.faceNum > 0)
            {
                if (multiFaceInfo.faceID != null)
                {
                    multiFaceInfoStruct.faceID = MemoryUtil.Malloc(multiFaceInfo.faceNum * MemoryUtil.SizeOf<int>());
                    Marshal.Copy(multiFaceInfo.faceID, 0, multiFaceInfoStruct.faceID, multiFaceInfo.faceNum);
                }
                multiFaceInfoStruct.faceOrient = MemoryUtil.Malloc(multiFaceInfo.faceNum * MemoryUtil.SizeOf<int>());
                Marshal.Copy(multiFaceInfo.faceOrients, 0, multiFaceInfoStruct.faceOrient, multiFaceInfo.faceNum);
                multiFaceInfoStruct.faceRect = MemoryUtil.Malloc(MemoryUtil.SizeOf<MRECT>() * multiFaceInfo.faceNum);
                byte[] allByte = new byte[MemoryUtil.SizeOf<MRECT>() * multiFaceInfo.faceNum];
                for (int i = 0; i < multiFaceInfo.faceNum; i++)
                {
                    byte[] tempBytes = MemoryUtil.StructToBytes(multiFaceInfo.faceRects[i]);
                    tempBytes.CopyTo(allByte, MemoryUtil.SizeOf<MRECT>() * i);
                }
                Marshal.Copy(allByte, 0, multiFaceInfoStruct.faceRect, allByte.Length);
                if(multiFaceInfo.wearGlasses != null)
                {
                    multiFaceInfoStruct.wearGlasses = MemoryUtil.Malloc(multiFaceInfo.faceNum * MemoryUtil.SizeOf<float>());
                    Marshal.Copy(multiFaceInfo.wearGlasses, 0, multiFaceInfoStruct.wearGlasses, multiFaceInfo.faceNum);
                }
                if(multiFaceInfo.leftEyeClosed != null)
                {
                    multiFaceInfoStruct.leftEyeClosed = MemoryUtil.Malloc(multiFaceInfo.faceNum * MemoryUtil.SizeOf<int>());
                    Marshal.Copy(multiFaceInfo.leftEyeClosed, 0, multiFaceInfoStruct.leftEyeClosed, multiFaceInfo.faceNum);
                }
                if (multiFaceInfo.rightEyeClosed != null)
                {
                    multiFaceInfoStruct.rightEyeClosed = MemoryUtil.Malloc(multiFaceInfo.faceNum * MemoryUtil.SizeOf<int>());
                    Marshal.Copy(multiFaceInfo.rightEyeClosed, 0, multiFaceInfoStruct.rightEyeClosed, multiFaceInfo.faceNum);
                }
                if(multiFaceInfo.faceShelter != null)
                {
                    multiFaceInfoStruct.faceShelter = MemoryUtil.Malloc(multiFaceInfo.faceNum * MemoryUtil.SizeOf<int>());
                    Marshal.Copy(multiFaceInfo.faceShelter, 0, multiFaceInfoStruct.faceShelter, multiFaceInfo.faceNum);
                }
                int len = MemoryUtil.SizeOf<ASF_FaceDataInfo>();
                byte[] allFaceDataByte = new byte[len * multiFaceInfo.faceNum];
                multiFaceInfoStruct.faceDataInfoList = MemoryUtil.Malloc(multiFaceInfo.faceNum * len);
                for (int i = 0; i < multiFaceInfo.faceNum; i++)
                {
                    ASF_FaceDataInfo faceDataInfo = new ASF_FaceDataInfo();
                    faceDataInfo.data = MemoryUtil.Malloc(multiFaceInfo.faceDataInfoList[i].dataSize);
                    Marshal.Copy(multiFaceInfo.faceDataInfoList[i].data, 0, faceDataInfo.data, multiFaceInfo.faceDataInfoList[i].dataSize);
                    faceDataInfo.dataSize = multiFaceInfo.faceDataInfoList[i].dataSize;
                    byte[] tempBytes = MemoryUtil.StructToBytes(faceDataInfo);
                    tempBytes.CopyTo(allFaceDataByte, len * i);
                    faceDataInfoIntPtrList.Add(faceDataInfo.data);
                }
                Marshal.Copy(allFaceDataByte, 0, multiFaceInfoStruct.faceDataInfoList, allFaceDataByte.Length);
            }

            MemoryUtil.StructureToPtr(multiFaceInfoStruct, pMultiFaceInfo);
            //调用SDK接口
            if (isEx)
            {
                ASF_ImageData asfInfoData = CommonUtil.TransImageDataStructByImageInfo(imageInfo);
                IntPtr pImageInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_ImageData>());
                MemoryUtil.StructureToPtr(asfInfoData, pImageInfo);
                retCode = ASFFunctions.ASFProcessEx(pEngine, pImageInfo, pMultiFaceInfo, combinedMask);
                MemoryUtil.Free(pImageInfo);
            }
            else
            {
                retCode = ASFFunctions.ASFProcess(pEngine, imageInfo.width, imageInfo.height, imageInfo.format, imageInfo.imgData, pMultiFaceInfo, combinedMask);
            }
            //释放内存
            MemoryUtil.FreeArray(imageInfo.imgData, multiFaceInfoStruct.faceDataInfoList, multiFaceInfoStruct.faceID, multiFaceInfoStruct.faceOrient, multiFaceInfoStruct.faceRect, multiFaceInfoStruct.faceShelter, multiFaceInfoStruct.leftEyeClosed, multiFaceInfoStruct.rightEyeClosed, multiFaceInfoStruct.wearGlasses, pMultiFaceInfo);
            MemoryUtil.FreeList(faceDataInfoIntPtrList);
            return retCode;
        }

        /// <summary>
        /// 年龄/性别/人脸3D角度（该接口仅支持RGB图像），最多支持4张人脸信息检测，超过部分返回未知
	    /// RGB活体仅支持单人脸检测，该接口不支持检测IR活体
        ///  图像数据以结构体形式传入，对采用更高字节对齐方式的图像兼容性更好
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="multiFaceInfo">多人脸对象，用户根据待检测的功能选择需要使用的人脸。</param>
        /// <param name="combinedMask">检测属性:只支持初始化时候指定需要检测的功能，在process时进一步在这个已经指定的功能集中继续筛选</param>
        /// <param name="imageFormat">图片格式</param>
        /// <returns>返回0表示正常；返回负数请根据ErrorCodeUtil类注释查看；其他值请在官网-帮助中心查询</returns>
        public int ASFProcessEx(Image image, MultiFaceInfo multiFaceInfo, int combinedMask, ASF_ImagePixelFormat imageFormat = ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8)
        {
            return ASFProcess(image, multiFaceInfo, combinedMask, imageFormat, true);
        }

        /// <summary>
        /// 该接口目前仅支持单人脸IR活体检测（不支持年龄、性别、3D角度的检测），默认取第一张人脸
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="multiFaceInfo">人脸信息，用户根据待检测的功能选择需要使用的人脸。</param>
        /// <param name="combinedMask">检测属性,目前只支持传入ASF_IR_LIVENESS属性的传入，且初始化接口需要传入 </param>
        /// <param name="imageFormat">图像格式</param>
        /// <param name="isEx">是否调用Ex类接口</param>
        /// <returns>返回0表示正常；返回负数请根据ErrorCodeUtil类注释查看；其他值请在官网-帮助中心查询</returns>
        public int ASFProcess_IR(Image image, MultiFaceInfo multiFaceInfo, int combinedMask, ASF_ImagePixelFormat imageFormat = ASF_ImagePixelFormat.ASVL_PAF_GRAY, bool isEx = false)
        {
            int retCode = -1;
            if (multiFaceInfo == null)
            {
                return ErrorCodeUtil.MULPTIFACEINFO_IS_NULL;
            }
            if (image == null)
            {
                return ErrorCodeUtil.IMAGE_IS_NULL;
            }
            ImageInfo imageInfo = ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8.Equals(imageFormat) ? ImageUtil.ReadBMP(image) : ImageUtil.ReadBMP_IR(image);
            if (imageInfo == null)
            {
                return ErrorCodeUtil.IMAGE_DATA_READ_FAIL;
            }
            //转化多人脸信息
            ASF_MultiFaceInfo multiFaceInfoStruct = new ASF_MultiFaceInfo();
            IntPtr pMultiFaceInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_MultiFaceInfo>());
            multiFaceInfoStruct.faceNum = multiFaceInfo.faceNum;
            List<IntPtr> faceDataInfoIntPtrList = new List<IntPtr>();
            if (multiFaceInfo.faceNum > 0)
            {
                if (multiFaceInfo.faceID != null)
                {
                    multiFaceInfoStruct.faceID = MemoryUtil.Malloc(multiFaceInfo.faceNum * MemoryUtil.SizeOf<int>());
                    Marshal.Copy(multiFaceInfo.faceID, 0, multiFaceInfoStruct.faceID, multiFaceInfo.faceNum);
                }
                multiFaceInfoStruct.faceOrient = MemoryUtil.Malloc(multiFaceInfo.faceNum * MemoryUtil.SizeOf<int>());
                Marshal.Copy(multiFaceInfo.faceOrients, 0, multiFaceInfoStruct.faceOrient, multiFaceInfo.faceNum);
                multiFaceInfoStruct.faceRect = MemoryUtil.Malloc(MemoryUtil.SizeOf<MRECT>() * multiFaceInfo.faceNum);
                byte[] allByte = new byte[MemoryUtil.SizeOf<MRECT>() * multiFaceInfo.faceNum];
                for (int i = 0; i < multiFaceInfo.faceNum; i++)
                {
                    byte[] tempBytes = MemoryUtil.StructToBytes(multiFaceInfo.faceRects[i]);
                    tempBytes.CopyTo(allByte, MemoryUtil.SizeOf<MRECT>() * i);
                }
                Marshal.Copy(allByte, 0, multiFaceInfoStruct.faceRect, allByte.Length);
                if (multiFaceInfo.wearGlasses != null)
                {
                    multiFaceInfoStruct.wearGlasses = MemoryUtil.Malloc(multiFaceInfo.faceNum * MemoryUtil.SizeOf<float>());
                    Marshal.Copy(multiFaceInfo.wearGlasses, 0, multiFaceInfoStruct.wearGlasses, multiFaceInfo.faceNum);
                }
                if (multiFaceInfo.leftEyeClosed != null)
                {
                    multiFaceInfoStruct.leftEyeClosed = MemoryUtil.Malloc(multiFaceInfo.faceNum * MemoryUtil.SizeOf<int>());
                    Marshal.Copy(multiFaceInfo.leftEyeClosed, 0, multiFaceInfoStruct.leftEyeClosed, multiFaceInfo.faceNum);
                }
                if (multiFaceInfo.rightEyeClosed != null)
                {
                    multiFaceInfoStruct.rightEyeClosed = MemoryUtil.Malloc(multiFaceInfo.faceNum * MemoryUtil.SizeOf<int>());
                    Marshal.Copy(multiFaceInfo.rightEyeClosed, 0, multiFaceInfoStruct.rightEyeClosed, multiFaceInfo.faceNum);
                }
                if (multiFaceInfo.faceShelter != null)
                {
                    multiFaceInfoStruct.faceShelter = MemoryUtil.Malloc(multiFaceInfo.faceNum * MemoryUtil.SizeOf<int>());
                    Marshal.Copy(multiFaceInfo.faceShelter, 0, multiFaceInfoStruct.faceShelter, multiFaceInfo.faceNum);
                }
                int len = MemoryUtil.SizeOf<ASF_FaceDataInfo>();
                byte[] allFaceDataByte = new byte[len * multiFaceInfo.faceNum];
                multiFaceInfoStruct.faceDataInfoList = MemoryUtil.Malloc(multiFaceInfo.faceNum * len);
                for (int i = 0; i < multiFaceInfo.faceNum; i++)
                {
                    ASF_FaceDataInfo faceDataInfo = new ASF_FaceDataInfo();
                    faceDataInfo.data = MemoryUtil.Malloc(multiFaceInfo.faceDataInfoList[i].dataSize);
                    Marshal.Copy(multiFaceInfo.faceDataInfoList[i].data, 0, faceDataInfo.data, multiFaceInfo.faceDataInfoList[i].dataSize);
                    faceDataInfo.dataSize = multiFaceInfo.faceDataInfoList[i].dataSize;
                    byte[] tempBytes = MemoryUtil.StructToBytes(faceDataInfo);
                    tempBytes.CopyTo(allFaceDataByte, len * i);
                    faceDataInfoIntPtrList.Add(faceDataInfo.data);
                }
                Marshal.Copy(allFaceDataByte, 0, multiFaceInfoStruct.faceDataInfoList, allFaceDataByte.Length);
            }
            MemoryUtil.StructureToPtr(multiFaceInfoStruct, pMultiFaceInfo);
            //调用SDK接口
            if (isEx)
            {
                ASF_ImageData asfInfoData = CommonUtil.TransImageDataStructByImageInfo(imageInfo);
                IntPtr pImageInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_ImageData>());
                MemoryUtil.StructureToPtr(asfInfoData, pImageInfo);
                retCode = ASFFunctions.ASFProcessEx_IR(pEngine, pImageInfo, pMultiFaceInfo, combinedMask);
                MemoryUtil.Free(pImageInfo);
            }
            else
            {
                retCode = ASFFunctions.ASFProcess_IR(pEngine, imageInfo.width, imageInfo.height, imageInfo.format, imageInfo.imgData, pMultiFaceInfo, combinedMask);
            }
            //释放内存
            MemoryUtil.FreeArray(imageInfo.imgData, multiFaceInfoStruct.faceDataInfoList, multiFaceInfoStruct.faceID, multiFaceInfoStruct.faceOrient, multiFaceInfoStruct.faceRect, multiFaceInfoStruct.faceShelter, multiFaceInfoStruct.leftEyeClosed, multiFaceInfoStruct.rightEyeClosed, multiFaceInfoStruct.wearGlasses, pMultiFaceInfo);
            MemoryUtil.FreeList(faceDataInfoIntPtrList);

            return retCode;
        }

        /// <summary>
        /// 该接口目前仅支持单人脸IR活体检测（不支持年龄、性别、3D角度的检测），默认取第一张人脸
	    /// 图像数据以结构体形式传入，对采用更高字节对齐方式的图像兼容性更好
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="multiFaceInfo">人脸信息，用户根据待检测的功能选择需要使用的人脸。</param>
        /// <param name="combinedMask">检测属性:目前只支持传入ASF_IR_LIVENESS属性的传入，且初始化接口需要传入</param>
        /// <param name="imageFormat">图像格式</param>
        /// <returns>返回0表示正常；返回负数请根据ErrorCodeUtil类注释查看；其他值请在官网-帮助中心查询</returns>
        public int ASFProcessEx_IR(Image image, MultiFaceInfo multiFaceInfo, int combinedMask, ASF_ImagePixelFormat imageFormat = ASF_ImagePixelFormat.ASVL_PAF_GRAY)
        {
            return ASFProcess_IR(image, multiFaceInfo, combinedMask, imageFormat, true);
        }

        /// <summary>
        /// 单人脸特征提取
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="multiFaceInfo">人脸框对象</param>
        /// <param name="registerOrNot"> 注册 1 识别为 0</param>
        /// <param name="faceFeature">[out]特征结果</param>
        /// <param name="faceIndex">人脸索引</param>
        /// <param name="isMask">是否戴口罩：true->是;false->否</param>
        /// <param name="imageFormat">图片格式</param>
        /// <param name="isEx">是否调用Ex类接口</param>
        /// <returns>返回0表示正常；返回负数请根据ErrorCodeUtil类注释查看；其他值请在官网-帮助中心查询</returns>
        public int ASFFaceFeatureExtract(Image image, MultiFaceInfo multiFaceInfo, ASF_RegisterOrNot registerOrNot, out FaceFeature faceFeature, int faceIndex = 0,bool isMask = false, ASF_ImagePixelFormat imageFormat = ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8, bool isEx = false)
        {
            int retCode = -1;
            faceFeature = new FaceFeature();
            if (multiFaceInfo == null)
            {
                return ErrorCodeUtil.MULPTIFACEINFO_IS_NULL;
            }
            if (faceIndex >= multiFaceInfo.faceNum)
            {
                return ErrorCodeUtil.FACEINDEX_INVALID;
            }
            if (image == null)
            {
                return ErrorCodeUtil.IMAGE_IS_NULL;
            }
            ImageInfo imageInfo = ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8.Equals(imageFormat) ? ImageUtil.ReadBMP(image) : ImageUtil.ReadBMP_IR(image);
            if (imageInfo == null)
            {
                return ErrorCodeUtil.IMAGE_DATA_READ_FAIL;
            }
            //获取某个单人脸信息
            ASF_SingleFaceInfo singleFaceInfo = new ASF_SingleFaceInfo();
            IntPtr pSingleFaceInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_SingleFaceInfo>());
            singleFaceInfo.faceRect = multiFaceInfo.faceRects[faceIndex];
            singleFaceInfo.faceOrient = multiFaceInfo.faceOrients[faceIndex];
            singleFaceInfo.faceDataInfo = new ASF_FaceDataInfo();
            singleFaceInfo.faceDataInfo.data = MemoryUtil.Malloc(multiFaceInfo.faceDataInfoList[faceIndex].dataSize);
            singleFaceInfo.faceDataInfo.dataSize = multiFaceInfo.faceDataInfoList[faceIndex].dataSize;
            MemoryUtil.Copy(multiFaceInfo.faceDataInfoList[faceIndex].data, 0, singleFaceInfo.faceDataInfo.data, multiFaceInfo.faceDataInfoList[faceIndex].dataSize);
            MemoryUtil.StructureToPtr(singleFaceInfo, pSingleFaceInfo);
            IntPtr pAsfFaceFeature = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_FaceFeature>());
            //调用SDK接口
            if (isEx)
            {
                ASF_ImageData asfInfoData = CommonUtil.TransImageDataStructByImageInfo(imageInfo);
                IntPtr pImageInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_ImageData>());
                MemoryUtil.StructureToPtr(asfInfoData, pImageInfo);
                retCode = ASFFunctions.ASFFaceFeatureExtractEx(pEngine, pImageInfo, pSingleFaceInfo, registerOrNot, isMask ? ASF_MaskOrNot.ASF_MASK : ASF_MaskOrNot.ASF_NOT, pAsfFaceFeature);
                MemoryUtil.Free(pImageInfo);
            }
            else
            {
                retCode = ASFFunctions.ASFFaceFeatureExtract(pEngine, imageInfo.width, imageInfo.height, imageInfo.format, imageInfo.imgData, pSingleFaceInfo, registerOrNot, isMask ? ASF_MaskOrNot.ASF_MASK : ASF_MaskOrNot.ASF_NOT, pAsfFaceFeature);
            }
            if (retCode == 0)
            {
                //获取特征结构体，并转化
                ASF_FaceFeature asfFeature = MemoryUtil.PtrToStructure<ASF_FaceFeature>(pAsfFaceFeature);
                byte[] feature = new byte[asfFeature.featureSize];
                MemoryUtil.Copy(asfFeature.feature, feature, 0, asfFeature.featureSize);
                faceFeature.featureSize = asfFeature.featureSize;
                faceFeature.feature = feature;
            }            
            MemoryUtil.FreeArray(pSingleFaceInfo, pAsfFaceFeature, imageInfo.imgData);
            return retCode;
        }

        /// <summary>
        /// 单人脸特征提取
        /// 图像数据以结构体形式传入，对采用更高字节对齐方式的图像兼容性更好
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="multiFaceInfo">人脸框对象</param>
        /// <param name="registerOrNot"> 注册 1 识别为 0</param>
        /// <param name="faceFeature">[out]特征结果</param>
        /// <param name="faceIndex">人脸索引</param>
        /// <param name="isMask">是否戴口罩：true->是;false->否</param>
        /// <param name="imageFormat">图片格式</param>
        /// <returns>返回0表示正常；返回负数请根据ErrorCodeUtil类注释查看；其他值请在官网-帮助中心查询</returns>
        public int ASFFaceFeatureExtractEx(Image image, MultiFaceInfo multiFaceInfo, ASF_RegisterOrNot registerOrNot, out FaceFeature faceFeature, int faceIndex = 0, bool isMask = false, ASF_ImagePixelFormat imageFormat = ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8)
        {
            return ASFFaceFeatureExtract(image, multiFaceInfo, registerOrNot, out faceFeature, faceIndex, isMask, imageFormat, false);
        }

        /// <summary>
        /// 人脸特征比对，推荐阈值 ASF_LIFE_PHOTO：0.80  ASF_ID_PHOTO：0.82
        /// </summary>
        /// <param name="faceFeature1">待比较人脸特征1</param>
        /// <param name="faceFeature2">待比较人脸特征2</param>
        /// <param name="similarity"> 比较结果，置信度数值</param>
        /// <param name="compareModel">ASF_LIFE_PHOTO：用于生活照之间的特征比对；ASF_ID_PHOTO：用于证件照或证件照和生活照之间的特征比对</param>
        /// <returns>返回0表示正常；返回负数请根据ErrorCodeUtil类注释查看；其他值请在官网-帮助中心查询</returns>
        public int ASFFaceFeatureCompare(FaceFeature faceFeature1, FaceFeature faceFeature2, out float similarity, ASF_CompareModel compareModel = ASF_CompareModel.ASF_LIFE_PHOTO)
        {
            int retCode = -1;
            similarity = 0f;
            if (faceFeature1 == null || faceFeature2 == null)
            {
                return ErrorCodeUtil.FEATURE_IS_NULL;
            }
            #region 将特征对象转化为特征结构体，再转化为非托管内存
            ASF_FaceFeature asfFeatureStruct1 = new ASF_FaceFeature();
            asfFeatureStruct1.featureSize = faceFeature1.featureSize;
            asfFeatureStruct1.feature = MemoryUtil.Malloc(asfFeatureStruct1.featureSize);
            MemoryUtil.Copy(faceFeature1.feature, 0, asfFeatureStruct1.feature, asfFeatureStruct1.featureSize);
            IntPtr pFeature1 = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_FaceFeature>());
            MemoryUtil.StructureToPtr(asfFeatureStruct1, pFeature1);

            ASF_FaceFeature asfFeatureStruct2 = new ASF_FaceFeature();
            asfFeatureStruct2.featureSize = faceFeature2.featureSize;
            asfFeatureStruct2.feature = MemoryUtil.Malloc(asfFeatureStruct2.featureSize);
            MemoryUtil.Copy(faceFeature2.feature, 0, asfFeatureStruct2.feature, asfFeatureStruct2.featureSize);
            IntPtr pFeature2 = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_FaceFeature>());
            MemoryUtil.StructureToPtr(asfFeatureStruct2, pFeature2);
            #endregion
            //调用SDK接口
            retCode = ASFFunctions.ASFFaceFeatureCompare(pEngine, pFeature1, pFeature2, ref similarity, compareModel);
            MemoryUtil.FreeArray(pFeature1, pFeature2, asfFeatureStruct1.feature, asfFeatureStruct2.feature);

            return retCode;
        }

        //******************************** 年龄相关 **********************************************

        /// <summary>
        /// 获取年龄结果
        /// </summary>
        /// <param name="pEngine">引擎handle</param>
        /// <param name="ageInfo">年龄结果</param>
        /// <returns>返回0表示正常；返回其他值请在官网-帮助中心查询</returns>
        public int ASFGetAge(out AgeInfo ageInfo)
        {
            int retCode = -1;
            ageInfo = new AgeInfo();
            IntPtr pAgeInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_AgeInfo>());
            //调用SDK接口
            retCode = ASFFunctions.ASFGetAge(pEngine, pAgeInfo);
            if (retCode != 0)
            {
                MemoryUtil.Free(pAgeInfo);
                return retCode;
            }
            //转化结果
            ASF_AgeInfo asfAgeInfo = MemoryUtil.PtrToStructure<ASF_AgeInfo>(pAgeInfo);
            ageInfo.num = asfAgeInfo.num;
            if (ageInfo.num > 0)
            {
                ageInfo.ageArray = new int[ageInfo.num];
                Marshal.Copy(asfAgeInfo.ageArray, ageInfo.ageArray, 0, ageInfo.num);
            }
            MemoryUtil.FreeArray(pAgeInfo);
            return retCode;
        }

        //******************************** 性别相关 **********************************************

        /// <summary>
        /// 获取性别结果
        /// </summary>
        /// <param name="pEngine">引擎handle</param>
        /// <param name="genderInfo">性别结果</param>
        /// <returns>返回0表示正常；返回其他值请在官网-帮助中心查询</returns>
        public int ASFGetGender(out GenderInfo genderInfo)
        {
            int retCode = -1;
            genderInfo = new GenderInfo();
            IntPtr pGenderInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_GenderInfo>());
            //调用SDK接口
            retCode = ASFFunctions.ASFGetGender(pEngine, pGenderInfo);
            if (retCode != 0)
            {
                MemoryUtil.Free(pGenderInfo);
                return retCode;
            }
            //转化结果
            ASF_GenderInfo asfGenderInfo = MemoryUtil.PtrToStructure<ASF_GenderInfo>(pGenderInfo);
            genderInfo.num = asfGenderInfo.num;
            if (genderInfo.num > 0)
            {
                genderInfo.genderArray = new int[genderInfo.num];
                Marshal.Copy(asfGenderInfo.genderArray, genderInfo.genderArray, 0, genderInfo.num);
            }
            MemoryUtil.FreeArray(pGenderInfo);
            return retCode;
        }

        //******************************** 人脸3D 角度信息 ***********************************

        /// <summary>
        /// 获取3D角度信息
        /// </summary>
        /// <param name="pEngine">引擎handle</param>
        /// <param name="faceAngle">3D角度信息结果</param>
        /// <returns>返回0表示正常；返回其他值请在官网-帮助中心查询</returns>
        public int ASFGetFace3DAngle(out Face3DAngle faceAngle)
        {
            int retCode = -1;
            faceAngle = new Face3DAngle();
            IntPtr pFaceAngle = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_Face3DAngle>());
            //调用SDK接口
            retCode = ASFFunctions.ASFGetFace3DAngle(pEngine, pFaceAngle);
            if (retCode != 0)
            {
                MemoryUtil.Free(pFaceAngle);
                return retCode;
            }
            //转化结果
            ASF_Face3DAngle asfFaceAngle = MemoryUtil.PtrToStructure<ASF_Face3DAngle>(pFaceAngle);
            faceAngle.num = asfFaceAngle.num;
            if (faceAngle.num > 0)
            {
                faceAngle.pitch = new float[faceAngle.num];
                Marshal.Copy(asfFaceAngle.pitch, faceAngle.pitch, 0, faceAngle.num);
                faceAngle.roll = new float[faceAngle.num];
                Marshal.Copy(asfFaceAngle.roll, faceAngle.roll, 0, faceAngle.num);
                faceAngle.yaw = new float[faceAngle.num];
                Marshal.Copy(asfFaceAngle.yaw, faceAngle.yaw, 0, faceAngle.num);
            }
            MemoryUtil.FreeArray(pFaceAngle);
            return retCode;
        }

        //******************************** 活体信息 ***********************************

        /// <summary>
        /// 设置活体阈值：取值范围[0-1]内部默认数值RGB-0.5，IR-0.7， 用户可以根据实际需求，设置不同的阈值
        /// </summary>
        /// <param name="rgbThreshold">RGB活体阈值</param>
        /// <param name="irThreshole">IR活体阈值</param>
        /// <returns>返回0表示正常；返回其他值请在官网-帮助中心查询</returns>
        public int ASFSetLivenessParam(float rgbThreshold = 0.5f, float irThreshole = 0.7f)
        {
            int retCode = -1;
            ASF_LivenessThreshold livebessThreshold = new ASF_LivenessThreshold();
            //对应设置阈值
            livebessThreshold.thresholdmodel_BGR = (rgbThreshold >= 0 && rgbThreshold <= 1) ? rgbThreshold : 0.5f;
            livebessThreshold.thresholdmodel_IR = (irThreshole >= 0 && irThreshole <= 1) ? irThreshole : 0.7f;
            IntPtr pLivenessThreshold = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_LivenessThreshold>());
            MemoryUtil.StructureToPtr(livebessThreshold, pLivenessThreshold);
            //调用SDK接口
            retCode = ASFFunctions.ASFSetLivenessParam(pEngine, pLivenessThreshold);
            MemoryUtil.Free(pLivenessThreshold);
            return retCode;
        }

        /// <summary>
        /// 获取RGB活体结果
        /// </summary>
        /// <param name="pEngine">引擎handle</param>
        /// <param name="livenessInfo">RGB活体结果</param>
        /// <returns>返回0表示正常；返回其他值请在官网-帮助中心查询</returns>
        public int ASFGetLivenessScore(out LivenessInfo livenessInfo)
        {
            int retCode = -1;
            livenessInfo = new LivenessInfo();
            IntPtr pLiveness = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_LivenessInfo>());
            //调用SDK接口
            retCode = ASFFunctions.ASFGetLivenessScore(pEngine, pLiveness);
            if (retCode != 0)
            {
                MemoryUtil.Free(pLiveness);
                return retCode;
            }
            //转化结果
            ASF_LivenessInfo asfLivenessInfo = MemoryUtil.PtrToStructure<ASF_LivenessInfo>(pLiveness);
            livenessInfo.num = asfLivenessInfo.num;
            if (asfLivenessInfo.num > 0)
            {
                livenessInfo.isLive = new int[asfLivenessInfo.num];
                Marshal.Copy(asfLivenessInfo.isLive, livenessInfo.isLive, 0, asfLivenessInfo.num);
            }
            MemoryUtil.FreeArray(pLiveness);
            return retCode;
        }        

        /// <summary>
        /// 获取IR活体结果
        /// </summary>
        /// <param name="livenessInfo">IR活体结果</param>
        /// <returns>返回0表示正常；返回其他值请在官网-帮助中心查询</returns>
        public int ASFGetLivenessScore_IR(out LivenessInfo livenessInfo)
        {
            int retCode = -1;
            livenessInfo = new LivenessInfo();
            IntPtr pLiveness = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_LivenessInfo>());
            //调用SDK接口
            retCode = ASFFunctions.ASFGetLivenessScore_IR(pEngine, pLiveness);
            if (retCode != 0)
            {
                MemoryUtil.Free(pLiveness);
                return retCode;
            }
            //转化结果
            ASF_LivenessInfo asfLivenessInfo = MemoryUtil.PtrToStructure<ASF_LivenessInfo>(pLiveness);
            livenessInfo.num = asfLivenessInfo.num;
            if (asfLivenessInfo.num > 0)
            {
                livenessInfo.isLive = new int[asfLivenessInfo.num];
                Marshal.Copy(asfLivenessInfo.isLive, livenessInfo.isLive, 0, asfLivenessInfo.num);
            }
            MemoryUtil.FreeArray(pLiveness);
            return retCode;
        }

        //******************************** 口罩检测相关 **********************************************

        /// <summary>
        /// 获取口罩检测的结果
        /// </summary>
        /// <param name="maskInfo">口罩检测的结果</param>
        /// <returns>返回0表示正常；返回其他值请在官网-帮助中心查询</returns>
        public int ASFGetMask(out MaskInfo maskInfo)
        {
            int retCode = -1;
            maskInfo = new MaskInfo();
            IntPtr pMaskInfo = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_MaskInfo>());
            //调用SDK接口
            retCode = ASFFunctions.ASFGetMask(pEngine, pMaskInfo);
            if (retCode != 0)
            {
                MemoryUtil.Free(pMaskInfo);
                return retCode;
            }
            //转化结果
            ASF_MaskInfo asfMaskInfo = MemoryUtil.PtrToStructure<ASF_MaskInfo>(pMaskInfo);
            maskInfo.num = asfMaskInfo.num;
            if (asfMaskInfo.num > 0)
            {
                maskInfo.maskArray = new int[asfMaskInfo.num];
                Marshal.Copy(asfMaskInfo.maskArray, maskInfo.maskArray, 0, asfMaskInfo.num);
            }
            MemoryUtil.FreeArray(pMaskInfo);
            return retCode;
        }

        //******************************** 额头区域检测相关 **********************************************
        /// <summary>
        ///  获取额头区域检测结果（当前只支持0, 90, 180, 270度角检测）
        /// </summary>
        /// <param name="faceLandMark">额头区域检测结果</param>
        /// <returns>返回0表示正常；返回其他值请在官网-帮助中心查询</returns>
        public int ASFGetFaceLandMark(out LandMarkInfo faceLandMark)
        {
            int retCode = -1;
            faceLandMark = new LandMarkInfo();
            IntPtr pFaceLandMark = MemoryUtil.Malloc(MemoryUtil.SizeOf<ASF_LandMarkInfo>());
            //调用SDK接口
            retCode = ASFFunctions.ASFGetFaceLandMark(pEngine, pFaceLandMark);
            if (retCode != 0)
            {
                MemoryUtil.Free(pFaceLandMark);
                return retCode;
            }
            //转化结果
            ASF_LandMarkInfo asfFaceLandMark = MemoryUtil.PtrToStructure<ASF_LandMarkInfo>(pFaceLandMark);
            faceLandMark.num = asfFaceLandMark.num;
            if (asfFaceLandMark.num > 0)
            {
                int size = MemoryUtil.SizeOf<ASF_FaceLandmark>();
                int markLength = 4;

                faceLandMark.pointAyy = new ASF_FaceLandmark[asfFaceLandMark.num][];
                byte[] data = MemoryUtil.PtrToBytes(asfFaceLandMark.point, asfFaceLandMark.num * markLength * size);
                for (int i = 0; i < asfFaceLandMark.num; i++)
                {
                    faceLandMark.pointAyy[i] = new ASF_FaceLandmark[4];
                    for (int j=0;j< markLength; j++)
                    {
                        byte[] subData = new byte[size];
                        for(int k = 0; k < size; k++)
                        {
                            subData[k] = data[i * (markLength * size) + j * size + k];
                        }
                        faceLandMark.pointAyy[i][j] = MemoryUtil.BytesToStuct<ASF_FaceLandmark>(subData);
                    }                    
                }
            }
            MemoryUtil.FreeArray(pFaceLandMark);
            return retCode;
        }

        /// <summary>
        /// 销毁引擎
        /// </summary>
        public void ASFUninitEngine()
        {
            try
            {
                if (!pEngine.Equals(IntPtr.Zero))
                {
                    ASFFunctions.ASFUninitEngine(pEngine);
                }
            }
            catch { }
        }

        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <param name="version">版本信息</param>
        public void ASFGetVersion(out SDKVersion version)
        {
            version = new SDKVersion();
            ASF_VERSION asfVersion = ASFFunctions.ASFGetVersion();
            version.version = Marshal.PtrToStringAnsi(asfVersion.Version);
            version.buildDate = Marshal.PtrToStringAnsi(asfVersion.BuildDate);
            version.copyRight = Marshal.PtrToStringAnsi(asfVersion.CopyRight);
        }

        /// <summary>
        /// 析构函数-注销引擎
        /// </summary>
        ~FaceEngine()
        {
            ASFUninitEngine();
        }
    }
}