using System;
using CsvBuddy.Services;
using FluentAssertions;
using Xunit;
using JetBrains.Annotations;

namespace CsvBuddy.Tests.Unit;

[TestSubject(typeof(TokenizerService))]
public class TokenizerTests
{
    [Fact]
    public void GetNext_ShouldReturnFirstCharacter_WhenStringNotEmpty()
    {
        var tokenizer = new TokenizerService("abc");
        var result = tokenizer.GetNext();
        result.Should().Be('a');
    }

    [Fact]
    public void GetNext_ShouldReturnEof_WhenStringEmpty()
    {
        var tokenizer = new TokenizerService("");
        var result = tokenizer.GetNext();
        result.Should().Be(CsvConstants.Eof);
    }

    [Fact]
    public void GetNext_ShouldReturnBacktrackedChar_AfterBacktrack()
    {
        var tokenizer = new TokenizerService("abc");
        tokenizer.Read(); // Read 'a'
        tokenizer.Backtrack('a');
        var result = tokenizer.GetNext();
        result.Should().Be('a');
    }

    [Fact]
    public void Read_ShouldAdvanceThroughString()
    {
        var tokenizer = new TokenizerService("abc");
        tokenizer.Read().Should().Be('a');
        tokenizer.Read().Should().Be('b');
        tokenizer.Read().Should().Be('c');
        tokenizer.Read().Should().Be(CsvConstants.Eof);
    }

    [Fact]
    public void Read_ShouldReturnBacktrackedChar_AfterBacktrack()
    {
        var tokenizer = new TokenizerService("abc");
        tokenizer.Read(); // Read 'a'
        tokenizer.Backtrack('a');
        var result = tokenizer.Read();
        result.Should().Be('a');
    }

    [Fact]
    public void Read_ShouldConvertCrToLf()
    {
        var tokenizer = new TokenizerService("a\rb");
        tokenizer.Read(); // Read 'a'
        var result = tokenizer.Read();
        result.Should().Be('\n');
    }

    [Fact]
    public void Read_ShouldSkipCrInCrLf()
    {
        var tokenizer = new TokenizerService("a\r\nb");
        tokenizer.Read(); // Read 'a'
        var result = tokenizer.Read();
        result.Should().Be('\n');
    }

    [Fact]
    public void Backtrack_ShouldThrowException_WhenCalledTwice()
    {
        var tokenizer = new TokenizerService("abc");
        tokenizer.Backtrack('x');
        tokenizer.Invoking(t => t.Backtrack('y'))
            .Should().Throw<Exception>()
            .WithMessage("Unread() cannot accept more than one pushed back character");
    }

    [Fact]
    public void GetNext_ShouldNotAdvancePosition()
    {
        var tokenizer = new TokenizerService("abc");
        var first = tokenizer.GetNext();
        var second = tokenizer.GetNext();
        first.Should().Be('a');
        second.Should().Be('a'); 
        tokenizer.Read().Should().Be('a');
        tokenizer.GetNext().Should().Be('b');
    }

    [Fact]
    public void ReadAndGetNext_ShouldWorkCorrectlyTogether()
    {
        var tokenizer = new TokenizerService("abc");
        tokenizer.GetNext().Should().Be('a'); // Peek at 'a'
        tokenizer.Read().Should().Be('a'); // Read 'a'
        tokenizer.GetNext().Should().Be('b'); // Peek at 'b'
        tokenizer.Read().Should().Be('b'); // Read 'b'
    }

    [Fact]
    public void MultipleLineEndings_ShouldBeNormalizedCorrectly()
    {
        var tokenizer = new TokenizerService("a\rb\r\nc\nd");
        tokenizer.Read().Should().Be('a');
        tokenizer.Read().Should().Be('\n'); // Converted \r
        tokenizer.Read().Should().Be('b');
        tokenizer.Read().Should().Be('\n'); // From \r\n
        tokenizer.Read().Should().Be('c');
        tokenizer.Read().Should().Be('\n');
        tokenizer.Read().Should().Be('d');
    }

    [Fact]
    public void Backtrack_ShouldRestoreCharacterForNextRead()
    {
        var tokenizer = new TokenizerService("abc");
        tokenizer.Read(); // Read 'a'
        tokenizer.Backtrack('a');
        tokenizer.Read().Should().Be('a');
        tokenizer.Read().Should().Be('b'); // Continues with the rest
    }

    [Fact]
    public void Backtrack_ShouldPreserveRestOfString()
    {
        var tokenizer = new TokenizerService("abc");
        tokenizer.Read(); // Read 'a'
        tokenizer.Backtrack('a');
        tokenizer.Read(); // Re-read 'a'
        tokenizer.Read().Should().Be('b');
        tokenizer.Read().Should().Be('c');
        tokenizer.Read().Should().Be(CsvConstants.Eof);
    }
}