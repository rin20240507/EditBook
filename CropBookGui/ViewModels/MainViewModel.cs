using System.ComponentModel;
using System.Diagnostics;
using EditBookLib.Crop;
using Reactive.Bindings;

namespace CropBookGui.ViewModels;

public class MainViewModel : BindableBase
{
  public ReactiveProperty<bool> IsClosing { get; set; } = new ReactiveProperty<bool>(false);
  public ReactiveProperty<string> InputDir { get; } = new("Z:\\scan");
  public ReactiveProperty<string> OutputDir { get; } = new("Z:\\WORK");
  
  public ReactiveProperty<bool> IsMultiDir { get; } = new(true);
  public ReactiveProperty<bool> IsAllCheck { get; } = new(true);
  
  public ReactiveProperty<bool> IsKindle { get; } = new(true);
  public ReactiveProperty<bool> IsBookLive { get; } = new(false);
  public ReactiveProperty<bool> IsPokeMaga { get; } = new(false);
  public ReactiveProperty<bool> IsCapture { get; } = new(false);
  public ReactiveProperty<bool> IsScanBody { get; } = new(false);
  
  public ReactiveProperty<bool> IsJpeg { get; } = new(true);
  public ReactiveProperty<bool> IsPng { get; } = new(false);
  
  public ReactiveProperty<string> StatusText { get; } = new("");
  public ReactiveProperty<string> ThreadCount { get; } = new("4");
  
  
  public ReactiveCommand ProcCommand { get; } = new();
  public ReactiveCommand ExitCommand { get; } = new();

  public MainViewModel()
  {
    ProcCommand.Subscribe(ProcCommandExecute);
    ExitCommand.Subscribe(ExitCommandExecute);
  }

  private void ProcCommandExecute()
  {
    Task.Run(CropImage);
  }

  private void CropImage()
  {
    Debug.WriteLine(IsKindle.Value);
    StatusText.Value = "";

    if (!int.TryParse(ThreadCount.Value, out var threadCount))
    {
      threadCount = 4;
    }

    ProcType procType;
    if (IsKindle.Value)
    {
      procType = ProcType.Kindle;
    }
    else if (IsBookLive.Value)
    {
      procType = ProcType.BookLive;
    }
    else if (IsPokeMaga.Value)
    {
      procType = ProcType.PokeMaga;
    }
    else if (IsScanBody.Value)
    {
      procType = ProcType.ScanBody;
    }
    else
    {
      procType = ProcType.Capture;
    }
    
    var cls = new ProcDir
    {
      ThreadCount = threadCount,
      IsJpeg = IsJpeg.Value,
      ProcType = procType,
      IsAllCheck = IsAllCheck.Value,
      WriteLog = WriteLog,
    };
    cls.Proc(InputDir.Value, OutputDir.Value, !IsMultiDir.Value);
    
    WriteLog("完了");
  }

  private void WriteLog(string log)
  {
    StatusText.Value = log;
  }

  private void ExitCommandExecute()
  {
    IsClosing.Value = true;
    Environment.Exit(0);
  }
 
}