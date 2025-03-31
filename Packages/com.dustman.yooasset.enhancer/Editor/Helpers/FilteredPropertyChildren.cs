using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

namespace YooAsset.Enhancer.Editor
{
    public class FilteredPropertyChildren
    {
        public InspectorProperty Property;
        public PropertyChildren Children;
        public PropertySearchFilter SearchFilter;
        private List<InspectorProperty> FilteredChildren;
        private int lastUpdatedChildrenCount = -1;

        public bool IsCurrentlyFiltered => this.FilteredChildren != null;

        public int Count
        {
            get
            {
                if (this.FilteredChildren == null)
                    return this.Children.Count;
                if (this.Children.Count != this.lastUpdatedChildrenCount)
                {
                    this.Update();
                    if (this.FilteredChildren == null)
                        return this.Children.Count;
                }
                return this.FilteredChildren.Count;
            }
        }

        public InspectorProperty this[int index]
        {
            get => this.FilteredChildren == null ? this.Children[index] : this.FilteredChildren[index];
        }

        public FilteredPropertyChildren(InspectorProperty property, PropertySearchFilter searchFilter)
        {
            this.Property = property;
            this.Children = property.Children;
            this.SearchFilter = searchFilter;
        }

        public void Update()
        {
            if (this.SearchFilter == null || string.IsNullOrEmpty(this.SearchFilter.SearchTerm))
            {
                this.FilteredChildren = (List<InspectorProperty>)null;
            }
            else
            {
                if (this.FilteredChildren != null)
                    this.FilteredChildren.Clear();
                else
                    this.FilteredChildren = new List<InspectorProperty>();
                for (int index = 0; index < this.Children.Count; ++index)
                {
                    InspectorProperty child = this.Children[index];
                    if (this.SearchFilter.IsMatch(child, this.SearchFilter.SearchTerm))
                        this.FilteredChildren.Add(child);
                    else if (this.SearchFilter.Recursive)
                    {
                        foreach (InspectorProperty property in child.Children.Recurse())
                        {
                            if (this.SearchFilter.IsMatch(property, this.SearchFilter.SearchTerm))
                            {
                                this.FilteredChildren.Add(child);
                                break;
                            }
                        }
                    }
                }
                this.lastUpdatedChildrenCount = this.Children.Count;
            }
        }

        public void ScheduleUpdate()
        {
            this.Property.Tree.DelayActionUntilRepaint((Action)(() =>
            {
                this.Update();
                GUIHelper.RequestRepaint();
            }));
        }
    }
}