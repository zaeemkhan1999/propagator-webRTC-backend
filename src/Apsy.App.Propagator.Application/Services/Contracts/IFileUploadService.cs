using Apsy.App.Propagator.Domain.Common.Inputs;
namespace Apsy.App.Propagator.Application.Services.Contracts
{
    public interface IFileUploadService
    {
        public Task<string> FileUploaderAsync(FileUploadDto uploadDto);
        public Task<string> ThumbnailUploaderAsync(string requestrgs);
        
        public FileUploadDto SavefileinlocalfolderProduct(ProductInput input);
        
    }
}
