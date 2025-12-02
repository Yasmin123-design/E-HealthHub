namespace E_PharmaHub.Services.FileStorageServ
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderName);
        void DeleteFile(string fileUrl, string containerName);
    }
}
