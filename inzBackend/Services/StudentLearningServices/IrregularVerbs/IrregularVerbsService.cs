using AutoMapper;
using inzBackend.Entities.LearningMaterials;
using inzBackend.Enums;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.StudentLearningModels.IrregularVerbModels;
using inzBackend.Services.AdminLearningServices.LessonPanel;
using inzBackend.Services.CreditServices;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.StudentLearningServices.IrregularVerbs
{
    public class IrregularVerbsService : IIrregularVerbsService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;
        private readonly ILessonPanelService _lessonPanelService;
        private readonly ICreditService _creditService;

        public IrregularVerbsService(
            GmitrzakEnglishAcademyDbContext dbContext,
            IUserContextService userContextService,
            IMapper mapper,
            ILessonPanelService lessonPanelService,
            ICreditService creditService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _mapper = mapper;
            _lessonPanelService = lessonPanelService;
            _creditService = creditService;
        }

        public List<IrregularVerbDto> GetIrregularVerbsByLevel(IrregularVerbLevel level)
        {
            var userId = _userContextService.GetUserId;
            if (userId is null) return new List<IrregularVerbDto>();

            EnsureDefaultVerbsExist(userId.Value, level);

            var verbs = _dbContext.IrregularVerbs
                .Where(x => x.UserId == userId.Value && x.Level == level)
                .OrderBy(x => x.NextReviewDate)
                .ToList();

            return _mapper.Map<List<IrregularVerbDto>>(verbs);
        }

        public void ReviewIrregularVerb(int id, ReviewIrregularVerbRequest request)
        {
            var userId = _userContextService.GetUserId;
            if (userId is null) return;

            var card = _dbContext.IrregularVerbs
                .FirstOrDefault(x => x.Id == id && x.UserId == userId.Value);

            if (card is null) return;

            var today = PolandTime.Today;

            switch (request.Quality.ToLower())
            {
                case "easy":
                    card.Interval = card.Interval == 0 ? 2 : card.Interval * 2;
                    card.NextReviewDate = today.AddDays(card.Interval);
                    card.EaseFactor = Math.Min(card.EaseFactor + 10, 300);
                    break;

                case "hard":
                    card.Interval = 1;
                    card.NextReviewDate = today.AddDays(1);
                    card.EaseFactor = Math.Max(card.EaseFactor - 15, 130);
                    break;

                case "incorrect":
                case "again_1m":
                    card.Interval = 0;
                    card.NextReviewDate = today;
                    card.EaseFactor = Math.Max(card.EaseFactor - 20, 130);
                    break;
            }

            card.IsLeech = card.EaseFactor <= 150;

            _lessonPanelService.AddActivityPoints(userId.Value, 2, "Irregular verb flashcard done");
            _creditService.CheckAndAwardDailyChallenge(userId.Value);
            _creditService.CheckAndAwardWeeklyChallenge(userId.Value);

            _dbContext.SaveChanges();
        }

        private void EnsureDefaultVerbsExist(int userId, IrregularVerbLevel level)
        {
            bool exists = _dbContext.IrregularVerbs.Any(x => x.UserId == userId && x.Level == level);
            if (exists) return;

            List<(string Polish, string English)> seedData = level == IrregularVerbLevel.Basic
                ? GetBasicDefaultVerbs()
                : GetAdvancedDefaultVerbs();

            var today = PolandTime.Today;
            var entities = seedData.Select(item => new IrregularVerb
            {
                UserId = userId,
                PolishTranslation = item.Polish,
                EnglishForms = item.English,
                Level = level,
                EaseFactor = 250,
                Interval = 0,
                IsLeech = false,
                NextReviewDate = today
            }).ToList();

            _dbContext.IrregularVerbs.AddRange(entities);
            _dbContext.SaveChanges();
        }

        private static List<(string Polish, string English)> GetBasicDefaultVerbs()
        {
            return new List<(string, string)>
            {
                ("jechać", "drive, drove, driven"),
                ("wypić", "drink, drank, drunk"),
                ("jeść", "eat, ate, eaten"),
                ("spadać", "fall, fell, fallen"),
                ("czuć", "feel, felt, felt"),
                ("walczyć", "fight, fought, fought"),
                ("znaleźć", "find, found, found"),
                ("latać", "fly, flew, flown"),
                ("zapomnieć", "forget, forgot, forgotten"),
                ("przebaczyć", "forgive, forgave, forgiven"),
                ("zamarznąć", "freeze, froze, frozen"),
                ("dostać", "get, got, got"),
                ("dać", "give, gave, given"),
                ("iść", "go, went, gone"),
                ("rosnąć", "grow, grew, grown"),
                ("powiesić", "hang, hung, hung"),
                ("mieć", "have, had, had"),
                ("usłyszeć", "hear, heard, heard"),
                ("chować", "hide, hid, hidden"),
                ("uderzyć", "hit, hit, hit"),
                ("trzymać", "hold, held, held"),
                ("boleć", "hurt, hurt, hurt"),
                ("trzymać", "keep, kept, kept"),
                ("wiedzieć", "know, knew, known"),
                ("kłaść", "lay, laid, laid"),
                ("prowadzić", "lead, led, led"),
                ("uczyć się", "learn, learnt, learnt"),
                ("opuścić", "leave, left, left"),
                ("pożyczać komuś", "lend, lent, lent"),
                ("pozwalać", "let, let, let"),
                ("leżeć", "lie, lay, lain"),
                ("stracić", "lose, lost, lost"),
                ("zrobić", "make, made, made"),
                ("oznaczać", "mean, meant, meant"),
                ("spotkać się", "meet, met, met"),
                ("zapłacić", "pay, paid, paid"),
                ("umieścić", "put, put, put"),
                ("czytać", "read, read, read"),
                ("jeździć", "ride, rode, ridden"),
                ("dzwonić", "ring, rang, rung"),
                ("wzrastać", "rise, rose, risen"),
                ("biec", "run, ran, run"),
                ("mówić", "say, said, said"),
                ("widzieć", "see, saw, seen"),
                ("sprzedać", "sell, sold, sold"),
                ("wysłać", "send, sent, sent"),
                ("pokazywać", "show, showed, shown"),
                ("zamknąć", "shut, shut, shut"),
                ("śpiewać", "sing, sang, sung"),
                ("tonąć", "sink, sank, sunk"),
                ("siedzieć", "sit, sat, sat"),
                ("spać", "sleep, slept, slept"),
                ("mówić", "speak, spoke, spoken"),
                ("spędzać", "spend, spent, spent"),
                ("stać", "stand, stood, stood"),
                ("śmierdzieć", "stink, stank, stunk"),
                ("pływać", "swim, swam, swum"),
                ("brać", "take, took, taken"),
                ("uczyć kogoś", "teach, taught, taught"),
                ("rozerwać", "tear, tore, torn"),
                ("powiedzieć", "tell, told, told"),
                ("myśleć", "think, thought, thought"),
                ("rzucić", "throw, threw, thrown"),
                ("zrozumieć", "understand, understood, understood"),
                ("budzić", "wake, woke, woken"),
                ("nosić", "wear, wore, worn"),
                ("wygrać", "win, won, won"),
                ("pisać", "write, wrote, written")
            };
        }

        private static List<(string Polish, string English)> GetAdvancedDefaultVerbs()
        {
            return new List<(string, string)>
            {
                ("powstawać, pojawiać się", "arise, arose, arisen"),
                ("budzić się, obudzić się", "awake, awoke, awoken"),
                ("znosić, wytrzymywać", "bear, bore, borne"),
                ("począć, spłodzić", "beget, begot, begotten"),
                ("oglądać, ujrzeć", "beheld, beheld, beheld"),
                ("oblegać, nękać", "beset, beset, beset"),
                ("wiązać", "bind, bound, bound"),
                ("hodować, rozmnażać", "breed, bred, bred"),
                ("rzucać (np. zaklęcie); obsadzać (kogoś w filmie)", "cast, cast, cast"),
                ("czepiać się, przylgnąć", "cling, clung, clung"),
                ("skradać się, pełzać", "creep, crept, crept"),
                ("radzić sobie, zajmować się", "deal, dealt, dealt"),
                ("mieszkać, przebywać", "dwell, dwelt, dwelt"),
                ("uciekać", "flee, fled, fled"),
                ("cisnąć, rzucać", "fling, flung, flung"),
                ("zabraniać", "forbid, forbade, forbidden"),
                ("prognozować, przewidywać", "forecast, forecast, forecast"),
                ("przewidywać (nie: predict)", "foresee, foresaw, foreseen"),
                ("porzucać, wyrzekać się", "forsake, forsook, forsaken"),
                ("marznąć, zamrażać", "freeze, froze, frozen"),
                ("mielić, szlifować", "grind, ground, ground"),
                ("rosnąć, uprawiać", "grow, grew, grown"),
                ("wieszać, zawieszać", "hang, hung, hung"),
                ("rąbać (drewno)", "hew, hewed, hewn"),
                ("ukrywać", "hide, hid, hidden"),
                ("klękać", "kneel, knelt, knelt"),
                ("kłaść, odkładać", "lay, laid, laid"),
                ("prowadzić, kierować (np. spotkanie)", "lead, led, led"),
                ("skakać (nie: jump)", "leap, leapt, leapt"),
                ("leżeć", "lie, lay, lain"),
                ("oświetlać, zapalać", "light, lit, lit"),
                ("wprowadzać w błąd", "mislead, misled, misled"),
                ("mylić, pomylić", "mistake, mistook, mistaken"),
                ("pokonywać, przezwyciężać", "overcome, overcame, overcome"),
                ("przesadzać, robić za dużo", "overdo, overdid, overdone"),
                ("nadzorować", "oversee, oversaw, overseen"),
                ("wyprzedzać, doganiać", "overtake, overtook, overtaken"),
                ("błagać, przyznawać (winę)", "plead, pled, pled"),
                ("udowadniać", "prove, proved, proven"),
                ("rezygnować, przestawać", "quit, quit, quit"),
                ("odbudowywać", "rebuild, rebuilt, rebuilt"),
                ("spłacać", "repay, repaid, repaid"),
                ("ponownie przejmować, odzyskiwać", "retake, retook, retaken"),
                ("pozbywać się, uwalniać", "rid, rid, rid"),
                ("dzwonić, dzwonić telefonem", "ring, rang, rung"),
                ("szukać, poszukiwać", "seek, sought, sought"),
                ("szyć", "sew, sewed, sewn"),
                ("potrząsać, wstrząsać", "shake, shook, shaken"),
                ("rzucać, zrzucać", "shed, shed, shed"),
                ("kurczyć się, maleć", "shrink, shrank, shrunk"),
                ("tonąć, zatapiać", "sink, sank, sunk"),
                ("zabijać, uśmiercać", "slay, slew, slain"),
                ("ślizgać się, przesuwać", "slide, slid, slid"),
                ("rzucać, ciskać", "sling, slung, slung"),
                ("rozciąć, przecinać", "slit, slit, slit"),
                ("siać", "sow, sowed, sown"),
                ("kręcić, obracać", "spin, spun, spun"),
                ("pluć", "spit, spat, spat"),
                ("dzielić, rozłupywać", "split, split, split"),
                ("skakać, wyskakiwać", "spring, sprang, sprung"),
                ("żądać, kłuć", "sting, stung, stung"),
                ("śmierdzieć", "stink, stank, stunk"),
                ("kroczyć", "stride, strode, stridden"),
                ("starać się, usiłować", "strive, strove, striven"),
                ("przysięgać", "swear, swore, sworn"),
                ("zamiatać", "sweep, swept, swept"),
                ("huśtać się, kołysać", "swing, swung, swung"),
                ("pchać, wpychać", "thrust, thrust, thrust"),
                ("stąpać, deptać", "tread, trod, trodden"),
                ("przechodzić przez, doświadczać", "undergo, underwent, undergone"),
                ("podejmować się, zobowiązywać się", "undertake, undertook, undertaken"),
                ("tkać", "weave, wove, woven"),
                ("opierać się, wytrzymywać", "withstand, withstood, withstood")
            };
        }
    }
}
