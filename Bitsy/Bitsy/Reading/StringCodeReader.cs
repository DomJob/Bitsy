namespace Bitsy.Reading;

public class StringCodeReader : IReader
{
    private readonly string text;
    private int column = 0;
    private int cursor;
    private int line = 1;

    public StringCodeReader(string text)
    {
        this.text = text;
    }

    public bool HasMore()
    {
        return cursor < text.Length;
    }

    public char Read()
    {
        if (!HasMore()) return '\0';
        var ch = text[cursor];

        if (ch == '\n')
        {
            line++;
            column = 0;
        }

        column++;
        cursor++;
        return ch;
    }

    public char Peek()
    {
        return cursor >= text.Length ? '\0' : text[cursor];
    }

    public Position GetPosition()
    {
        return new Position
        {
            Column = column,
            Line = line,
            FileName = "<String>"
        };
    }
}