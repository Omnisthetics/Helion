namespace Helion.Util.Parser
{
    /// <summary>
    /// All the different token types the parser supports.
    /// </summary>
    public enum TokenType
    {
        Number,
        Identifier,
        QuotedString,
        Backtick,
        Tilde,
        Exclamation,
        At,
        Hash,
        Dollar,
        Percent,
        Caret,
        Ampersand,
        Asterisk,
        ParenLeft,
        ParenRight,
        Plus,
        Minus,
        Underscore,
        Equals,
        BracketLeft,
        BracketRight,
        BraceLeft,
        BraceRight,
        Backslash,
        Pipe,
        AngleLeft,
        AngleRight,
        Comma,
        Period,
        Slash,
        QuestionMark,
    }
}