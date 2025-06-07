using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PartyPlanner.Models;

[Index(nameof(Code), IsUnique = true)]
public class Invitation
{
    public int Id { get; set; }

    [Required, StringLength(16, MinimumLength = 6)]
    public string Code { get; set; } = default!;

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    [EmailAddress, MaxLength(255)]
    public string? Email { get; set; }

    // ---------- FK ----------
    public int PartyId { get; set; }
    public Party Party { get; set; } = default!;

    // ---------- Navigatie ----------
    public IReadOnlyCollection<Response> Responses => _responses.AsReadOnly();
    private readonly List<Response> _responses = new();
}
