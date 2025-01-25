using ChatAPI.Helpers;
using System.ComponentModel.DataAnnotations;

namespace ChatAPI.Attributes
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private string[] _validTypes { get; }
        private bool _nullable { get; }

        public AllowedExtensionsAttribute(string[] validTypes, bool nullable)
        {
            _validTypes = validTypes;
            _nullable = nullable;
        }

        public override bool IsValid(object? value)
        {
            if (_nullable == true && value == null)
                return true;
            if (value is IFormFile)
            {
                var file = value as IFormFile;
                if (file != null)
                {
                    return ServerFile.CheckFileExtension(file, _validTypes);
                }
            }
            else if (value is IEnumerable<IFormFile>)
            {
                var files = value as List<IFormFile>;
                foreach (var file in files)
                {
                    if (file != null && ServerFile.CheckFileExtension(file, _validTypes) == false)
                        return false;
                }
                return true;
            }
            return false;

        }

    }
}
