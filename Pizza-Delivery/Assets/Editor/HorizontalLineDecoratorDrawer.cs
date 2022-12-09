using _Game.CodeBase.Utility;
using UnityEngine;
using UnityEditor;

namespace PXELDAR
{
    [CustomPropertyDrawer(typeof(HorizontalLineAttribute))]
    public class HorizontalLineDecoratorDrawer : DecoratorDrawer
    {
        //===============================================================================================

        public override float GetHeight()
        {
            HorizontalLineAttribute lineAttribute = (HorizontalLineAttribute)attribute;
            return EditorGUIUtility.singleLineHeight + lineAttribute.Height;
        }

        //===============================================================================================

        public override void OnGUI(Rect position)
        {
            Rect rect = EditorGUI.IndentedRect(position);
            rect.y += EditorGUIUtility.singleLineHeight / 3.0f;
            HorizontalLineAttribute lineAttribute = (HorizontalLineAttribute)attribute;
            rect.height = lineAttribute.Height;
            EditorGUI.DrawRect(rect, lineAttribute.Color.GetColor());
        }

        //===============================================================================================

    }
}