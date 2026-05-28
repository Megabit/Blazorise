namespace Blazorise.Utilities;

internal readonly struct OnScreenKeyboardInputComposition
{
    public OnScreenKeyboardInputComposition( string value, bool canCommit )
    {
        Value = value;
        CanCommit = canCommit;
    }

    public string Value { get; }

    public bool CanCommit { get; }
}