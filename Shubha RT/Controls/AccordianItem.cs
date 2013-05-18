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
    /// AccordianItem
    /// </summary>
    public class AccordianItem : HeaderedContentControl
    {
        #region IsExpanded

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
            "IsExpanded", typeof(bool), typeof(AccordianItem), new PropertyMetadata(false, new PropertyChangedCallback(OnIsExpandedChanged)));

        private static void OnIsExpandedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            AccordianItem item = sender as AccordianItem;
            if (item != null)
            {
                item.OnIsExpandedChanged(e);
            }
        }

        protected virtual void OnIsExpandedChanged(DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;

            if (newValue)
            {
                this.OnExpanded();
            }
            else
            {
                this.OnCollapsed();
            }
        } 

        #endregion

        #region Expand Events

        /// <summary>
        /// Raised when selected
        /// </summary>
        public event RoutedEventHandler Expanded
        {
            add { AddHandler(ExpandedEvent, value); }
            remove { RemoveHandler(ExpandedEvent, value); }
        }

        public static RoutedEvent ExpandedEvent = EventManager.RegisterRoutedEvent(
            "Expanded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AccordianItem));

        /// <summary>
        /// Raised when unselected
        /// </summary>
        public event RoutedEventHandler Collapsed
        {
            add { AddHandler(CollapsedEvent, value); }
            remove { RemoveHandler(CollapsedEvent, value); }
        }

        public static RoutedEvent CollapsedEvent = EventManager.RegisterRoutedEvent(
            "Collapsed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AccordianItem));

        protected virtual void OnExpanded()
        {
            Accordian parentAccordian = this.ParentAccordian;
            if (parentAccordian != null)
            {
                parentAccordian.ExpandedItem = this;
            }
            RaiseEvent(new RoutedEventArgs(ExpandedEvent, this));
        }

        protected virtual void OnCollapsed()
        {
            RaiseEvent(new RoutedEventArgs(CollapsedEvent, this));            
        }

        #endregion

        #region ExpandCommand

        public static RoutedCommand ExpandCommand = new RoutedCommand("Expand", typeof(AccordianItem));

        private static void OnExecuteExpand(object sender, ExecutedRoutedEventArgs e)
        {
            AccordianItem item = sender as AccordianItem;
            if (!item.IsExpanded)
            {
                item.IsExpanded = true;
            }
        }

        private static void CanExecuteExpand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = sender is AccordianItem;
        }

        #endregion

        #region ParentAccordian

        private Accordian ParentAccordian
        {
            get { return ItemsControl.ItemsControlFromItemContainer(this) as Accordian; }
        } 

        #endregion

        #region Constructor

        static AccordianItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AccordianItem), new FrameworkPropertyMetadata(typeof(AccordianItem)));

            CommandBinding expandCommandBinding = new CommandBinding(ExpandCommand, OnExecuteExpand, CanExecuteExpand);
            CommandManager.RegisterClassCommandBinding(typeof(AccordianItem), expandCommandBinding);
        }    

        #endregion     
    }
}
