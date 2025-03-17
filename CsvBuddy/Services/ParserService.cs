// Recursive descent parser

using System;

namespace CsvBuddy.Services;

public class ParserService
{
    public void Parse(ITokenizer reader)
    {
        while (reader.GetNext() != CsvConstants.Eof)
        {
            ParseCsvRecord(reader);
        }
    }

    private void ParseCsvRecord(ITokenizer reader)
    {
        ParseCsvStringList(reader);
        char ch = reader.Read();
        if (ch == CsvConstants.Eof)
        {
            reader.Backtrack(ch);
            ch = '\n';
        }
        if (ch != '\n')
        {
            throw new Exception("End of record was expected but more data exists.");
        }
    }

    private void ParseCsvStringList(ITokenizer reader)
    {
        char ch;
        do
        {
            ParseRawString(reader);
            ch = reader.Read();
        } while (ch == ',');
        reader.Backtrack(ch);
    }

    private void ParseRawString(ITokenizer reader)
    {
        ParseOptionalSpaces(reader);
        ParseRawField(reader);
        if (!IsFieldTerminator(reader.GetNext()))
            ParseOptionalSpaces(reader);
    }

    private void ParseRawField(ITokenizer reader)
    {
        char ch = reader.GetNext();
        if (!IsFieldTerminator(ch))
        {
            if (ch == '"')
                ParseQuotedField(reader);
            else
                ParseSimpleField(reader);
        }
    }

    private string ParseQuotedField(ITokenizer reader)
    {
        reader.Read(); // Read and discard initial quote
        string field = ParseEscapedField(reader);
        char character = reader.Read();
        if (character != '"')
        {
            reader.Backtrack(character);
            throw new Exception("Quoted field has no terminating double quote");
        }
        return field;
    }

    private string ParseEscapedField(ITokenizer reader)
    {
        System.Text.StringBuilder builder = new();
        ParseSubField(reader, builder);
        char character = reader.Read();
        while (ProcessDoubleQuote(reader, character))
        {
            builder.Append('"');
            ParseSubField(reader, builder);
            character = reader.Read();
        }
        reader.Backtrack(character);
        return builder.ToString();
    }

    private void ParseSubField(ITokenizer reader, System.Text.StringBuilder builder)
    {
        char character = reader.Read();
        while (character != '"' && character != CsvConstants.Eof)
        {
            builder.Append(character);
            character = reader.Read();
        }
        reader.Backtrack(character);
    }

    private string ParseSimpleField(ITokenizer reader)
    {
        System.Text.StringBuilder builder = new();
        char character = reader.Read();
        while (!IsBadSimpleFieldChar(character))
        {
            builder.Append(character);
            character = reader.Read();
        }
        reader.Backtrack(character);
        return builder.ToString();
    }

    private bool IsFieldTerminator(char c) => c == ',' || c == '\n' || c == CsvConstants.Eof;
    private static bool IsSpace(char c) => c == ' ' || c == '\t';
    private void ParseOptionalSpaces(ITokenizer reader) { while (IsSpace(reader.Read())) { } reader.Backtrack(reader.GetNext()); }
    private static bool ProcessDoubleQuote(ITokenizer reader, char ch) => ch == '"' && reader.GetNext() == '"' && reader.Read() != '\0';
    private bool IsBadSimpleFieldChar(char c) => IsSpace(c) || IsFieldTerminator(c) || c == '"';
}

