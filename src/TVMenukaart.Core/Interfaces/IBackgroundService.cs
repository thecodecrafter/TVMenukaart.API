using CloudinaryDotNet.Actions;

namespace TVMenukaart.Interfaces
{
    public interface IBackgroundService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
