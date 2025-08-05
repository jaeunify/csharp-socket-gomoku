using System;
using System.Collections.Generic;

public class LogStore : Instance
{
    public List<string> Logs { get; private set; } = new();

    public event Action? OnChange;

    public void AddLog(string message)
    {
        Logs.Add($"[{DateTime.Now:T}] {message}");

        if (Logs.Count > 100)
            Logs.RemoveAt(0);

        OnChange?.Invoke();
    }
}
