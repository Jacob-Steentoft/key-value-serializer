using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace KeyValueSerializer;

public sealed class KeyValueConfiguration
{
    [SetsRequiredMembers]
    public KeyValueConfiguration()
    {
        var newLine = Encoding.UTF8.GetBytes(Environment.NewLine);

        CommentStart = "//"u8.ToArray();
        CommentEnd = newLine;
        ArrayStart = (byte)'{';
        ArrayEnd = (byte)'}';
        ArraySeparator = (byte)',';
        StringSeparator = (byte)'"';
        ValueStart = (byte)'=';
        ValueEnd = (byte)';';
        NewLine = newLine;
        Space = (byte)' ';
        WhiteSpaces = " \t"u8.ToArray();
        StringIgnoreCharacter = (byte)'\\';
        DateTimeFormat = 'O';

        // Calculated
        KeyValueSeparator = new[] { Space, ValueStart, Space };

        ArraySeparatorAndSpace = new[] { ArraySeparator, Space };

        EndAndNewLine = new byte[1 + NewLine.Length];
        EndAndNewLine[0] = ValueEnd;
        NewLine.CopyTo(EndAndNewLine, 1);

        SkipFiller = new byte[WhiteSpaces.Length + 1 + NewLine.Length];
        WhiteSpaces.CopyTo(SkipFiller, 0);
        NewLine.CopyTo(SkipFiller, WhiteSpaces.Length);
        SkipFiller[^1] = ValueEnd;
    }

    public required byte[] CommentStart { get; init; }
    public required byte[] CommentEnd { get; init; }
    public required byte Space { get; init; }
    public required byte ArrayStart { get; init; }
    public required byte ArrayEnd { get; init; }
    public required byte ArraySeparator { get; init; }
    public required byte StringSeparator { get; init; }
    public required byte ValueStart { get; init; }
    public required byte ValueEnd { get; init; }
    public required byte StringIgnoreCharacter { get; init; }
    public required char DateTimeFormat { get; init; }
    public required byte[] ArraySeparatorAndSpace { get; init; }
    public required byte[] KeyValueSeparator { get; init; }
    public required byte[] EndAndNewLine { get; init; }
    public required byte[] NewLine { get; init; }
    public required byte[] WhiteSpaces { get; init; }
    public required byte[] SkipFiller { get; init; }
}
