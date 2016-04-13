using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace SSMSAddin
{

    internal static class Interop
    {

        internal static Control GetControl(int root, string name)
        {
            try
            {
                return Enumerable.FirstOrDefault<Control>(Enumerable.Select<Control, Control>(Enumerable.Where<Control>(Enumerable.Select<IntPtr, Control>(Enumerable.Select<IntPtr, IntPtr>(Interop.GetChildWindows(new IntPtr(root)), (Func<IntPtr, IntPtr>)(p =>
                {
                    return p;
                })), (Func<IntPtr, Control>)(pp => Control.FromHandle(pp) ?? Control.FromChildHandle(pp))), (Func<Control, bool>)(ctrl => ctrl != null)), (Func<Control, Control>)(p =>
                {
                    return p;
                })), (Func<Control, bool>)(ctrl => ctrl.Name == name));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal static IEnumerable<Control> GetControls(int root, string name)
        {
            try
            {
                return Enumerable.Where<Control>(Enumerable.Where<Control>(Enumerable.Select<IntPtr, Control>(Enumerable.Select<IntPtr, IntPtr>(Interop.GetChildWindows(new IntPtr(root)), (Func<IntPtr, IntPtr>)(p =>
                {
                    return p;
                })), (Func<IntPtr, Control>)(pp => Control.FromHandle(pp) ?? Control.FromChildHandle(pp))), (Func<Control, bool>)(ctrl => ctrl != null)), (Func<Control, bool>)(ctrl => ctrl.Name == name));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal static T GetControl<T>(Control c) where T : Control
        {
            if (c == null)
                return default(T);
            if (c is T)
                return (T)c;
            return Enumerable.FirstOrDefault<T>(Enumerable.Select<Control, T>(Enumerable.Cast<Control>((IEnumerable)c.Controls), (Func<Control, T>)(child => Interop.GetControl<T>(child))), (Func<T, bool>)(ret => (object)ret != null));
        }

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, Interop.EnumWindowProc callback, IntPtr i);

        private static IEnumerable<IntPtr> GetChildWindows(IntPtr parent)
        {
            try
            {
                List<IntPtr> list = new List<IntPtr>();
                GCHandle gcHandle = GCHandle.Alloc((object)list);
                try
                {
                    Interop.EnumWindowProc callback = new Interop.EnumWindowProc(Interop.EnumWindow);
                    Interop.EnumChildWindows(parent, callback, GCHandle.ToIntPtr(gcHandle));
                }
                finally
                {
                    if (gcHandle.IsAllocated)
                        gcHandle.Free();
                }
                return (IEnumerable<IntPtr>)list;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            List<IntPtr> list = GCHandle.FromIntPtr(pointer).Target as List<IntPtr>;
            if (list == null)
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            list.Add(handle);
            return true;
        }

        public delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);
    }
}