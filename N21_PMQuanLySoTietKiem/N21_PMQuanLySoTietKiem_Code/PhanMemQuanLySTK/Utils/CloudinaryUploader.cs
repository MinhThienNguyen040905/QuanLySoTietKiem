using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemQuanLySTK.Utils
{
    public class CloudinaryUploader
    {
        private readonly Cloudinary _cloudinary;

        private const string CloudName = "dpnvyfwnp";  
        private const string ApiKey = "133881715324257";     
        private const string ApiSecret = "ZCQu9YAA7imY1XYtoko7WuAHsDE"; 

        public CloudinaryUploader()
        {
            var account = new Account(CloudName, ApiKey, ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public string UploadImage(string filePath)
        {
            try
            {
                var file = new FileInfo(filePath);

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.Name, file.FullName),
                };

                var uploadResult = _cloudinary.Upload(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return uploadResult.SecureUrl.ToString();
                }
                else
                {
                    throw new Exception("Failed to upload image.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error during upload: " + ex.Message);
            }
        }
    }
}
