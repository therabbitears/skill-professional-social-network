using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;
using System.Net;
using System.Web;
using Amazon.S3.Model;
using Amazon.S3;
using System.Text.RegularExpressions;

namespace Wrly.Storage
{
    public static class ImageProcessor
    {
        static HttpServerUtility Server { get { return HttpContext.Current.Server; } }
        static bool UseLocalStorage { get; set; }
        static string bucketName = "sklative-main";
        public static ImageAttribute UploadImage
            (
                    byte[] content,
                    Enums.ImageObject imageObject,
                    string fileName,
                    bool useDefaultName,
                    Enums.FileType fileType,
                    string uniqueID,
                    Enums.StorageProvider provider,
                    string siteUrl
            )
        {
            // Add code is use to local storage
            if (provider == Enums.StorageProvider.Local)
            {
                string httpPath = "/cdn/imagestore/{0}/{1}";
                string filePath = Server.MapPath("~/imageStore");
                if (string.IsNullOrEmpty(fileName) && !useDefaultName)
                    throw new Exception("File name cannnot be left blank, please spcifiy either file name or specify is need to use default file name.");

                if (string.IsNullOrEmpty(fileName))
                {
                    if (fileType == Enums.FileType.Image)
                    {
                        fileName = string.Format("{0}.png", uniqueID);
                    }
                }

                httpPath = string.Format(httpPath, fileName, Path.GetExtension(fileName).Replace(".", string.Empty));
                filePath = string.Format("{0}/{1}", filePath, fileName);
                File.WriteAllBytes(filePath, content);
                httpPath = siteUrl+ httpPath + "/width_{0}";
                return new ImageAttribute() { FileName = httpPath };
            }
            else
            {
                string serviceUrl = "https://sklative-main.s3.amazonaws.com/";
                var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
                try
                {
                    var variations = GetAmazonVariations(content, imageObject, uniqueID);

                    foreach (var item in variations)
                    {
                        PutObjectRequest putRequest = new PutObjectRequest
                        {
                            BucketName = bucketName,
                            Key = item.KeyName,
                            InputStream = item.Content,
                            ContentType = "image",
                            // Add canned ACL.
                            CannedACL = S3CannedACL.PublicRead
                        };
                        PutObjectResponse response = client.PutObject(putRequest);
                    }
                    var keyName = variations.OrderByDescending(c => c.Size).FirstOrDefault().KeyName;
                    keyName = new Regex("width_[0-9]*$").Replace(keyName, "width_{0}");
                    fileName = string.Format("{0}{1}", serviceUrl, keyName);
                    return new ImageAttribute() { FileName = fileName };
                }
                catch (AmazonS3Exception amazonS3Exception)
                {
                    if (amazonS3Exception.ErrorCode != null &&
                        (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                        ||
                        amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                    {
                        throw new Exception("Check the provided AWS Credentials.");
                    }
                    else
                    {
                        throw new Exception("Error occurred: " + amazonS3Exception.Message);
                    }
                }
            }
        }

        private static List<Amazonvariation> GetAmazonVariations(byte[] content, Enums.ImageObject imageObject, string uniqueID)
        {
            string httpPath = "/cdn/imagestore/{0}/{1}";
            var fileName = uniqueID;
            fileName = string.Format("{0}.png", fileName);
            var variations = new List<Amazonvariation>();
            var boolSavedAsPng = false;
            if (imageObject == Enums.ImageObject.UserCoverImage)
            {
                httpPath = string.Format(httpPath, fileName, Path.GetExtension(fileName).Replace(".", string.Empty));
                var newContent = ResizeFile(content, 900, ref boolSavedAsPng);
                variations.Add(new Amazonvariation()
                {
                    Content = new MemoryStream(newContent),
                    KeyName = httpPath + "/width_900",
                    Size = 900
                });

                newContent = ResizeFile(content, 500, ref boolSavedAsPng);
                variations.Add(new Amazonvariation()
                {
                    Content = new MemoryStream(newContent),
                    KeyName = httpPath + "/width_500",
                    Size = 500
                });
            }
            if (imageObject == Enums.ImageObject.UserProfileImage)
            {
                httpPath = string.Format(httpPath, fileName, Path.GetExtension(fileName).Replace(".", string.Empty));

                var newContent = ResizeFile(content, 200, ref boolSavedAsPng);
                variations.Add(new Amazonvariation()
                {
                    Content = new MemoryStream(newContent),
                    KeyName = httpPath + "/width_200",
                    Size = 200
                });


                newContent = ResizeFile(content, 100, ref boolSavedAsPng);
                variations.Add(new Amazonvariation()
                {
                    Content = new MemoryStream(newContent),
                    KeyName = httpPath + "/width_100",
                    Size = 100
                });


                newContent = ResizeFile(content, 50, ref boolSavedAsPng);
                variations.Add(new Amazonvariation()
                {
                    Content = new MemoryStream(newContent),
                    KeyName = httpPath + "/width_50",
                    Size = 50
                });
            }

            if (imageObject == Enums.ImageObject.GroupImage)
            {
                httpPath = string.Format(httpPath, fileName, Path.GetExtension(fileName).Replace(".", string.Empty));

                var newContent = content;
                variations.Add(new Amazonvariation()
                {
                    Content = new MemoryStream(newContent),
                    KeyName = httpPath + "/width_200",
                    Size = 200
                });


                newContent = ResizeFile(content, 100, ref boolSavedAsPng);
                variations.Add(new Amazonvariation()
                {
                    Content = new MemoryStream(newContent),
                    KeyName = httpPath + "/width_100",
                    Size = 100
                });


                newContent = ResizeFile(content, 50, ref boolSavedAsPng);
                variations.Add(new Amazonvariation()
                {
                    Content = new MemoryStream(newContent),
                    KeyName = httpPath + "/width_50",
                    Size = 50
                });
            }
            if (imageObject == Enums.ImageObject.News)
            {
                httpPath = string.Format(httpPath, fileName, Path.GetExtension(fileName).Replace(".", string.Empty));

                var newContent = ResizeFile(content, 800, ref boolSavedAsPng);
                variations.Add(new Amazonvariation()
                {
                    Content = new MemoryStream(newContent),
                    KeyName = httpPath + "/width_800",
                    Size = 800
                });


                newContent = ResizeFile(content, 500, ref boolSavedAsPng);
                variations.Add(new Amazonvariation()
                {
                    Content = new MemoryStream(newContent),
                    KeyName = httpPath + "/width_500",
                    Size = 500
                });
            }
            if (imageObject == Enums.ImageObject.BusinessCoverImage)
            {
                httpPath = string.Format(httpPath, fileName, Path.GetExtension(fileName).Replace(".", string.Empty));
                var newContent = ResizeFile(content, 900, ref boolSavedAsPng);
                variations.Add(new Amazonvariation()
                {
                    Content = new MemoryStream(newContent),
                    KeyName = httpPath + "/width_900",
                    Size = Int16.MaxValue
                });

            }
            if (imageObject == Enums.ImageObject.BusinessProfileImage)
            {
                httpPath = string.Format(httpPath, fileName, Path.GetExtension(fileName).Replace(".", string.Empty));

                var newContent = ResizeFile(content, 200, ref boolSavedAsPng);
                variations.Add(new Amazonvariation()
                {
                    Content = new MemoryStream(newContent),
                    KeyName = httpPath + "/width_200",
                    Size = 200
                });


                newContent = ResizeFile(content, 100, ref boolSavedAsPng);
                variations.Add(new Amazonvariation()
                {
                    Content = new MemoryStream(newContent),
                    KeyName = httpPath + "/width_100",
                    Size = 100
                });


                newContent = ResizeFile(content, 50, ref boolSavedAsPng);
                variations.Add(new Amazonvariation()
                {
                    Content = new MemoryStream(newContent),
                    KeyName = httpPath + "/width_50",
                    Size = 50
                });
            }

            return variations;
        }

        public static ImageAttribute ValidateImage(byte[] content, string fileName)
        {
            try
            {
                using (var imgOriginal = System.Drawing.Image.FromStream(new MemoryStream(content)))
                {
                    return new ImageAttribute() { IsValidImage = true, Size = content.Length, HeightInPixels = imgOriginal.Height, WidthInPixels = imgOriginal.Width, Type = Enums.FileType.Image };
                }
            }
            catch
            {
                return new ImageAttribute() { IsValidImage = false, Message = "File contents doesn't seems to be a valid image content, the file either been corrupted or the extension is altered." };
            }
        }

        public static byte[] ResizeFile
                             (
                                 byte[] bytFileContent,
                                 int intTargetSize,
                                 ref bool blSavedAsPng
                             )
        {
            if (bytFileContent == null)
                return null;

            blSavedAsPng = false;

            using (
                System.Drawing.Image imgOriginal = System.Drawing.Image.FromStream(new MemoryStream(bytFileContent)))
            {
                /// Calculate and get the new dimension of image as per the new resolution passed.
                Size newSize = Compute.Dimensions(imgOriginal.Size, intTargetSize);

                using (Bitmap imgNew = new Bitmap(newSize.Width, newSize.Height))
                {
                    using (Graphics canvas = Graphics.FromImage(imgNew))
                    {
                        canvas.SmoothingMode = SmoothingMode.HighQuality;
                        canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        canvas.CompositingQuality = CompositingQuality.HighQuality;
                        canvas.DrawImage(imgOriginal, new Rectangle(0, 0, newSize.Width, newSize.Height));

                        MemoryStream m = new MemoryStream();

                        if (ImageFormat.Tiff.Equals(imgOriginal.RawFormat))
                        {
                            FrameDimension frameDimensions = new FrameDimension(imgOriginal.FrameDimensionsList[0]);

                            // Selects first frame and save as jpeg. 
                            imgNew.SelectActiveFrame(frameDimensions, 0);

                            imgNew.Save(m, ImageFormat.Jpeg);
                        }
                        else if (ImageFormat.Png.Equals(imgOriginal.RawFormat) || ImageFormat.Gif.Equals(imgOriginal.RawFormat))
                        {
                            if (ContainsTransparent(imgNew))
                            {
                                blSavedAsPng = true;
                                imgNew.Save(m, ImageFormat.Png);
                            }
                            else
                            {
                                imgNew.Save(m, ImageFormat.Jpeg);
                            }
                        }
                        else
                            imgNew.Save(m, ImageFormat.Jpeg);

                        return m.ToArray();
                    }
                }
            }
        }

        private static bool ContainsTransparent(Bitmap imgNew)
        {
            return true;
        }
    }

    public class Amazonvariation
    {
        public string KeyName { get; set; }
        public Stream Content { get; set; }
        public short Size { get; set; }
    }
}
