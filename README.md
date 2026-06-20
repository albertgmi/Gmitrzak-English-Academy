<div align="center">

# 🎓 Gmitrzak English Academy

### Production-grade EdTech SaaS backend for managing a language school

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Database-336791?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Containerized-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![EF Core](https://img.shields.io/badge/EF%20Core-8.0-512BD4)](https://docs.microsoft.com/ef/core/)

</div>

---

## 📖 What is this?

**Gmitrzak English Academy** is the backend of a full SaaS platform built to run a real, operating language school — replacing spreadsheets and manual tracking with a role-isolated **Student / Admin** system that handles enrollment, lesson delivery, spaced-repetition vocabulary training, AI-assisted writing & pronunciation feedback, gamification, and automated reporting.

It's not a tutorial CRUD app. It's a system designed under real constraints: real users, real data integrity requirements, and real operational needs (financial/occupancy-style reporting, bulk data import, file generation, third-party AI services with latency and cost trade-offs).

> Built and maintained solo, end-to-end - from data modeling to deployment

## 🧱 Architecture

The backend follows a clean, layered architecture with strict separation of concerns:

```
inzBackend/
├── Controllers/        # 24 REST controllers - thin, delegate to services
├── Services/           # ~30 interface-backed services - business logic lives here
│   ├── AiIntegrationServices/    # Groq (LLM) + Gemini (multimodal) integrations
│   ├── StudentLearningServices/  # Flashcards, SRS, vocabulary, pronunciation
│   ├── ReportServices/           # PDF/Excel/ZIP report generation
│   └── ...
├── Entities/            # 48 entity classes mapped via EF Core (Npgsql/PostgreSQL)
│   ├── Identity/         # Users, profiles, login logs
│   ├── Curriculum/        # Programs, courses, modules, lessons
│   ├── SpacedRepetition/  # Flashcards, memory items, study logs
│   ├── Gamification/      # Credits, ranking, shop, streaks
│   └── ...
├── Middlewares/         # Centralized exception handling → consistent error contract
├── Profiles/            # AutoMapper entity ↔ DTO mappings
├── Migrations/           # EF Core migration history
└── Program.cs            # Composition root: DI, JWT auth, CORS, Swagger, middleware pipeline
```

**Key architectural decisions:**

- **Interface-first services** - every service is registered against an interface (`IFlashcardsService`, `IEssayService`, `IPronunciationService`...), keeping controllers thin and the codebase testable/mockable.
- **Centralized exception handling** - a single `ExceptionHandlingMiddleware` translates domain exceptions (`NotFoundException`, `BadRequestException`) into consistent HTTP responses, instead of scattering try/catch across controllers.
- **JWT Bearer auth with role-based access control** - `[Authorize(Roles = "Admin")]` / `"User"` gates every sensitive endpoint, with a fully separate Student vs. Admin experience on the frontend.
- **Auto-applied migrations on startup** - the app checks for and applies pending EF Core migrations on boot, so a fresh container is always schema-consistent without a manual step.
- **Config-driven external integrations** - Groq, Gemini, and Cloudinary clients are wired through `IConfiguration` and registered once in the composition root, not hardcoded anywhere in business logic.

## ✨ Standout features

**🤖 Multimodal AI pronunciation scoring** - students record themselves saying a target word; the audio is streamed directly (as raw bytes, no intermediate transcription step) to Gemini's multimodal API alongside a tightly engineered phoneme-level grading prompt, and the structured JSON verdict (score, pass/fail, specific phonetic feedback) is parsed and persisted per attempt.

**🧠 Custom Spaced Repetition System with automatic "leech" detection** - flashcard review intervals and ease factors evolve per answer (correct → interval doubles, ease factor rises; incorrect → interval resets, ease factor drops), inspired by SM-2 but tuned for the platform's content. Cards that a student keeps failing are automatically flagged as **leeches** (`EaseFactor <= 150`) and surfaced separately, so weak vocabulary doesn't silently get stuck in an endless review loop.

**✍️ AI-assisted essay grading & sentence checking** - Groq-backed (Llama 3.3 70B via an OpenAI-compatible client) pipelines evaluate free-text student writing, giving structured, automatable feedback instead of requiring a teacher to manually grade every submission.

**📊 Automated reporting pipeline** - admins can export module/course data as PDF (via QuestPDF) and bulk-import or export structured data via Excel (ClosedXML), with multi-file results streamed back as ZIP archives — no manual spreadsheet wrangling.

**🎮 Gamification layer** - a credits/points economy, a ranking system with reactions, a content "shop," and streak shields to encourage daily engagement without resorting to dark patterns.

**🔐 Defense in depth on auth** - JWT issuance and validation, password hashing via `IPasswordHasher`, request validation via FluentValidation, and role checks enforced server-side on every protected route (never trusted from the client).

## 🛠 Tech Stack

| Layer | Technology |
|---|---|
| **Runtime / Framework** | .NET 8, ASP.NET Core Web API |
| **Database** | PostgreSQL, Entity Framework Core 8 (Npgsql) |
| **Auth** | JWT Bearer tokens, ASP.NET Core Identity password hashing, Role-Based Access Control |
| **Validation & Mapping** | FluentValidation, AutoMapper |
| **AI / LLM Integration** | Groq API (Llama 3.3 70B, OpenAI-compatible client), Google Gemini (multimodal audio scoring) |
| **File generation** | QuestPDF (PDF reports), ClosedXML (Excel import/export) |
| **Media storage** | Cloudinary |
| **API docs** | Swagger / Swashbuckle |
| **Logging** | NLog |
| **Containerization** | Docker, Docker Compose |
| **Deployment** | Railway (API), Vercel (frontend) |

## 📐 By the numbers

- **48** entity classes mapped to a relational PostgreSQL schema
- **24** REST controllers / **~30** dedicated service classes
- **12** sequential EF Core migrations tracking schema evolution from scratch
- **2** isolated user experiences (Student / Admin) sharing one codebase
- **3** external AI/media services orchestrated server-side (Groq, Gemini, Cloudinary)

## 🏁 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/) & Docker Compose
- PostgreSQL (or use the bundled Docker Compose service)

### Run with Docker Compose (recommended)

```bash
git clone https://github.com/albertgmi/Gmitrzak-English-Academy.git
cd Gmitrzak-English-Academy

# Configure secrets locally - see "Configuration" below
docker compose up --build
```

The API will be available at `http://localhost:8080`, with Swagger UI at `/swagger` in Development mode.

### Run locally with the .NET CLI

```bash
cd inzBackend
dotnet restore
dotnet user-secrets set "ConnectionStrings:GmitrzakEnglishAppConnectionString" "Host=localhost;Port=5432;Database=inz_database;Username=postgres;Password=yourpassword"
dotnet user-secrets set "Authentication:JwtKey" "your-local-dev-secret-min-64-chars"
dotnet run
```

### ⚙️ Configuration

This project keeps secrets out of source control via `dotnet user-secrets` / environment variables. You'll need to provide your own values for:

| Setting | Purpose |
|---|---|
| `ConnectionStrings:GmitrzakEnglishAppConnectionString` | PostgreSQL connection |
| `Authentication:JwtKey` | JWT signing key (min. 64 chars) |
| `GroqSettings:ApiKey` | Groq API key (essay grading, sentence checking) |
| `GeminiSettings:ApiKey` | Google Gemini API key (pronunciation scoring) |
| `CloudinarySettings:CloudName / ApiKey / ApiSecret` | Media storage |

> **Note:** `appsettings.json` ships with placeholder values only — none of the above will work out of the box without your own keys.
