using System;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using EditBookLib.Join;
using WpfBase.ViewModels;

namespace JoinPageGui.ViewModels;

public class MainViewModel : ViewModelBase
{
  public static string Title => "画像結合";
  private const int MessageLineCount = 20;
  
  public ReactiveProperty<bool> Close { get; }
  public ReactiveProperty<string> Input { get; }
  public ReactiveProperty<string> Output { get; }
  public ReactiveProperty<string> HeightSize { get; }
  public ReactiveProperty<bool> IsNoJoin { get; }
  
  /// <summary>
  /// 実行ボタン
  /// </summary>
  public ReactiveProperty<bool> IsExecuteCommand { get; }
  public ReactiveCommand ExecuteCommand { get; }
  /// <summary>
  /// 閉じるボタン
  /// </summary>
  public ReactiveCommand CloseCommand { get; }

  private List<string> _messageLine;

  public ReactiveProperty<string> Message { get; }

  public MainViewModel()
  {
    Close = new ReactiveProperty<bool>(false);
    
    Input = new ReactiveProperty<string>(@"").AddTo(Disposable);
    Output = new ReactiveProperty<string>(@"Z:\wuxga").AddTo(Disposable);
    HeightSize = new ReactiveProperty<string>("1600").AddTo(Disposable);
    IsNoJoin = new ReactiveProperty<bool>(false).AddTo(Disposable);
    
    IsExecuteCommand = new ReactiveProperty<bool>(true).AddTo(Disposable);
    // ExecuteCommand = new ReactiveCommand().AddTo(Disposable);
    ExecuteCommand = IsExecuteCommand.ToReactiveCommand();
    ExecuteCommand.Subscribe(_ => Process());
    
    CloseCommand = new ReactiveCommand().AddTo(Disposable);
    CloseCommand.Subscribe(_ => Close.Value = true);
    
    _messageLine = [];
    Message = new ReactiveProperty<string>().AddTo(Disposable);
  }

  private void WriteLog(string log)
  {
    if (_messageLine.Count >= MessageLineCount)
    {
      _messageLine = _messageLine.Skip(_messageLine.Count - MessageLineCount).ToList();
    }

    _messageLine.Add(log);
    Message.Value = string.Join(Environment.NewLine, _messageLine);
  }

  private void Process()
  {
    if (Input.Value == "") return;
    if (Output.Value == "") return;
    if (!int.TryParse(HeightSize.Value, out var height)) return;

    if (IsNoJoin.Value)
    {
      Task.Run(NoJoinImage);
    }
    else
    {
      Task.Run(JoinImage);
    }
  }

  private void JoinImage()
  {
    IsExecuteCommand.Value = false;
    if (!int.TryParse(HeightSize.Value, out var height)) { return; }
    ProcDir.JoinRunDir(Input.Value, Output.Value, height, WriteLog);
    IsExecuteCommand.Value = true;
  }

  private void NoJoinImage()
  {
    IsExecuteCommand.Value = false;
    if (!int.TryParse(HeightSize.Value, out var height)) { return; }
    
    ProcDir.NoJoinRunDir(Input.Value, Output.Value, height, WriteLog);
    
    IsExecuteCommand.Value = true;
  }
}