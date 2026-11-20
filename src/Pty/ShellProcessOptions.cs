namespace TermSharp.Pty;

public class ShellProcessOptions
{
    public string ShellPath { get; set; } = Environment.GetEnvironmentVariable("SHELL") ?? "/bin/bash";
    public int Columns { get; set; } = 80;
    public int Rows { get; set; } = 24;
    public string HomeDir { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
}