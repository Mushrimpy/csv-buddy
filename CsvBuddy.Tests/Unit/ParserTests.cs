using System;
using CsvBuddy.Services;
using Xunit;
using JetBrains.Annotations;

namespace CsvBuddy.Tests.Unit;

[TestSubject(typeof(ParserService))]
public class ParserTests
{
    [Fact]
    public void Parse_EmptyString_SignalsEndOfFile()
    {
        var tokenizer = new TokenizerService("");
        var tester = new ParserTester("eof");
        var parser = new ParserService();
        parser.Parse(tokenizer, tester);

    }
    
    [Fact]
    public void Parse_SingleEmptyField_ProcessesCorrectly()
    {
        var tokenizer = new TokenizerService("\n");
        var tester = new ParserTester("", "eor", "eof");
        var parser = new ParserService();
        parser.Parse(tokenizer, tester);
    }

    [Fact]
    public void Parse_SimpleValues_ProcessesCorrectly()
    {
        var tokenizer = new TokenizerService("abc,def,ghi\n");
        var tester = new ParserTester("abc", "def", "ghi", "eor", "eof");
        var parser = new ParserService();
        parser.Parse(tokenizer, tester);
    }

    [Fact]
    public void Parse_QuotedValues_ProcessesCorrectly()
    {
        var tokenizer = new TokenizerService("\"abc\",\"def\",\"ghi\"\n");
        var tester = new ParserTester("abc", "def", "ghi", "eor", "eof");
        var parser = new ParserService();
        parser.Parse(tokenizer, tester);
    }

    [Fact]
    public void Parse_MixedQuotedAndSimpleValues_ProcessesCorrectly()
    {

        var tokenizer = new TokenizerService("abc,\"def\",ghi\n");
        var tester = new ParserTester("abc", "def", "ghi", "eor", "eof");
        var parser = new ParserService();
        parser.Parse(tokenizer, tester);
    }

    [Fact]
    public void Parse_ValuesWithSpaces_ProcessesCorrectly()
    {
        var tokenizer = new TokenizerService("abc , def , ghi \n");
        var tester = new ParserTester("abc", "def", "ghi", "eor", "eof");
        var parser = new ParserService();
        parser.Parse(tokenizer, tester);
    }

    [Fact]
    public void Parse_EscapedQuotes_ProcessesCorrectly()
    {
        var tokenizer = new TokenizerService("\"a\"\"bc\",\"d\"\"e\"\"f\",\"ghi\"\n");
        var tester = new ParserTester("a\"bc", "d\"e\"f", "ghi", "eor", "eof");
        var parser = new ParserService();
        parser.Parse(tokenizer, tester);
    }

    [Fact]
    public void Parse_EmptyFields_ProcessesCorrectly()
    {
        var tokenizer = new TokenizerService(",,\n");
        var tester = new ParserTester("", "", "", "eor", "eof");
        var parser = new ParserService();
        parser.Parse(tokenizer, tester);
    }

    [Fact]
    public void Parse_MultipleRecords_ProcessesCorrectly()
    {
        var tokenizer = new TokenizerService("a,b,c\nd,e,f\n");
        var tester = new ParserTester("a", "b", "c", "eor", "d", "e", "f", "eor", "eof");
        var parser = new ParserService();
        parser.Parse(tokenizer, tester);
    }

    [Fact]
    public void Parse_CarriageReturnLineFeeds_ProcessesCorrectly()
    {
        var tokenizer = new TokenizerService("a,b,c\r\nd,e,f\r\n");
        var tester = new ParserTester("a", "b", "c", "eor", "d", "e", "f", "eor", "eof");
        var parser = new ParserService();
        parser.Parse(tokenizer, tester);
    }

    [Fact]
    public void Parse_FieldWithCommasAndQuotes_ProcessesCorrectly()
    {
        var tokenizer = new TokenizerService("\"a,b,c\",\"hello \"\"world\"\"\",d\n");
        var tester = new ParserTester("a,b,c", "hello \"world\"", "d", "eor", "eof");
        var parser = new ParserService();
        parser.Parse(tokenizer, tester);
    }
}

public class ParserTester(params string[] expectedFields) : IConsumer
    {
        private int _fieldIndex;
        public void ConsumeField(string s)
        {
            if (expectedFields[_fieldIndex] != s)
            {
                throw new Exception(
                    string.Format("field [{0}] expected, but [{1}] returned",
                        expectedFields[_fieldIndex], s));
            }
            _fieldIndex++;
        }
        public void SignalEndOfRecord()
        {
            if (expectedFields[_fieldIndex++] != "eor")
            {
                throw new Exception(
                    "End of record signalled but not expected");
            }
        }
        public void SignalEndOfFile()
        {
            if (expectedFields[_fieldIndex++] != "eof")
            {
                throw new Exception(
                    "End of file signalled but not expected");
            }
        }
    }
