using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ELearningF8.Models;
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
                    return NotFound(new { Status = 404, Message = "Không tìm thấy file" });

                var fileName = file.FileName.Split('.').First();

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
                    var result = uploadResult.Uri.ToString();

                    return Ok(new { Status = 200, Message = "Success", Data = result });
                }

            }
            catch (Exception ex)
            {
                // Xử lý lỗi tải lên hình ảnh
                return BadRequest(new { Status = 400, Message = ex.Message });
            }
        }
        //public async Task<string?> SaveVideoAsync(IFormFile videoFile)
        //{
        //    try
        //    {
        //        var cloudinary = new Cloudinary();
        //        if (videoFile == null || videoFile.Length == 0)
        //            return null;

        //        var fileName = videoFile.FileName.Split(".").First();

        //        using (var stream = videoFile.OpenReadStream())
        //        {
        //            var uploadParams = new VideoUploadParams
        //            {
        //                File = new FileDescription(videoFile.FileName, stream),
        //                PublicId = fileName + "_" + DateTime.UtcNow.Ticks,
        //                Folder = "ELearningF8/Videos" // Tên thư mục trên Cloudinary (tùy chọn)
        //            };

        //            var uploadResult = await cloudinary.UploadAsync(uploadParams);

        //            // Trả về URL công khai của video đã tải lên
        //            return uploadResult.Uri.ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Xử lý lỗi tải lên video
        //        return ex.Message;
        //    }
        //}
    }
}
