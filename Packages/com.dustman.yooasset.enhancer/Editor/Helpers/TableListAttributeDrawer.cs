using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using YooAsset.Enhancer.Runtime;
using Object = UnityEngine.Object;

namespace YooAsset.Enhancer.Editor
{
    /// <summary>The TableList attirbute drawer.</summary>
    /// <seealso cref="T:Sirenix.OdinInspector.TableListAttribute" />
    public abstract class TableListAttributeDrawer<TAttribute> : OdinAttributeDrawer<TAttribute>, IDisposable where TAttribute : RichTableListAttribute
    {
        protected static readonly int TableListDrawerId = "id_TableListDrawer".GetHashCode();
        protected IOrderedCollectionResolver resolver;
        protected LocalPersistentContext<bool> isPagingExpanded;
        protected LocalPersistentContext<Vector2> scrollPos;
        protected LocalPersistentContext<int> currPage;
        protected GUITableRowLayoutGroup table;
        protected HashSet<string> seenColumnNames;
        protected List<Column> columns;
        protected int colOffset;
        protected GUIContent indexLabel;
        protected bool isReadOnly;
        protected int indexLabelWidth;
        protected Rect columnHeaderRect;
        protected GUIPagingHelper paging;
        protected bool drawAsList;
        protected bool isFirstFrame = true;
        protected MultiCollectionFilter<IOrderedCollectionResolver> filter;

        /// <summary>
        /// Determines whether this instance [can draw attribute property] the specified property.
        /// </summary>
        protected override bool CanDrawAttributeProperty(InspectorProperty property)
        {
            return property.ChildResolver is IOrderedCollectionResolver;
        }

        /// <summary>Initializes this instance.</summary>
        protected override void Initialize()
        {
            this.drawAsList = false;
            this.isReadOnly = this.Attribute.IsReadOnly || !this.Property.ValueEntry.IsEditable;
            this.indexLabelWidth = (int)SirenixGUIStyles.Label.CalcSize(new GUIContent("100")).x + 15;
            this.indexLabel = new GUIContent();
            this.colOffset = 0;
            this.seenColumnNames = new HashSet<string>();
            this.table = new GUITableRowLayoutGroup();
            this.table.MinScrollViewHeight = this.Attribute.MinScrollViewHeight;
            this.table.MaxScrollViewHeight = this.Attribute.MaxScrollViewHeight;
            this.resolver = this.Property.ChildResolver as IOrderedCollectionResolver;
            this.scrollPos = this.GetPersistentValue<Vector2>("scrollPos", Vector2.zero);
            this.currPage = this.GetPersistentValue<int>("currPage");
            this.isPagingExpanded = this.GetPersistentValue<bool>("expanded");
            this.columns = new List<Column>(10);
            this.paging = new GUIPagingHelper();
            this.paging.NumberOfItemsPerPage = this.Attribute.NumberOfItemsPerPage > 0 ? this.Attribute.NumberOfItemsPerPage : GlobalConfig<GeneralDrawerConfig>.Instance.NumberOfItemsPrPage;
            this.paging.IsExpanded = this.isPagingExpanded.Value;
            this.paging.IsEnabled = GlobalConfig<GeneralDrawerConfig>.Instance.ShowPagingInTables || this.Attribute.ShowPaging;
            this.paging.CurrentPage = this.currPage.Value;
            this.Property.ValueEntry.OnChildValueChanged += new Action<int>(this.OnChildValueChanged);
            this.filter = new MultiCollectionFilter<IOrderedCollectionResolver>(this.Property, this.Property.ChildResolver as IOrderedCollectionResolver);
            if (this.Attribute.AlwaysExpanded)
                this.Property.State.Expanded = true;
            int cellPadding = this.Attribute.CellPadding;
            if (cellPadding > 0)
                this.table.CellStyle = new GUIStyle() { padding = new RectOffset(cellPadding, cellPadding, cellPadding, cellPadding) };
            GUIHelper.RequestRepaint();
            if (this.Attribute.ShowIndexLabels)
            {
                ++this.colOffset;
                this.columns.Add(new Column(this.indexLabelWidth, true, false, (string)null, ColumnType.Index));
            }
            // if (this.isReadOnly) // comment by tiansheng 2024/05/23
            if (!IsDrawDeleteButton()) // add by tiansheng 2024/05/23
                return;
            this.columns.Add(new Column(22, true, false, (string)null, ColumnType.DeleteButton));
        }

