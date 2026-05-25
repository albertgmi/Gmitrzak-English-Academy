using AutoMapper;
using inzBackend.Exceptions;
using inzBackend.Models;
using inzBackend.Models.ModuleModels;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.ModuleServices
{
    public class ModuleService : IModuleService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IMapper _mapper;
        public ModuleService(GmitrzakEnglishAcademyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
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

        public Module createModule(CreateModuleRequest request)
        {
            var newModule = new Module
            {
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                IsHidden = request.IsHidden,
                TheaterItemId = request.Category == "Watching" ? request.TheaterItemId : null
            };

            _dbContext.Modules.Add(newModule);
            _dbContext.SaveChanges();
            return newModule;
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
            var module = _dbContext
                .Modules
                .FirstOrDefault(x => x.Id == moduleId);
            if (module is null)
                throw new NotFoundException($"Module with id {moduleId} was not found");

            module.Name = request.Name;
            module.Description = request.Description;
            module.Category = request.Category;
            module.IsHidden = request.IsHidden;
            module.TheaterItemId = request.Category == "Watching" ? request.TheaterItemId : null;

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
    }
}
