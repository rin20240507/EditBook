using System.ComponentModel;
using System.Diagnostics;
using Reactive.Bindings.Disposables;

namespace CropBookGui.ViewModels;

/// <summary>
/// ViewModelのベース
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
{
  public event PropertyChangedEventHandler? PropertyChanged;
  protected void NotifyPropertyChanged(string parameter)
    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(parameter));
    
  // Disposeが必要なReactivePropertyやReactiveCommandを集約させるための仕掛け
  protected CompositeDisposable Disposable { get; } = new CompositeDisposable();
    
  public virtual void Dispose()
  {
    // まとめてDisposeする
    Disposable.Dispose();
    Debug.Write("Dispose");
  }
}