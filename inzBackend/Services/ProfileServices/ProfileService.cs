using AutoMapper;
using inzBackend.Exceptions;
using inzBackend.Models.ProfileModels;
using inzBackend.Models;
using inzBackend.Enums;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.ProfileServices
{
    public class ProfileService : IProfileService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProfileService(GmitrzakEnglishAcademyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public ProfileDto getProfile(int userId)
        {
            var profile = _dbContext.Profiles
                .Include(x => x.User)
                .FirstOrDefault(x => x.UserId == userId);

            if (profile is null)
                throw new NotFoundException("Profile not found");

            return _mapper.Map<ProfileDto>(profile);
        }

        public void updateProfile(int userId, UpdateProfileRequest request)
        {
            var profile = _dbContext.Profiles
                .FirstOrDefault(x => x.UserId == userId);

            if (profile is null)
                throw new NotFoundException("Profile not found");

            if (Enum.TryParse<EnglishLevel>(request.EnglishLevel, out var level))
                profile.EnglishLevel = level;

            profile.AvatarUrl = request.AvatarUrl;
            profile.CurrentSemester = request.CurrentSemester;
            profile.Semester1 = request.Semester1;
            profile.Semester2 = request.Semester2;
            profile.Semester3 = request.Semester3;
            profile.Semester4 = request.Semester4;
            profile.Semester5 = request.Semester5;
            profile.Semester6 = request.Semester6;
            profile.Semester7 = request.Semester7;
            profile.Semester8 = request.Semester8;
            profile.Semester9 = request.Semester9;
            profile.Semester10 = request.Semester10;
            profile.Semester11 = request.Semester11;
            profile.Semester12 = request.Semester12;
            profile.Semester13 = request.Semester13;
            profile.Semester14 = request.Semester14;
            profile.Semester15 = request.Semester15;
            profile.Semester16 = request.Semester16;
            profile.Semester17 = request.Semester17;
            profile.Semester18 = request.Semester18;
            profile.Semester19 = request.Semester19;
            profile.Semester20 = request.Semester20;

            _dbContext.SaveChanges();
        }

        public async Task<string> uploadAvatar(int userId, IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new BadRequestException("Only .jpg, .jpeg and .png files are allowed");

            var profile = _dbContext.Profiles.FirstOrDefault(x => x.UserId == userId);
            if (profile is null)
                throw new NotFoundException("Profile not found");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatars");
            Directory.CreateDirectory(uploadsFolder);

            if (!string.IsNullOrEmpty(profile.AvatarUrl))
            {
                var oldFileName = Path.GetFileName(profile.AvatarUrl);
                var oldFilePath = Path.Combine(uploadsFolder, oldFileName);
                if (File.Exists(oldFilePath))
                    File.Delete(oldFilePath);
            }

            var fileName = $"{userId}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            profile.AvatarUrl = $"/avatars/{fileName}";
            _dbContext.SaveChanges();

            return profile.AvatarUrl;
        }
    }
}
