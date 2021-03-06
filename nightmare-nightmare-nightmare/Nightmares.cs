﻿using System;
using System.IO;
using System.Collections.Generic;
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
    private OkayBoomer.OkayBoomer okayBoomer {get; set;}

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
      okayBoomer = new OkayBoomer.OkayBoomer(){};

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
      int numInputs = inputs.Length;

      //List to keep track of clips to add to txt file later
      List<string> finishedClips = new List<string>(){};

      Console.WriteLine($"About to create {numClips} clips from {numInputs} videos");

      for(int i = 0; i < numClips; i++)
      {
        int inputsIndex = okayBoomer.GetRandom(numInputs);
        
        Console.WriteLine($"i = {i}, numInputs = {numInputs}, inputs = {inputs}, inputs.Length > inputsIndex =  {inputs.Length > inputsIndex}, inputsIndex = {inputsIndex}");
        if(inputs.Length > inputsIndex)
          finishedClips.Add(CreateClip(inputs[inputsIndex], $"clip{i}"));
      }

      Console.WriteLine($"Created {numClips} clips at {_outputFolder}");

      //Add clips to a txt file
      File.WriteAllLines("./Vids/AllClips.txt", finishedClips);

      Console.WriteLine($"Creating final product at: {_finalProductLocation}");
      //Create Final Product
      ShellHelper.Bash($"ffmpeg -f concat -safe 0 -i ./Vids/AllClips.txt -c copy {_finalProductLocation}/finalProduct.mov");

      Console.WriteLine("Gott'im");
    }

    ///<summary>
    /// Can't be bothered to actually learn how to use
    /// ffmpeg, so instead I just searched around on google
    /// for some bash commands to do what I want and then
    /// pasted them here into the ShellHelper I stole
    ///
    /// I have no idea what all the flags on these commands do or what these commands mean
    ///</summary>
    public string CreateClip(string inputFileName, string outputFileName)
    {
      outputFileName = _outputFolder + "/" + outputFileName;

      //Get the maximum length of the input so we know how far into the video we can clip
      string inputLengthString = ShellHelper.Bash($"ffprobe -i {inputFileName} -show_format -v quiet | sed -n 's/duration=//p'");

      //Get it into a format to pass to the token boomer
      int start;
      if(Int32.TryParse(inputLengthString, out int inputLength))
      {
        start = okayBoomer.GetRandom(inputLength);
        //If the start is too close to the end
        //set it to something that will still
        //give the same clip duration
        start = start >= inputLength - _clipDuration ? 0 : inputLength - _clipDuration;
      }
      else
        start = 0;

      //Actually go and create the clip now
      ShellHelper.Bash($"ffmpeg -ss {start} -i {inputFileName} -t {_clipDuration} -c copy {outputFileName + _videoType}");

      //Return file name in format that concatonation script needs
      return "file '" + outputFileName.Substring(7, outputFileName.Length-7) + ".mov'";
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
  }
}
