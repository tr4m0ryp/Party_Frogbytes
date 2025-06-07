using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PartyPlanner.Domain.Models;

[Index(nameof(QuestionId))]
public class Option
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Text { get; set; } = default!;

    // ---------- FK ----------
    public int QuestionId { get; set; }
    public Question Question { get; set; } = default!;
}