        /// <summary>
        /// 是否绘制列表按钮
        /// author: tiansheng
        /// date: 2024/05/23
        /// </summary>
        protected virtual bool IsDrawAsListButton()
        {
            return !isReadOnly;
        }

        /// <summary>
        /// 是否绘制添加按钮
        /// author: tiansheng
        /// date: 2024/05/23
        /// </summary>
        protected virtual bool IsDrawAddButton()
        {
            return !isReadOnly;
        }

        /// <summary>
        /// 调用自定义添加函数
        /// author: tiansheng
        /// date: 2024/08/21
        /// </summary>
        protected virtual bool InvokeCustomAddFunction()
        {
            return false;
        }

        /// <summary>
        /// 是否绘制删除按钮
        /// author: tiansheng
        /// date: 2024/05/23
        /// </summary>
        protected virtual bool IsDrawDeleteButton()
        {
            return !isReadOnly;
        }

        /// <summary>Draws the property layout.</summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (this.drawAsList)
            {
                if (GUILayout.Button("Draw as table"))
                    this.drawAsList = false;
                this.CallNextDrawer(label);
            }
            else
            {
                this.paging.Update(this.filter.GetCount());
                this.currPage.Value = this.paging.CurrentPage;
                this.isPagingExpanded.Value = this.paging.IsExpanded;
                Rect rect = SirenixEditorGUI.BeginIndentedVertical(SirenixGUIStyles.PropertyMargin);
                if (!this.Attribute.HideToolbar)
                    this.DrawToolbar(label);
                if (this.Attribute.AlwaysExpanded)
                {
                    this.Property.State.Expanded = true;
                    this.DrawColumnHeaders();
                    this.DrawTable();
                }
                else
                {
                    if (SirenixEditorGUI.BeginFadeGroup((object)this, this.Property.State.Expanded) && this.Property.Children.Count > 0)
                    {
                        this.DrawColumnHeaders();
                        this.DrawTable();
                    }
                    SirenixEditorGUI.EndFadeGroup();
                }
                SirenixEditorGUI.EndIndentedVertical();
                if (Event.current.type == EventType.Repaint)
                    SirenixEditorGUI.DrawBorders(rect, 1, 1, !this.Attribute.HideToolbar ? 1 : 0, 1);
                this.DropZone(rect);
                this.HandleObjectPickerEvents();
                if (Event.current.type != EventType.Repaint)
                    return;
                this.isFirstFrame = false;
            }
        }

        protected virtual void OnChildValueChanged(int index)
        {
            IPropertyValueEntry valueEntry = this.Property.Children[index].ValueEntry;
            if (valueEntry == null || !typeof(ScriptableObject).IsAssignableFrom(valueEntry.TypeOfValue))
                return;
            for (int index1 = 0; index1 < valueEntry.ValueCount; ++index1)
            {
                Object weakValue = valueEntry.WeakValues[index1] as Object;
                if ((bool)weakValue)
                    EditorUtility.SetDirty(weakValue);
            }
        }

