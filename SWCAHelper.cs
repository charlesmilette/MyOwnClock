using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media;

namespace MyOwnClock
{
    /// <summary>
    /// Helper functions and enums for the undocumented function SetWindowCompositionAttribute.
    /// </summary>
    static class SWCAHelper
    {
        private enum WindowCompositionAttribute
        {
            WCA_ACCENT_POLICY = 19
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WindowCompositionAttributeData
        {
            internal WindowCompositionAttribute Attribute;
            internal IntPtr Data;
            internal int SizeOfData;
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        /// <summary>
        /// The possible states of a window.
        /// </summary>
        public enum AccentState
        {
            /// <summary>Like TRANSPARENTGRADIENT with a GradientColor of 0xFF000000. Ignores provided GradientColor value.</summary>
            /// <seealso cref="ACCENT_ENABLE_TRANSPARENTGRADIENT"/>
            ACCENT_DISABLED = 0,
            /// <summary>Like TRANSPARENTGRADIENT, but ignores the alpha channel of GradientColor.</summary>
            /// <seealso cref="ACCENT_ENABLE_TRANSPARENTGRADIENT"/>
            ACCENT_ENABLE_GRADIENT = 1,
            /// <summary>If the target window has transparent parts, will apply GradientColor to said parts. If it doesn't it will apply GradientColor to all black pixels.</summary>
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            /// <summary>Like TRANSPARENTGRADIENT, but applies a gaussian blur.</summary>
            /// <seealso cref="ACCENT_ENABLE_TRANSPARENTGRADIENT"/>
            ACCENT_ENABLE_BLURBEHIND = 3,
            /// <summary>Like TRANSPARENTGRADIENT with a GradientColor of 0x00000000. Ignores provided GradientColor value.</summary>
            /// <seealso cref="ACCENT_ENABLE_TRANSPARENTGRADIENT"/>
            ACCENT_INVALID_STATE = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AccentPolicy
        {
            internal AccentState AccentState;
            internal int AccentFlags;
            internal uint GradientColor;
            internal int AnimationId;
        }

        /// <summary>
        /// Gracefully allows managed code to easily call SetWindowCompositionAttribute without messing with memory and unsigned integers.
        /// </summary>
        /// <remarks>
        /// If the target window uses system window decorations, the accent policy will bleed over the edges of the decoration.
        /// </remarks>
        /// <param name="hWnd">
        /// The target window's handle.
        /// </param>
        /// <param name="state">
        /// The accent state to apply.
        /// </param>
        /// <param name="color">
        /// The value of GradientColor. Will default to 0x00FFFFFF.
        /// </param>
        /// <seealso cref="AccentState"/>
        public static void SetWindowCompositionAttribute(IntPtr hWnd, AccentState state = AccentState.ACCENT_ENABLE_BLURBEHIND, SolidColorBrush color = null)
        {
            if (color == null)
                color = Brushes.Transparent;

            var hexColor = (uint)((color.Color.A << 24) + (color.Color.B << 16) + (color.Color.G << 8) + color.Color.R);

            var accent = new AccentPolicy()
            {
                AccentState = state,
                AccentFlags = 2,
                GradientColor = hexColor
            };
            var accentStructSize = Marshal.SizeOf(accent);
            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData()
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                Data = accentPtr,
                SizeOfData = accentStructSize
            };

            SetWindowCompositionAttribute(hWnd, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        /// <summary>
        /// Provides a shorthand for SetWindowCompositionAttribute on WindowInteropHelpers.
        /// </summary>
        /// <remarks>
        /// If the target window uses system window decorations, the accent policy will bleed over the edges of the decoration.
        /// </remarks>
        /// <param name="state">
        /// The accent state to apply.
        /// </param>
        /// <param name="color">
        /// The value of GradientColor. Will default to 0x00FFFFFF.
        /// </param>
        /// <seealso cref="SetWindowCompositionAttribute(IntPtr, AccentState, SolidColorBrush)"/>
        /// <seealso cref="AccentState"/>
        public static void SetCompositionAttribute(this WindowInteropHelper window, AccentState state = AccentState.ACCENT_ENABLE_BLURBEHIND, SolidColorBrush color = null) => SetWindowCompositionAttribute(window.Handle, state, color);
    }
}
