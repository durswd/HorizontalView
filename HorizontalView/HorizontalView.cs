using System;
using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

namespace HorizontalView
{
    public class HorizontalView : AbsoluteLayout, INotifyPropertyChanged
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            "ItemsSource",
            typeof (ICollection),
            typeof (HorizontalView),
            null,
            BindingMode.TwoWay,
            null,
            HorizontalView.OnItemsSourcePropertyChanged,
            null,
            null);

        DataTemplate _ElementTemplate;

        public DataTemplate ElementTemplate
        {
            get { return _ElementTemplate; }
            set
            {
                SetProperty(ref _ElementTemplate, value);
            }
        }


        ICollection _ItemsSource = null;

        public ICollection ItemsSource
        {
            get { return _ItemsSource; }
            set
            {
                SetProperty(ref _ItemsSource, value);
                elements.Clear();
                FitElements();
            }
        }

        public int ElementWidth
        {
            get;
            set;
        }

        public int CacheWidth
        {
            get;
            set;
        }

        public Action<int, object> Added;


        public Action<int, object> Removed;

        List<View> elements;

        public HorizontalView()
        {
            elements = new List<View>();
        }

        public void ShowElement(int x)
        {
            if (ItemsSource == null) return;
            var objs = new object[ItemsSource.Count];
            ItemsSource.CopyTo(objs, 0);

            var minX = x - CacheWidth;
            var maxX = x + CacheWidth;

            var countMinX = 0;
            var countMaxX = ItemsSource.Count - 1;

            if(ElementWidth > 0)
            {
                countMinX = minX / ElementWidth;
                countMaxX = maxX / ElementWidth - 1;
                countMinX = Math.Max(0, countMinX);
                countMaxX = Math.Min(ItemsSource.Count - 1, countMaxX);
            }

            // TODO optimize it
            for (int i = countMinX; i <= countMaxX; i++)
            {
                if (elements[i] != null) continue;

                // Add view
                var o = CreateContent() as View;

                if(o != null)
                {
                    o.BindingContext = objs[i];
                    Children.Add(o, new Point((double)i * ElementWidth, 0.0));
                    elements[i] = o;

                    if(Added != null)
                    {
                        Added(i, objs[i]);
                    }
                }
            }

            for (int i = 0; i < elements.Count; i++)
            {
                if (countMinX <= i && i <= countMaxX) continue;
                if (elements[i] == null) continue;

                // Remove view
                Children.Remove(elements[i]);
                elements[i] = null;

                if(Removed != null)
                {
                    Removed(i, objs[i]);
                }
            }
        }

        object CreateContent()
        {
            if (ElementTemplate == null) return null;
            var o = ElementTemplate.CreateContent();

            return o;
        }

        void FitElements()
        {
            if (elements.Count == _ItemsSource.Count) return;

            if(elements.Count > ItemsSource.Count)
            {
                while(elements.Count != ItemsSource.Count)
                {
                    var e = elements[elements.Count - 1];
                    Children.Remove(e);
                    elements.RemoveAt(elements.Count-1);
                }
            }

            if (elements.Count < ItemsSource.Count)
            {
                while (elements.Count != ItemsSource.Count)
                {
                    elements.Add(null);
                }
            }
        }

        private static void OnItemsSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as HorizontalView;
            var v = newValue as ICollection;
            if (view == null|| v == null)
            {
                return;
            }

            view.ItemsSource = v;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(field, value)) { return false; }
            field = value;
            var h = this.PropertyChanged;
            if (h != null) { h(this, new PropertyChangedEventArgs(propertyName)); }
            return true;
        }

        void NotifyChanged(string propertyName)
        {
            var h = this.PropertyChanged;
            if (h != null) { h(this, new PropertyChangedEventArgs(propertyName)); }
        }
    }
}
