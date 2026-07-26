namespace Ris.Idl.Gui.Models;

public enum LogSeverity
{
    Info,
    Warning,
    Error,
}

/// <summary>
/// The log message.
/// </summary>
public record Log
{
    /// <summary>
    /// The log message.
    /// </summary>
    public string Message { get; set; } = string.Empty; 
    
    /// <summary>
    /// The log severity.
    /// </summary>
    public LogSeverity Severity { get; set; } = LogSeverity.Info;
}