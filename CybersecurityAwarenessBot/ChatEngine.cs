using System.Text;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityAwarenessBot;



public class ChatEngine
{
    // Delegate: every intent has a handler with this signature
    public delegate string IntentHandler(string input);

    // An intent = a name + a test (Predicate delegate) + a handler (delegate)
    private sealed record Intent(string Name, Predicate<string> Matches, IntentHandler Handle);

    private static readonly Regex InterestRx = new(
        @"(?:interested in|favou?rite topic is|care about|love learning about)\s+(?<t>[a-z0-9' \-]+)");
    private static readonly Regex NameRx = new(@"(?:my name is|call me)\s+(?<n>[a-z]+)");

    private static readonly string[] ExitWords = { "exit", "quit", "bye", "goodbye" };

    private static readonly List<string> Intros = new()
    {
        "Great question, {name}! Here's a {topic} tip:",
        "Good choice, {name}. Here's something worth knowing about {topic}:",
        "Sure thing, {name}! A {topic} tip for you:",
        "Happy to help, {name}. About {topic}:"
    };
    // generic collection
    // avoids repeating a response
    private readonly Dictionary<string, Topic> _topics = TopicLibrary.Build();   
    private readonly Dictionary<string, int> _lastPick = new();                  
    private readonly List<Intent> _intents;
    private readonly Random _rng = new();

    private Topic? _currentTopic;   
    private int _replyCount;

    public UserMemory Memory { get; } = new();

    public ChatEngine()
    {
        // Order matters: the first intent whose test passes wins.
        _intents = new List<Intent>
        {
            new("Memory",    IsMemoryStatement, HandleMemoryStatement),
            new("Recall",    i => HasAny(i, "what do you remember", "do you remember", "what do you know about me"), HandleRecall),
            new("FollowUp",  IsFollowUp, HandleFollowUp),
            new("Sentiment", i => SentimentDetector.Detect(i) != null, HandleSentiment),
            new("Topic",     i => FindTopic(i) != null, HandleTopic),
            new("SmallTalk", IsSmallTalk, HandleSmallTalk)
        };
    }

    // ---------- Public API ----------

    public static bool IsExit(string raw) => ExitWords.Contains(Normalise(raw));

    public string GetResponse(string rawInput)
    {
        // Error handling: empty input
        if (string.IsNullOrWhiteSpace(rawInput))
            return "I didn't receive any input. Please type a question.";

        string input = Normalise(rawInput);
        _replyCount++;

        foreach (Intent intent in _intents)
            if (intent.Matches(input))
                return intent.Handle(input);

        // Error handling: unknown input
        return $"I'm not sure I understand, {Memory.Name}. Can you try rephrasing?\n" +
               "You can ask about passwords, phishing, scams, privacy, safe browsing, malware or two-factor authentication.";
    }

    // ---------- Intent tests ----------

    private bool IsMemoryStatement(string i) => InterestRx.IsMatch(i) || NameRx.IsMatch(i);

    private bool IsFollowUp(string i) => HasAny(i,
        "another tip", "another one", "another", "tell me more", "explain more", "explain further",
        "more detail", "more info", "elaborate", "go on", "one more", "next tip",
        "what do you mean", "don't understand", "dont understand", "confused");

    private bool IsSmallTalk(string i) => HasAny(i,
        "how are you", "your purpose", "what can i ask", "what do you do", "help", "hello", "hi", "hey");

    // ---------- Intent handlers ----------

    private string HandleMemoryStatement(string input)
    {
        Match nameMatch = NameRx.Match(input);
        if (nameMatch.Success)
        {
            string n = nameMatch.Groups["n"].Value;
            Memory.Name = char.ToUpper(n[0]) + n.Substring(1);
            return $"Nice to meet you, {Memory.Name}! I'll remember your name.";
        }

        string raw = InterestRx.Match(input).Groups["t"].Value.Trim();
        Topic? topic = FindTopic(raw);
        Memory.FavouriteTopic = topic?.Name ?? raw;

        string reply = $"Great! I'll remember that you're interested in {Memory.FavouriteTopic}. " +
                       "It's a crucial part of staying safe online.";

        if (topic != null)
        {
            SetCurrent(topic);
            reply += "\n\nHere's a tip to start: " + Pick(topic.Name + "|tip", topic.Tips);
        }
        return reply;
    }

    private string HandleRecall(string input)
    {
        var sb = new StringBuilder("Here's what I remember about you:\n");
        sb.AppendLine($"  • Name: {Memory.Name}");
        sb.AppendLine(Memory.FavouriteTopic == ""
            ? "  • Favourite topic: not told yet (try \"I'm interested in privacy\")"
            : $"  • Interested in: {Memory.FavouriteTopic}");
        if (Memory.TopicsDiscussed.Count > 0)
            sb.AppendLine($"  • Topics we've covered: {string.Join(", ", Memory.TopicsDiscussed)}");
        return sb.ToString().TrimEnd();
    }

