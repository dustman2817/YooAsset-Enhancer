using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ActionResolvers;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;
using YooAsset.Enhancer.Runtime;
using ActionResolverNamedValue = Sirenix.OdinInspector.Editor.ActionResolvers.NamedValue;
using ValueResolverNamedValue = Sirenix.OdinInspector.Editor.ValueResolvers.NamedValue;

namespace YooAsset.Enhancer.Editor
{
    public class RichTableListAttributeDrawer : TableListAttributeDrawer<RichTableListAttribute>
    {
        private static class Styles
        {
            public static Dictionary<TableColumnHeaderAlignments, GUIStyle> ColumnHeaderStyles = new Dictionary<TableColumnHeaderAlignments, GUIStyle>() { { TableColumnHeaderAlignments.Left, SirenixGUIStyles.Title }, { TableColumnHeaderAlignments.Centered, SirenixGUIStyles.TitleCentered }, { TableColumnHeaderAlignments.Right, SirenixGUIStyles.TitleRight } };
            public static Dictionary<TableColumnHeaderAlignments, GUIStyle> ColumnHeaderBoldStyles = new Dictionary<TableColumnHeaderAlignments, GUIStyle>() { { TableColumnHeaderAlignments.Left, SirenixGUIStyles.BoldTitle }, { TableColumnHeaderAlignments.Centered, SirenixGUIStyles.BoldTitleCentered }, { TableColumnHeaderAlignments.Right, SirenixGUIStyles.BoldTitleRight } };
            public static GUIContent ColumnHeaderContent = new GUIContent("");

            public const float COLUMN_SORT_ICON_SIZE = 5;
            public const float COLUMN_SORT_ICON_BOLD_SIZE = 6;
            public const float COLUMN_SORT_ICON_PADDING = 2;
            public const float COLUMN_SORT_ICON_MARGIN = 2;
            public const float COLUMN_SORT_ICON_BOLD_MARGIN = 4;
        }

        private bool hideAsListButton;
        private bool hideAddButton;
        public ValueResolver<bool> customAddFunction;
        private bool hideRemoveButton;
        private bool boldColumnHeader;
        private TableColumnHeaderAlignments columnHeaderAlignments;
        private bool openColumnSort;
        private IconAlignment columnSortIconAlignment;
        private ActionResolver sortFunction;
        public ValueResolver<Rect> onTitleBarGUI;

        private FilteredPropertyChildren filteredChildren;

        private float columnSortIconSize;
        private float columnSortIconMargin;

        private Dictionary<string, SortOrders> sortOrders;
        private string curSortColumn;
        private string oldSortColumn;

        protected override void Initialize()
        {
            hideAsListButton = Attribute.HideAsListButton;
            hideAddButton = Attribute.HideAddButton;
            customAddFunction = !string.IsNullOrEmpty(Attribute.CustomAddFunction) ? ValueResolver.Get<bool>(Property, Attribute.CustomAddFunction, new ValueResolverNamedValue("bool", typeof(bool))) : null;
            hideRemoveButton = Attribute.HideRemoveButton;
            boldColumnHeader = Attribute.BoldColumnHeader;
            columnHeaderAlignments = Attribute.ColumnHeaderAlignments;
            openColumnSort = Attribute.OpenColumnSort;
            columnSortIconAlignment = Attribute.ColumnSortIconAlignment;
            sortFunction = openColumnSort && !string.IsNullOrEmpty(Attribute.SortFunction) ? ActionResolver.Get(Property, Attribute.SortFunction, new ActionResolverNamedValue("header", typeof(string)), new ActionResolverNamedValue("order", typeof(SortOrders))) : null;
            onTitleBarGUI = !string.IsNullOrEmpty(Attribute.OnTitleBarGUI) ? ValueResolver.Get<Rect>(Property, Attribute.OnTitleBarGUI, new ValueResolverNamedValue("rect", typeof(Rect))) : null;

            base.Initialize();

            columnSortIconSize = boldColumnHeader ? Styles.COLUMN_SORT_ICON_BOLD_SIZE : Styles.COLUMN_SORT_ICON_SIZE;
            columnSortIconMargin = boldColumnHeader ? Styles.COLUMN_SORT_ICON_BOLD_MARGIN : Styles.COLUMN_SORT_ICON_MARGIN;

            sortOrders = new Dictionary<string, SortOrders>();
            curSortColumn = null;
            oldSortColumn = null;
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            ValueResolver.DrawErrors(onTitleBarGUI);

            base.DrawPropertyLayout(label);
        }

