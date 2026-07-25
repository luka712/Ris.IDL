using Ris.Idl.Gui.Models;

namespace Ris.Idl.Gui.Services;


/// <summary>
/// The logging service.
/// </summary>
public class AppLogger
{
    private readonly List<Log> _logs = new();

    /// <summary>
    /// Called whenever a new log is added.
    /// </summary>
    public event EventHandler<Log>? OnLog;
    
    /// <summary>
    /// Logs an info message.
    /// </summary>
    /// <param name="message">The message to add.</param>
    public void Info(string message)
    {
        var log = new Log()
        {
            Severity = LogSeverity.Info,
            Message = message
        };
        _logs.Add(log);
        OnLog?.Invoke(this, log);   
    }
}