using inzBackend.Entities.Curriculum;
using inzBackend.Entities.Gamification;
using inzBackend.Enums;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.AcademyExamModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace inzBackend.Services.AcademyExamServices
{
    public class AcademyExamService : IAcademyExamService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;

        public AcademyExamService(GmitrzakEnglishAcademyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<AcademyExamDto> GetExamsByLevel(ExamLevel level, int currentUserId)
        {
            var exams = _dbContext.AcademyExams
                .Include(e => e.Signups)
                    .ThenInclude(s => s.User)
                        .ThenInclude(u => u.Profile)
                .Where(e => e.Level == level && !e.IsDeleted)
                .OrderByDescending(e => e.CreatedAt)
                .ToList();

            return exams.Select(e => MapToDto(e, currentUserId)).ToList();
        }

        public AcademyExamDto GetExamById(int examId, int currentUserId)
        {
            var exam = _dbContext.AcademyExams
                .Include(e => e.Signups)
                    .ThenInclude(s => s.User)
                        .ThenInclude(u => u.Profile)
                .FirstOrDefault(e => e.Id == examId && !e.IsDeleted);

            if (exam == null)
            {
                throw new NotFoundException("Exam not found.");
            }

            return MapToDto(exam, currentUserId);
        }

        public void SignUpForExam(int examId, int currentUserId)
        {
            var exam = _dbContext.AcademyExams
                .Include(e => e.Signups)
                .FirstOrDefault(e => e.Id == examId && !e.IsDeleted);

            if (exam == null)
            {
                throw new NotFoundException("Exam not found.");
            }

            if (!exam.IsActive)
            {
                throw new BadRequestException("This exam is currently not active for signups.");
            }

            if (exam.SignupDeadline < PolandTime.DateTimeNow)
            {
                throw new BadRequestException("The signup deadline for this exam has passed.");
            }

            var existingSignup = exam.Signups.FirstOrDefault(s => s.UserId == currentUserId);

            if (existingSignup != null)
            {
                if (existingSignup.Status != ExamSignupStatus.Cancelled)
                {
                    throw new BadRequestException("You are already registered for this exam.");
                }

                existingSignup.Status = ExamSignupStatus.Registered;
                existingSignup.SignedUpAt = PolandTime.DateTimeNow;
            }
            else
            {
                var newSignup = new AcademyExamSignup
                {
                    ExamId = examId,
                    UserId = currentUserId,
                    SignedUpAt = PolandTime.DateTimeNow,
                    Status = ExamSignupStatus.Registered
                };
                _dbContext.AcademyExamSignups.Add(newSignup);
            }

            _dbContext.SaveChanges();
        }

        public void UnsignFromExam(int examId, int currentUserId)
        {
            var exam = _dbContext.AcademyExams
                .FirstOrDefault(e => e.Id == examId && !e.IsDeleted);

            if (exam == null)
            {
                throw new NotFoundException("Exam not found.");
            }

            var signup = _dbContext.AcademyExamSignups
                .FirstOrDefault(s => s.ExamId == examId && s.UserId == currentUserId);

            if (signup == null || signup.Status == ExamSignupStatus.Cancelled)
            {
                throw new NotFoundException("You are not registered for this exam.");
            }

            if (signup.Status != ExamSignupStatus.Registered)
            {
                throw new BadRequestException("Cannot unsign from an exam that has already been evaluated.");
            }

            if (exam.SignupDeadline < PolandTime.DateTimeNow)
            {
                throw new BadRequestException("Cannot unsign after the signup deadline has passed.");
            }

            _dbContext.AcademyExamSignups.Remove(signup);
            _dbContext.SaveChanges();
        }

        public int CreateExam(CreateAcademyExamDto dto)
        {
            var materialsJson = dto.Materials != null && dto.Materials.Any()
                ? JsonSerializer.Serialize(dto.Materials)
                : null;

            var exam = new AcademyExam
            {
                Title = dto.Title,
                Description = dto.Description,
                Level = dto.Level,
                MaterialsUrl = dto.MaterialsUrl,
                MaterialsJson = materialsJson,
                RewardCredits = dto.RewardCredits,
                PassingThreshold = dto.PassingThreshold,
                SignupDeadline = dto.SignupDeadline,
                IsActive = dto.IsActive
            };

            _dbContext.AcademyExams.Add(exam);
            _dbContext.SaveChanges();

            return exam.Id;
        }

        public void UpdateExam(int examId, UpdateAcademyExamDto dto)
        {
            var exam = _dbContext.AcademyExams.FirstOrDefault(e => e.Id == examId && !e.IsDeleted);
            if (exam == null)
            {
                throw new NotFoundException("Exam not found.");
            }

            var materialsJson = dto.Materials != null && dto.Materials.Any()
                ? JsonSerializer.Serialize(dto.Materials)
                : null;

            exam.Title = dto.Title;
            exam.Description = dto.Description;
            exam.Level = dto.Level;
            exam.MaterialsUrl = dto.MaterialsUrl;
            exam.MaterialsJson = materialsJson;
            exam.RewardCredits = dto.RewardCredits;
            exam.PassingThreshold = dto.PassingThreshold;
            exam.SignupDeadline = dto.SignupDeadline;
            exam.IsActive = dto.IsActive;

            _dbContext.SaveChanges();
        }

        public void DeleteExam(int examId)
        {
            var exam = _dbContext.AcademyExams.FirstOrDefault(e => e.Id == examId && !e.IsDeleted);
            if (exam == null)
            {
                throw new NotFoundException("Exam not found.");
            }

            exam.IsDeleted = true;
            _dbContext.SaveChanges();
        }

        public void MarkTakerStatus(int examId, int targetUserId, ExamSignupStatus status)
        {
            var exam = _dbContext.AcademyExams.FirstOrDefault(e => e.Id == examId && !e.IsDeleted);
            if (exam == null)
            {
                throw new NotFoundException("Exam not found.");
            }

            var signup = _dbContext.AcademyExamSignups
                .FirstOrDefault(s => s.ExamId == examId && s.UserId == targetUserId);

            if (signup == null)
            {
                throw new NotFoundException("User signup for this exam was not found.");
            }

            var previousStatus = signup.Status;
            signup.Status = status;

            if (status == ExamSignupStatus.Passed && previousStatus != ExamSignupStatus.Passed && exam.RewardCredits > 0)
            {
                var credit = new Credit
                {
                    UserId = targetUserId,
                    Amount = exam.RewardCredits,
                    Reason = $"Passed exam: {exam.Title}",
                    Date = DateOnly.FromDateTime(PolandTime.DateTimeNow)
                };

                _dbContext.Credits.Add(credit);
            }
            else if (previousStatus == ExamSignupStatus.Passed && status != ExamSignupStatus.Passed && exam.RewardCredits > 0)
            {
                var credit = new Credit
                {
                    UserId = targetUserId,
                    Amount = -exam.RewardCredits,
                    Reason = $"Revoked credits for exam: {exam.Title}",
                    Date = DateOnly.FromDateTime(PolandTime.DateTimeNow)
                };

                _dbContext.Credits.Add(credit);
            }

            _dbContext.SaveChanges();
        }

        private static AcademyExamDto MapToDto(AcademyExam exam, int currentUserId)
        {
            var activeSignups = exam.Signups
                .Where(s => s.Status != ExamSignupStatus.Cancelled)
                .ToList();

            var userSignup = activeSignups.FirstOrDefault(s => s.UserId == currentUserId);
            var isSignedUp = userSignup != null;
            var isDeadlinePassed = exam.SignupDeadline < PolandTime.DateTimeNow;

            var takers = activeSignups
                .OrderByDescending(s => s.SignedUpAt)
                .Select(s => new ExamTakerDto
                {
                    UserId = s.UserId,
                    Username = s.User?.Username ?? "Unknown User",
                    AvatarUrl = s.User?.Profile?.AvatarUrl,
                    SignedUpAt = s.SignedUpAt,
                    Status = s.Status
                })
                .ToList();

            var materials = new List<ExamMaterialDto>();
            if (!string.IsNullOrWhiteSpace(exam.MaterialsJson))
            {
                try
                {
                    materials = JsonSerializer.Deserialize<List<ExamMaterialDto>>(exam.MaterialsJson) ?? new();
                }
                catch
                {
                    materials = new();
                }
            }

            if (!materials.Any() && !string.IsNullOrWhiteSpace(exam.MaterialsUrl))
            {
                materials.Add(new ExamMaterialDto { Title = "Study Materials", Url = exam.MaterialsUrl });
            }

            return new AcademyExamDto
            {
                Id = exam.Id,
                Title = exam.Title,
                Description = exam.Description,
                Level = exam.Level,
                MaterialsUrl = exam.MaterialsUrl,
                Materials = materials,
                RewardCredits = exam.RewardCredits,
                PassingThreshold = exam.PassingThreshold,
                SignupDeadline = exam.SignupDeadline,
                IsActive = exam.IsActive,
                CreatedAt = exam.CreatedAt.DateTime,

                IsCurrentUserSignedUp = isSignedUp,
                CurrentUserStatus = userSignup?.Status,
                CanSignUp = !isSignedUp && exam.IsActive && !isDeadlinePassed,
                CanUnsign = isSignedUp && (userSignup?.Status == ExamSignupStatus.Registered) && !isDeadlinePassed,
                TakersCount = takers.Count,
                Takers = takers
            };
        }
    }
}
