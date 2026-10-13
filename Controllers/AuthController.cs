
using Microsoft.AspNetCore.Mvc;
using Township_API.Models;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Township_API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;
using Image = Township_API.Models.Image;
using System.ComponentModel;
using System.Data;
 


namespace Township_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //private readonly UserManager<IdentityUser> _userManager;
        //private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly AppDBContext _context;

        public AuthController(IConfiguration configuration, AppDBContext context)
        {
            //UserManager<IdentityUser> userManager, SignInManager< IdentityUser > signInManager,
            //_userManager = userManager;
            //_signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _context.UserRegisters.Where(p => (p.uid == model.Email || p.email == model.Email) && p.password == model.Password).ToListAsync();
            if (user != null)
            {
                if (user.Count > 0)
                { 
                    //  var roles = await _context.Roles.Where(p => p.RoleID == user[0].roleID).FirstOrDefault();
                    // var token = GenerateJwtToken(user[0], roles);
                    var token = GenerateJwtToken(user[0].name);
                    return Ok(new { token });
                    //return Ok(user);
                }
            }
            return Unauthorized();
        }

        private string GenerateJwtToken(IdentityUser user, IList<string> roles)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private string GenerateJwtToken(string username)
        {
            var jwtSettings = _configuration.GetSection("Jwt:Key");

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("CardHolderSearch/{search}")]
        public async Task<IActionResult> CardHolderSearch(string? search = null)
        {
            List<AllCardHolder> cardHolder = new List<AllCardHolder>();

            try
            {
                //item.Column.Contains("value")
                if (search == null)
                    search = "";
                if (search.Length > 2)
                {
                    cardHolder = await _context.AllCardHolders.Where(p => (p.IDNumber.Contains(search)) || (p.shortname.Contains(search))).ToListAsync();
                    if (cardHolder != null)
                    {
                        if (cardHolder.Count > 0)
                        {
                            return Ok(cardHolder);
                        }

                    }

                }
            }
            catch (Exception ex)
            {

            }

            return Ok(cardHolder);

        }


        //    [HttpGet("{imageName}")]
        //    public IActionResult GetImage(string imageName)
        //    {
        //        try
        //        {
        //            // Path to your image file
        //            var imagePath = Path.Combine("Images", imageName);

        //            if (!System.IO.File.Exists(imagePath))
        //            {
        //                return NotFound();
        //            }

        //            var imageBytes = System.IO.File.ReadAllBytes(imagePath);
        //            var contentType = GetContentType(imagePath); // Determine MIME type

        //            return File(imageBytes, contentType)

        //}
        //    catch (Exception ex)
        //    {
        //        var r = ex.Message.ToString();
        //    }

        //}
        //public async Task<Image?> GetImageByIdAsync(string filenm)
        //{
        //    try
        //    {
        //        var img = (Image)_context.Images.Where(p => p.FileName == filenm);
        //        return img;

        //    }

        //    catch (Exception ex)
        //    {
        //        var r = ex.Message.ToString();
        //    }
        //    return null;

        //}
        [HttpGet("GetImageStream/{imageName}")]
        public IActionResult GetImageStream(string imageName)
        {
            try
            {
                var imagePath = Path.Combine("Images", imageName);

                if (!System.IO.File.Exists(imagePath))
                {
                    var image = _context.Images.Where(p => p.FileName.Contains(imageName)).FirstOrDefault();
                    if (image == null || image.Data == null)
                        return NotFound();
                     

                    var base64 = Convert.ToBase64String(image.Data);
                    return File(base64, GetContentType(image.ContentType).Replace(".",""));
                    //return Ok(new
                    //{
                    //    image.ImageId,
                    //    image.FileName,
                    //    image.ContentType,
                    //    Base64 = $"data:{image.ContentType};base64,{base64}"
                    //});
                }

                var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                var contentType = GetContentType(imagePath);

                return File(stream, contentType);
            }
            catch (Exception ex)
            {
                var r = ex.Message.ToString();
                return BadRequest("error: " + r.ToString());
            }
        }

        [HttpGet("GetImage/{filenm}")]
        public async Task<IActionResult> GetImage(string filenm)
        {
            try
            {
                //var image = await GetImageByIdAsync(filenm);
                var image = _context.Images.Where(p => p.FileName.Contains(filenm)).FirstOrDefault();
                if (image == null || image.Data == null)
                    return NotFound();

                // return File(image.Data,"jpg"); // Serves binary as proper image

                var base64 = Convert.ToBase64String(image.Data);
                return Ok(new
                {
                    image.ImageId,
                    image.FileName,
                    image.ContentType,
                    Base64 = $"data:{image.ContentType};base64,{base64}"
                });
            }
            catch (Exception ex)
            {
                var r = ex.Message.ToString();
                return BadRequest("error: " + r.ToString());
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("File is empty");

                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var imageData = ms.ToArray();


                var existFile = _context.Images.Where(p => p.FileName == file.FileName).FirstOrDefault();
                if (existFile != null)
                {
                    existFile.FileName = file.FileName;
                    existFile.ContentType = file.ContentType;
                    existFile.Data = imageData;
                    _context.Entry(existFile).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return Ok(existFile);
                }
                else 
                {
                    var imageEntity = new Image
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Data = imageData
                };
                    _context.Images.Add(imageEntity);
                    await _context.SaveChangesAsync();
                    return Ok(new { imageEntity.ImageId });
                }
                
            }
            catch (Exception ex)
            {
                var r = ex.Message.ToString();
                return BadRequest("error: " + r.ToString());
            }

        }

        private string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream",
            };
        }
    }

    /* 
     //private string UpdateCardHolderImage(int user,  )
     //{


     //}

     private BitmapImage LoadImageFromDatabase(int imageId)
     {
         byte[] imageBytes = null;

         using (SqlConnection conn = new SqlConnection("Your_Connection_String"))
         {
             conn.Open();
             using (SqlCommand cmd = new SqlCommand("SELECT ImageData FROM Images WHERE Id = @Id", conn))
             {
                 cmd.Parameters.AddWithValue("@Id", imageId);
                 using (SqlDataReader reader = cmd.ExecuteReader())
                 {
                     if (reader.Read())
                     {
                         imageBytes = (byte[])reader["ImageData"];
                     }
                 }
             }
         }

         if (imageBytes != null)
         {
             using (MemoryStream ms = new MemoryStream(imageBytes))
             {
                 BitmapImage bitmap = new BitmapImage();
                 bitmap.BeginInit();
                 bitmap.CacheOption = BitmapCacheOption.OnLoad;
                 bitmap.StreamSource = ms;
                 bitmap.EndInit();
                 return bitmap;
             }
         }

         return null;
     }

     private BitmapImage LoadImageFromDatabase(int imageId)
     {
         byte[] imageBytes = null;

         using (SqlConnection conn = new SqlConnection(connectionString))
         {
             conn.Open();
             using (SqlCommand cmd = new SqlCommand("SELECT ImageData FROM Images WHERE Id = @Id", conn))
             {
                 cmd.Parameters.AddWithValue("@Id", imageId);
                 using (SqlDataReader reader = cmd.ExecuteReader())
                 {
                     if (reader.Read())
                     {
                         imageBytes = (byte[])reader["ImageData"];

                     }
                 }
             }
         }
         if (imageBytes != null)
             return imageBytes;

         //if (imageBytes != null)
         //{
         //    using (MemoryStream ms = new MemoryStream(imageBytes))
         //    {
         //        BitmapImage bitmap = new BitmapImage();
         //        bitmap.BeginInit();
         //        bitmap.CacheOption = BitmapCacheOption.OnLoad;
         //        bitmap.StreamSource = ms;
         //        bitmap.EndInit();
         //        return bitmap;
         //    }
         //}

         return null;
     }
     */
}
