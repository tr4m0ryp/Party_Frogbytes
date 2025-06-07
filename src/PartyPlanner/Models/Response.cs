using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PartyPlanner.Domain.Models;

[Index(nameof(InvitationId), nameof(QuestionId), IsUnique = true)]
public class Response
{
    public int Id { get; set; }

    // ---------- FK’s ----------
    public int InvitationId { get; set; }
    public Invitation Invitation { get; set; } = default!;

    public int QuestionId { get; set; }
    public Question Question { get; set; } = default!;

    /// <summary>Antwoord voor open vraag.</summary>
    [MaxLength(500)]
    public string? AnswerText { get; set; }

    /// <summary>Gekozen optie bij multiple choice (nullable).</summary>
    public int? SelectedOptionId { get; set; }
    public Option? SelectedOption { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = default!;
}