        protected virtual void DropZone(Rect rect)
        {
            // if (this.isReadOnly) // comment by tiansheng 2024/05/23
            if (!IsDrawDeleteButton()) // add by tiansheng 2024/05/23
                return;
            EventType type = Event.current.type;
            switch (type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!rect.Contains(Event.current.mousePosition))
                        break;
                    Object[] objectArray = (Object[])null;
                    if (((IEnumerable<Object>)DragAndDrop.objectReferences).Any<Object>((Func<Object, bool>)(n => n != (Object)null && this.resolver.ElementType.IsAssignableFrom(n.GetType()))))
                        objectArray = ((IEnumerable<Object>)DragAndDrop.objectReferences).Where<Object>((Func<Object, bool>)(x => x != (Object)null && this.resolver.ElementType.IsAssignableFrom(x.GetType()))).Reverse<Object>().ToArray<Object>();
                    else if (this.resolver.ElementType.InheritsFrom(typeof(Component)))
                        objectArray = (Object[])DragAndDrop.objectReferences.OfType<GameObject>().Select<GameObject, Component>((Func<GameObject, Component>)(x => x.GetComponent(this.resolver.ElementType))).Where<Component>((Func<Component, bool>)(x => (Object)x != (Object)null)).Reverse<Component>().ToArray<Component>();
                    else if (this.resolver.ElementType.InheritsFrom(typeof(Sprite)) && ((IEnumerable<Object>)DragAndDrop.objectReferences).Any<Object>((Func<Object, bool>)(n => n is Texture2D && AssetDatabase.Contains(n))))
                        objectArray = (Object[])DragAndDrop.objectReferences.OfType<Texture2D>().Select<Texture2D, Sprite>((Func<Texture2D, Sprite>)(x => AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath((Object)x)))).Where<Sprite>((Func<Sprite, bool>)(x => (Object)x != (Object)null)).Reverse<Sprite>().ToArray<Sprite>();
                    if (objectArray == null || objectArray.Length == 0)
                        break;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Event.current.Use();
                    if (type != EventType.DragPerform)
                        break;
                    DragAndDrop.AcceptDrag();
                    foreach (Object @object in objectArray)
                    {
                        object[] values = new object[this.Property.ParentValues.Count];
                        for (int index = 0; index < values.Length; ++index)
                            values[index] = (object)@object;
                        this.resolver.QueueAdd(values);
                    }
                    break;
            }
        }

        protected virtual void AddColumns(int rowIndexFrom, int rowIndexTo)
        {
            if (Event.current.type != EventType.Layout)
                return;
            for (int index1 = rowIndexFrom; index1 < rowIndexTo; ++index1)
            {
                int num = 0;
                InspectorProperty child1 = this.Property.Children[index1];
                for (int index2 = 0; index2 < child1.Children.Count; ++index2)
                {
                    InspectorProperty child2 = child1.Children[index2];
                    if (this.seenColumnNames.Add(child2.Name))
                    {
                        if (this.GetColumnAttribute<HideInTablesAttribute>(child2) != null)
                        {
                            ++num;
                        }
                        else
                        {
                            bool preserveWidth = false;
                            bool resizable = true;
                            bool flag = true;
                            int minWidth = this.Attribute.DefaultMinColumnWidth;
                            TableColumnWidthAttribute columnAttribute = this.GetColumnAttribute<TableColumnWidthAttribute>(child2);
                            if (columnAttribute != null)
                            {
                                preserveWidth = !columnAttribute.Resizable;
                                resizable = columnAttribute.Resizable;
                                minWidth = columnAttribute.Width;
                                flag = false;
                            }
                            Column column = new Column(minWidth, preserveWidth, resizable, child2.Name, ColumnType.Property) { NiceName = child2.NiceName };
                            column.NiceNameLabelWidth = (int)SirenixGUIStyles.Label.CalcSize(new GUIContent(column.NiceName)).x;
                            column.PreferWide = flag;
                            this.columns.Insert(Math.Min(index2 + this.colOffset - num, this.columns.Count), column);
                            GUIHelper.RequestRepaint();
                        }
                    }
                }
            }
        }

        protected virtual void DrawToolbar(GUIContent label)
        {
            Rect rect1 = GUILayoutUtility.GetRect(0.0f, 22f);
            bool flag = Event.current.type == EventType.Repaint;
            if (flag)
                SirenixGUIStyles.ToolbarBackground.Draw(rect1, GUIContent.none, 0);
            if (IsDrawToolbarExtension()) // add by tiansheng 2024/06/24
            {
                rect1 = DrawToolbarExtension(rect1);
            }
            // if (!this.isReadOnly) // comment by tiansheng 2024/05/23
            if (IsDrawAddButton()) // add by tiansheng 2024/05/23
            {
                Rect rect2 = rect1.AlignRight(23f);
                rect1.xMax = rect2.xMin;
                if (GUI.Button(rect2, GUIContent.none, SirenixGUIStyles.ToolbarButton))
                {
                    if (!InvokeCustomAddFunction())
                    {
                        // OdinObjectSelector.Show(rect1, (object)this, TableListDrawerId, (object)null, this.resolver.ElementType, this.resolver.ElementType, !typeof(ScriptableObject).IsAssignableFrom(this.resolver.ElementType), !this.Property.ValueEntry.SerializationBackend.SupportsPolymorphism, this.Property); // comment by tiansheng 2024/05/23
                        OdinObjectSelectorReflection.Show(rect1, (object)this, TableListDrawerId, (object)null, this.resolver.ElementType, this.resolver.ElementType, !typeof(ScriptableObject).IsAssignableFrom(this.resolver.ElementType), !this.Property.ValueEntry.SerializationBackend.SupportsPolymorphism, this.Property); // add by tiansheng 2024/05/23
                    }
                }
                SdfIcons.DrawIcon(rect2.AlignCenter(13f), SdfIconType.Plus);
            }
            // if (!this.isReadOnly) // comment by tiansheng 2024/05/23
            if (IsDrawAsListButton()) // add by tiansheng 2024/05/23
            {
                Rect rect3 = rect1.AlignRight(23f);
                rect1.xMax = rect3.xMin;
                if (GUI.Button(rect3, GUIContent.none, SirenixGUIStyles.ToolbarButton))
                    this.drawAsList = !this.drawAsList;
                SdfIcons.DrawIcon(rect3.AlignCenter(13f), SdfIconType.ListOl);
            }
            this.paging.DrawToolbarPagingButtons(ref rect1, this.Property.State.Expanded, true);
            if (label == null)
                label = GUIHelper.TempContent("");
            Rect rect4 = rect1;
            rect4.x += 5f;
            rect4.y += 3f;
            rect4.height = 16f;
            if (this.filter.IsUsed)
            {
                Vector2 vector2 = EditorStyles.label.CalcSize(label);
                Rect fromRight = rect4.TakeFromRight((float)((double)rect4.width - (double)vector2.x - 20.0));
                fromRight.width -= 10f;
                this.filter.Draw(fromRight);
            }
            if (this.Property.Children.Count > 0)
            {
                GUIHelper.PushHierarchyMode(false);
                if (this.Attribute.AlwaysExpanded)
                    GUI.Label(rect4, label);
                else
                    this.Property.State.Expanded = SirenixEditorGUI.Foldout(rect4, this.Property.State.Expanded, label);
                GUIHelper.PushHierarchyMode(true);
            }
            else
            {
                if (!flag)
                    return;
                GUI.Label(rect4, label);
            }
        }

        /// <summary>
        /// 是否绘制工具栏扩展
        /// author: tiansheng
        /// date: 2024/06/24
        /// </summary>
        protected virtual bool IsDrawToolbarExtension()
        {
            return false;
        }

        /// <summary>
        /// 绘制工具栏扩展
        /// author: tiansheng
        /// date: 2024/06/24
        /// </summary>
        protected virtual Rect DrawToolbarExtension(Rect rect)
        {
            return rect;
        }

        protected virtual void DrawColumnHeaders()
        {
            if (this.Property.Children.Count == 0)
                return;
            this.columnHeaderRect = GUILayoutUtility.GetRect(0.0f, 21f);
            ++this.columnHeaderRect.height;
            --this.columnHeaderRect.y;
            if (Event.current.type == EventType.Repaint)
            {
                SirenixEditorGUI.DrawBorders(this.columnHeaderRect, 1);
                EditorGUI.DrawRect(this.columnHeaderRect, SirenixGUIStyles.ColumnTitleBg);
            }
            this.columnHeaderRect.width -= this.columnHeaderRect.width - this.table.ContentRect.width;
            GUITableUtilities.ResizeColumns<Column>(this.columnHeaderRect, (IList<Column>)this.columns);
            // if (Event.current.type != EventType.Repaint) // comment by tiansheng 2023/05/22
            if (!IsRealTimeDrawColumnHeader() && Event.current.type != EventType.Repaint) // add by tiansheng 2023/05/22
                return;
            GUITableUtilities.DrawColumnHeaderSeperators<Column>(this.columnHeaderRect, (IList<Column>)this.columns, SirenixGUIStyles.BorderColor);
            Rect columnHeaderRect = this.columnHeaderRect;
            for (int index = 0; index < this.columns.Count; ++index)
            {
                Column column = this.columns[index];
                if ((double)columnHeaderRect.x > (double)this.columnHeaderRect.xMax)
                    break;
                columnHeaderRect.width = column.ColWidth;
                columnHeaderRect.xMax = Mathf.Min(this.columnHeaderRect.xMax, columnHeaderRect.xMax);
                if (column.NiceName != null)
                    // GUI.Label(columnHeaderRect, column.NiceName, SirenixGUIStyles.LabelCentered); // comment by tiansheng 2024/05/23
                    DrawColumnHeaderItem(columnHeaderRect, column); // add by tiansheng 2024/05/23
                columnHeaderRect.x += column.ColWidth;
            }
        }

        /// <summary>
        /// 是否实时绘制列头
        /// author: tiansheng
        /// date: 2024/05/23
        /// </summary>
        protected virtual bool IsRealTimeDrawColumnHeader()
        {
            return false;
        }

        /// <summary>
        /// 绘制列头
        /// author: tiansheng
        /// date: 2024/05/23
        /// </summary>
        protected virtual void DrawColumnHeaderItem(Rect rect, Column column)
        {
            if (column.NiceName != null)
            {
                GUI.Label(rect, column.NiceName, SirenixGUIStyles.LabelCentered);
            }
        }

        protected virtual void DrawTable()
        {
            GUIHelper.PushHierarchyMode(false);
            this.table.DrawScrollView = this.Attribute.DrawScrollView && (this.paging.IsExpanded || !this.paging.IsEnabled);
            this.table.ScrollPos = this.scrollPos.Value;
            this.table.BeginTable(this.paging.EndIndex - this.paging.StartIndex);
            this.AddColumns(this.table.RowIndexFrom, this.table.RowIndexTo);
            this.DrawListItemBackGrounds();
            float xOffset = 0.0f;
            for (int index = 0; index < this.columns.Count; ++index)
            {
                Column column = this.columns[index];
                int width = (int)column.ColWidth;
                if (this.isFirstFrame && column.PreferWide)
                    width = 200;
                this.table.BeginColumn((int)xOffset, width);
                GUIHelper.PushLabelWidth((float)width * 0.3f);
                xOffset += column.ColWidth;
                for (int rowIndexFrom = this.table.RowIndexFrom; rowIndexFrom < this.table.RowIndexTo; ++rowIndexFrom)
                {
                    this.table.BeginCell(rowIndexFrom);
                    this.DrawCell(column, rowIndexFrom);
                    this.table.EndCell(rowIndexFrom);
                }
                GUIHelper.PopLabelWidth();
                this.table.EndColumn();
            }
            this.DrawRightClickContextMenuAreas();
            this.table.EndTable();
            this.scrollPos.Value = this.table.ScrollPos;
            this.DrawColumnSeperators();
            GUIHelper.PopHierarchyMode();
            if (this.columns.Count <= 0 || this.columns[0].ColumnType != ColumnType.Index)
                return;
            this.columns[0].ColWidth = (float)this.indexLabelWidth;
            this.columns[0].MinWidth = (float)this.indexLabelWidth;
        }

        protected virtual void DrawColumnSeperators()
        {
            if (Event.current.type != EventType.Repaint)
                return;
            Color borderColor = SirenixGUIStyles.BorderColor;
            borderColor.a *= 0.4f;
            GUITableUtilities.DrawColumnHeaderSeperators<Column>(this.table.OuterRect, (IList<Column>)this.columns, borderColor);
        }

        protected virtual void DrawListItemBackGrounds()
        {
            if (Event.current.type != EventType.Repaint)
                return;
            for (int rowIndexFrom = this.table.RowIndexFrom; rowIndexFrom < this.table.RowIndexTo; ++rowIndexFrom)
            {
                // Color color = new Color();
                EditorGUI.DrawRect(this.table.GetRowRect(rowIndexFrom), rowIndexFrom % 2 == 0 ? SirenixGUIStyles.ListItemColorEven : SirenixGUIStyles.ListItemColorOdd);
            }
        }

        protected virtual void DrawRightClickContextMenuAreas()
        {
            for (int rowIndexFrom = this.table.RowIndexFrom; rowIndexFrom < this.table.RowIndexTo; ++rowIndexFrom)
            {
                Rect rowRect = this.table.GetRowRect(rowIndexFrom);
                this.Property.Children[rowIndexFrom].Update();
                PropertyContextMenuDrawer.AddRightClickArea(this.Property.Children[rowIndexFrom], rowRect);
            }
        }

        protected virtual void DrawCell(Column col, int rowIndex)
        {
            rowIndex += this.paging.StartIndex;
            if (col.ColumnType == ColumnType.Index)
            {
                Rect rect = GUILayoutUtility.GetRect(0.0f, 16f);
                rect.xMin += 5f;
                rect.width -= 2f;
                if (Event.current.type != EventType.Repaint)
                    return;
                this.indexLabel.text = rowIndex.ToString();
                GUI.Label(rect, this.indexLabel, SirenixGUIStyles.Label);
                this.indexLabelWidth = Mathf.Max(this.indexLabelWidth, (int)SirenixGUIStyles.Label.CalcSize(this.indexLabel).x + 15);
            }
            else if (col.ColumnType == ColumnType.DeleteButton)
            {
                if (!SirenixEditorGUI.SDFIconButton(GUILayoutUtility.GetRect(20f, 20f).AlignCenter(13f, 13f), SdfIconType.X, IconAlignment.LeftOfText, SirenixGUIStyles.IconButton))
                    return;
                this.resolver.QueueRemoveAt(this.filter.GetCollectionIndex(rowIndex));
                this.filter.Update();
            }
            else
            {
                if (col.ColumnType != ColumnType.Property)
                    throw new NotImplementedException(col.ColumnType.ToString());
                this.filter[rowIndex].Children[col.Name]?.Draw((GUIContent)null);
            }
        }

        protected virtual void HandleObjectPickerEvents()
        {
            if (!OdinObjectSelector.IsReadyToClaim((object)this, TableListDrawerId))
                return;
            this.resolver.QueueAdd(OdinObjectSelector.ClaimMultiple(this.Property.Tree.WeakTargets.Count));
        }

        protected virtual IEnumerable<InspectorProperty> EnumerateGroupMembers(InspectorProperty groupProperty)
        {
            for (int i = 0; i < groupProperty.Children.Count; ++i)
            {
                if (groupProperty.Children[i].Info.PropertyType != PropertyType.Group)
                {
                    yield return groupProperty.Children[i];
                }
                else
                {
                    foreach (InspectorProperty enumerateGroupMember in this.EnumerateGroupMembers(groupProperty.Children[i]))
                        yield return enumerateGroupMember;
                }
            }
        }

        protected virtual T GetColumnAttribute<T>(InspectorProperty col) where T : Attribute
        {
            return col.Info.PropertyType != PropertyType.Group ? col.GetAttribute<T>() : this.EnumerateGroupMembers(col).Select<InspectorProperty, T>((Func<InspectorProperty, T>)(c => c.GetAttribute<T>())).FirstOrDefault<T>((Func<T, bool>)(c => (object)c != null));
        }

        public void Dispose() => this.filter?.Dispose();

        protected enum ColumnType
        {
            Property,
            Index,
            DeleteButton,
        }

        protected class Column : IResizableColumn
        {
            public string Name;
            public float ColWidth;
            public float MinWidth;
            public bool Preserve;
            public bool Resizable;
            public string NiceName;
            public int NiceNameLabelWidth;
            public ColumnType ColumnType;
            public bool PreferWide;

            public Column(int minWidth, bool preserveWidth, bool resizable, string name, ColumnType colType)
            {
                this.MinWidth = (float)minWidth;
                this.ColWidth = (float)minWidth;
                this.Preserve = preserveWidth;
                this.Name = name;
                this.ColumnType = colType;
                this.Resizable = resizable;
            }

            float IResizableColumn.ColWidth
            {
                get => this.ColWidth;
                set => this.ColWidth = value;
            }

            float IResizableColumn.MinWidth => this.MinWidth;

            bool IResizableColumn.PreserveWidth => this.Preserve;

            bool IResizableColumn.Resizable => this.Resizable;
        }
    }
}