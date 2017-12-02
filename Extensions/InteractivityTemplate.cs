using System.Collections.Generic;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// <see cref="FrameworkTemplate"/> for InteractivityElements instance
    /// <remarks>can't use <see cref="FrameworkTemplate"/> directly due to some internal abstract member</remarks>
    /// </summary>
    public class InteractivityTemplate : DataTemplate
    {
    }

    /// <summary>
    /// Holder for interactivity entries
    /// </summary>
    public class InteractivityItems : FrameworkElement
    {
        private List<Behavior> _behaviors;
        private List<TriggerBase> _triggers;

        /// <summary>
        /// Storage for triggers
        /// </summary>
        public new List<TriggerBase> Triggers
        {
            get { return _triggers ?? (_triggers = new List<TriggerBase>()); }
        }

        /// <summary>
        /// Storage for Behaviors
        /// </summary>
        public List<Behavior> Behaviors
        {
            get { return _behaviors ?? (_behaviors = new List<Behavior>()); }
        }

        #region Template attached property

        public static InteractivityTemplate GetTemplate(DependencyObject obj)
        {
            return (InteractivityTemplate)obj.GetValue(TemplateProperty);
        }

        public static void SetTemplate(DependencyObject obj, InteractivityTemplate value)
        {
            obj.SetValue(TemplateProperty, value);
        }

        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.RegisterAttached("Template", typeof(InteractivityTemplate), typeof(InteractivityItems),
                                                new PropertyMetadata(default(InteractivityTemplate), OnTemplateChanged));

        private static void OnTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DependencyObject target = d;
            if (e.OldValue != null)
            {
                InteractivityTemplate interactivityTemplate = (InteractivityTemplate)e.OldValue;
                InteractivityItems interactivityItems = (InteractivityItems)interactivityTemplate.LoadContent();
                BehaviorCollection behaviorCollection = Interaction.GetBehaviors(target);
                TriggerCollection triggerCollection = Interaction.GetTriggers(target);

                interactivityItems.Behaviors.ForEach(x => behaviorCollection.Remove(x));
                interactivityItems.Triggers.ForEach(x => triggerCollection.Remove(x));
            }

            if (e.NewValue != null)
            {
                InteractivityTemplate interactivityTemplate = (InteractivityTemplate)e.NewValue;
                interactivityTemplate.Seal();
                InteractivityItems interactivityItems = (InteractivityItems)interactivityTemplate.LoadContent();
                BehaviorCollection behaviorCollection = Interaction.GetBehaviors(target);
                TriggerCollection triggerCollection = Interaction.GetTriggers(target);

                interactivityItems.Behaviors.ForEach(behaviorCollection.Add);
                interactivityItems.Triggers.ForEach(triggerCollection.Add);
            }
        }

        #endregion Template attached property
    }
}