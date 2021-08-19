using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace AsyncImageLoading
{

    //https://docs.microsoft.com/en-us/archive/msdn-magazine/2014/march/async-programming-patterns-for-asynchronous-mvvm-applications-data-binding
    public sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
    {
        public NotifyTaskCompletion(Task<TResult> task)
        {
            Task = task;
            if (!task.IsCompleted)
            {
                var _ = WatchTaskAsync(task);
            }
        }

        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch
            {
            }
            var propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged(this, new PropertyChangedEventArgs("Status"));
            propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
            propertyChanged(this, new PropertyChangedEventArgs("IsNotCompleted"));
            if (task.IsCanceled)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
            }
            else if (task.IsFaulted)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
                propertyChanged(this, new PropertyChangedEventArgs("Exception"));
                propertyChanged(this,
                  new PropertyChangedEventArgs("InnerException"));
                propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
            }
            else
            {
                propertyChanged(this,
                  new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
                propertyChanged(this, new PropertyChangedEventArgs("Result"));
            }
        }
        public Task<TResult> Task { get; private set; }
        public TResult Result
        {
            get
            {
                return (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default(TResult);
            }
        }
        public TaskStatus Status { get { return Task.Status; } }
        public bool IsCompleted { get { return Task.IsCompleted; } }
        public bool IsNotCompleted { get { return !Task.IsCompleted; } }
        public bool IsSuccessfullyCompleted
        {
            get
            {
                return Task.Status == TaskStatus.RanToCompletion;
            }
        }
        public bool IsCanceled { get { return Task.IsCanceled; } }
        public bool IsFaulted { get { return Task.IsFaulted; } }
        public AggregateException Exception { get { return Task.Exception; } }
        public Exception InnerException
        {
            get
            {
                return (Exception == null) ? null : Exception.InnerException;
            }
        }
        public string ErrorMessage
        {
            get
            {
                return (InnerException == null) ? null : InnerException.Message;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }


//    one can use the following "skin" to show errors, progress etc
//<Window x:Class= "MainWindow"
//        xmlns= "https://schemas.microsoft.com/winfx/2006/xaml/presentation"
//        xmlns:x= "https://schemas.microsoft.com/winfx/2006/xaml" >
//  < Window.Resources >
//    < BooleanToVisibilityConverter x:Key= "BooleanToVisibilityConverter" />
//  </ Window.Resources >
//  < Grid >
//    < !--Busy indicator -->
//    <Label Content = "Loading..." Visibility= "{Binding UrlByteCount.IsNotCompleted,
//      Converter={ StaticResource BooleanToVisibilityConverter}
//}
//"/>
//< !--Results-- >

//< Label Content = "{Binding UrlByteCount.Result}" Visibility = "{Binding
//      UrlByteCount.IsSuccessfullyCompleted,
//      Converter ={ StaticResource BooleanToVisibilityConverter}}"/>
//< !--Error details-- >

//< Label Content = "{Binding UrlByteCount.ErrorMessage}" Background = "Red"
//      Visibility = "{Binding UrlByteCount.IsFaulted,
//      Converter ={ StaticResource BooleanToVisibilityConverter}}"/>
//</ Grid >
//</ Window >


}
