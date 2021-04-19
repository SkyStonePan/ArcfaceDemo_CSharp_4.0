﻿using ArcFaceSDK.Entity;
using ArcFaceSDK.SDKModels;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ArcFaceSDK.Utils
{
    /// <summary>
    /// 图像处理方法
    /// </summary>
    public static class ImageUtil
    {
        /// <summary>
        /// 获取RGB图片信息
        /// </summary>
        /// <param name="image">图片</param>
        /// <returns>图片数据</returns>
        public static ImageInfo ReadBMP(Image image)
        {
            ImageInfo imageInfo = new ImageInfo();

            //将Image转换为Format24bppRgb格式的BMP
            Bitmap bm = new Bitmap(image);
            BitmapData data = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                //位图中第一个像素数据的地址。它也可以看成是位图中的第一个扫描行
                IntPtr ptr = data.Scan0;

                //定义数组长度
                int soureBitArrayLength = data.Height * Math.Abs(data.Stride);
                byte[] sourceBitArray = new byte[soureBitArrayLength];

                //将bitmap中的内容拷贝到ptr_bgr数组中
                MemoryUtil.Copy(ptr, sourceBitArray, 0, soureBitArrayLength);

                //填充引用对象字段值
                imageInfo.width = data.Width;
                imageInfo.height = data.Height;
                imageInfo.format = (int)ASF_ImagePixelFormat.ASVL_PAF_RGB24_B8G8R8;
                //步长的设置
                imageInfo.widthStep = data.Stride;
                                
                imageInfo.imgData = MemoryUtil.Malloc(sourceBitArray.Length);
                MemoryUtil.Copy(sourceBitArray, 0, imageInfo.imgData, sourceBitArray.Length);
                return imageInfo;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            finally
            {
                bm.UnlockBits(data);
                bm.Dispose();
            }
        }

        /// <summary>
        /// 获取IR图片信息
        /// </summary>
        /// <param name="image">图片</param>
        /// <returns>图片数据</returns>
        public static ImageInfo ReadBMP_IR(Image image)
        {
            ImageInfo imageInfo = new ImageInfo();

            //将Image转换为Format24bppRgb格式的BMP
            Bitmap bm = new Bitmap(image);
            BitmapData data = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                //位图中第一个像素数据的地址。它也可以看成是位图中的第一个扫描行
                IntPtr ptr = data.Scan0;

                //定义数组长度
                int soureBitArrayLength = data.Height * Math.Abs(data.Stride);
                byte[] sourceBitArray = new byte[soureBitArrayLength];

                //将bitmap中的内容拷贝到ptr_bgr数组中
                MemoryUtil.Copy(ptr, sourceBitArray, 0, soureBitArrayLength);

                //填充引用对象字段值
                imageInfo.width = data.Width;
                imageInfo.height = data.Height;
                imageInfo.format = (int)ASF_ImagePixelFormat.ASVL_PAF_GRAY;
                //步长的设置
                imageInfo.widthStep = data.Width;

                //获取去除对齐位后度图像数据
                int line = imageInfo.width;
                int ir_len = line * imageInfo.height;
                byte[] destBitArray = new byte[ir_len];

                //灰度化
                int j = 0;
                double colortemp = 0;
                for (int i = 0; i < sourceBitArray.Length; i += 3)
                {
                    colortemp = sourceBitArray[i + 2] * 0.299 + sourceBitArray[i + 1] * 0.587 + sourceBitArray[i] * 0.114;
                    destBitArray[j++] = (byte)colortemp;
                }
                
                imageInfo.imgData = MemoryUtil.Malloc(destBitArray.Length);
                MemoryUtil.Copy(destBitArray, 0, imageInfo.imgData, destBitArray.Length);

                return imageInfo;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            finally
            {
                bm.UnlockBits(data);
                bm.Dispose();
            }
        }
        
        /// <summary>
        /// 用矩形框标记图片指定区域
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="startX">矩形框左上角X坐标</param>
        /// <param name="startY">矩形框左上角Y坐标</param>
        /// <param name="width">矩形框宽度</param>
        /// <param name="height">矩形框高度</param>
        /// <returns>标记后的图片</returns>
        public static Image MarkRect(Image image, int startX, int startY, int width, int height)
        {
            Image clone = (Image)image.Clone();
            Graphics g = Graphics.FromImage(clone);
            try
            {
                Brush brush = new SolidBrush(Color.Red);
                Pen pen = new Pen(brush, 2);
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                g.DrawRectangle(pen, new Rectangle(startX, startY, width, height));
                return clone;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            finally
            {
                g.Dispose();
            }
        }

        /// <summary>
        /// 用矩形框标记图片指定区域，添加年龄和性别标注
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="ageAyy">年龄数组</param>
        /// <param name="genderAyy">性别数组</param>
        /// <param name="maskAyy">口罩数组</param>
        /// <param name="scaleRate">缩放比例</param>
        /// <param name="faceSize">人脸数量</param>
        /// <returns>标记后的图片</returns>
        public static Image MarkRectAndString(Image image, MRECT[] mrectAyy, int[] ageAyy, int[] genderAyy, bool[] maskAyy, int showWidth, float scaleRate,int faceSize)
        {
            Image clone = (Image)image.Clone();
            Graphics g = Graphics.FromImage(clone);
            try
            {
                Brush brush = new SolidBrush(Color.Red);
                int penWidth = image.Width / showWidth;
                Pen pen = new Pen(brush, penWidth > 1 ? 2 * penWidth : 2);
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                for(int faceIndex = 0;faceIndex< faceSize; faceIndex++)
                {
                    int startX = (int)(mrectAyy[faceIndex].left * scaleRate);
                    int startY = (int)(mrectAyy[faceIndex].top * scaleRate);
                    int width = (int)(mrectAyy[faceIndex].right * scaleRate) - (int)(mrectAyy[faceIndex].left * scaleRate);
                    int height = (int)(mrectAyy[faceIndex].bottom * scaleRate) - (int)(mrectAyy[faceIndex].top * scaleRate);

                    g.DrawRectangle(pen, new Rectangle(startX < 1 ? 0 : startX, startY < 1 ? 0 : startY, width, height));
                    string genderStr = string.Empty;
                    switch (genderAyy[faceIndex])
                    {
                        case 0:
                            genderStr = "Male"; break;
                        case 1:
                            genderStr = "Female";break;
                        default:break;
                    }
                    g.DrawString(string.Format("Face:{0}, {1}, {2}", faceIndex, genderStr, maskAyy[faceIndex]?"Mask":"No mask"), new Font(FontFamily.GenericSerif, 12), brush, startX < 1 ? 0 : startX, (startY - 20) < 1 ? 0 : startY - 20);
                }
                return clone;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            finally
            {
                g.Dispose();
            }
        }

        /// <summary>
        /// 按指定宽高缩放图片
        /// </summary>
        /// <param name="image">原图片</param>
        /// <param name="dstWidth">目标图片宽</param>
        /// <param name="dstHeight">目标图片高</param>
        /// <returns></returns>
        public static Image ScaleImage(Image image, int dstWidth, int dstHeight)
        {
            Graphics g = null;
            try
            {
                //按比例缩放           
                float scaleRate = GetWidthAndHeight(image.Width, image.Height, dstWidth, dstHeight);
                int width = (int)(image.Width * scaleRate);
                int height = (int)(image.Height * scaleRate);

                //将宽度调整为4的整数倍
                if (width % 4 != 0)
                {
                    width -= width % 4;
                }

                Bitmap destBitmap = new Bitmap(width, height);
                g = Graphics.FromImage(destBitmap);
                g.Clear(Color.Transparent);

                //设置画布的描绘质量         
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, new Rectangle(0, 0, width, height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);

                return destBitmap;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            finally
            {
                if (g != null)
                {
                    g.Dispose();
                }
            }
        }

        /// <summary>
        /// 剪裁图片
        /// </summary>
        /// <param name="src">原图片</param>
        /// <param name="left">左坐标</param>
        /// <param name="top">顶部坐标</param>
        /// <param name="right">右坐标</param>
        /// <param name="bottom">底部坐标</param>
        /// <returns>剪裁后的图片</returns>
        public static Image CutImage(Image src, int left, int top, int right, int bottom)
        {
            Bitmap srcBitmap = new Bitmap(src);
            int width = right - left;
            int height = bottom - top;
            Bitmap destBitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(destBitmap))
            {
                g.Clear(Color.Transparent);

                //设置画布的描绘质量         
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(srcBitmap, new Rectangle(0, 0, width, height), left, top, width, height, GraphicsUnit.Pixel);
            }

            return destBitmap;
        }

        /// <summary>
        /// 根据图片路径读取图片
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        public static Image ReadFromFile(string imageUrl)
        {
            Image img = null;
            FileStream fs = null;
            try
            {
                fs = new FileStream(imageUrl, FileMode.Open, FileAccess.Read);
                img = Image.FromStream(fs);
            }
            finally
            {
                fs.Close();
            }
            return img;
        }
                
        /// <summary>
        /// 获取图片缩放比例
        /// </summary>
        /// <param name="oldWidth">原图片宽</param>
        /// <param name="oldHeigt">原图片高</param>
        /// <param name="newWidth">目标图片宽</param>
        /// <param name="newHeight">目标图片高</param>
        /// <returns></returns>
        public static float GetWidthAndHeight(int oldWidth, int oldHeigt, int newWidth, int newHeight)
        {
            //按比例缩放           
            float scaleRate;
            if (oldWidth >= newWidth && oldHeigt >= newHeight)
            {
                int widthDis = oldWidth - newWidth;
                int heightDis = oldHeigt - newHeight;
                if (widthDis > heightDis)
                {
                    scaleRate = newWidth * 1f / oldWidth;
                }
                else
                {
                    scaleRate = newHeight * 1f / oldHeigt;
                }
            }
            else if (oldWidth >= newWidth)
            {
                scaleRate = newWidth * 1f / oldWidth;
            }
            else if (oldHeigt >= newHeight)
            {
                scaleRate = newHeight * 1f / oldHeigt;
            }
            else
            {
                int widthDis = newWidth - oldWidth;
                int heightDis = newHeight - oldHeigt;
                if (widthDis > heightDis)
                {
                    scaleRate = newHeight * 1f / oldHeigt;
                }
                else
                {
                    scaleRate = newWidth * 1f / oldWidth;
                }
            }
            return scaleRate;
        }
        
    }
}
