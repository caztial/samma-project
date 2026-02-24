# Active Context

## Current Phase: Question Domain Enhancements Complete ✅

Recent work completed on the Question aggregate:
1. **Tag Refactoring** - Changed from 1:N to M:N (many-to-many) relationship
2. **Delete Question Endpoint** - Added for Admin/Moderator roles
3. **Question Number Field** - Added mandatory `Number` field (string, max 50 chars)

### Question Number Field Implementation (Feb 25, 2026)

Added a mandatory `Number` field to the Question aggregate root for question identification.

#### Entity Changes
```
Question (Abstract Aggregate Root)
├── Number: string (required, max 50 chars)  <-- NEW
├── Text: string
├── Description: string?
├── DurationSeconds: int?
├── MediaMetadatas: ICollection<MediaMetadata>
├── QuestionTags: ICollection<QuestionTag>
├── CreatedBy: string
├── IsVerified: bool
└── QuestionType: string
```

#### API Changes
- **CreateMCQQuestionRequest**: Added `Number` (required)
- **UpdateMCQQuestionRequest**: Added `Number` (optional)
- **QuestionResponse/MCQQuestionResponse**: Added `Number` field

#### Search Enhancement
- `SearchText` parameter now searches both `Text` and `Number` fields
- Example: Searching "Q1" will find questions with number "Q1"

#### Validation
- `Number` is required on create (non-empty, max 50 characters)
- `Number` cannot be empty when provided on update

## Recent Changes (Feb 24, 2026)

### Tag Refactoring (M:N Relationship)

Tags are now **reusable** across questions via a join table.

#### Entity Structure
```
Tag : BaseEntity
├── Name: string
├── NormalizedName: string (unique, lowercase)
├── UsageCount: int (popularity tracking)
└── QuestionTags: ICollection<QuestionTag>

QuestionTag : BaseEntity (Join Table)
├── QuestionId: Guid
├── TagId: Guid
├── Question: Question?
└── Tag: Tag?

Question
└── QuestionTags: ICollection<QuestionTag> (changed from Tags)
```

#### New Services
- **ITagRepository / TagRepository** - Tag data access
- **ITagService / TagService** - Tag business logic with find-or-create pattern

#### New/Updated Endpoints

| Endpoint | Method | Route | Description |
|----------|--------|-------|-------------|
| SearchTagsEndpoint | GET | `/tags?search={text}` | Top 10 matching tags (typeahead) |
| AddTagEndpoint | POST | `/questions/{QuestionId}/tags` | Add tag by name (reuse if exists) |
| RemoveTagEndpoint | DELETE | `/questions/{QuestionId}/tags/{TagId}` | Remove tag from question only |
| DeleteQuestionEndpoint | DELETE | `/questions/{Id}` | Delete question (Admin/Moderator) |

#### Key Behavior
- Adding tag by name: Creates new tag if not exists (case-insensitive), otherwise reuses
- Removing tag: Only removes QuestionTag association, preserves Tag entity
- Tag search: Returns top 10 results ordered by UsageCount (popularity)

### Question Hierarchy Implementation

#### Class Structure
```
Question (Abstract Aggregate Root)
├── Text: string
├── Description: string?
├── DurationSeconds: int?
├── MediaMetadatas: ICollection<MediaMetadata> (owned)
├── QuestionTags: ICollection<QuestionTag> (M:N)
├── CreatedBy: string
├── IsVerified: bool
├── QuestionType: string (protected set)
└── AddMedia(MediaMetadata)

McqQuestion : Question
├── AnswerOptions: ICollection<McqAnswerOption>
├── AddAnswerOption(text, order, points, isCorrect)
└── ValidateMCQ() - validates exactly one correct answer
```

### Service Architecture

**IQuestionService** (Generic):
- `GetByIdAsync`, `GetAllAsync`
- `DeleteAsync`
- `AddTagAsync`, `RemoveTagAsync`, `GetAllTagsAsync`, `GetByTagAsync`
- `AddMediaAsync`, `UpdateMediaAsync`, `RemoveMediaAsync`

**IMcqQuestionService** (MCQ-specific):
- `CreateAsync` - Create MCQ with answer options
- `GetByIdAsync`, `UpdateAsync`
- `AddAnswerOptionAsync`, `RemoveAnswerOptionAsync`, `UpdateAnswerOptionAsync`

**ITagService** (Tag operations):
- `SearchAsync` - Typeahead search (top 10)
- `AddTagToQuestionAsync` - Find or create tag, add to question
- `RemoveTagFromQuestionAsync` - Remove association only

### Database Schema (TPT)

```
Questions Table (Base)
├── Id (PK)
├── Text
├── Description
├── DurationSeconds
├── CreatedBy
├── IsVerified
├── QuestionType
├── CreatedAt
└── UpdatedAt

MCQQuestions Table (Inherits from Questions)
└── Id (PK, FK to Questions.Id)

AnswerOptions Table
├── Id (PK)
├── McqQuestionId (FK)
├── Text
├── Order
├── Points
├── IsCorrect
├── CreatedAt
└── UpdatedAt

Tags Table (Reusable)
├── Id (PK)
├── Name
├── NormalizedName (Unique Index)
├── UsageCount
├── CreatedAt
└── UpdatedAt

QuestionTags Table (Join Table)
├── Id (PK)
├── QuestionId (FK)
├── TagId (FK)
├── CreatedAt
└── UpdatedAt
└── Unique Index (QuestionId, TagId)
```

## Key Design Decisions

1. **M:N Tags**: Tags are now reusable entities with usage count for popularity
2. **Tag Search**: Typeahead behavior with top 10 results, ordered by popularity
3. **Abstract Question Class**: Question is abstract, cannot be instantiated directly
4. **QuestionType Property**: Set by derived classes in constructor (e.g., "MCQ")
5. **Separated Services**: IMcqQuestionService for create/update, IQuestionService for generic operations

## Running Services
- API: http://localhost:8080 (Docker)
- PostgreSQL: localhost:5432

## Next Steps
1. Add more question types (TrueFalseQuestion, ShortAnswerQuestion)
2. Session Management domain
3. Real-time Q&A with SignalR