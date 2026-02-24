using API.DTOs.Questions;
using FastEndpoints;
using FluentValidation;

namespace API.Validators;

/// <summary>
/// Validator for CreateMCQQuestionRequest.
/// </summary>
public class CreateMCQQuestionRequestValidator : Validator<CreateMCQQuestionRequest>
{
    public CreateMCQQuestionRequestValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty()
            .WithMessage("Question number is required")
            .MaximumLength(50)
            .WithMessage("Question number must not exceed 50 characters");

        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Question text is required")
            .MaximumLength(2000)
            .WithMessage("Question text must not exceed 2000 characters");

        RuleFor(x => x.Description)
            .MaximumLength(5000)
            .WithMessage("Description must not exceed 5000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.DurationSeconds)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than 0 seconds")
            .LessThanOrEqualTo(3600)
            .WithMessage("Duration must not exceed 3600 seconds (1 hour)")
            .When(x => x.DurationSeconds.HasValue);

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count <= 10)
            .WithMessage("A question cannot have more than 10 tags");

        RuleForEach(x => x.Tags)
            .NotEmpty()
            .WithMessage("Tag name cannot be empty")
            .MaximumLength(100)
            .WithMessage("Tag name must not exceed 100 characters")
            .When(x => x.Tags != null);

        RuleFor(x => x.AnswerOptions)
            .Must(options => options == null || options.Count >= 2)
            .WithMessage("MCQ questions must have at least 2 answer options")
            .Must(options => options == null || options.Count <= 10)
            .WithMessage("MCQ questions cannot have more than 10 answer options");

        RuleForEach(x => x.AnswerOptions)
            .ChildRules(option =>
            {
                option.RuleFor(o => o.Text)
                    .NotEmpty()
                    .WithMessage("Answer option text is required")
                    .MaximumLength(1000)
                    .WithMessage("Answer option text must not exceed 1000 characters");

                option.RuleFor(o => o.Points)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Points must be non-negative");
            })
            .When(x => x.AnswerOptions != null);

        // Validate MCQ has exactly one correct answer
        RuleFor(x => x.AnswerOptions)
            .Must(options => options != null && options.Count(o => o.IsCorrect) == 1)
            .WithMessage("MCQ questions must have exactly one correct answer")
            .When(x => x.AnswerOptions != null && x.AnswerOptions.Any());

        // Validate media metadata collection if provided
        RuleFor(x => x.MediaMetadatas)
            .Must(media => media == null || media.Count <= 5)
            .WithMessage("A question cannot have more than 5 media attachments");

        RuleForEach(x => x.MediaMetadatas)
            .ChildRules(media =>
            {
                media.RuleFor(m => m.MediaType)
                    .Must(BeValidMediaType)
                    .WithMessage("Media type must be 'Audio' or 'Video'");

                media.RuleFor(m => m.Url)
                    .NotEmpty()
                    .WithMessage("Media URL is required when media is provided")
                    .MaximumLength(2000)
                    .WithMessage("Media URL must not exceed 2000 characters");
            })
            .When(x => x.MediaMetadatas != null);
    }

    private bool BeValidMediaType(string? mediaType)
    {
        return mediaType?.Equals("Audio", StringComparison.OrdinalIgnoreCase) == true
            || mediaType?.Equals("Video", StringComparison.OrdinalIgnoreCase) == true;
    }
}

/// <summary>
/// Validator for UpdateMCQQuestionRequest.
/// </summary>
public class UpdateMCQQuestionRequestValidator : Validator<UpdateMCQQuestionRequest>
{
    public UpdateMCQQuestionRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Question ID is required");

