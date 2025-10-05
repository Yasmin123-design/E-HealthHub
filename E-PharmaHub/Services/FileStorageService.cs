namespace E_PharmaHub.Services
{
    public class FileStorageService : IFileStorageService
    {
            private readonly IWebHostEnvironment _env;

            public FileStorageService(IWebHostEnvironment env)
            {
                _env = env;
            }

            public async Task<string> SaveFileAsync(IFormFile file, string folderName)
            {
                if (file == null || file.Length == 0)
                    return null;

                var webRoot = _env.WebRootPath;
                 if (string.IsNullOrEmpty(webRoot))
                  {
                     webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                  }

            var uploadsFolder = Path.Combine(webRoot, folderName);
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return $"/{folderName}/{uniqueFileName}";
            }

            public void DeleteFile(string filePath)
            {
                var fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }
        }

    
}
