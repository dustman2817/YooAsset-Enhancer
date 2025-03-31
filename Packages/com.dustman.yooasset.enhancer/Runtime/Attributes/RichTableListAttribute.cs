using System;
using System.Diagnostics;
using Sirenix.OdinInspector;

namespace YooAsset.Enhancer.Runtime
{
    [AttributeUsage(AttributeTargets.All)]
    [Conditional("UNITY_EDITOR")]
    public class RichTableListAttribute : TableListAttribute
    {
        public bool HideAsListButton = false;

        public bool HideAddButton = false;

        public string CustomAddFunction;

        public bool HideRemoveButton = false;

        // public string CustomRemoveIndexFunction;

        // public string CustomRemoveElementFunction;

        public bool BoldColumnHeader = false;

        public TableColumnHeaderAlignments ColumnHeaderAlignments = TableColumnHeaderAlignments.Left;

        public bool OpenColumnSort = false;

        public IconAlignment ColumnSortIconAlignment = IconAlignment.RightOfText;

        public string SortFunction = null;

        public string OnTitleBarGUI = null;
    }
}