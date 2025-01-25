namespace ChatAPI.Helpers
{
    public static class ServerFile
    {
        public static string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public static string GetExtensionFromBase64(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return null;
            
            var data = base64String.Substring(0, 5);

            switch (data.ToUpper())
            {
                case "IVBOR":
                    return ".png";
                case "/9J/4":
                    return ".jpg";
                case "AAAAF":
                    return ".mp4";
                case "JVBER":
                    return ".pdf";
                case "AAABA":
                    return ".ico";
                case "UMFYI":
                    return ".rar";
                case "E1XYD":
                    return ".rtf";
                case "U1PKC":
                    return ".txt";
                case "MQOWM":
                case "77U/M":
                    return ".srt";
                default:
                    return null;
            }
        }

        public static bool Upload(IFormFile file, string serverFullPath)
        {
            if (file != null)
            {
                using (var localFile = File.OpenWrite(serverFullPath))
                using (var uploadedFile = file.OpenReadStream())
                {
                    uploadedFile.CopyTo(localFile);
                }
                return true;
            }
            return false;

        }
        public static bool Upload(string base64String, string serverFullPath) 
        {
            if(string.IsNullOrEmpty(base64String) || string.IsNullOrEmpty(serverFullPath))
                return false;

            byte[] fileData = Convert.FromBase64String(base64String);
            File.WriteAllBytesAsync(serverFullPath, fileData).Wait();
            return true;
        }
        public static void Delete(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static bool CheckFileExtension(IFormFile file, string[] validTypes)
        {
            string FileExtension = GetExtension(file.FileName);
            if (validTypes.Contains(FileExtension))
            {
                return true;
            }
            return false;
        }
    }
}
