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
                .ThenBy(x => x.PolishTranslation)
                .ToList();
            return _mapper.Map<List<IrregularVerbDto>>(verbs);
        }
        public List<IrregularVerbDto> GetAllIrregularVerbs()
        {
            var userId = _userContextService.GetUserId;
            if (userId is null) return new List<IrregularVerbDto>();
            EnsureDefaultVerbsExist(userId.Value, IrregularVerbLevel.Basic);
            EnsureDefaultVerbsExist(userId.Value, IrregularVerbLevel.Advanced);
            var verbs = _dbContext.IrregularVerbs
                .Where(x => x.UserId == userId.Value)
                .OrderBy(x => x.NextReviewDate)
                .ThenBy(x => x.PolishTranslation)
                .ToList();
            return _mapper.Map<List<IrregularVerbDto>>(verbs);
        }
        public List<IrregularVerbDto> GetLeeches()
        {
            var userId = _userContextService.GetUserId;
            if (userId is null) return new List<IrregularVerbDto>();
            var verbs = _dbContext.IrregularVerbs
                .Where(x => x.UserId == userId.Value && (x.IsLeech || x.EaseFactor <= 150))
                .OrderBy(x => x.EaseFactor)
                .ToList();
            return _mapper.Map<List<IrregularVerbDto>>(verbs);
        }
        public List<IrregularVerbDto> GetStudiedToday()
        {
            var userId = _userContextService.GetUserId;
            if (userId is null) return new List<IrregularVerbDto>();
            var today = PolandTime.Today;
            var verbs = _dbContext.IrregularVerbs
                .Where(x => x.UserId == userId.Value && x.LastReviewDate == today)
                .OrderByDescending(x => x.NextReviewDate)
                .ToList();
            return _mapper.Map<List<IrregularVerbDto>>(verbs);
        }
        public List<IrregularVerbDto> SearchIrregularVerbs(string query)
        {
            var userId = _userContextService.GetUserId;
            if (userId is null || string.IsNullOrWhiteSpace(query)) return new List<IrregularVerbDto>();
            var q = query.Trim().ToLower();
            var verbs = _dbContext.IrregularVerbs
                .Where(x => x.UserId == userId.Value &&
                            (x.PolishTranslation.ToLower().Contains(q) || x.EnglishForms.ToLower().Contains(q)))
                .OrderBy(x => x.PolishTranslation)
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
            card.LastReviewDate = today;
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
        public void EnsureDefaultVerbsExist(int userId, IrregularVerbLevel level)
        {
            SyncAndCleanUserVerbs(userId);
        }
        public void SyncAndCleanUserVerbs(int userId)
        {
            var existingVerbs = _dbContext.IrregularVerbs
                .Where(x => x.UserId == userId)
                .ToList();

            var basicDefaults = GetBasicDefaultVerbs();
            var advancedDefaults = GetAdvancedDefaultVerbs();

            var basicMap = basicDefaults.ToDictionary(
                v => GetBaseVerbKey(v.English),
                v => v,
                StringComparer.OrdinalIgnoreCase
            );

            var advancedMap = advancedDefaults.ToDictionary(
                v => GetBaseVerbKey(v.English),
                v => v,
                StringComparer.OrdinalIgnoreCase
            );

            bool changed = false;
            var today = PolandTime.Today;

            var grouped = existingVerbs
                .GroupBy(x => GetBaseVerbKey(x.EnglishForms))
                .ToList();

            var verbsToKeep = new List<IrregularVerb>();
            var verbsToRemove = new List<IrregularVerb>();

            foreach (var group in grouped)
            {
                var key = group.Key;
                if (string.IsNullOrEmpty(key)) continue;

                IrregularVerbLevel targetLevel;
                if (advancedMap.ContainsKey(key))
                {
                    targetLevel = IrregularVerbLevel.Advanced;
                }
                else if (basicMap.ContainsKey(key))
                {
                    targetLevel = IrregularVerbLevel.Basic;
                }
                else
                {
                    verbsToKeep.Add(group.First());
                    verbsToRemove.AddRange(group.Skip(1));
                    continue;
                }

                var best = group
                    .OrderByDescending(x => x.Interval)
                    .ThenByDescending(x => x.EaseFactor)
                    .First();

                if (best.Level != targetLevel)
                {
                    best.Level = targetLevel;
                    changed = true;
                }

                verbsToKeep.Add(best);

                var duplicates = group.Where(x => x.Id != best.Id).ToList();
                if (duplicates.Count > 0)
                {
                    verbsToRemove.AddRange(duplicates);
                    changed = true;
                }
            }

            if (verbsToRemove.Count > 0)
            {
                _dbContext.IrregularVerbs.RemoveRange(verbsToRemove);
                changed = true;
            }

            var existingKeys = verbsToKeep
                .Select(x => GetBaseVerbKey(x.EnglishForms))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var newEntities = new List<IrregularVerb>();

            foreach (var b in basicDefaults)
            {
                var key = GetBaseVerbKey(b.English);
                if (!existingKeys.Contains(key))
                {
                    newEntities.Add(new IrregularVerb
                    {
                        UserId = userId,
                        PolishTranslation = b.Polish,
                        EnglishForms = b.English,
                        Level = IrregularVerbLevel.Basic,
                        EaseFactor = 250,
                        Interval = 0,
                        IsLeech = false,
                        NextReviewDate = today
                    });
                    changed = true;
                }
            }

            foreach (var a in advancedDefaults)
            {
                var key = GetBaseVerbKey(a.English);
                if (!existingKeys.Contains(key))
                {
                    newEntities.Add(new IrregularVerb
                    {
                        UserId = userId,
                        PolishTranslation = a.Polish,
                        EnglishForms = a.English,
                        Level = IrregularVerbLevel.Advanced,
                        EaseFactor = 250,
                        Interval = 0,
                        IsLeech = false,
                        NextReviewDate = today
                    });
                    changed = true;
                }
            }

            if (newEntities.Count > 0)
            {
                _dbContext.IrregularVerbs.AddRange(newEntities);
            }

            if (changed)
            {
                _dbContext.SaveChanges();
            }
        }
        private static string GetBaseVerbKey(string englishForms)
        {
            if (string.IsNullOrWhiteSpace(englishForms)) return string.Empty;
            return englishForms.Split(new[] { ',', '-', '/' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim().ToLower();
        }
        public static List<(string Polish, string English)> GetBasicDefaultVerbs()
        {
            return new List<(string, string)>
            {
                ("być", "be, was/were, been"),
                ("stawać się", "become, became, become"),
                ("zaczynać", "begin, began, begun"),
                ("łamać / tłuc", "break, broke, broken"),
                ("przynosić", "bring, brought, brought"),
                ("budować", "build, built, built"),
                ("kupować", "buy, bought, bought"),
                ("łapać", "catch, caught, caught"),
                ("wybierać", "choose, chose, chosen"),
                ("przychodzić", "come, came, come"),
                ("kosztować", "cost, cost, cost"),
                ("ciąć / kroić", "cut, cut, cut"),
                ("robić", "do, did, done"),
                ("rysować", "draw, drew, drawn"),
                ("wypić", "drink, drank, drunk"),
                ("jechać / prowadzić", "drive, drove, driven"),
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
                ("trzymać / zachować", "keep, kept, kept"),
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
                ("mówić (w języku)", "speak, spoke, spoken"),
                ("spędzać", "spend, spent, spent"),
                ("stać", "stand, stood, stood"),
                ("pływać", "swim, swam, swum"),
                ("brać", "take, took, taken"),
                ("uczyć kogoś", "teach, taught, taught"),
                ("rozerwać", "tear, tore, torn"),
                ("powiedzieć komuś", "tell, told, told"),
                ("myśleć", "think, thought, thought"),
                ("rzucić", "throw, threw, thrown"),
                ("zrozumieć", "understand, understood, understood"),
                ("budzić", "wake, woke, woken"),
                ("nosić (ubranie)", "wear, wore, worn"),
                ("wygrać", "win, won, won"),
                ("pisać", "write, wrote, written")
            };
        }
        public static List<(string Polish, string English)> GetAdvancedDefaultVerbs()
        {
            return new List<(string, string)>
            {
                ("powstawać, pojawiać się", "arise, arose, arisen"),
                ("budzić się, obudzić się", "awake, awoke, awoken"),
                ("znosić, wytrzymywać", "bear, bore, borne"),
                ("bić, pokonać", "beat, beat, beaten"),
                ("począć, spłodzić", "beget, begot, begotten"),
                ("oglądać, ujrzeć", "beheld, beheld, beheld"),
                ("ginać, zginać", "bend, bent, bent"),
                ("oblegać, nękać", "beset, beset, beset"),
                ("założyć się", "bet, bet, bet"),
                ("licytować, rozkazywać", "bid, bid, bid"),
                ("wiązać", "bind, bound, bound"),
                ("gryźć", "bite, bit, bitten"),
                ("krwawić", "bleed, bled, bled"),
                ("dąć, dmuchać", "blow, blew, blown"),
                ("hodować, rozmnażać", "breed, bred, bred"),
                ("nadawać (program)", "broadcast, broadcast, broadcast"),
                ("palić, płonąć", "burn, burned/burnt, burned/burnt"),
                ("pękać, wybuchać", "burst, burst, burst"),
                ("rzucać, obsadzać (w filmie)", "cast, cast, cast"),
                ("czepiać się, przylgnąć", "cling, clung, clung"),
                ("skradać się, pełzać", "creep, crept, crept"),
                ("radzić sobie, zajmować się", "deal, dealt, dealt"),
                ("kopać (w ziemi)", "dig, dug, dug"),
                ("śnić, marzyć", "dream, dreamed/dreamt, dreamed/dreamt"),
                ("mieszkać, przebywać", "dwell, dwelt, dwelt"),
                ("uciekać", "flee, fled, fled"),
                ("cisnąć, rzucać", "fling, flung, flung"),
                ("zabraniać", "forbid, forbade, forbidden"),
                ("prognozować, przewidywać", "forecast, forecast, forecast"),
                ("przewidywać", "foresee, foresaw, foreseen"),
                ("porzucać, wyrzekać się", "forsake, forsook, forsaken"),
                ("mielić, szlifować", "grind, ground, ground"),
                ("rąbać (drewno)", "hew, hewed, hewn"),
                ("klękać", "kneel, knelt, knelt"),
                ("skakać", "leap, leapt, leapt"),
                ("oświetlać, zapalać", "light, lit, lit"),
                ("wprowadzać w błąd", "mislead, misled, misled"),
                ("mylić, pomylić", "mistake, mistook, mistaken"),
                ("pokonywać, przezwyciężać", "overcome, overcame, overcome"),
                ("przesadzać, robić za dużo", "overdo, overdid, overdone"),
                ("nadzorować", "oversee, oversaw, overseen"),
                ("wyprzedzać, doganiać", "overtake, overtook, overtaken"),
                ("błagać, przyznawać winę", "plead, pled, pled"),
                ("udowadniać", "prove, proved, proven"),
                ("rezygnować, przestawać", "quit, quit, quit"),
                ("odbudowywać", "rebuild, rebuilt, rebuilt"),
                ("spłacać", "repay, repaid, repaid"),
                ("ponownie przejmować", "retake, retook, retaken"),
                ("pozbywać się, uwalniać", "rid, rid, rid"),
                ("szukać, poszukiwać", "seek, sought, sought"),
                ("szyć", "sew, sewed, sewn"),
                ("potrząsać, wstrząsać", "shake, shook, shaken"),
                ("rzucać, zrzucać", "shed, shed, shed"),
                ("świecić", "shine, shone, shone"),
                ("strzelać", "shoot, shot, shot"),
                ("kurczyć się, maleć", "shrink, shrank, shrunk"),
                ("zabijać, uśmiercać", "slay, slew, slain"),
                ("ślizgać się, przesuwać", "slide, slid, slid"),
                ("rzucać, ciskać", "sling, slung, slung"),
                ("rozciąć, przecinać", "slit, slit, slit"),
                ("siać", "sow, sowed, sown"),
                ("kręcić, obracać", "spin, spun, spun"),
                ("pluć", "spit, spat, spat"),
                ("dzielić, rozłupywać", "split, split, split"),
                ("skakać, wyskakiwać", "spring, sprang, sprung"),
                ("kraść", "steal, stole, stolen"),
                ("przyklejać, wtykać", "stick, stuck, stuck"),
                ("żądać, kłuć", "sting, stung, stung"),
                ("kroczyć", "stride, strode, stridden"),
                ("starać się, usiłować", "strive, strove, striven"),
                ("przysięgać", "swear, swore, sworn"),
                ("zamiatać", "sweep, swept, swept"),
                ("huśtać się, kołysać", "swing, swung, swung"),
                ("pchać, wpychać", "thrust, thrust, thrust"),
                ("stąpać, deptać", "tread, trod, trodden"),
                ("przechodzić przez, doświadczać", "undergo, underwent, undergone"),
                ("podejmować się", "undertake, undertook, undertaken"),
                ("tkać", "weave, wove, woven"),
                ("opierać się, wytrzymywać", "withstand, withstood, withstood")
            };
        }
    }
}
