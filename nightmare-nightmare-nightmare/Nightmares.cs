using System;
using System.IO;
using System.Diagnostics;
using OkayBoomer;

///<summary>
/// Uses ffmpeg to cut video clips out
/// to smush together in order to trully
/// have one button randomized solution
/// to content upload
///</summary>
namespace nightmare_nightmare_nightmare
{
  public class Nightmares
  {
    private readonly string _inputFolder;
    private readonly string _outputFolder;
    private readonly string _finalProductLocation;
    private readonly int _clipDuration;
    private readonly int _finalDuration;
    private readonly string _videoType;

    private int iterator {get; set;}

    public Nightmares(int finalDuration = 300, string inputFolder = "", string outputFolder = "", string finalProductLocation = "", int clipDuration = 3, string videoType = ".mov")
    {
      Console.WriteLine("Nightmare Nightmare Nightmare");

      //Initializing stuff and things
      _inputFolder = DetectAndCreateDir(String.IsNullOrEmpty(inputFolder) ? "./Vids/Inputs" : inputFolder);
      _outputFolder = DetectAndCreateDir(String.IsNullOrEmpty(outputFolder) ? "./Vids/Outputs" : outputFolder);
      _finalProductLocation = DetectAndCreateDir(String.IsNullOrEmpty(_finalProductLocation) ? "./Vids/FinalProduct" : finalProductLocation);
      _clipDuration = clipDuration;
      _videoType = videoType;
      _finalDuration = finalDuration;
      
      GetClips();

      Console.WriteLine("Nightmare over, but you'll never sleep the same again.");
    }

    ///<summary>
    /// Use the _finalDuration and _clipDuration to figure out how many
    /// clips are needed and then start randomly generating clips
    ///</summary>
    public void GetClips()
    {
      decimal finalDuration = Convert.ToDecimal(_finalDuration);
      decimal clipDuration = Convert.ToDecimal(_clipDuration);

      int numClips = Decimal.ToInt32(Math.Floor(finalDuration/clipDuration));
      string[] inputs = Directory.GetFiles(_inputFolder, "*" + _videoType);

      Console.WriteLine($"inputs = {inputs[0]}");

      for(int i = 0; i < numClips; i++)
      {
        
      }
    }

    public string DetectAndCreateDir(string dir)
    {
      if(!Directory.Exists(dir))
      {
        Console.WriteLine($"Creating Directory: {dir}");
        Directory.CreateDirectory(dir);
      }
      return dir;
    }

    public void Testing123()
    {
      int start = 1;
      int duration = 2;
      string inputFileName = "./Vids/copy.mov";
      string outputFileName = "./Vids/output.mov";
      //Console Command courtesy of Mark Heath from markheath.net
      string consoleScript = $"ffmpeg -ss {start} -i {inputFileName} -t {duration} -c copy {outputFileName}";
      //Console Command Courtesy of Stack Overflow user: evilsoup
      string durationScript = $"ffprobe -i {inputFileName} -show_format -v quiet | sed -n 's/duration=//p'";

      ShellHelper.Bash(consoleScript);
      Console.WriteLine($"Duration of input: {ShellHelper.Bash(durationScript)}");

      //Command to combine video files listed in a txt
      //Courtesy of ffmpeg wiki
      //ffmpeg -f concat -safe 0 -i mylist.txt -c copy output.wav
    }
  }
}
