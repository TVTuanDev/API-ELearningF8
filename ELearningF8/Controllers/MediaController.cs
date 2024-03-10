using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;

namespace ELearningF8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;

        public MediaController(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        [HttpPost("/upload-image")]
        public async Task<IActionResult> UploadImageAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { Message = "Không tìm thấy file" });

                var fileName = file.FileName.Split(".")[0];

                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(fileName, stream),
                        PublicId = fileName + "_" + DateTime.UtcNow.Ticks,
                        Folder = "ELearningF8/Images"
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    // Trả về URL công khai của hình ảnh đã tải lên
                    return Ok(uploadResult?.Uri.ToString());
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi tải lên hình ảnh
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("/upload-list-img")]
        public async Task<IActionResult> UploadListImgAsync([FromForm] List<IFormFile> files)
        {
            try
            {
                if (files == null || files.Count == 0)
                    return BadRequest(new { Message = "Không tìm thấy file" });

                List<string> uploadedUrls = new List<string>();

                foreach (var file in files)
                {
                    var fileName = file.FileName.Split(".")[0];
                    using (var stream = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(fileName, stream),
                            PublicId = fileName + "_" + DateTime.UtcNow.Ticks,
                            Folder = "ELearningF8/Images" // Tên thư mục trên Cloudinary
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        uploadedUrls.Add(uploadResult.Uri.ToString());
                    }
                }

                // Trả về URL công khai của các hình ảnh đã tải lên
                return Ok(uploadedUrls);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi tải lên hình ảnh
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("/upload-video")]
        public async Task<IActionResult> UploadVideoAsync(IFormFile videoFile)
        {
            try
            {
                if (videoFile == null || videoFile.Length == 0)
                    return BadRequest(new { Message = "Không tìm thấy file" });

                var fileName = videoFile.FileName.Split(".")[0];

                using (var stream = videoFile.OpenReadStream())
                {
                    var uploadParams = new VideoUploadParams
                    {
                        File = new FileDescription(videoFile.FileName, stream),
                        PublicId = fileName + "_" + DateTime.UtcNow.Ticks,
                        Folder = "ELearningF8/Videos" // Tên thư mục trên Cloudinary (tùy chọn)
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    // Trả về URL công khai của video đã tải lên
                    return Ok(uploadResult.Uri.ToString());
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi tải lên video
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
