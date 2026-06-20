using AutoMapper;
using inzBackend.Entities.Curriculum;
using inzBackend.Exceptions;
using inzBackend.Models;
using inzBackend.Models.MatrixModels;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.MatrixServices
{
    public class MatrixService : IMatrixService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IMapper _mapper;
        public MatrixService(GmitrzakEnglishAcademyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public List<MatrixDto> GetAllMatrices()
        {
            var matrices = _dbContext
                .Matrices
                .Include(m => m.MatrixModules)
                    .ThenInclude(mm => mm.Module)
                .Include(m => m.CourseMatrices)
                    .ThenInclude(cm => cm.Course)
                .ToList();

            return _mapper.Map<List<MatrixDto>>(matrices);
        }

        public Matrix CreateMatrix(CreateMatrixRequest request)
        {
            var matrix = new Matrix
            {
                Name = request.Name,
                Description = request.Description,
                IsHidden = request.IsHidden,
                RefreshIntervalDays = request.RefreshIntervalDays
            };
            _dbContext.Matrices.Add(matrix);
            _dbContext.SaveChanges();
            return matrix;
        }

        public void UpdateMatrix(int matrixId, UpdateMatrixRequest request)
        {
            var matrix = _dbContext
                .Matrices
                .FirstOrDefault(m => m.Id == matrixId);
            if (matrix is null)
                throw new NotFoundException($"Matrix with id {matrixId} was not found");

            matrix.Name = request.Name;
            matrix.Description = request.Description;
            matrix.RefreshIntervalDays = request.RefreshIntervalDays;
            matrix.IsHidden = request.IsHidden;

            _dbContext.SaveChanges();
        }

        public void DeleteMatrix(int matrixId)
        {
            var matrix = _dbContext
                .Matrices
                .FirstOrDefault(m => m.Id == matrixId);
            if (matrix is null)
                throw new NotFoundException($"Matrix with id {matrixId} was not found");

            _dbContext.Remove(matrix);
            _dbContext.SaveChanges();
        }

        public void AssignCourse(int matrixId, int courseId)
        {
            var matrix = _dbContext
                .Matrices
                .FirstOrDefault(m => m.Id == matrixId);
            if (matrix is null)
                throw new NotFoundException($"Matrix with id {matrixId} was not found");

            var course = _dbContext
                .Courses
                .FirstOrDefault(c => c.Id == courseId);
            if (course is null)
                throw new NotFoundException($"Course with id {courseId} was not found");

            var alreadyExists = _dbContext
                .CourseMatrices
                .Any(cm => cm.MatrixId == matrixId && cm.CourseId == courseId);
            if (alreadyExists)
                throw new BadRequestException($"Matrix {matrix.Name} is already assigned to course: {course.Name}");

            var courseMatrix = new CourseMatrix
            {
                MatrixId = matrixId,
                CourseId = courseId
            };

            _dbContext.CourseMatrices.Add(courseMatrix);
            _dbContext.SaveChanges();
        }

        public void DetachCourse(int matrixId, int courseId)
        {
            var matrix = _dbContext
                .Matrices
                .FirstOrDefault(m => m.Id == matrixId);
            if (matrix is null)
                throw new NotFoundException($"Matrix with id {matrixId} was not found");

            var course = _dbContext
                .Courses
                .FirstOrDefault(c => c.Id == courseId);
            if (course is null)
                throw new NotFoundException($"Course with id {courseId} was not found");

            var courseMatrix = _dbContext
                .CourseMatrices
                .FirstOrDefault(cm => cm.MatrixId == matrixId && cm.CourseId == courseId);
            if (courseMatrix is null)
                throw new NotFoundException($"Matrix {matrix.Name} is not assigned to course: {course.Name}");

            _dbContext.CourseMatrices.Remove(courseMatrix);
            _dbContext.SaveChanges();
        }
    }
}
