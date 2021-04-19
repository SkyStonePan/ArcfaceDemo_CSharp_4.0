﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ArcFaceSDK.Utils
{
    /// <summary>
    /// 内存操作方法
    /// </summary>
    public static class MemoryUtil
    {
        /// <summary>
        /// 申请内存
        /// </summary>
        /// <param name="len">内存长度(单位:字节)</param>
        /// <returns>内存首地址</returns>
        public static IntPtr Malloc(int len)
        {
            return Marshal.AllocHGlobal(len);
        }

        /// <summary>
        /// 释放ptr托管的内存
        /// </summary>
        /// <param name="ptr">托管指针</param>
        public static void Free(IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
        }

        /// <summary>
        /// 释放多个ptr托管的内存
        /// </summary>
        /// <param name="ptr">托管指针</param>
        public static void FreeArray(params IntPtr[] ptrList)
        {
            if (ptrList != null && ptrList.Length > 0)
            {
                foreach (IntPtr subPtr in ptrList)
                {
                    if (!subPtr.Equals(IntPtr.Zero))
                    {
                        Marshal.FreeHGlobal(subPtr);
                    }
                }
            }
        }

        /// <summary>
        /// 释放多个ptr托管的内存
        /// </summary>
        /// <param name="ptr">托管指针</param>
        public static void FreeList(List<IntPtr> ptrList)
        {
            if (ptrList != null && ptrList.Count > 0)
            {
                foreach (IntPtr subPtr in ptrList)
                {
                    if (!subPtr.Equals(IntPtr.Zero))
                    {
                        Marshal.FreeHGlobal(subPtr);
                    }
                }
            }
        }

        /// <summary>
        /// 将字节数组的内容拷贝到托管内存中
        /// </summary>
        /// <param name="source">元数据</param>
        /// <param name="startIndex">元数据拷贝起始位置</param>
        /// <param name="destination">托管内存</param>
        /// <param name="length">拷贝长度</param>
        public static void Copy(byte[] source, int startIndex, IntPtr destination, int length)
        {
            Marshal.Copy(source, startIndex, destination, length);
        }

        /// <summary>
        /// 将托管内存的内容拷贝到字节数组中
        /// </summary>
        /// <param name="source">托管内存</param>
        /// <param name="destination">目标字节数组</param>
        /// <param name="startIndex">拷贝起始位置</param>
        /// <param name="length">拷贝长度</param>
        public static void Copy(IntPtr source, byte[] destination, int startIndex, int length)
        {
            Marshal.Copy(source, destination, startIndex, length);
        }

        /// <summary>
        /// 将ptr托管的内存转化为结构体对象
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="ptr">托管指针</param>
        /// <returns>转化后的对象</returns>
        public static T PtrToStructure<T>(IntPtr ptr)
        {
            return Marshal.PtrToStructure<T>(ptr);
        }
        
        /// <summary>
        /// 将结构体对象复制到ptr托管的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="ptr"></param>
        public static void StructureToPtr<T>(T t, IntPtr ptr)
        {
            if(t == null)
            {
                return;
            }
            Marshal.StructureToPtr(t, ptr, false);
        }

        /// <summary>
        /// 获取类型的大小
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <returns>类型的大小</returns>
        public static int SizeOf<T>()
        {
            return Marshal.SizeOf<T>();
        }

        /// <summary>
        /// 从struct转换到byte[] 
        /// </summary>
        /// <param name="structObj"></param>
        /// <returns></returns>
        public static byte[] StructToBytes(object structObj)
        {
            //返回类的非托管大小（以字节为单位）  
            int size = Marshal.SizeOf(structObj);
            //分配大小  
            byte[] bytes = new byte[size];
            //从进程的非托管堆中分配内存给structPtr  
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将数据从托管对象structObj封送到非托管内存块structPtr  
            Marshal.StructureToPtr(structObj, structPtr, false);
            //将数据从非托管内存指针复制到托管 8 位无符号整数数组  
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放以前使用 AllocHGlobal 从进程的非托管内存中分配的内存  
            Marshal.FreeHGlobal(structPtr);
            return bytes;
        }

        /// <summary>
        /// byte数组转化为结构体
        /// </summary>
        /// <param name="bytes">数组</param>
        /// <param name="type">结构体类型</param>
        /// <returns></returns>
        public static T BytesToStuct<T>(byte[] bytes)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(typeof(T));
            //byte数组长度小于结构体的大小
            if (size > bytes.Length)
            {
                //返回空
                return default(T);
            }
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构体
            T obj = (T)Marshal.PtrToStructure(structPtr, typeof(T));
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构体
            return obj;
        }

        /// <summary>
        /// 从Ptr转换到byte[] 
        /// </summary>
        /// <param name="ptr">Ptr</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static byte[] PtrToBytes(IntPtr ptr,int length)
        {
            byte[] bytes = new byte[length];
            Marshal.Copy(ptr, bytes, 0, length);
            return bytes;
        }
    }
}
