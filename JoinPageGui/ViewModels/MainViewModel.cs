using System;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using EditBookLib.Join;

namespace JoinPageGui.ViewModels;

public class MainViewModel : ViewModelBase
{
  public static string Title => "画像結合";
  
  public ReactiveProperty<bool> Close { get; }
  public ReactiveProperty<string> InputDirectory { get; }
  public ReactiveProperty<string> OutputDirectory { get; }
  public ReactiveProperty<string> Height { get; }
  
  public ReactiveCommand ProcessCommand { get; }
  public ReactiveCommand CloseCommand { get; }
  
  public ReactiveCollection<string> ProcessList { get; }
  public ReactiveProperty<string> Status { get; }

  public MainViewModel()
  {
    Close = new ReactiveProperty<bool>(false);
    
    InputDirectory= new ReactiveProperty<string>("").AddTo(Disposable);
    OutputDirectory = new ReactiveProperty<string>(@"Z:\wuxga").AddTo(Disposable);
    Height = new ReactiveProperty<string>("1600").AddTo(Disposable);
    
    ProcessCommand = new ReactiveCommand().AddTo(Disposable);
    ProcessCommand.Subscribe(_ => Process());
    
    CloseCommand = new ReactiveCommand().AddTo(Disposable);
    CloseCommand.Subscribe(_ => CloseWindow());
    
    ProcessList = new ReactiveCollection<string>().AddTo(Disposable);
    Status = new ReactiveProperty<string>().AddTo(Disposable);
  }

  private void WriteLog(string log)
  {
    // ProcessList.Add(log);
    Status.Value = log;
  }
  

  private void Process()
  {
    if (InputDirectory.Value == "") return;
    if (OutputDirectory.Value == "") return;
    if (!int.TryParse(Height.Value, out var height)) return;

    Task.Run(JoinImage);
  }

  private void JoinImage()
  {
    if (!int.TryParse(Height.Value, out var height)) { return; }
    ProcDir.JoinRunDir(InputDirectory.Value, OutputDirectory.Value, height, WriteLog);
  }

  private void CloseWindow()
  {
    Close.Value = true;
  }
}