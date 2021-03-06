using VRBuilder.Core.Conditions;
using VRBuilder.Editor.UI.StepInspector.Menu;

namespace VRBuilder.Editor.StatesAndData.UI.Conditions
{
    /// <inheritdoc />
    public class CompareTextMenuItem : MenuItem<ICondition>
    {
        /// <inheritdoc />
        public override string DisplayedName { get; } = "States and Data/Compare Text";

        /// <inheritdoc />
        public override ICondition GetNewItem()
        {
            return new CompareValuesCondition<string>("Compare Text");
        }
    }
}
