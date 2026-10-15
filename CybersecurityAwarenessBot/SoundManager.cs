using System;
using System.IO;
using System.Linq;
using System.Media;

namespace CybersecurityAwarenessBot;

/// <summary>Plays the WAV voice greeting from Part 1 (Assets folder).</summary>
// kept alive so playback isn't cut off
public static class SoundManager
{
    private const string FileName = "Welcome.wav";
    private static SoundPlayer? _player;   

    public static void PlayWelcomeSound()
    {
        try
        {
            string folder = Path.Combine(AppContext.BaseDirectory, "Assets");
            string path = Path.Combine(folder, FileName);

            // Fall back to any .wav in the Assets folder
            // throws if the file isn't a valid PCM WAV
            // asynchronous, doesn't freeze the GUI
            if (!File.Exists(path) && Directory.Exists(folder))
                path = Directory.GetFiles(folder, "*.wav").FirstOrDefault() ?? path;

            if (!File.Exists(path))
            {
                System.Windows.MessageBox.Show(
                    "Voice greeting not found.\nExpected: " + path,
                    "Sound", System.Windows.MessageBoxButton.OK);
                return;
            }

            _player = new SoundPlayer(path);
            _player.Load();    
            _player.Play();    
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                "Could not play the voice greeting:\n" + ex.Message,
                "Sound", System.Windows.MessageBoxButton.OK);
        }
    }
}