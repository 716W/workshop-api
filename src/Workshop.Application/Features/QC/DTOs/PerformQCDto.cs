namespace Workshop.Application.Features.QC.DTOs;

public class PerformQCDto
{
    public bool IsPassed { get; set; }
    public string Notes { get; set; } = string.Empty;
}
