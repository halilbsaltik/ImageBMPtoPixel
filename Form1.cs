using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BMPTOPixel
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        public Color[][] getBitmapColorMatrix(string filePath)
        {
            Bitmap bmp = new Bitmap(filePath);
            Color[][] matrix;
            int height = bmp.Height;
            int width = bmp.Width;
            if (height > width)
            {
                matrix = new Color[bmp.Width][];
                for (int i = 0; i <= bmp.Width - 1; i++)
                {
                    matrix[i] = new Color[bmp.Height];
                    for (int j = 0; j < bmp.Height - 1; j++)
                    {
                        matrix[i][j] = bmp.GetPixel(i, j);
                    }
                }
            }
            else
            {
                matrix = new Color[bmp.Height][];
                for (int i = 0; i <= bmp.Height - 1; i++)
                {
                    matrix[i] = new Color[bmp.Width];
                    for (int j = 0; j < bmp.Width - 1; j++)
                    {
                        matrix[i][j] = bmp.GetPixel(i, j);
                    }
                }
            }
            return matrix;
        }

        public Color[][] GetBitMapColorMatrix(string bitmapFilePath)
        {
            Bitmap b1 = new Bitmap(bitmapFilePath);

            int hight = b1.Height;
            int width = b1.Width;

            Color[][] colorMatrix = new Color[width][];
            for (int i = 0; i < width; i++)
            {
                colorMatrix[i] = new Color[hight];
                for (int j = 0; j < hight; j++)
                {
                    colorMatrix[i][j] = b1.GetPixel(i, j);
                }
            }
            return colorMatrix;
        }

        public static Bitmap ArrayToImage(byte[,,] pixelArray)
        {
            int width = pixelArray.GetLength(1);
            int height = pixelArray.GetLength(0);
            int stride = (width % 4 == 0) ? width : width + 4 - width % 4;
            int bytesPerPixel = 3;

            byte[] bytes = new byte[stride * height * bytesPerPixel];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int offset = (y * stride + x) * bytesPerPixel;
                    bytes[offset + 0] = pixelArray[y, x, 2]; // blue
                    bytes[offset + 1] = pixelArray[y, x, 1]; // green
                    bytes[offset + 2] = pixelArray[y, x, 0]; // red
                }
            }

            PixelFormat formatOutput = PixelFormat.Format24bppRgb;
            Rectangle rect = new Rectangle(0, 0, width, height);
            Bitmap bmp = new Bitmap(stride, height, formatOutput);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, formatOutput);
            Marshal.Copy(bytes, 0, bmpData.Scan0, bytes.Length);
            bmp.UnlockBits(bmpData);

            Bitmap bmp2 = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            Graphics gfx2 = Graphics.FromImage(bmp2);
            gfx2.DrawImage(bmp, 0, 0);

            return bmp2;
        }


        public byte[,,] ImageToArray(Bitmap bmp)
        {
            int bytesPerPixel = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            int byteCount = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] bytes = new byte[byteCount];
            Marshal.Copy(bmpData.Scan0, bytes, 0, byteCount);
            bmp.UnlockBits(bmpData);

            byte[,,] pixelValues = new byte[bmp.Height, bmp.Width, 3];
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    int offset = (y * bmpData.Stride) + x * bytesPerPixel;
                    pixelValues[y, x, 0] = bytes[offset + 2]; // red
                    pixelValues[y, x, 1] = bytes[offset + 1]; // green
                    pixelValues[y, x, 2] = bytes[offset + 0]; // blue
                }
            }

            return pixelValues;
        }





        private void Form1_Load(object sender, EventArgs e)
        {
            string path = @"C:\Users\AIBlack\Pictures\1.png";
            //GetBitMapColorMatrix(path);

            Bitmap bm = new Bitmap(path);

            Bitmap vv =  ArrayToImage(ImageToArray(bm));

            Clipboard.SetImage(vv);

            using (var src = new Bitmap(path))
            using (var bmp = new Bitmap(100, 100, PixelFormat.Format32bppPArgb))
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.Clear(Color.Blue);
                gr.DrawImage(src, new Rectangle(0, 0, bmp.Width, bmp.Height));
                Clipboard
                bmp.Save("c:/temp/result.png", ImageFormat.Png);
            }


        }
    }
}
