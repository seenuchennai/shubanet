using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AccordianDemo.Controls
{
    /// <summary>
    /// Accordian
    /// </summary>
    public class Accordian : ItemsControl
    {
        #region ExpandedItem

        /// <summary>
        /// Gets/Sets which item to expand
        /// </summary>
        public object ExpandedItem
        {
            get { return (object)GetValue(ExpandedItemProperty); }
            set { SetValue(ExpandedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpandedItemProperty = DependencyProperty.Register(
            "ExpandedItem", typeof(object), typeof(Accordian),
            new UIPropertyMetadata(null, new PropertyChangedCallback(OnExpandedItemChanged)));

        private static void OnExpandedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Accordian shelf = sender as Accordian;
            if (shelf != null)
            {
                shelf.OnExpandedItemChanged(e.OldValue, e.NewValue);
            }
        }

        protected virtual void OnExpandedItemChanged(object oldValue, object newValue)
        {
            AccordianItem oldItem = this.ItemContainerGenerator.ContainerFromItem(oldValue) as AccordianItem;

            if (oldItem != null)
            {
                oldItem.IsExpanded = false;
            }
        }

        #endregion

        #region Constructors

        static Accordian()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Accordian), new FrameworkPropertyMetadata(typeof(Accordian)));
        } 

        #endregion

        #region Overrides

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new AccordianItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is AccordianItem;
        }

        #endregion
    }
}
