using System;
using System.Windows.Input;

namespace XBee.Util
{
    public class RelayCommand : ICommand
    {
        /// <summary>
        ///     The action (or parameterized action) that will be called when the command is invoked.
        /// </summary>
        protected Action Action = null;

        protected Action<object> ParameterizedAction = null;

        /// <summary>
        ///     Bool indicating whether the command can execute.
        /// </summary>
        private readonly Func<bool> _canExecute;



        /// <summary>
        ///     Initializes a new instance of the <see cref="RelayCommand" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="canExecute">if set to <c>true</c> [can execute].</param>
        public RelayCommand(Action action, Func<bool> canExecute = null)
        {
            //  Set the action.
            Action = action;
            _canExecute = canExecute;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RelayCommand" /> class.
        /// </summary>
        /// <param name="parameterizedAction">The parameterized action.</param>
        /// <param name="canExecute">if set to <c>true</c> [can execute].</param>
        public RelayCommand(Action<object> parameterizedAction, Func<bool> canExecute = null)
        {
            //  Set the action.
            ParameterizedAction = parameterizedAction;
            _canExecute = canExecute;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance can execute.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance can execute; otherwise, <c>false</c>.
        /// </value>
        public bool CanExecute
        {
            get { return _canExecute(); }
        }

        public void UpdateCanExecute()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command.
        ///     If the command does not require data to be passed,
        ///     this object can be set to null.
        /// </param>
        /// <returns>
        ///     true if this command can be executed; otherwise, false.
        /// </returns>
        bool ICommand.CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        /// <summary>
        ///     Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command.
        ///     If the command does not require data to be passed,
        ///     this object can be set to null.
        /// </param>
        void ICommand.Execute(object parameter)
        {
            InvokeAction(parameter);
        }

        /// <summary>
        ///     Occurs when can execute is changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;


        protected void InvokeAction(object param)
        {
            Action theAction = Action;
            Action<object> theParameterizedAction = ParameterizedAction;
            if (theAction != null)
                theAction();
            else if (theParameterizedAction != null)
                theParameterizedAction(param);
        }
    }
}
