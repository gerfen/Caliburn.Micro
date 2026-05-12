namespace Caliburn.Micro
{
    using System;
using Microsoft.UI.Xaml;

    /// <summary>
    /// Represents a parameter of an <see cref="ActionMessage"/>.
    /// </summary>
public class Parameter : DependencyObject, IAttachedObject
{
    DependencyObject _associatedObject;
        WeakReference _owner;

        /// <summary>
        /// A dependency property representing the parameter's value.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(object),
                typeof(Parameter),
                new PropertyMetadata(null, OnValueChanged)
                );

        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

DependencyObject IAttachedObject.AssociatedObject
{
            get { return _associatedObject; }
        }


        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        internal ActionMessage Owner
        {
            get { return _owner == null ? null : _owner.Target as ActionMessage; }
            set { _owner = new WeakReference(value); }
        }

void IAttachedObject.Attach(DependencyObject dependencyObject)
{
            _associatedObject = dependencyObject;
        }

        void IAttachedObject.Detach()
        {
            _associatedObject = null;
        }

        /// <summary>
        /// Makes the parameter aware of the <see cref="ActionMessage"/> that it's attached to.
        /// </summary>
        /// <param name="actionMessageOwner">The action message.</param>
        internal void MakeAwareOf(ActionMessage actionMessageOwner)
        {
            Owner = actionMessageOwner;
        }

        internal static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var parameter = (Parameter)d;
            var owner = parameter.Owner;

            if (owner != null)
            {
                owner.UpdateAvailability();
            }
        }
    }
}
