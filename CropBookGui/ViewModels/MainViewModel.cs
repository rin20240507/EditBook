using System.Diagnostics;
using System.Windows.Documents;
using EditBookLib.Crop;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace CropBookGui.ViewModels;

public class MainViewModel : WpfBase.ViewModels.ViewModelBase
{
  public string Title => "余白削除";
  public ReactiveProperty<bool> Close { get; set; } = new ReactiveProperty<bool>(false);
  public ReactiveProperty<string> InputDir { get; } = new("Z:\\scan");
  public ReactiveProperty<string> OutputDir { get; } = new("Z:\\WORK");
  
  public ReactiveProperty<bool> IsMultiDir { get; } = new(true);
  public ReactiveProperty<bool> IsAllCheck { get; } = new(true);
  
  public ReactiveProperty<bool> IsKindle { get; } = new(true);
  public ReactiveProperty<bool> IsBookLive { get; } = new(false);
  public ReactiveProperty<bool> IsPokeMaga { get; } = new(false);
  public ReactiveProperty<bool> IsFanza4K { get; } = new(false);
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
    var infoList = this.GetType().GetProperties();
    foreach (var propertyInfo in infoList)
    {
      if (propertyInfo.PropertyType == typeof(ReactiveProperty<bool>))
      {
        var val =  propertyInfo.GetValue(this, null) as ReactiveProperty<bool>;
        val?.AddTo(Disposable);
      }
      else if (propertyInfo.PropertyType == typeof(ReactiveProperty<string>))
      {
        var val =  propertyInfo.GetValue(this, null) as ReactiveProperty<string>;
        val?.AddTo(Disposable);
      }
      else if (propertyInfo.PropertyType == typeof(ReactiveCommand))
      {
        var val =  propertyInfo.GetValue(this, null) as ReactiveCommand;
        val?.AddTo(Disposable);
      }

      // if (propertyInfo.PropertyType.IsGenericType)
      // var generic = propertyInfo.PropertyType.GetGenericTypeDefinition();
      // Debug.WriteLine($"Name:[{propertyInfo.Name}]");
      // Debug.WriteLine($"Type:[{propertyInfo.PropertyType.Name}]");
      // Debug.WriteLine($"IsGenericType:[{propertyInfo.PropertyType.IsGenericType}]");
      // if (propertyInfo.PropertyType.IsGenericType)
      // {
      //   Debug.WriteLine($"GenericType:[{propertyInfo.PropertyType.GetGenericArguments()[0].Name}]");
      //   Debug.WriteLine($"IsReactiveProperty<>:[{propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(ReactiveProperty<>)}]");
      //   if (propertyInfo.PropertyType == typeof(ReactiveProperty<bool>))
      //   {
      //     var val =  propertyInfo.GetValue(this, null) as ReactiveProperty<bool>;
      //   }
      // }
      // this.GetType().GetProperty(propertyInfo.Name)?.SetValue(this, propertyInfo.GetValue(this, null));
      
    }
    
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
    else if (IsFanza4K.Value)
    {
      procType = ProcType.Fanza4K;
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
    Close.Value = true;
  }
 
}