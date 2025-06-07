using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PartyPlanner.Domain.Models;

[Index(nameof(OwnerId), nameof(Date))]
public class Party
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Title { get; set; } = default!;

    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>Datum & tijd van het event, in lokale tijdzone.</summary>
    public DateTimeOffset Date { get; set; }

    /// <summary>ForeignKey naar ASP.NET Identity‐user.</summary>
    [Required]
    public string OwnerId { get; set; } = default!;

    // ---------- Navigatie ----------
    public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();
    private readonly List<Question> _questions = new();

    public IReadOnlyCollection<Invitation> Invitations => _invitations.AsReadOnly();
    private readonly List<Invitation> _invitations = new();

    /// <summary>Voor optimistic concurrency.</summary>
    [Timestamp]
    public byte[] RowVersion { get; set; } = default!;
}
