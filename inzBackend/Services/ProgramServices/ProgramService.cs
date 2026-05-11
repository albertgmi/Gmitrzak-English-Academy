using AutoMapper;
using inzBackend.Exceptions;
using inzBackend.Models;
using inzBackend.Models.ProgramModels;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.ProgramServices
{
    public class ProgramService : IProgramService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProgramService(GmitrzakEnglishAcademyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public List<ProgramDto> getAllPrograms()
        {
            var programs = _dbContext
                .Programs
                .Include(p => p.ProgramCourses)
                        .ThenInclude(pc => pc.Course)
                .ToList();

            var dtos = _mapper.Map<List<ProgramDto>>(programs);
            return dtos;
        }

        public void updateProgram(int programId, UpdateProgramRequest request)
        {
            var program = _dbContext
                .Programs
                .FirstOrDefault(p => p.Id == programId);
            if (program is null)
                throw new NotFoundException($"Program with id {programId} was not found");

            program.Description = request.Description;
            program.Name = request.Name;
            program.IsHidden = request.isHidden;
            _dbContext.SaveChanges();
        }

        public void deleteProgram(int programId)
        {
            var program = _dbContext
                .Programs
                .FirstOrDefault(p => p.Id == programId);
            if (program is null)
                throw new NotFoundException($"Program with id {programId} was not found");

            _dbContext.Programs.Remove(program);
            _dbContext.SaveChanges();
        }

        public Models.Program createProgram(CreateProgramRequest request)
        {
            var newProgram = new Models.Program
            {
                Name = request.Name,
                Description = request.Description,
                IsHidden = request.IsHidden
            };
            _dbContext.Programs.Add(newProgram);
            _dbContext.SaveChanges();
            return newProgram;
        }
    }
}
