using System.ComponentModel.DataAnnotations;

namespace ChatAPI.Attributes
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private int _maxFileSize { get; }
        private bool _nullable { get; }


        public MaxFileSizeAttribute(int maxSizeInBytes, bool nullable)
        {
            _maxFileSize = maxSizeInBytes;
            _nullable = nullable;
        }

        public override bool IsValid(object? value)
        {
            if(_nullable == true && value == null)
                return true;

            if (value is IFormFile)
            {
                var file = value as IFormFile;
                if (file != null)
                    return file.Length <= _maxFileSize ? true : false;
            }
            else if (value is IEnumerable<IFormFile>) 
            {
                var files = value as List<IFormFile>;
                foreach (var file in files)
                {
                    if (file != null && file?.Length > _maxFileSize)
                        return false;                        
                }
                return true;
            }
            return false;
        }
    }
}
