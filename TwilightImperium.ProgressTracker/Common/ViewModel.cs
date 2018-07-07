using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TwilightImperium.ProgressTracker
{
    public class PropertyGroup : Attribute
    {
        public object[] Groups { get; }

        /// <summary>
        /// Provide one or more (separated by ',' propertygroups)
        /// </summary>
        /// <param name="Group_s"></param>
        public PropertyGroup(string Group_s) : this(Group_s.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Cast<object>().ToArray())
        {
        }

        public PropertyGroup(params object[] groups)
        {
            Groups = groups;

        }
    }
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> execute)
                       : this(execute, null)
        {
        }

        public DelegateCommand(Action execute)
                       : this((o) => execute(), null)
        {
        }

        public DelegateCommand(Action<object> execute,
                       Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
    public abstract class ViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The event called when property changed. Should only be called from baseClass methods
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// We store the properties of current item in the memory to speed up reflection
        /// </summary>
        protected readonly List<PropertyInfo> _properties;
        protected ViewModel()
        {
            _properties = this.GetType().GetProperties().ToList();
        }


        /// <summary>
        /// Calls PropertyChanged for specific property
        /// </summary>
        /// <param name="name"></param>
        public void PropChanged(params string[] names)
        {
            //If propertychanged is not null, it is called
            foreach (var name in names)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        /// <summary>
        /// Calls PropertyChanged for the property where the method was called from.
        ///  If this method is not called from a property or it is called from a property that does not exist in current object, nothing happens
        /// </summary>
        protected void ThisPropChanged([CallerMemberName]string memberName = "")
        {
            if (!string.IsNullOrEmpty(memberName))
            {
                PropChanged(memberName);
            }
        }
        /// <summary>
        /// Calls PropertyChanged event for all properties in the class that contain 'PropertyGroup' attribute which contains the name of the desired group
        /// </summary>
        /// <param name="GroupName"></param>
        public void PropGroupChanged(params object[] groups)
        {
            foreach (var prop in _properties)
                foreach (var group in groups)
                {
                    var atts = prop.GetCustomAttributes(true)?.ToList();
                    var att = atts?.Find(e => e is PropertyGroup);
                    if (att == null) continue;
                    if (!(att is PropertyGroup)) continue;
                    if ((att as PropertyGroup).Groups.All(e => e is string) && group is string)
                    {
                        if ((att as PropertyGroup).Groups.Cast<string>().ToList().Exists(e =>
                            String.Equals(e, group as string, StringComparison.CurrentCultureIgnoreCase)))
                            PropChanged(prop.Name);

                    }
                    else
                        if ((att as PropertyGroup).Groups.Contains(group))
                        PropChanged(prop.Name);



                }
        }
        /// <summary>
        /// Calls the ProeprtyChanged event for all properties.
        /// </summary>
        public void AllPropsChanged()
        {
            foreach (var prop in _properties)
            {
                PropChanged(prop.Name);
            }
        }


        //protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        //{
        //    if (object.Equals(storage, value)) return false;

        //    storage = value;
        //    this.PropChanged(propertyName);

        //    return true;
        //}
    }

    public abstract class ViewModel<T> : ViewModel where T : class
    {
        protected T ParentModel { get; private set; }
        protected ViewModel(T Parent)
        {

            UpdateProperties(Parent);
        }

        public void UpdateProperties(T Parent)
        {
            ParentModel = Parent;
            var parentProps = ParentModel.GetType().GetProperties().ToList();
            foreach (var prop in _properties.FindAll(e => e.CanWrite))
            {
                var parentProp = parentProps.Find(e => e.Name == prop.Name);
                if (parentProp != null)
                {
                    try
                    {
                        prop.SetValue(this, parentProp.GetValue(ParentModel, null), null);
                    }
                    catch
                    {

                    }
                }
            }
        }

        public void UpdateSource()
        {
            var parentProps = ParentModel.GetType().GetProperties().ToList();
            foreach (var prop in _properties.FindAll(e => e.CanWrite))
            {
                var parentProp = parentProps.Find(e => e.Name == prop.Name);
                if (parentProp != null)
                {
                    try
                    {
                        parentProp.SetValue(ParentModel, prop.GetValue(this, null), null);
                    }
                    catch
                    {

                    }
                }
            }
        }

        public bool IsInSyncWithSource()
        {
            var parentProps = ParentModel.GetType().GetProperties().ToList();
            foreach (var prop in _properties.FindAll(e => e.CanWrite))
            {
                var parentProp = parentProps.Find(e => e.Name == prop.Name);
                if (parentProp != null)
                {

                    try
                    {
                        var oldV = parentProp.GetValue(ParentModel, null);
                        var newV = prop.GetValue(this, null);
                        if (!oldV.Equals(newV))
                        {
                            return false;
                        }

                    }
                    catch
                    {

                    }
                }
            }
            return true;
        }
    }
    public abstract class ChildViewModel<TViewParent> : ViewModel where TViewParent : ViewModel
    {
        [JsonIgnore]
        public TViewParent Parent { get; }
        protected ChildViewModel(TViewParent parent)
        {
            Parent = parent;
        }

    }

}
