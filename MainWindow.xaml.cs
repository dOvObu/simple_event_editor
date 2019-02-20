using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Controls.Primitives;
using System.Reflection;
using System.Text.RegularExpressions;

/*
        _______________________
       /\                      \
       \_|        Сасай        |
         |        писос        |
         |   __________________|__
          \_/____________________/ 
 */



namespace eventEditor
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>


	public partial class MainWindow : Window
	{
		List<ComboBox> comboBoxes = new List<ComboBox>(); // список всех полей с именами персонажей
		private Dictionary<string, string> attackDescription = new Dictionary<string, string> (); // библиотека доступных атак (не привязанных к персонажу)
		private Dictionary<string, string> reactionDescription = new Dictionary<string, string> (); // библиотека доступниых реакций (не прив. к перс.)
		private Dictionary<string, string> lootDescription = new Dictionary<string, string> ();
		private static readonly Regex _regex = new Regex ("[^0-9]+"); //regex которое пропускает только цифры
		static private int numOfPossReplica = 0; // порядковый номер возможной реплики

		private static bool IsTextAllowed (string text) // Метод проверки, хранит ли строка целое беззнаковое число 
		{
			return !_regex.IsMatch (text);
		}
		private void setPerson(Object obj)							// Установка во всех выподающих списков персонажей одного и того-же значения  
		{
			foreach (ComboBox jt in comboBoxes)
			{
				jt.SelectedItem = obj;
			}
		}
		private void initFieldsByDefault ()							// Установка начальных значений 
		{
			DialogPerson.Items.Add ("Mike");
			DialogPerson.Items.Add ("Ilya");
			DialogPerson.Items.Add ("Sergei");
			DialogPerson.SelectedItem = DialogPerson.Items[0];

			comboBoxes.Add (AttackPerson);
			comboBoxes.Add (VictimOfAttack);
			comboBoxes.Add (PersonOwner);
			comboBoxes.Add (PersonsClothes);
			comboBoxes.Add (PersonAboutToBeMovedInLocation);
			comboBoxes.Add (PersonToTravelBetweenLocations);
			comboBoxes.Add (PersonThatLooking);

			foreach (String it in DialogPerson.Items)
			{
				foreach (ComboBox jt in comboBoxes) { jt.Items.Add (it); }
			}

			setPerson (DialogPerson.Items[0]);

			TreeViewItem root1 = new TreeViewItem ();
			root1.Header = DialogPerson.SelectedItem + ": " + ReplicaTextBox.Text;
			DialogTree.Items.Add (root1);
			root1 = new TreeViewItem ();
			root1.Header = "root";
			EventTree.Items.Add (root1);

			ListOfPossibleReplics.Items.Add ("root");
			AttackType.Items.Add ("Пендель");
			AttackType.Text = (String)AttackType.Items[0];
			attackDescription.Add (AttackType.Text, "Размашистый не сильный удар ногой");
			ReactionType.Items.Add ("Крик");
			ReactionType.Text = (String)ReactionType.Items[0];
			reactionDescription.Add (ReactionType.Text, "Ор выше гор");

			EditTypeOfAttack.IsChecked = true;
		}

		//-----------Настройка события "Диалог"-----------
		private void AddCopyOfThisReplica_Click (object sender, RoutedEventArgs e)
		{
			if (DialogTree.SelectedItem != null)
			{
				if (DialogTree.SelectedItem.GetType () == typeof (TreeViewItem))
				{
					TreeViewItem node = new TreeViewItem ();
					node.Header = DialogPerson.Text + ": " + ReplicaTextBox.Text;
					((TreeViewItem)DialogTree.SelectedItem).Items.Add (node);

					int n = ((TreeViewItem)DialogTree.SelectedItem).Items.Count - 1;
					((TreeViewItem)DialogTree.SelectedItem).IsExpanded = true;
					((TreeViewItem)((TreeViewItem)DialogTree.SelectedItem).Items[n]).IsSelected = true;
				}
			}
		}
		private void ReplicaTextBox_TextChanged (object sender, TextChangedEventArgs e)
		{
			if (DialogTree != null)
			{
				if (DialogTree.SelectedItem != null)
				{
					((TreeViewItem)DialogTree.SelectedItem).Header = DialogPerson.Text + ": " + ReplicaTextBox.Text;
				}
			}
		}
		private void DialogPerson_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			if (DialogTree != null)
			{
				if (DialogTree.SelectedItem != null)
				{
					((TreeViewItem)DialogTree.SelectedItem).Header = DialogPerson.SelectedItem + ": " + ReplicaTextBox.Text;
				}
			}
		}
		private void PossibleReplica_TextChanged (object sender, TextChangedEventArgs e)
		{
			if (ListOfPossibleReplics != null)
			{
				if (ListOfPossibleReplics.SelectedIndex != -1)
				{
					numOfPossReplica = ListOfPossibleReplics.SelectedIndex;
					ListOfPossibleReplics.Items[numOfPossReplica] = PossibleReplica.Text;
				}
				else if (ListOfPossibleReplics.Items.Count !=0 && numOfPossReplica != -1)
				{
					ListOfPossibleReplics.Items[numOfPossReplica] = PossibleReplica.Text;
				}
			}
		}
		private void AddNewPossibleReplica_Click (object sender, RoutedEventArgs e)
		{
			if (ListOfPossibleReplics != null && DialogTree != null)
			{
				if (ListOfPossibleReplics.Items.Count != 0 && numOfPossReplica != -1)
				{
					ListOfPossibleReplics.Items.Add (ListOfPossibleReplics.Items[numOfPossReplica]);
				}
				else { ListOfPossibleReplics.Items.Add (PossibleReplica.Text); }

				numOfPossReplica = ListOfPossibleReplics.Items.Count - 1;
			}
		}
		private void DeledThisPossibleReplica_Click (object sender, RoutedEventArgs e)
		{
			if (ListOfPossibleReplics != null && DialogTree != null)
			{
				if (ListOfPossibleReplics.Items.Count != 0 )
				{
					if (numOfPossReplica != -1)
					{
						ListOfPossibleReplics.Items.RemoveAt (numOfPossReplica);
						--numOfPossReplica;
					}
					else { ListOfPossibleReplics.Items.RemoveAt (0); }
				}
			}
		}

		private TreeViewItem GetParentOf (TreeViewItem node, TreeViewItem container)
		{
			if (container.Items.Contains (node)) return container;
			TreeViewItem ret = null;
			foreach (TreeViewItem it in container.Items)
			{
				ret = GetParentOf (node, it);
				if (ret != null) break;
			}

			return ret;
		}
		private TreeViewItem GetParentOf (TreeViewItem node, TreeView container)
		{
			TreeViewItem ret = null;
			foreach (TreeViewItem it in container.Items)
			{
				ret = GetParentOf (node, it);
				if (ret != null) break;
			}
			return ret;
		}
		private void DeepCopy (TreeViewItem from, TreeViewItem to)
		{
			foreach (TreeViewItem it in from.Items)
			{
				int i = to.Items.Count;
				to.Items.Add (new TreeViewItem ());
				((TreeViewItem)to.Items[i]).Header = it.Header.ToString ();
				((TreeViewItem)to.Items[i]).IsExpanded = true;
				if (it.Items.Count != 0) { DeepCopy (it, (TreeViewItem)to.Items[i]); } // ссанина
				to.UpdateLayout ();
			}
		}
		private void DeleteThisReplica0_Click (object sender, RoutedEventArgs e)
		{
			if (DialogTree.SelectedItem != null)
			{
				if (!DialogTree.SelectedItem.Equals (DialogTree.Items[0])) // если удаляем не корень
				{
					TreeViewItem prew = GetParentOf ((TreeViewItem)DialogTree.SelectedItem, DialogTree);

					DeepCopy ((TreeViewItem)DialogTree.SelectedItem, prew);
					prew.Items.Remove (DialogTree.SelectedItem);
				}
				// а корень удалять запрещено :)
			}
		}
		private void DeleteThisReplica1_Click (object sender, RoutedEventArgs e)
		{
			DeleteThisReplica0_Click (sender, e);
		}
		private void DialogTree_Click (object sender, RoutedEventArgs e)
		{
			String str = ((TreeViewItem)DialogTree.SelectedItem).Header.ToString ();
			String[] substr = str.Split (':');
			int i = -1;
			foreach (ComboBox cb in comboBoxes)
			{
				++i;
				if (cb.Text.Equals (substr[0])) break;
			}
			DialogPerson.Text = substr[0];
			if (substr[1].Length != 0) ReplicaTextBox.Text = substr[1].Substring (1);
		}
		private void AddNewListOfPossibleReplics_Click (object sender, RoutedEventArgs e)
		{
			if (DialogTree.SelectedItem != null)
			{
				if (DialogTree.SelectedItem.GetType () == typeof (TreeViewItem))
				{
					((TreeViewItem)DialogTree.SelectedItem).IsExpanded = true;
					foreach (String it in ListOfPossibleReplics.Items)
					{
						TreeViewItem node = new TreeViewItem ();
						node.Header = DialogPerson.Text + ": " + it;
						((TreeViewItem)DialogTree.SelectedItem).Items.Add (node);
						DialogTree.UpdateLayout ();
					}
					ListOfPossibleReplics.Items.Clear ();
				}
			}
		}

		//-----------Настройка события "Атака"-------------
		private bool slider = false;
		private void ChangeComboBoxAndDictionaryContent (ref Dictionary<String,String> dDescr, ref ComboBox type, String name)
		{
			String description = dDescr[type.Text];
			dDescr.Remove (type.Text);
			dDescr.Add (name, description);
			int i = 0;
			foreach (String it in type.Items)
			{
				if (it == type.Text) { break; }
				++i;
			}
			type.Items.RemoveAt (i);
			type.Items.Add (name);
			type.Text = name;
		}
		private void AtackType_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			if (EditTypeOfAttack.IsChecked == true)
			{
				NameOfAttackOrReactionType.Text = AttackType.Text;
				DescriptionOfAttackOrReactionType.Text = attackDescription[AttackType.Text];
			}
		}
		private void NameOfAttackOrReactionType_TextChanged (object sender, TextChangedEventArgs e)
		{
			if (EditTypeOfAttack.IsChecked == true) //если режим редактирования атаки
			{
				ChangeComboBoxAndDictionaryContent (ref attackDescription, ref AttackType, NameOfAttackOrReactionType.Text);
			}
			else if (reactionDescription.Keys.Contains (ReactionType.Text))
			{
				ChangeComboBoxAndDictionaryContent (ref reactionDescription, ref ReactionType, NameOfAttackOrReactionType.Text);
			}
		}
		private void EditTypeOfAttack_Checked (object sender, RoutedEventArgs e)
		{
			NameOfAttackOrReactionType.Text = AttackType.Text;
			DescriptionOfAttackOrReactionType.Text = attackDescription[AttackType.Text];
		}
		private void ReactionType_SelectionChanged (object sender, SelectionChangedEventArgs e)
		{
			if (EditTypeOfReaction.IsChecked == true)
			{
				NameOfAttackOrReactionType.Text = ReactionType.Text;
				DescriptionOfAttackOrReactionType.Text = reactionDescription[ReactionType.Text];
			}
		}
		private void EditTypeOfReaction_Checked (object sender, RoutedEventArgs e)
		{
			NameOfAttackOrReactionType.Text = ReactionType.Text;
			DescriptionOfAttackOrReactionType.Text = reactionDescription[ReactionType.Text];
		}
		private void DamageSlider_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (slider && DamageTextBox.Text.Length > 0)
			{
				DamageTextBox.Text = ((int)(DamageSlider.Value * 10.0f)).ToString ();
			}
			slider = true;
		}
		private void DamageTextBox_TextChanged (object sender, TextChangedEventArgs e)
		{
			if (!slider && DamageTextBox.Text.Length > 0)
			{
				if (!IsTextAllowed (DamageTextBox.Text))
				{
					DamageSlider.Value = 0;
					DamageTextBox.Text = "0";
				}
				else
				{
					float val = float.Parse (DamageTextBox.Text) / 10.0f;
					if (val > 10)
					{
						DamageSlider.Value = 10;
						DamageTextBox.Text = "100";
					}
					if (val < 0)
					{
						DamageSlider.Value = 0;
						DamageTextBox.Text = "0";
					}
					else DamageSlider.Value = val;
				}
			}
			slider = false;
		}
		private void AddNewTypeOfAttackOrReaction_Click (object sender, RoutedEventArgs e)
		{
			if (EditTypeOfAttack.IsChecked == true)
			{
				// добавляем новый тип атаки
				List<String> container = new List<String> ();
				foreach (String it in AttackType.Items) container.Add (it);
				String name = AttackType.Text + container.Count (p => p == AttackType.Text).ToString ();
				AttackType.Items.Add (name);
				AttackType.Text = name;
				attackDescription.Add (name, DescriptionOfAttackOrReactionType.Text);
			}
			else
			{
				// добавляем новый тип реакции
				List<String> container = new List<String> ();
				foreach (String it in ReactionType.Items) container.Add (it);
				String name = ReactionType.Text + container.Count (p => p == ReactionType.Text).ToString ();
				ReactionType.Items.Add (name);
				ReactionType.Text = name;
				reactionDescription.Add (name, DescriptionOfAttackOrReactionType.Text);
			}
		}
		private void DeleteThisTypeOfAttackOrReaction_Click (object sender, RoutedEventArgs e)
		{
			if (EditTypeOfAttack.IsChecked == true && AttackType.Items.Count > 1)
			{
				// удаление типа атаки
				String name = AttackType.Text;
				attackDescription.Remove (name);
				int i = 0;
				foreach (String it in AttackType.Items)
				{
					if (it == name) { break; }
					++i;
				}
				AttackType.Items.RemoveAt (i);
			}
			else if (ReactionType.Items.Count > 1)
			{
				// удаление типа реакции
				String name = ReactionType.Text;
				reactionDescription.Remove (name);
				int i = 0;
				foreach (String it in ReactionType.Items)
				{
					if (it == name) { break; }
					++i;
				}
				ReactionType.Items.RemoveAt (i);
			}
		}
		private void DescriptionOfAttackOrReactionType_TextChanged (object sender, TextChangedEventArgs e)
		{
			if (EditTypeOfAttack.IsChecked == true)
			{
				attackDescription[AttackType.Text] = DescriptionOfAttackOrReactionType.Text;
			}
			else
			{
				reactionDescription[ReactionType.Text] = DescriptionOfAttackOrReactionType.Text;
			}
		}

		//---------------------------------------------------------------------------

		public MainWindow ()
		{
			InitializeComponent ();
			initFieldsByDefault ();
		}

		//---------------------------------------------------------------------------
	}
}
