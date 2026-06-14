using AutoMapper;
using inzBackend.Models.CourseModels;
using inzBackend.Models.MatrixModels;
using inzBackend.Models.ModuleModels;
using inzBackend.Models.ProfileModels;
using inzBackend.Models.ProgramModels;
using inzBackend.Models.StudentCourseModels;
using inzBackend.Models.StudentLearningModels.AssignmentStudentModels;
using inzBackend.Models.StudentLearningModels.FlashcardModels;
using inzBackend.Models.StudentLearningModels.MemoryModels;
using inzBackend.Models.StudentLearningModels.PronunciationEntryModels;
using inzBackend.Models.StudentLearningModels.SentenceModels;
using inzBackend.Models.GlobalVocabularyModels;
using inzBackend.Models.UserModels;
using inzBackend.Models.StudentLearningModels.VocabularyModels;
using inzBackend.Models.CatalogueModels;
using inzBackend.Entities.Identity;
using inzBackend.Entities.Curriculum;
using inzBackend.Entities.LearningMaterials;
using inzBackend.Entities.SpacedRepetition;
using inzBackend.Entities.Assignments;
using inzBackend.Entities.Resources;
using inzBackend.Entities.Gamification;

namespace inzBackend.Profiles
{
    public class GmitrzakEnglishAppMappingProfile : AutoMapper.Profile
    {
        public GmitrzakEnglishAppMappingProfile()
        {
            CreateMap<AppUser, AppUserDto>()
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Profile != null ? src.Profile.AvatarUrl : null));

            CreateMap<Entities.Identity.Profile, ProfileDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.User.Role.ToString()))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.User.IsActive))
                .ForMember(dest => dest.EnglishLevel, opt => opt.MapFrom(src => src.EnglishLevel.ToString()));

            CreateMap<Entities.Curriculum.Program, ProgramDto>()
                .ForMember(dest => dest.CourseDtos, opt => opt.MapFrom(src =>
                    src.ProgramCourses.Select(pc => pc.Course)));

            CreateMap<Entities.Curriculum.Program, ProgramSimpleDto>();
            CreateMap<Module, ModuleSimpleDto>();
            CreateMap<Course, CourseSimpleDto>();
            CreateMap<Matrix, MatrixSimpleDto>();

            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.MatrixDtos, opt => opt.MapFrom(src =>
                    src.CourseMatrices.Select(cm => cm.Matrix)))
                .ForMember(dest => dest.Programs, opt => opt.MapFrom(src =>
                    src.ProgramCourses.Select(pc => pc.Program)));

            CreateMap<Matrix, MatrixDto>()
                .ForMember(dest => dest.Modules, opt => opt.MapFrom(src =>
                    src.MatrixModules.Select(mm => mm.Module)))
                .ForMember(dest => dest.Courses, opt => opt.MapFrom(src =>
                    src.CourseMatrices.Select(cm => cm.Course)));

            CreateMap<Module, ModuleDto>()
                .ForMember(dest => dest.Matrices, opt => opt.MapFrom(src =>
                    src.MatrixModules.Select(mm => mm.Matrix)));

            CreateMap<Grade, GradeDto>();
            CreateMap<Sentence, SentenceDto>();
            CreateMap<Flashcard, FlashcardDto>()
                .ForMember(dest => dest.Front, opt => opt.MapFrom(src =>
                    src.Vocabulary != null ? src.Vocabulary.Front : string.Empty))
                .ForMember(dest => dest.Back, opt => opt.MapFrom(src =>
                    src.Vocabulary != null ? src.Vocabulary.Back : string.Empty))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
                    src.Vocabulary != null ? src.Vocabulary.Category : string.Empty));

            CreateMap<Memory, MemoryDto>();
            CreateMap<PronunciationEntry, PronunciationEntryDto>();

            CreateMap<UserModuleAssignment, AssignmentStudentDto>()
                .ForMember(dest => dest.ModuleId, opt => opt.MapFrom(src => src.ModuleId))
                .ForMember(dest => dest.ModuleName, opt => opt.MapFrom(src => src.Module.Name))
                .ForMember(dest => dest.ModuleDescription, opt => opt.MapFrom(src => src.Module.Description))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Module.Category));

            CreateMap<FlashcardStudyLog, FlashcardStudyLogDto>()
                .ForMember(dest => dest.FlashcardFront, opt => opt.MapFrom(src =>
                    src.Flashcard.Vocabulary != null ? src.Flashcard.Vocabulary.Front : string.Empty));

            CreateMap<Vocabulary, VocabularyDto>();
            CreateMap<Vocabulary, GlobalVocabularyDto>();

            CreateMap<Catalogue, CatalogueDto>()
                .ForMember(dest => dest.UploadedBy, opt => opt.MapFrom(src =>
                    src.UploadedBy != null ? src.UploadedBy.Username : string.Empty))
                .ForMember(dest => dest.EntryCount, opt => opt.MapFrom(src =>
                    src.Entries != null ? src.Entries.Count() : 0));
        }
    }
}