using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace JabbadabbadoeBooking.Services
{
    public interface IImageStorageService
    {
        Task<string> SaveImageAsync(IFormFile file, int spaceId);
        Task DeleteImageAsync(string relativePath);
        void DeleteImage(string relativePath); // sync helper voor bestaande code
    }

    public class ImageStorageService : IImageStorageService
    {
        private readonly string _root;

        public ImageStorageService(IWebHostEnvironment env)
        {
            _root = Path.Combine(env.WebRootPath ?? "wwwroot", "images", "spaces");
            Directory.CreateDirectory(_root);
        }

        public async Task<string> SaveImageAsync(IFormFile file, int spaceId)
        {
            if (file == null || file.Length == 0) throw new ArgumentException("Upload ontbreekt.");
            const long MaxImageBytes = 5 * 1024 * 1024; // 5 MB
            if (file.Length > MaxImageBytes) throw new InvalidOperationException("Afbeelding is te groot (max 5MB).");

            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase){ ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext) || !allowed.Contains(ext)) throw new InvalidOperationException("Bestandstype niet toegestaan.");
            if (string.IsNullOrWhiteSpace(file.ContentType) || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Alleen afbeeldingsuploads zijn toegestaan.");

            var folder = Path.Combine(_root, spaceId.ToString());
            Directory.CreateDirectory(folder);

            var safeName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext.ToLowerInvariant();
            var fileName = safeName;
            var path = Path.Combine(folder, fileName);

            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/spaces/{spaceId}/{fileName}";
        }

        public async Task DeleteImageAsync(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;
            var candidate = relativePath.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
            string baseRoot = _root;
            // Als relativePath al /images/spaces/... bevat, strip dat weg naar fysiek pad
            if (candidate.StartsWith("images" + Path.DirectorySeparatorChar + "spaces", StringComparison.OrdinalIgnoreCase))
            {
                candidate = candidate.Substring(("images" + Path.DirectorySeparatorChar + "spaces" + Path.DirectorySeparatorChar).Length);
                baseRoot = Path.Combine(Directory.GetParent(_root)!.FullName, "images", "spaces"); // wwwroot/images/spaces
            }
            var full = Path.Combine(baseRoot, candidate);
            if (File.Exists(full)) File.Delete(full);
            await Task.CompletedTask;
        }

        public void DeleteImage(string relativePath)
        {
            // sync wrapper voor bestaande code
            DeleteImageAsync(relativePath).GetAwaiter().GetResult();
        }
    }
}
