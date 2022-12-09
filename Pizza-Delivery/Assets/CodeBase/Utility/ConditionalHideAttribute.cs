using System;
using UnityEngine;

namespace CodeBase.Utility
{
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct,
        Inherited = true)]
    public class ConditionalHideAttribute : PropertyAttribute
    {
        //=============================================================================

        //The name of the bool field that will be in control
        public string conditionalSourceField = "";

        //TRUE = Hide in inspector / FALSE = Disable in inspector 
        public bool hideInInspector;

        //=============================================================================

        public ConditionalHideAttribute(string conditionalSourceField)
        {
            this.conditionalSourceField = conditionalSourceField;
            this.hideInInspector = false;
        }

        //=============================================================================

        public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector)
        {
            this.conditionalSourceField = conditionalSourceField;
            this.hideInInspector = hideInInspector;
        }

        //=============================================================================
    }
}

