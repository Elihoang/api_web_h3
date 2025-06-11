using API_WebH3.Repository;

namespace API_WebH3.Service;

public class ImageService
{
    private readonly IImageRepository _repository;
        private readonly string _imageStoragePath;

        public ImageService(IImageRepository repository)
        {
            _repository = repository;
            _imageStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(_imageStoragePath))
            {
                Directory.CreateDirectory(_imageStoragePath);
            }
        }

        public async Task<string> UploadUserProfileImageAsync(Guid userId, IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("Vui lòng cung cấp tệp ảnh.");

            var user = await _repository.GetUserByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("Không tìm thấy người dùng.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Định dạng ảnh không được hỗ trợ. Chỉ chấp nhận .jpg, .jpeg, .png, .gif.");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_imageStoragePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            if (!string.IsNullOrEmpty(user.ProfileImage))
            {
                var oldImagePath = Path.Combine(_imageStoragePath, Path.GetFileName(user.ProfileImage));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            user.ProfileImage = $"/images/{fileName}";
            await _repository.UpdateUserAsync(user);

            return user.ProfileImage;
        }

        public async Task<string> UploadPostImageAsync(Guid postId, IFormFile image)
        {
            if (image == null || image.Length == 0)
                AppLogger.LogError("Vui lòng cung cấp tệp ảnh.");

            var post = await _repository.GetPostByIdAsync(postId);
            if (post == null)
                AppLogger.LogError("Không tìm thấy bài viết.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                AppLogger.LogError("Định dạng ảnh không được hỗ trợ. Chỉ chấp nhận .jpg, .jpeg, .png, .gif.");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_imageStoragePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            if (!string.IsNullOrEmpty(post.UrlImage))
            {
                var oldImagePath = Path.Combine(_imageStoragePath, Path.GetFileName(post.UrlImage));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            post.UrlImage = $"/images/{fileName}";
            await _repository.UpdatePostAsync(post);

            return post.UrlImage;
        }
}