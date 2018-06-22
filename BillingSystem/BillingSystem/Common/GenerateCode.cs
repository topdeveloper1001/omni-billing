using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using BillingSystem.Model.CustomModel;
using GenCode128;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;


namespace BillingSystem.Common
{
    public class GenerateCode
    {
        /// <summary>
        /// Generate Bar code 
        /// </summary>
        /// <param name="items"> item detail</param>
        /// <param name="path">path to save code image</param>
        public static void GenerateBarCode(GenerateBarCode items, string path)
        {
            //path = path.Replace("/", "_");
            var myimg = Code128Rendering.MakeBarcodeImage(items.BarCodeReadValue, 2, true);
            myimg.Save(path);

        }


        public static void GenerateBarCode(string barCodeValue, string path)
        {
            //path = path.Replace("/", "_");
            var myimg = Code128Rendering.MakeBarcodeImage(barCodeValue, 2, true);
            myimg.Save(path);
        }
    }
}
