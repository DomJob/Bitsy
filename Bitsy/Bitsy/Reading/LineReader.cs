namespace Bitsy.Reading;

public class LineReader : IReader
{
    private readonly string text;
    private int column;
    private int cursor;
    private int line = 1;

    public LineReader(string text)
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
            Line = line
        };
    }
}