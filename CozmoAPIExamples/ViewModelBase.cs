using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace CozmoAPIExamples
{
    public class ViewModelBase : INotifyPropertyChanged, ICommand        
    {
        public void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand CommandLink
        {
            get { return this; }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        protected virtual void DoCommand(object parameter)
        {
        }

        public void Execute(object parameter)
        {
            DoCommand(parameter);
        }
    }
}
