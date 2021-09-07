using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Insurance.Domain.Common;

namespace Be.Library
{
    public static class FileHelper
    {
        public static string CompilePath(this string path, bool isWindow = true)
        {
            if (isWindow)
                return path.Replace(@"/", @"\");
            return path.Replace(@"\", @"/");
        }

        public static async Task<BaseFileEntity> UploadFile(string storagePath, IFormFile fileStream)
        {
            if (fileStream == null)
            {
                return null;
            }
            if (storagePath != null && !storagePath.EndsWith("\\"))
            {
                storagePath += "\\";
            }

            var fileName = Path.GetFileNameWithoutExtension(fileStream.FileName);

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = fileStream.FileName;
            }
            var fileModel = new BaseFileEntity
            {
                OriginalFileName = fileName,
                FileName = fileName
                ,
                Size = fileStream.Length.ToString()
            };

            if (IsValidFileUpload(storagePath, fileStream, ref fileModel))
            {
                storagePath = CompilePath(storagePath);
                if (!Directory.Exists(storagePath))
                {
                    Directory.CreateDirectory(storagePath);
                }
                var path = Path.Combine(storagePath, fileModel.FileName);
                path = CompilePath(path);
                fileModel.Path = path;
                await using var stream = File.Create(path);
                await fileStream.CopyToAsync(stream);

                await stream.DisposeAsync();
                return fileModel;
            }
            return null;
        }

        public static int UploadFileMaxFileSizeInMb
        {
            get
            {
                if (int.TryParse("100", out var number))
                {
                    return number;
                }
                return 10;
            }
        }

        public static bool IsValidFileUpload(string fileStoragePath, IFormFile fileStream, ref BaseFileEntity fileEntity)
        {
            var fileExt = Path.GetExtension(fileStream.FileName);
            {
                fileEntity.FileExtension = fileExt;
                fileExt = fileExt.Replace(".", "");
                //if (UploadFileAllowExt.IndexOf("," + fileExt + ",", StringComparison.Ordinal) < 0)
                //{
                //    return false;
                //}
                if (fileStream.Length > UploadFileMaxFileSizeInMb * 1024 * 1024)
                {
                    return false;
                }

                if (!fileStoragePath.EndsWith("\\"))
                {
                    fileStoragePath += "\\";
                }

                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileEntity.FileName);

                fileEntity.FileName = fileNameWithoutExt + "." + fileExt;

                var fullPath = Path.Combine(fileStoragePath, fileEntity.FileName);

                if (File.Exists(fullPath))
                {
                    fileEntity.FileName = Path.GetFileNameWithoutExtension(fileEntity.FileName) + "-" +
                                          DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + fileExt;
                }
            }
            return true;
        }

        public static bool IsIFormFileValidSizeAndExtension(this IFormFile file, out List<string> errors)
        {
            errors = new List<string>();
            if (file.Length > UploadFileMaxFileSizeInMb * 1024 * 1024)
            {
                errors.Add($"Kích thước file phải nhỏ hơn {UploadFileMaxFileSizeInMb} Mbs");
            }
            return !errors.Any();
        }

        public static string GetMimeTypeByFileExtension(string fileExt)
        {
            fileExt = fileExt.ToLower().Replace(".", "");
            switch (fileExt)
            {
                case "doc":
                    return "application/msword";

                case "docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                case "xls":
                    return "application/vnd.ms-excel";

                case "xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                case "jpg":
                    return "image/jpeg";

                case "png":
                    return "image/png";

                default:
                    return "application/pdf";
            }
        }

        public static int GetInt(string s)
        {
            if (int.TryParse(s, out var number))
            {
                return number;
            }
            return 0;
        }
    }
}