        RuleFor(x => x.Number)
            .NotEmpty()
            .WithMessage("Question number cannot be empty when provided")
            .MaximumLength(50)
            .WithMessage("Question number must not exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.Number));

        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Question text cannot be empty when provided")
            .MaximumLength(2000)
            .WithMessage("Question text must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Text));

        RuleFor(x => x.Description)
            .MaximumLength(5000)
            .WithMessage("Description must not exceed 5000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.DurationSeconds)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than 0 seconds")
            .LessThanOrEqualTo(3600)
            .WithMessage("Duration must not exceed 3600 seconds (1 hour)")
            .When(x => x.DurationSeconds.HasValue);
    }
}

/// <summary>
/// Validator for AddTagRequest.
/// </summary>
public class AddTagRequestValidator : Validator<AddTagRequest>
{
    public AddTagRequestValidator()
    {
        RuleFor(x => x.QuestionId)
            .NotEmpty()
            .WithMessage("Question ID is required");

        RuleFor(x => x.TagName)
            .NotEmpty()
            .WithMessage("Tag name is required")
            .MaximumLength(100)
            .WithMessage("Tag name must not exceed 100 characters");
    }
}

/// <summary>
/// Validator for AddAnswerOptionRequest.
/// </summary>
public class AddAnswerOptionRequestValidator : Validator<AddAnswerOptionRequest>
{
    public AddAnswerOptionRequestValidator()
    {
        RuleFor(x => x.QuestionId)
            .NotEmpty()
            .WithMessage("Question ID is required");

        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Answer option text is required")
            .MaximumLength(1000)
            .WithMessage("Answer option text must not exceed 1000 characters");

        RuleFor(x => x.Points)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Points must be non-negative");
    }
}

/// <summary>
/// Validator for UpdateAnswerOptionRequest.
/// </summary>
public class UpdateAnswerOptionRequestValidator : Validator<UpdateAnswerOptionRequest>
{
    public UpdateAnswerOptionRequestValidator()
    {
        RuleFor(x => x.QuestionId)
            .NotEmpty()
            .WithMessage("Question ID is required");

        RuleFor(x => x.AnswerOptionId)
            .NotEmpty()
            .WithMessage("Answer option ID is required");

        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Answer option text cannot be empty when provided")
            .MaximumLength(1000)
            .WithMessage("Answer option text must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Text));

        RuleFor(x => x.Points)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Points must be non-negative")
            .When(x => x.Points.HasValue);
    }
}

/// <summary>
/// Validator for AddMediaRequest.
/// </summary>
public class AddMediaRequestValidator : Validator<AddMediaRequest>
{
    public AddMediaRequestValidator()
    {
        RuleFor(x => x.QuestionId)
            .NotEmpty()
            .WithMessage("Question ID is required");

        RuleFor(x => x.MediaType)
            .NotEmpty()
            .WithMessage("Media type is required")
            .Must(BeValidMediaType)
            .WithMessage("Media type must be 'Audio' or 'Video'");

        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage("Media URL is required")
            .MaximumLength(2000)
            .WithMessage("Media URL must not exceed 2000 characters");

        RuleFor(x => x.MimeType)
            .MaximumLength(100)
            .WithMessage("MIME type must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.MimeType));

        RuleFor(x => x.ThumbnailUrl)
            .MaximumLength(2000)
            .WithMessage("Thumbnail URL must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.ThumbnailUrl));

        RuleFor(x => x.DurationSeconds)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than 0 seconds")
            .When(x => x.DurationSeconds.HasValue);
    }

    private bool BeValidMediaType(string? mediaType)
    {
        return mediaType?.Equals("Audio", StringComparison.OrdinalIgnoreCase) == true
            || mediaType?.Equals("Video", StringComparison.OrdinalIgnoreCase) == true;
    }
}

/// <summary>
/// Validator for UpdateMediaRequest.
/// </summary>
public class UpdateMediaRequestValidator : Validator<UpdateMediaRequest>
{
    public UpdateMediaRequestValidator()
    {
        RuleFor(x => x.QuestionId)
            .NotEmpty()
            .WithMessage("Question ID is required");

        RuleFor(x => x.MediaId)
            .NotEmpty()
            .WithMessage("Media ID is required");

        RuleFor(x => x.MediaType)
            .Must(BeValidMediaType)
            .WithMessage("Media type must be 'Audio' or 'Video'")
            .When(x => !string.IsNullOrEmpty(x.MediaType));

        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage("Media URL cannot be empty when provided")
            .MaximumLength(2000)
            .WithMessage("Media URL must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Url));

        RuleFor(x => x.MimeType)
            .MaximumLength(100)
            .WithMessage("MIME type must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.MimeType));

        RuleFor(x => x.ThumbnailUrl)
            .MaximumLength(2000)
            .WithMessage("Thumbnail URL must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.ThumbnailUrl));

        RuleFor(x => x.DurationSeconds)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than 0 seconds")
            .When(x => x.DurationSeconds.HasValue);
    }

    private bool BeValidMediaType(string? mediaType)
    {
        return mediaType?.Equals("Audio", StringComparison.OrdinalIgnoreCase) == true
            || mediaType?.Equals("Video", StringComparison.OrdinalIgnoreCase) == true;
    }
}

/// <summary>
/// Validator for DeleteMediaRequest.
/// </summary>
public class DeleteMediaRequestValidator : Validator<DeleteMediaRequest>
{
    public DeleteMediaRequestValidator()
    {
        RuleFor(x => x.QuestionId)
            .NotEmpty()
            .WithMessage("Question ID is required");

        RuleFor(x => x.MediaId)
            .NotEmpty()
            .WithMessage("Media ID is required");
    }
}
