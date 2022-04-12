﻿#if VR_BUILDER
using VRBuilder.Core.Behaviors;
using VRBuilder.Editor.UI.StepInspector.Menu;
using VRBuilder.Animations.Behaviors;

namespace VRBuilder.Editor.Animations.UI.Behaviors
{
    /// <inheritdoc />
    public class AnimateTransformMenuItem : MenuItem<IBehavior>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "Animation/Animate Transform";

        /// <inheritdoc />
        public override IBehavior GetNewItem()
        {
            return new AnimateTransformBehavior();
        }
    }
}
#endif