
using System;

namespace System.Windows.Forms
{
    public class DListView :ListView
    {   
        public DListView()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint,
                     true);
            UpdateStyles();
        }
    }
    public class DFlowLayoutPanel : FlowLayoutPanel
    {
        public DFlowLayoutPanel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint,
                     true);
            UpdateStyles();
        }
    }
    public class DListBox : ListBox
    {
        public DListBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
         ControlStyles.AllPaintingInWmPaint,
         true);
            UpdateStyles();
        }
    }
}