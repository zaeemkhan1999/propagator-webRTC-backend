
namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class UploadDto
    {
        public string FileName { get; set; }
        public bool IsSuccessFull { get; set; }
        public string ErrorMessage { get; set; }
    }
}