using System;

namespace CsvBuddy.Services;

public interface ITokenizer
{
    char Read();
    char GetNext();
    void Backtrack(char c);
}

public class TokenizerService(string input) : ITokenizer
{
    private int _index;
    private bool _hasBacktracked;
    private char _backtrackedChar;

    public char GetNext()
    {
        if (_hasBacktracked)
            return _backtrackedChar;
        if (_index < input.Length)
            return NormalizeLineEnding(input[_index]);
        return CsvConstants.Eof;
    }

    public char Read()
    {
        if (_hasBacktracked)
        {
            _hasBacktracked = false;
            return _backtrackedChar;
        }
        if (_index < input.Length)
        {
            SkipLineEnding();
            return NormalizeLineEnding(input[_index++]);
        }
        return CsvConstants.Eof;
    }

    public void Backtrack(char c)
    {
        if (_hasBacktracked)
            throw new Exception("Unread() cannot accept more than one pushed back character");
        _hasBacktracked = true;
        _backtrackedChar = c;
    }

    private void SkipLineEnding()
    {
        if (_index < input.Length - 1 && input[_index] == '\r' && input[_index + 1] == '\n')
            _index++;
    }
    private static char NormalizeLineEnding(char c) => c == '\r' ? '\n' : c;
}

public static class CsvConstants
{
    public const char Eof = '\0';
}