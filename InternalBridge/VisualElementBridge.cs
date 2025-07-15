using UnityEngine;
using UnityEngine.UIElements;
using TextElement = UnityEngine.UIElements.TextElement;

#if !UNITY_6000_3_OR_NEWER
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.TextCore.Text;
using TextAlignment = UnityEngine.TextCore.Text.TextAlignment;
using TextGenerationSettings = UnityEngine.TextCore.Text.TextGenerationSettings;
using TextUtilities = UnityEngine.UIElements.TextUtilities;
#endif

namespace Unity.GraphToolkit.InternalBridge
{
    static class VisualElementBridge
    {
        public static Matrix4x4 GetWorldTransformInverse(this VisualElement ve)
        {
            return ve.worldTransformInverse;
        }

        public static void SetRenderHintsClipWithScissors(this VisualElement ve)
        {
            ve.renderHints = RenderHints.ClipWithScissors;
        }

        public static bool HasFocus<TValue>(this TextInputBaseField<TValue> ve)
        {
            return ve.hasFocus;
        }

        public static void SetCheckedPseudoState(this VisualElement ve)
        {
            ve.pseudoStates |= PseudoStates.Checked;
        }

        public static void ClearCheckedPseudoState(this VisualElement ve)
        {
            ve.pseudoStates &= ~PseudoStates.Checked;
        }

        public static bool GetHoverPseudoState(this VisualElement ve)
        {
            return (ve.pseudoStates & PseudoStates.Hover) == PseudoStates.Hover;
        }

#if !UNITY_6000_3_OR_NEWER
        static TextGenerationSettings s_TextGenerationSettings = new();
#endif

        public static float GetTextWidthWithFontSize(this TextElement element, float fontSize)
        {
#if UNITY_6000_3_OR_NEWER
            return element.MeasureTextSize(element.text, float.NaN, VisualElement.MeasureMode.Undefined, float.NaN, VisualElement.MeasureMode.Undefined, fontSize).x;
#else
            var style = element.computedStyle;

            s_TextGenerationSettings.textSettings = TextUtilities.GetTextSettingsFrom(element);
            if (!s_TextGenerationSettings.textSettings)
                return 0;

            s_TextGenerationSettings.fontAsset = TextUtilities.GetFontAsset(element);
            if (!s_TextGenerationSettings.fontAsset)
                return 0;

#if !GTK_TEXTGENSETTINGS_REMOVED_CONSTANTS
            s_TextGenerationSettings.material = s_TextGenerationSettings.fontAsset.material;
#endif
            s_TextGenerationSettings.fontStyle = TextGeneratorUtilities.LegacyStyleToNewStyle(style.unityFontStyleAndWeight);
            s_TextGenerationSettings.textAlignment = TextGeneratorUtilities.LegacyAlignmentToNewAlignment(style.unityTextAlign);
#if GTK_UNITY_6000_0_B15_OR_NEWER
            s_TextGenerationSettings.textWrappingMode = style.whiteSpace == WhiteSpace.Normal ? TextWrappingMode.Normal : TextWrappingMode.NoWrap;
#else
            s_TextGenerationSettings.wordWrap = style.whiteSpace == WhiteSpace.Normal;
#endif
#if !GTK_TEXTGENSETTINGS_REMOVED_CONSTANTS
            s_TextGenerationSettings.wordWrappingRatio = 0.4f;
#endif
            s_TextGenerationSettings.richText = element.enableRichText;
            s_TextGenerationSettings.overflowMode = TextOverflowMode.Overflow;
            s_TextGenerationSettings.characterSpacing = style.letterSpacing.value;
            s_TextGenerationSettings.wordSpacing = style.wordSpacing.value;
            s_TextGenerationSettings.paragraphSpacing = style.unityParagraphSpacing.value;
#if !GTK_TEXTGENSETTINGS_REMOVED_CONSTANTS
            s_TextGenerationSettings.inverseYAxis = true;
#endif
            s_TextGenerationSettings.text = element.text;
            s_TextGenerationSettings.screenRect = new Rect(0, 0, 32000, 32000);
            s_TextGenerationSettings.fontSize = (int)fontSize;
#if GTK_UNITY_2023_3_B10_OR_NEWER
            var size = TextHandle.generators[JobsUtility.ThreadIndex].GetPreferredValues(s_TextGenerationSettings, TextHandle.textInfoCommon);
#else
            var size = TextGenerator.GetPreferredValues(s_TextGenerationSettings, TextHandle.textInfoCommon);#endif
#endif
            return size.x;
#endif
        }

