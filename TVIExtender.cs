﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;


namespace eventEditor
{
	public class TVIExtender
	{
		private TreeViewItem _item;

		public static DependencyProperty UseExtenderProperty =
			DependencyProperty.RegisterAttached ("UseExtender", typeof (bool), typeof (TVIExtender),
																					new PropertyMetadata (false, new PropertyChangedCallback (OnChangedUseExtender)));

		public static bool GetUseExtender (DependencyObject sender)
		{
			return (bool)sender.GetValue (UseExtenderProperty);
		}
		public static void SetUseExtender (DependencyObject sender, bool useExtender)
		{
			sender.SetValue (UseExtenderProperty, useExtender);
		}

		private static void OnChangedUseExtender (DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			TreeViewItem item = sender as TreeViewItem;
			if (null != item)
			{
				if ((bool)e.NewValue)
				{
					if (item.ReadLocalValue (ItemExtenderProperty) == DependencyProperty.UnsetValue)
					{
						TVIExtender extender = new TVIExtender (item);
						item.SetValue (ItemExtenderProperty, extender);
					}
				}
				else
				{
					if (item.ReadLocalValue (ItemExtenderProperty) != DependencyProperty.UnsetValue)
					{
						TVIExtender extender = (TVIExtender)item.ReadLocalValue (ItemExtenderProperty);
						extender.Detach ();
						item.SetValue (ItemExtenderProperty, DependencyProperty.UnsetValue);
					}
				}
			}
		}

		public static DependencyProperty ItemExtenderProperty =
			DependencyProperty.RegisterAttached ("ItemExtender", typeof (TVIExtender), typeof (TVIExtender));

		public static DependencyProperty IsLastOneProperty =
			DependencyProperty.RegisterAttached ("IsLastOne", typeof (bool), typeof (TVIExtender));

		public static bool GetIsLastOne (DependencyObject sender)
		{
			return (bool)sender.GetValue (IsLastOneProperty);
		}
		public static void SetIsLastOne (DependencyObject sender, bool isLastOne)
		{
			sender.SetValue (IsLastOneProperty, isLastOne);
		}

		public TVIExtender (TreeViewItem item)
		{
			_item = item;

			ItemsControl ic = ItemsControl.ItemsControlFromItemContainer (_item);
			ic.ItemContainerGenerator.ItemsChanged += OnItemsChangedItemContainerGenerator;

			_item.SetValue (IsLastOneProperty,
							 ic.ItemContainerGenerator.IndexFromContainer (_item) == ic.Items.Count - 1);
		}


		void OnItemsChangedItemContainerGenerator (object sender, ItemsChangedEventArgs e)
		{
			ItemsControl ic = ItemsControl.ItemsControlFromItemContainer (_item);

			if (null != ic)
			{
				int shift = 1, a = ic.ItemContainerGenerator.IndexFromContainer (_item);
				if (a == -1) shift = 2;
				_item.SetValue (IsLastOneProperty, a == ic.Items.Count - shift);
			}
		}

		private void Detach ()
		{
			if (_item != null)
			{
				ItemsControl ic = ItemsControl.ItemsControlFromItemContainer (_item);
				if (ic != null)
				{
					ic.ItemContainerGenerator.ItemsChanged -= OnItemsChangedItemContainerGenerator;
				}
				_item = null;
			}
		}
	}
}
