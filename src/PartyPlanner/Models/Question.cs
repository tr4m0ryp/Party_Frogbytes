using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using PartyPlanner.Domain.Enums;

namespace PartyPlanner.Domain.Models;

[Index(nameof(PartyId))]
public class Question
{
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string Text { get; set; } = default!;

    public QuestionType Type { get; set; }

    // ---------- FK ----------
    public int PartyId { get; set; }
    public Party Party { get; set; } = default!;

    // ---------- Navigatie ----------
    public IReadOnlyCollection<Option> Options => _options.AsReadOnly();
    private readonly List<Option> _options = new();

    public IReadOnlyCollection<Response> Responses => _responses.AsReadOnly();
    private readonly List<Response> _responses = new();
}