        public static float GetTextWidth(this TextElement element)
        {
#if GTK_NEW_MEASURETEXT_API
            return element.MeasureTextSize(element.text, float.NaN, VisualElement.MeasureMode.Undefined, float.NaN, VisualElement.MeasureMode.Undefined).x;
#else
            var style = element.computedStyle;

            s_TextGenerationSettings.textSettings = TextUtilities.GetTextSettingsFrom(element);

            FontAsset fontAsset = null;
            if (element.computedStyle.unityFontDefinition.fontAsset != null)
                fontAsset = element.computedStyle.unityFontDefinition.fontAsset;

#if UGTK_UNITY_NEW_GETCACHEDFONTASSET
            if (element.computedStyle.unityFontDefinition.font != null)
                fontAsset = s_TextGenerationSettings.textSettings.GetCachedFontAsset(element.computedStyle.unityFontDefinition.font);
            else if (element.computedStyle.unityFont != null)
                fontAsset = s_TextGenerationSettings.textSettings.GetCachedFontAsset(element.computedStyle.unityFont);
#else
            if (element.computedStyle.unityFontDefinition.font != null)
                fontAsset = s_TextGenerationSettings.textSettings.GetCachedFontAsset(element.computedStyle.unityFontDefinition.font, TextShaderUtilities.ShaderRef_MobileSDF);
            else if (element.computedStyle.unityFont != null)
                fontAsset = s_TextGenerationSettings.textSettings.GetCachedFontAsset(element.computedStyle.unityFont, TextShaderUtilities.ShaderRef_MobileSDF);
#endif

            if (fontAsset == null)
                return 0;

            s_TextGenerationSettings.fontAsset = fontAsset;
#if !GTK_TEXTGENSETTINGS_REMOVED_CONSTANTS
            s_TextGenerationSettings.material = fontAsset.material;
#endif
            s_TextGenerationSettings.fontStyle = TextGeneratorUtilities.LegacyStyleToNewStyle(style.unityFontStyleAndWeight);
            s_TextGenerationSettings.textAlignment = TextAlignment.MiddleLeft;
#if GTK_UNITY_6000_0_B15_OR_NEWER
            s_TextGenerationSettings.textWrappingMode = TextWrappingMode.NoWrap;
#else
            s_TextGenerationSettings.wordWrap = false;
#endif
            s_TextGenerationSettings.overflowMode = TextOverflowMode.Overflow;
#if !GTK_TEXTGENSETTINGS_REMOVED_CONSTANTS
            s_TextGenerationSettings.inverseYAxis = true;
#endif
            s_TextGenerationSettings.text = element.text;
            s_TextGenerationSettings.screenRect = new Rect(0, 0, 32000, 32000);
            s_TextGenerationSettings.fontSize = (int)element.computedStyle.fontSize.value;
            if (s_TextGenerationSettings.fontSize == 0)
                return 0;

#if GTK_UNITY_2023_3_B10_OR_NEWER
            var size = TextHandle.generators[JobsUtility.ThreadIndex].GetPreferredValues(s_TextGenerationSettings, TextHandle.textInfoCommon);
#else
            var size = TextGenerator.GetPreferredValues(s_TextGenerationSettings, TextHandle.textInfoCommon);
#endif
            return Mathf.Ceil(size.x);
#endif
        }

        public static Color GetPlayModeTintColor(this VisualElement ve)
        {
            return ve.playModeTintColor;
        }
    }
}
