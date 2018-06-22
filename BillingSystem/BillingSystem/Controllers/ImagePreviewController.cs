using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using BillingSystem.Common;
using BillingSystem.Common.Common;

namespace BillingSystem.Controllers
{
    public class ImagePreviewController : BaseController
    {
        /// <summary>
        /// Get the content of file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AjaxSubmit(int? id)
        {
            var userId = Helpers.GetLoggedInUserId();
            if (Request.Files != null && Request.Files.Count > 0 && userId > 0)
            {
                //var pid = Request[SessionEnum.PatientId.ToString()];
                var uploadedFile = Request.Files[0];
                if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
                {
                    HttpPostedFileBase uploadFile = Request.Files[0];
                    var fi = new FileInfo(uploadFile.FileName);
                    HttpContextSessionWrapperExtension.ContentStream = null;

                    Session[SessionEnum.TempProfileFile.ToString()] = uploadedFile;

                    HttpContextSessionWrapperExtension.ContentLength = uploadedFile.ContentLength;
                    HttpContextSessionWrapperExtension.ContentType = uploadedFile.ContentType;
                    byte[] b = new byte[uploadedFile.ContentLength];
                    uploadedFile.InputStream.Read(b, 0, uploadedFile.ContentLength);
                    Stream stream = new MemoryStream(b);
                    var orginalImage = new Bitmap(stream);
                    var orginalWidth = orginalImage.Width;

                    if (orginalWidth > 650)
                    {
                        var bitmap0 = (Bitmap)Helpers.CreateThumbnailModify(stream, 650, 0);
                        var fileInBytesNew = Helpers.ImageToByte(bitmap0);
                        HttpContextSessionWrapperExtension.ContentStream = fileInBytesNew;
                    }
                    else
                        HttpContextSessionWrapperExtension.ContentStream = b;

                    Image s = Image.FromStream(uploadedFile.InputStream);
                    Session[SessionEnum.ProfileImage.ToString()] = s;

                    if (Request["currentProfileImageSource"] != null)
                    {
                        var oldImage = Server.MapPath("~" + Request["currentProfileImageSource"]);
                        Session[SessionEnum.OldDoc.ToString()] = oldImage;
                    }

                    return Content(uploadedFile.ContentType + ";" + uploadedFile.ContentLength);
                }
            }
            return Content("");
        }

        /// <summary>
        /// Load image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ImageLoad(int? id)
        {
            var b = (byte[])HttpContextSessionWrapperExtension.ContentStream;
            var length = (int)HttpContextSessionWrapperExtension.ContentLength;
            var type = (string)HttpContextSessionWrapperExtension.ContentType;
            HttpContextSessionWrapperExtension.CroppedContentType = type;
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = type;
            Response.BinaryWrite(b);
            Response.Flush();
            Response.End();
            return Content("");
        }

        #region CroppedImageLoad
        /// <summary>
        /// CroppedImageLoad
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public ActionResult CroppedImageLoad(int x, int y, int w, int h)
        {
            var type = (string)HttpContextSessionWrapperExtension.CroppedContentType;
            Stream stream = new MemoryStream((byte[])HttpContextSessionWrapperExtension.ContentStream);
            var a = new Bitmap(stream);
            var img1 = CropBitmap(a, x, y, w, h);
            byte[] b1 = GetBytesOfImage(img1);
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = type;
            Response.BinaryWrite(b1);
            Response.Flush();
            Response.End();
            HttpContextSessionWrapperExtension.ContentLength = null;
            HttpContextSessionWrapperExtension.ContentType = null;
            HttpContextSessionWrapperExtension.CroppedContentType = null;
            HttpContextSessionWrapperExtension.ContentStream = b1;
            return Content("");
        }
        /// <summary>
        /// Crop an image and return as bitmap
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="cropX"></param>
        /// <param name="cropY"></param>
        /// <param name="cropWidth"></param>
        /// <param name="cropHeight"></param>
        /// <returns></returns>
        public Bitmap CropBitmap(Bitmap bitmap, int cropX, int cropY, int cropWidth, int cropHeight)
        {
            var rect = new Rectangle(cropX, cropY, cropWidth, cropHeight);
            var cropped = bitmap.Clone(rect, bitmap.PixelFormat);

            return cropped;
        }
        /// <summary>
        /// conevrt an image to byte []
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static byte[] GetBytesOfImage(Image img)
        {
            var converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
        #endregion CroppedImageLoad
        /// <summary>
        /// validate file size
        /// </summary>
        /// <returns></returns>
        public string CheckFileSize()
        {
            var returnValue = "";
            if (Session[SessionEnum.ProfileImage.ToString()] != null)
            {
                var tempImage = (Image)Session[SessionEnum.ProfileImage.ToString()];
                returnValue = tempImage.Width + "`" + tempImage.Height;
            }
            return returnValue;
        }


        /// <summary>
        /// Remove image bytes session value
        /// </summary>
        /// <returns></returns>
        //public bool RemoveImageByteSession()
        //{
        //    bool returnValue = false;
        //    if (Session[SessionEnum.TempProfileFile.ToString()] != null)
        //    {
        //        Session[SessionEnum.TempProfileFile.ToString()] = null;
        //        returnValue = true;
        //    }
        //    return returnValue;
        //}

        private Image SaveImage(Stream prop, string ext)
        {
            Image img1 = Image.FromStream(prop);
            return img1;
        }




    }
}