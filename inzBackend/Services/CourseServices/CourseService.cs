using AutoMapper;
using inzBackend.Entities.Curriculum;
using inzBackend.Exceptions;
using inzBackend.Models;
using inzBackend.Models.CourseModels;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.CourseServices
{
    public class CourseService : ICourseService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IMapper _mapper;
        public CourseService(GmitrzakEnglishAcademyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public List<CourseDto> getAllCourses()
        {
            var courses = _dbContext
                .Courses
                .Include(c => c.CourseMatrices)
                    .ThenInclude(cm => cm.Matrix)
                .Include(c => c.ProgramCourses)
                    .ThenInclude(pc => pc.Program)
                .ToList();

            return _mapper.Map<List<CourseDto>>(courses);
        }
        
        public Course createCourse(CreateCourseRequest request)
        {
            var newCourse = new Course
            {
                Name = request.Name,
                Description = request.Description,
                IsHidden = request.IsHidden
            };
            _dbContext.Courses.Add(newCourse);
            _dbContext.SaveChanges();
            return newCourse;
        }

        public void updateCourse(int courseId, UpdateCourseRequest request)
        {
            var course = _dbContext
                .Courses
                .FirstOrDefault(c => c.Id == courseId);
            if (course is null)
                throw new NotFoundException($"Course with id: {courseId} was not found");

            course.Name = request.Name;
            course.Description = request.Description;
            course.IsHidden = request.IsHidden;
            _dbContext.SaveChanges();

        }

        public void deleteCourse(int courseId)
        {
            var course = _dbContext
                .Courses
                .FirstOrDefault(c => c.Id == courseId);
            if (course is null)
                throw new NotFoundException($"Course with id: {courseId} was not found");

            _dbContext.Courses.Remove(course);
            _dbContext.SaveChanges();
        }

        public void assignProgram(int courseId, int programId)
        {
            var course = _dbContext
                .Courses
                .FirstOrDefault(c => c.Id == courseId);
            if (course is null)
                throw new NotFoundException($"Course with id {courseId} was not found");

            var program = _dbContext
                .Programs
                .FirstOrDefault(p => p.Id == programId);
            if (program is null)
                throw new NotFoundException($"Program with id {programId} was not found");

            var alreadyExists = _dbContext.ProgramCourses
                .Any(pc => pc.CourseId == courseId && pc.ProgramId == programId);
            if (alreadyExists)
                throw new BadRequestException($"Course {course.Name} is already assigned to program: {program.Name}");

            var programCourse = new ProgramCourse
            {
                CourseId = courseId,
                ProgramId = programId
            };

            _dbContext.ProgramCourses.Add(programCourse);
            _dbContext.SaveChanges();
        }

        public void removeProgram(int courseId, int programId)
        {
            var course = _dbContext.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course is null)
                throw new NotFoundException($"Course with id {courseId} was not found");

            var program = _dbContext
                .Programs
                .FirstOrDefault(p => p.Id == programId);
            if (program is null)
                throw new NotFoundException($"Program with id {programId} was not found");

            var programCourse = _dbContext
                .ProgramCourses
                .FirstOrDefault(pc => pc.CourseId == courseId && pc.ProgramId == programId);
            if (programCourse is null)
                throw new NotFoundException($"Course {course.Name} is not assigned to program: {program.Name}");

            _dbContext.ProgramCourses.Remove(programCourse);
            _dbContext.SaveChanges();
        }
    }
}
