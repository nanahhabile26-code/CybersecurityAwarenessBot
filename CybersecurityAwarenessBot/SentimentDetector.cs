using System;
using System.Collections.Generic;

using System.Text.RegularExpressions;

namespace CybersecurityAwarenessBot;

/// <summary>Simple keyword-based sentiment detection with empathetic replies.</summary>
public static class SentimentDetector
{
    private static readonly Dictionary<string, string[]> Keywords = new()
    {
        ["worried"]      = new[] { "worried", "worry", "scared", "afraid", "anxious", "nervous", "concerned", "fear" },
        ["frustrated"]   = new[] { "frustrated", "annoyed", "angry", "fed up", "irritated", "sick of" },
        ["overwhelmed"]  = new[] { "overwhelmed", "too much", "unsure", "not sure", "don't know where to start", "dont know where to start" },
        ["curious"]      = new[] { "curious", "wondering", "i wonder", "want to learn", "keen to learn" },
        ["happy"]        = new[] { "happy", "awesome", "excited", "thanks", "thank you", "great" }
    };

    public static readonly Dictionary<string, List<string>> Replies = new()
    {
        ["worried"] = new()
        {
            "It's completely understandable to feel that way. Cyber threats can be very convincing, but you can protect yourself.",
            "I hear you. Feeling cautious online is actually a good instinct. Let's turn that worry into action.",
            "Don't worry, you're already taking the right step by learning about it."
        },
        ["frustrated"] = new()
        {
            "I'm sorry this is frustrating. Security can feel like a lot, so let's keep it simple.",
            "That's fair, it can be annoying. Let's take it one small step at a time."
        },
        ["overwhelmed"] = new()
        {
            "No pressure at all. You don't need to learn everything at once. Let's start with one simple thing.",
            "It's okay to feel unsure. Everyone starts somewhere, and small habits make a big difference."
        },
        ["curious"] = new()
        {
            "Love the curiosity! Asking questions is the best way to stay safe online.",
            "Great attitude! Curious users are harder for scammers to fool."
        },
        ["happy"] = new()
        {
            "Glad to hear it! Ask me anything else whenever you like.",
            "You're welcome! Keep up the safe habits. Want another tip?"
        }
    };

    /// <summary>Returns the detected sentiment name, or null if none found.</summary>
    public static string? Detect(string input)
    {
        foreach (var pair in Keywords)
            foreach (string word in pair.Value)
                if (Regex.IsMatch(input, $@"\b{Regex.Escape(word)}"))
                    return pair.Key;
        return null;
    }
}
