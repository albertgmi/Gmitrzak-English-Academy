using AutoMapper;
using inzBackend.Entities;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.ModuleModels;
using inzBackend.Models.StudentCourseModels;
using inzBackend.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.ModuleServices
{
    public class ModuleService : IModuleService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;
        public ModuleService(GmitrzakEnglishAcademyDbContext dbContext, IMapper mapper, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public List<ModuleDto> getAllModules()
        {
            var modules = _dbContext
                .Modules
                .Include(m => m.MatrixModules)
                    .ThenInclude(mm => mm.Matrix)
                .Include(m => m.TheaterItem)
                .ToList();
            if (modules is null || modules.Count == 0)
                throw new NotFoundException("No modules were found");

            return _mapper.Map<List<ModuleDto>>(modules);
        }

        public List<ModuleDto> getSentenceModulesForStudent(int studentId)
        {
            var studentMatrixIds = _dbContext.UserMatrixAssignments
                .Where(uma => uma.UserId == studentId)
                .Select(uma => uma.MatrixId)
                .ToList();

            var studentDirectModuleIds = _dbContext.UserModuleAssignments
                .Where(uma => uma.UserId == studentId)
                .Select(uma => uma.ModuleId)
                .ToList();

            var modules = _dbContext.Modules
                .Where(m => m.Category == "Sentences" &&
                            (studentDirectModuleIds.Contains(m.Id) ||
                             m.MatrixModules.Any(mm => studentMatrixIds.Contains(mm.MatrixId))))
                .ToList();

            return _mapper.Map<List<ModuleDto>>(modules);
        }

        public ModuleDto createModule(CreateModuleRequest request)
        {
            var module = new Module
            {
                Name = request.Name,
                Description = request.Description,
                IsHidden = request.IsHidden ?? false,
                Category = request.Category,
                TheaterItemId = request.Category == "Watching" ? request.TheaterItemId : null
            };

            _dbContext.Modules.Add(module);
            _dbContext.SaveChanges();

            if (request.Category == "Presentation" &&
                (!string.IsNullOrWhiteSpace(request.PresentationUrl) ||
                 !string.IsNullOrWhiteSpace(request.PresentationText)))
            {
                _dbContext.ModulePresentations.Add(new ModulePresentation
                {
                    ModuleId = module.Id,
                    Url = request.PresentationUrl,
                    Text = request.PresentationText
                });
                _dbContext.SaveChanges();
            }

            return _mapper.Map<ModuleDto>(module);
        }

        public void deleteModule(int moduleId)
        {
            var module = _dbContext
                .Modules
                .FirstOrDefault(x => x.Id == moduleId);
            if (module is null)
                throw new NotFoundException($"Module with id {moduleId} was not found");

            _dbContext.Modules.Remove(module);
            _dbContext.SaveChanges();
        }

        public void updateModule(int moduleId, UpdateModuleRequest request)
        {
            var module = _dbContext.Modules
                .Include(m => m.Presentation)
                .FirstOrDefault(x => x.Id == moduleId)
                ?? throw new NotFoundException("Module not found");

            if (request.Name is not null) module.Name = request.Name;
            if (request.Description is not null) module.Description = request.Description;
            if (request.IsHidden is not null) module.IsHidden = request.IsHidden;
            if (request.Category is not null) module.Category = request.Category;

            module.TheaterItemId = request.Category == "Watching" ? request.TheaterItemId : null;

            if (request.Category == "Presentation")
            {
                if (module.Presentation is not null)
                {
                    module.Presentation.Url = request.PresentationUrl;
                    module.Presentation.Text = request.PresentationText;
                }
                else if (!string.IsNullOrWhiteSpace(request.PresentationUrl) ||
                         !string.IsNullOrWhiteSpace(request.PresentationText))
                {
                    _dbContext.ModulePresentations.Add(new ModulePresentation
                    {
                        ModuleId = moduleId,
                        Url = request.PresentationUrl,
                        Text = request.PresentationText
                    });
                }
            }
            else if (module.Presentation is not null)
            {
                _dbContext.ModulePresentations.Remove(module.Presentation);
            }

            _dbContext.SaveChanges();
        }

        public void assignMatrix(int moduleId, int matrixId, AssignModuleToMatrixRequest request)
        {
            var module = _dbContext
                .Modules
                .FirstOrDefault(x => x.Id == moduleId);
            if (module is null)
                throw new NotFoundException($"Module with id {moduleId} was not found");

            var matrix = _dbContext
                .Matrices
                .FirstOrDefault(x => x.Id == matrixId);
            if (matrix is null)
                throw new NotFoundException($"Matrix with id {matrixId} was not found");

            var alreadyExists = _dbContext
                .MatrixModules
                .Any(mm => mm.MatrixId == matrixId && mm.ModuleId == moduleId);
            if (alreadyExists)
                throw new BadRequestException($"Module {module.Name} is already assigned to matrix {matrix.Name}");

            var matrixModule = new MatrixModule
            {
                MatrixId = matrixId,
                ModuleId = moduleId,
                WeekNumber = request.WeekNumber,
                DayOfWeek = request.DayOfWeek
            };
            _dbContext.MatrixModules.Add(matrixModule);
            _dbContext.SaveChanges();
        }

        public void detachMatrix(int moduleId, int matrixId)
        {
            var module = _dbContext
                .Modules
                .FirstOrDefault(x => x.Id == moduleId);
            if (module is null)
                throw new NotFoundException($"Module with id {moduleId} was not found");

            var matrix = _dbContext
                .Matrices
                .FirstOrDefault(x => x.Id == matrixId);
            if (matrix is null)
                throw new NotFoundException($"Matrix with id {matrixId} was not found");

            var matrixModule = _dbContext
                .MatrixModules
                .FirstOrDefault(mm => mm.MatrixId == matrixId && mm.ModuleId == moduleId);
            if (matrixModule is null)
                throw new NotFoundException($"Module {module.Name} is not assigned to matrix {matrix.Name}");

            _dbContext.MatrixModules.Remove(matrixModule);
            _dbContext.SaveChanges();
        }

        private (int days, int required, bool canComplete, string? blockReason) getActivityStatus(int userId, int moduleId, string category, DateOnly today)
        {
            const int REQUIRED_DAYS = 3;

            switch (category)
            {
                case "Flashcards":
                    {
                        var days = _dbContext.FlashcardStudyLogs
                            .Where(x => x.UserId == userId)
                            .Select(x => x.StudyDate)
                            .Distinct()
                            .Count();

                        return (days, REQUIRED_DAYS, days >= REQUIRED_DAYS,
                            days >= REQUIRED_DAYS ? null
                                : $"Study flashcards for {REQUIRED_DAYS - days} more day(s).");
                    }
                case "SentenceFlashcards":
                    {
                        var days = _dbContext.SectionActivityLogs
                            .Where(x => x.UserId == userId && x.Section == "sentenceflashcards")
                            .Select(x => x.ActivityDate)
                            .Distinct()
                            .Count();
                        return (days, REQUIRED_DAYS, days >= REQUIRED_DAYS,
                            days >= REQUIRED_DAYS ? null
                                : $"Practice sentence flashcards for {REQUIRED_DAYS - days} more day(s).");
                    }
                case "Sentences":
                    return (0, 0, true, null);
                case "Memories":
                    {
                        var days = _dbContext.SectionActivityLogs
                            .Where(x => x.UserId == userId && x.Section == "memories")
                            .Select(x => x.ActivityDate)
                            .Distinct().Count();
                        return (days, REQUIRED_DAYS, days >= REQUIRED_DAYS,
                            days >= REQUIRED_DAYS ? null
                                : $"Visit Memories for {REQUIRED_DAYS - days} more day(s).");
                    }
                case "Pronunciation":
                    {
                        var days = _dbContext.SectionActivityLogs
                            .Where(x => x.UserId == userId && x.Section == "pronunciation")
                            .Select(x => x.ActivityDate)
                            .Distinct()
                            .Count();
                        return (days, REQUIRED_DAYS, days >= REQUIRED_DAYS,
                            days >= REQUIRED_DAYS ? null
                                : $"Practice pronunciation for {REQUIRED_DAYS - days} more day(s).");
                    }
                default:
                    return (0, 0, true, null);
            }
        }
    }
}