        protected override bool IsDrawToolbarExtension()
        {
            return onTitleBarGUI != null && !onTitleBarGUI.HasError;
        }

        protected override Rect DrawToolbarExtension(Rect rect1)
        {
            var rect = base.DrawToolbarExtension(rect1);

            if (onTitleBarGUI != null && !onTitleBarGUI.HasError)
            {
                onTitleBarGUI.Context.NamedValues.Set("rect", rect);
                return onTitleBarGUI.GetValue();
            }

            return rect;
        }

        protected override bool IsDrawAsListButton()
        {
            return !isReadOnly && !hideAsListButton;
        }

        protected override bool IsDrawAddButton()
        {
            return !isReadOnly && !hideAddButton;
        }

        protected override bool InvokeCustomAddFunction()
        {
            if (customAddFunction != null && !customAddFunction.HasError)
            {
                return customAddFunction.GetValue();
            }
            return false;
        }

        protected override bool IsDrawDeleteButton()
        {
            return !isReadOnly && !hideRemoveButton;
        }

        protected override bool IsRealTimeDrawColumnHeader()
        {
            return openColumnSort;
        }

        protected override void DrawColumnHeaderItem(Rect rect, Column column)
        {
            var name = column.Name;
            var text = column.NiceName;
            var sort = openColumnSort && sortFunction != null && !string.IsNullOrEmpty(text);

            // Draw Sort Button
            Styles.ColumnHeaderContent.tooltip = sort && sortFunction.HasError ? sortFunction.ErrorMessage : null;
            if (sort && GUI.Button(rect, Styles.ColumnHeaderContent, SirenixGUIStyles.Label))
            {
                if (string.IsNullOrEmpty(curSortColumn) || !curSortColumn.Equals(text))
                {
                    oldSortColumn = curSortColumn;
                    if (!string.IsNullOrEmpty(oldSortColumn) && sortOrders.TryGetValue(oldSortColumn, out _))
                    {
                        sortOrders[oldSortColumn] = SortOrders.None;
                    }
                    curSortColumn = text;
                }
                if (!sortOrders.TryGetValue(curSortColumn, out var order))
                {
                    order = SortOrders.None;
                    sortOrders.Add(curSortColumn, order);
                }
                switch (order)
                {
                    case SortOrders.None:
                    case SortOrders.Ascending:
                        order = SortOrders.Descending;
                        break;
                    case SortOrders.Descending:
                        order = SortOrders.Ascending;
                        break;
                    default:
                        order = SortOrders.None;
                        break;
                }
                sortOrders[curSortColumn] = order;
                if (sortFunction != null && !sortFunction.HasError)
                {
                    sortFunction.Context.NamedValues.Set("header", name);
                    sortFunction.Context.NamedValues.Set("order", order);
                    sortFunction.DoAction();
                }
            }

            // Header Rect
            var rectHeader = new Rect(rect);
            rectHeader.xMin += table.CellStyle.padding.left;
            rectHeader.xMax -= table.CellStyle.padding.right;

            // Draw Title
            var label = new GUIContent(text);
            var style = boldColumnHeader ? Styles.ColumnHeaderBoldStyles[columnHeaderAlignments] : Styles.ColumnHeaderStyles[columnHeaderAlignments];
            var padding = boldColumnHeader ? style.margin : style.padding;
            var labelWidth = !string.IsNullOrEmpty(text) ? style.CalcSize(label).x : 0.0f;
            Rect rectTitle = new Rect(rectHeader);
            switch (columnHeaderAlignments)
            {
                case TableColumnHeaderAlignments.Centered:
                    rectTitle = rectTitle.AlignCenter(labelWidth);
                    if (!sort)
                    {
                        rectTitle.width += padding.horizontal;
                    }
                    break;
                case TableColumnHeaderAlignments.Right:
                    rectTitle = rectTitle.AlignRight(labelWidth);
                    if (!sort || columnSortIconAlignment == IconAlignment.LeftEdge)
                    {
                        rectTitle.x -= padding.right + padding.horizontal;
                        rectTitle.width += padding.horizontal;
                    }
                    if (sort)
                    {
                        if (columnSortIconAlignment == IconAlignment.LeftOfText)
                        {
                            rectTitle.x -= Styles.COLUMN_SORT_ICON_PADDING;
                        }
                        else if (columnSortIconAlignment == IconAlignment.RightEdge || columnSortIconAlignment == IconAlignment.RightOfText)
                        {
                            rectTitle.x -= Styles.COLUMN_SORT_ICON_PADDING + columnSortIconSize + columnSortIconMargin;
                        }
                    }
                    break;
                case TableColumnHeaderAlignments.Left:
                default:
                    rectTitle = rectTitle.AlignLeft(labelWidth);
                    if (!sort || columnSortIconAlignment == IconAlignment.RightEdge)
                    {
                        rectTitle.x += padding.left;
                        rectTitle.width += padding.horizontal;
                    }
                    if (sort)
                    {
                        if (columnSortIconAlignment == IconAlignment.RightOfText)
                        {
                            rectTitle.x += Styles.COLUMN_SORT_ICON_PADDING;
                        }
                        else if (columnSortIconAlignment == IconAlignment.LeftEdge || columnSortIconAlignment == IconAlignment.LeftOfText)
                        {
                            rectTitle.x += Styles.COLUMN_SORT_ICON_PADDING + columnSortIconSize + columnSortIconMargin;
                        }
                    }
                    break;
            }
            GUI.Label(rectTitle, label, style);

            // Draw Sort
            if (sort)
            {
                var labelHeight = style.lineHeight;
                var iconHeight = Mathf.Clamp(columnSortIconSize, 0.0f, labelHeight / 2);
                var rectSort = new Rect(rectHeader);
                rectSort.width = columnSortIconSize;
                rectSort.height = iconHeight;
                switch (columnSortIconAlignment)
                {
                    case IconAlignment.LeftEdge:
                        rectSort.x = rectHeader.xMin + Styles.COLUMN_SORT_ICON_PADDING;
                        break;
                    case IconAlignment.LeftOfText:
                        rectSort.x = rectTitle.xMin - columnSortIconMargin - columnSortIconSize;
                        break;
                    case IconAlignment.RightEdge:
                        rectSort.x = rectHeader.xMax - Styles.COLUMN_SORT_ICON_PADDING - columnSortIconSize;
                        break;
                    case IconAlignment.RightOfText:
                        rectSort.x = rectTitle.xMax + columnSortIconMargin;
                        break;
                }
                var yMin = rectTitle.yMin;
                var yMax = rectTitle.yMax;
                var yHeight = rectTitle.height;
                switch (style.alignment)
                {
                    case TextAnchor.UpperLeft:
                    case TextAnchor.UpperCenter:
                    case TextAnchor.UpperRight:
                        yMin += padding.top;
                        yMax = yMin + labelHeight;
                        break;
                    case TextAnchor.MiddleLeft:
                    case TextAnchor.MiddleCenter:
                    case TextAnchor.MiddleRight:
                        yMin += padding.top + yHeight / 2 - labelHeight / 2;
                        yMax = yMin + labelHeight;
                        break;
                    case TextAnchor.LowerLeft:
                    case TextAnchor.LowerCenter:
                    case TextAnchor.LowerRight:
                    default:
                        yMax -= padding.bottom;
                        yMin = yMax - labelHeight;
                        break;
                }
                var yMid = (yMin + yMax) / 2;

                SdfIconType up, down;
                var order = sortOrders.GetValueOrDefault(text, SortOrders.None);
                switch (order)
                {
                    case SortOrders.Ascending:
                        up = SdfIconType.CaretUpFill;
                        down = SdfIconType.CaretDown;
                        break;
                    case SortOrders.Descending:
                        up = SdfIconType.CaretUp;
                        down = SdfIconType.CaretDownFill;
                        break;
                    case SortOrders.None:
                    default:
                        up = SdfIconType.CaretUp;
                        down = SdfIconType.CaretDown;
                        break;
                }
                rectSort.y = yMid - iconHeight;
                SdfIcons.DrawIcon(rectSort, up);
                rectSort.y = yMid;
                SdfIcons.DrawIcon(rectSort, down);
            }
        }
    }
}