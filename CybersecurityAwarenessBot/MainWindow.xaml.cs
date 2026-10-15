using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CybersecurityAwarenessBot;

/// <summary>
/// GUI layer only. All "thinking" is done by <see cref="ChatEngine"/>.
/// Replaces the console classes from Part 1 (BotUI / BotInput).
/// </summary>
public partial class MainWindow : Window
{
    private enum ChatState { AwaitingName, Chatting }

    private readonly ChatEngine _engine = new();
    private ChatState _state = ChatState.AwaitingName;
    private bool _busy;
    private bool _closing;

    public MainWindow()
    {
        InitializeComponent();
        AsciiArt.Text = AsciiLogo.Text;                 
        Loaded += async (s, e) => await StartAsync();
    }

    // ---------- Startup ----------//

    private async Task StartAsync()
    {
        SoundManager.PlayWelcomeSound();                
        _busy = true;
        SetInputEnabled(false);
        await BotSayAsync("Hello! Welcome to the Cybersecurity Awareness Bot. " +
                          "I'm here to help you stay safe online.\n\nPlease enter your name to begin:");
        _busy = false;
        SetInputEnabled(true);
    }

    // ---------- Event handlers ----------

    private async void SendButton_Click(object sender, RoutedEventArgs e) => await SendAsync(InputBox.Text);

    private async void InputBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) await SendAsync(InputBox.Text);
    }

    private async void TopicButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button b && b.Tag is string text)
        {
            if (_state == ChatState.AwaitingName)
            {
                await BotSayAsync("Please enter your name first.");
                return;
            }
            await SendAsync(text);
        }
    }

    // ---------- Core flow ----------

    private async Task SendAsync(string text)
    {
        if (_busy || _closing) return;
        _busy = true;
        SetInputEnabled(false);

        try
        {
            text = (text ?? "").Trim();

            // Input validation: empty entries
            if (text.Length == 0)
            {
                await BotSayAsync(_state == ChatState.AwaitingName
                    ? "Your name cannot be empty. Please enter your name."
                    : "I didn't receive any input. Please type a question.");
                return;
            }

            AddBubble(_state == ChatState.AwaitingName ? "You" : _engine.Memory.Name, text, isBot: false);
            InputBox.Clear();

            if (_state == ChatState.AwaitingName)
            {
                await HandleNameAsync(text);
                return;
            }

            if (ChatEngine.IsExit(text))
            {
                _closing = true;
                await BotSayAsync($"Goodbye {_engine.Memory.Name}! Stay safe online.");
                await Task.Delay(1500);
                Close();
                return;
            }

            await BotSayAsync(_engine.GetResponse(text));
        }
        catch (Exception)
        {
            // Never crash on unexpected input
            AddBubble("Bot", "Oops, something went wrong on my side. Please try again.", isBot: true);
        }
        finally
        {
            _busy = false;
            if (!_closing)
            {
                SetInputEnabled(true);
                InputBox.Focus();
            }
        }
    }

    private async Task HandleNameAsync(string text)
    {
        // Allow "my name is Sam" as well as just "Sam"
        string name = Regex.Replace(text, @"^(my name is|i am|i'm|call me)\s+", "", RegexOptions.IgnoreCase).Trim();

        if (!Regex.IsMatch(name, @"^\p{L}[\p{L} '\-]{0,29}$"))
        {
            await BotSayAsync("That doesn't look like a valid name. Please use letters only (max 30 characters).");
            return;
        }

        _engine.Memory.Name = char.ToUpper(name[0]) + name.Substring(1);
        _state = ChatState.Chatting;

        string border = new string('═', 36);
        await BotSayAsync(
            $"{border}\n   Welcome, {_engine.Memory.Name}!\n{border}\n\n" +
            $"Hello {_engine.Memory.Name}! I am here to help you stay safe and secure online.\n\n" +
            "You can ask me about:\n" +
            "  • Password safety\n  • Phishing & scams\n  • Privacy\n  • Safe browsing\n" +
            "  • Malware\n  • Two-factor authentication\n\n" +
            "Tip: try \"I'm worried about scams\", \"tell me more\" or \"I'm interested in privacy\".\n" +
            "Type 'exit' to leave.");
    }

    // ---------- Chat bubble helpers ----------

    /// <summary>Shows a bot message with a short "typing" delay and a typewriter effect.</summary>
    private async Task BotSayAsync(string text)
    {
        TextBlock body = AddBubble("Bot", "...", isBot: true);
        await Task.Delay(450);

        body.Text = "";
        int chunk = Math.Max(1, text.Length / 120);     
        for (int i = 0; i < text.Length; i += chunk)
        {
            body.Text = text.Substring(0, Math.Min(text.Length, i + chunk));
            ChatScroll.ScrollToEnd();
            await Task.Delay(12);
        }
        body.Text = text;
        ChatScroll.ScrollToEnd();
    }

    private TextBlock AddBubble(string sender, string text, bool isBot)
    {
        var header = new TextBlock
        {
            Text = sender,
            FontWeight = FontWeights.Bold,
            FontSize = 12,
            Foreground = Colour(isBot ? "#00E5FF" : "#FFD54F"),
            Margin = new Thickness(0, 0, 0, 4)
        };
        var body = new TextBlock
        {
            Text = text,
            TextWrapping = TextWrapping.Wrap,
            Foreground = Brushes.White,
            FontSize = 14
        };
        var stack = new StackPanel();
        stack.Children.Add(header);
        stack.Children.Add(body);

        var bubble = new Border
        {
            Child = stack,
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(12, 8, 12, 10),
            Margin = new Thickness(8, 4, 8, 4),
            MaxWidth = 560,
            HorizontalAlignment = isBot ? HorizontalAlignment.Left : HorizontalAlignment.Right,
            Background = Colour(isBot ? "#1F2937" : "#0B3D5C")
        };

        ChatPanel.Children.Add(bubble);
        ChatScroll.ScrollToEnd();
        return body;
    }

    private static Brush Colour(string hex) => (Brush)new BrushConverter().ConvertFromString(hex)!;

    private void SetInputEnabled(bool enabled)
    {
        InputBox.IsEnabled = enabled;
        SendButton.IsEnabled = enabled;
    }
}
