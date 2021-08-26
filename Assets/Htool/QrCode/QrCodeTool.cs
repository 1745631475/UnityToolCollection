using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace Htool
{
    /// <summary>
    /// 二维码工具
    /// </summary>
    public sealed class QrCodeTool : Singleton<QrCodeTool>
    {
        #region 生成二维码（返回Texture2D）
        /// <summary>
        /// 绘制指定信息的二维码图片到Texture2D（只能生成256*256）
        /// </summary>
        /// <param name="content">要绘制的二维码字符串信息</param>
        /// <returns>返回绘制好的Texture2D</returns>
        public Texture2D DrawQRCode(string content)
        {
            Texture2D Temp = new Texture2D(256, 256);
            Temp.SetPixels32(GeneQRCodeColor32(content));
            Temp.Apply();
            return Temp;
        }

        /// <summary>
        /// 绘制指定信息的二维码图片到Texture2D（可以生成任意尺寸的正方形）
        /// </summary>
        /// <param name="content">要绘制的二维码字符串信息</param>
        /// <param name="width">二维码的宽度</param>
        /// <param name="height">二维码的高度</param>
        /// <param name="Show">二维码的前景色</param>
        /// <param name="hide">二维码的背景色</param>
        /// <returns>返回绘制好的Texture2D</returns>
        public Texture2D DrawQRCode(string content, int width, int height, Color? Show = null, Color? hide = null)
        {
            Show = Show ?? Color.black;
            hide = hide ?? Color.white;
            BitMatrix bitMatrix = GeneQRCodeBitMatrix(content, width, height);
            Texture2D Temp = new Texture2D(width, height);
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    if (bitMatrix[x, y])
                    {
                        Temp.SetPixel(y, x, Show.Value);
                    }
                    else
                    {
                        Temp.SetPixel(y, x, hide.Value);
                    }
                }
            }
            Temp.Apply();
            return Temp;
        }

        /// <summary>
        /// 绘制指定信息的二维码图片到Texture2D 并添加小图（可以生成任意尺寸的正方形）
        /// </summary>
        /// <param name="content">要绘制的二维码字符串信息</param>
        /// <param name="width">二维码的宽度</param>
        /// <param name="height">二维码的高度</param>
        /// <param name="centerIcon">小图Icon</param>
        /// <param name="reduceMultiple">小图占大图的比例 0-1 </param>
        /// <param name="Show">二维码的前景色</param>
        /// <param name="hide">二维码的背景色</param>
        /// <returns></returns>
        public Texture2D DrawQRCode(string content, int width, int height, Texture2D centerIcon, float reduceMultiple = 0.2f, Color? Show = null, Color? hide = null)
        {
            Show = Show ?? Color.black;
            hide = hide ?? Color.white;
            Texture2D Temp = DrawQRCode(content, width, height, Show, hide);
            if (centerIcon.width != centerIcon.height) return Temp;
            int centerIconWH = (int)(width * reduceMultiple);
            centerIcon = ScaleTexture(centerIcon, centerIconWH, centerIconWH);
            #region 添加小图
            int halfWidth = Temp.width / 2;
            int halfHeight = Temp.height / 2;
            int halfWidthOfIcon = centerIcon.width / 2;
            int halfHeightOfIcon = centerIcon.height / 2;
            int centerOffsetX = 0;
            int centerOffsetY = 0;
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    centerOffsetX = x - halfWidth;
                    centerOffsetY = y - halfHeight;
                    if (Mathf.Abs(centerOffsetX) <= halfWidthOfIcon && Mathf.Abs(centerOffsetY) <= halfHeightOfIcon)
                    {
                        Temp.SetPixel(x, y, centerIcon.GetPixel(centerOffsetX + halfWidthOfIcon, centerOffsetY + halfHeightOfIcon));
                    }
                }
            }
            Temp.Apply();
            return Temp;
            #endregion
        }
        #endregion

        #region 保存二维码到本地
        /// <summary>
        /// 保存二维码到本地
        /// </summary>
        /// <param name="QRCode">要保存的二维码</param>
        /// <param name="Path">保存路径</param>
        /// <param name="Name">保存名称</param>
        public void SaveQRCode(Texture2D QRCode, string Path, string Name)
        {
            byte[] bytes = null;
            string[] End = Name.Split('.');
            switch (End[End.Length - 1].ToUpper())
            {
                case "JPG":
                    bytes = QRCode.EncodeToJPG();
                    break;
                case "PNG":
                    bytes = QRCode.EncodeToPNG();
                    break;
                default:
                    Name = null;
                    for (int i = 0; i < End.Length; i++)
                    {
                        if (i == End.Length - 1)
                        {
                            Name += ".PNG";
                        }
                        else
                        {
                            Name += End[i];
                        }
                    }
                    bytes = QRCode.EncodeToPNG();
                    break;
            }
            string path = System.IO.Path.Combine(Path, Name);
            System.IO.File.WriteAllBytes(path, bytes);
        }
        #endregion

        #region 私有辅助方法
        /// <summary>
        /// 将指定字符串信息转换成二维码图片信息（只能生成256*256）
        /// </summary>
        /// <param name="content">要转换的字符串信息</param>
        /// <param name="width">二维码的宽度</param>
        /// <param name="height">二维码的高度</param>
        /// <returns>返回二维码图片的颜色数组信息</returns>
        private Color32[] GeneQRCodeColor32(string content)
        {
            #region 在绘制二维码之前进行一些设置 使用QrCodeEncodingOptions类设置
            QrCodeEncodingOptions options = new QrCodeEncodingOptions();
            //设置错误矫正级别
            options.ErrorCorrection = ErrorCorrectionLevel.H;
            //设置字符串转换格式，保证字符串信息正确
            options.CharacterSet = "UTF-8";
            //如果设置了UTF-8，此项应设置为True
            options.DisableECI = true;
            //设置绘制区域的宽度和高度（单位像素）
            options.Width = 256;
            options.Height = 256;
            //设置二维码边缘留白宽度（值越大，六百宽度大，二维码就减小）
            options.Margin = 1;

            #endregion

            #region 绘制二维码 使用BarcodeWriter类生成
            BarcodeWriter barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = options
            };
            return barcodeWriter.Write(content);
            #endregion
        }

        /// <summary>
        /// 将指定字符串信息转换成二维位矩阵
        /// </summary>
        /// <param name="content">要转换的字符串信息</param>
        /// <param name="width">二维码的宽度</param>
        /// <param name="height">二维码的高度</param>
        /// <returns></returns>
        private BitMatrix GeneQRCodeBitMatrix(string content, int width, int height)
        {
            //使用MultiFormatWriter生成，生成前增加一些设置
            MultiFormatWriter multiFormatWriter = new MultiFormatWriter();
            System.Collections.Generic.Dictionary<EncodeHintType, object> hints = new System.Collections.Generic.Dictionary<EncodeHintType, object>();
            hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            hints.Add(EncodeHintType.MARGIN, 1);
            hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
            return multiFormatWriter.encode(content, BarcodeFormat.QR_CODE, width, height, hints);
        }

        /// <summary>
        /// 纹理缩放
        /// </summary>
        /// <param name="source">要缩放的纹理</param>
        /// <param name="newWidth">指定宽</param>
        /// <param name="newHeight">指定高</param>
        /// <returns></returns>
        private Texture2D ScaleTexture(Texture2D source, int newWidth, int newHeight)
        {
            source.filterMode = FilterMode.Point;
            RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
            rt.filterMode = FilterMode.Point;
            RenderTexture.active = rt;
            Graphics.Blit(source, rt);
            var nTex = new Texture2D(newWidth, newHeight);
            nTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
            nTex.Apply();
            RenderTexture.active = null;
            return nTex;
        }
        #endregion
    }
}