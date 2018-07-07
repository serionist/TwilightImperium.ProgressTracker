using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TwilightImperium.ProgressTracker
{
    public abstract class CustomWindow:Window
    {
        private bool IsSingleInstance { get; }
        private static readonly Dictionary<Type, CustomWindow> Instances = new Dictionary<Type, CustomWindow>();
        private static readonly object locker = new object();

        protected CustomWindow(bool isSingleInstance, bool closesOnESC)
        {
            IsSingleInstance = isSingleInstance;
            if (closesOnESC)
                this.PreviewKeyDown += HandleEscPressed;
        }

        private void HandleEscPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        public new bool? ShowDialog()
        {
            if (IsSingleInstance)
            {
                bool skipShow = false;
                lock (locker)
                    if (Instances.TryGetValue(this.GetType(), out var instance))
                    {
                        instance.Dispatcher.Invoke(() => instance.Activate());
                        skipShow = true;
                    }
                    else
                        Instances.Add(this.GetType(), this);

                if (skipShow)
                    return false;
            }
            return base.ShowDialog();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (IsSingleInstance)
                lock (locker)
                    if (!e.Cancel && Instances.TryGetValue(this.GetType(), out var i) && i.Equals(this))
                        Instances.Remove(this.GetType());

        }
    }
}
