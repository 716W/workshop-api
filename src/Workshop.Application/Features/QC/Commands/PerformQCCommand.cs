using System;

namespace Workshop.Application.Features.QC.Commands;

public class PerformQCCommand
{
    public Guid ServiceRequestId { get; set; }
    public bool IsPassed { get; set; }
    public string Notes { get; set; } = string.Empty;
}