    private string HandleFollowUp(string input)
    {
        Topic? topic = FindTopic(input) ?? _currentTopic;
        if (topic == null)
            return "I'd be happy to go further! Which topic would you like: passwords, phishing, scams, privacy, safe browsing, malware or 2FA?";

        SetCurrent(topic);

        bool wantsTip = HasAny(input, "another", "next tip", "one more");
        bool confused = HasAny(input, "confused", "don't understand", "dont understand", "what do you mean");

        if (wantsTip)
            return $"Here's another {topic.Name} tip:\n{Pick(topic.Name + "|tip", topic.Tips)}";

        string prefix = confused
            ? "No problem, let me explain that more simply.\n"
            : $"Sure, here's more on {topic.Name}:\n";
        return prefix + Pick(topic.Name + "|detail", topic.Details);
    }

    private string HandleSentiment(string input)
    {
        string sentiment = SentimentDetector.Detect(input)!;
        Memory.LastSentiment = sentiment;
        string reply = Pick("sent|" + sentiment, SentimentDetector.Replies[sentiment]);

        if (sentiment == "happy") return reply;

        // Share a tip straight away so the user doesn't have to ask again
        Topic topic = FindTopic(input)
                      ?? _currentTopic
                      ?? FavouriteTopic()
                      ?? _topics["cybersecurity"];
        SetCurrent(topic);
        return $"{reply}\n\nLet me share a tip about {topic.Name}:\n{Pick(topic.Name + "|tip", topic.Tips)}";
    }

    private string HandleTopic(string input)
    {
        Topic topic = FindTopic(input)!;
        bool isNew = SetCurrent(topic);

        string intro = Pick("intro", Intros)
            .Replace("{name}", Memory.Name)
            .Replace("{topic}", topic.Name);

        var sb = new StringBuilder();
        sb.Append(intro).Append('\n').Append(Pick(topic.Name + "|tip", topic.Tips));

        // Memory recall: personalise using the favourite topic
        if (Memory.FavouriteTopic != "")
        {
            if (Memory.FavouriteTopic.Equals(topic.Name, StringComparison.OrdinalIgnoreCase))
                sb.Append($"\n\nAs someone interested in {Memory.FavouriteTopic}, {topic.Personal}");
            else if (_replyCount % 3 == 0)
                sb.Append($"\n\nBy the way, as someone interested in {Memory.FavouriteTopic}, feel free to ask me more about it any time.");
        }

        if (isNew)
            sb.Append("\n\n(Say \"another tip\" or \"tell me more\" to keep going.)");

        return sb.ToString();
    }

    private string HandleSmallTalk(string input)
    {
        if (HasAny(input, "how are you"))
            return $"I'm doing great, thank you for asking, {Memory.Name}! Ready to talk cybersecurity?";

        if (HasAny(input, "your purpose", "what do you do"))
            return "My purpose is to help you stay safe online by teaching you about passwords, phishing, scams, privacy and more.";

        if (HasAny(input, "what can i ask", "help"))
            return "You can ask me about:\n  • Password safety\n  • Phishing\n  • Scams\n  • Privacy\n  • Safe browsing\n  • Malware\n  • Two-factor authentication\n" +
                   "Try telling me how you feel (\"I'm worried about scams\") or what you're interested in.";

        return $"Hello {Memory.Name}! What would you like to learn about online safety today?";
    }

    // ---------- Helpers ----------

    /// <summary>Lowercases, removes punctuation and extra spaces.</summary>
    private static string Normalise(string s)
    {
        s = s.ToLowerInvariant().Replace('’', '\'');
        s = Regex.Replace(s, @"[^a-z0-9'\- ]", " ");
        return Regex.Replace(s, @"\s+", " ").Trim();
    }

    /// <summary>True if any keyword appears at the start of a word (so "passwords" matches "password").</summary>
    private static bool HasAny(string input, params string[] keys) =>
        keys.Any(k => Regex.IsMatch(input, $@"\b{Regex.Escape(k)}"));

    private Topic? FindTopic(string input) =>
        _topics.Values.FirstOrDefault(t => HasAny(input, t.Keywords));

    private Topic? FavouriteTopic() =>
        _topics.Values.FirstOrDefault(t => t.Name.Equals(Memory.FavouriteTopic, StringComparison.OrdinalIgnoreCase));

    /// <summary>Remembers the current topic. Returns true if it's the first time we discuss it.</summary>
    private bool SetCurrent(Topic topic)
    {
        _currentTopic = topic;
        if (Memory.TopicsDiscussed.Contains(topic.Name)) return false;
        Memory.TopicsDiscussed.Add(topic.Name);
        return true;
    }

    /// <summary>Random response that never repeats the previous pick for the same key.</summary>
    private string Pick(string key, List<string> items)
    {
        if (items.Count == 1) return items[0];

        int index;
        do { index = _rng.Next(items.Count); }
        while (_lastPick.TryGetValue(key, out int last) && last == index);

        _lastPick[key] = index;
        return items[index];
    }
}
