using UnityEngine;
using VRBuilder.Core.ProcessUtils;

namespace VRBuilder.StatesAndData.ProcessUtils
{
    /// <summary>
    /// Multiplies left by right.
    /// </summary>
    public class MaxOperation : IOperationCommand<float, float>
    {
        /// <inheritdoc/>
        public float Execute(float leftOperand, float rightOperand)
        {
            return Mathf.Max(leftOperand, rightOperand);
        }
    }
}