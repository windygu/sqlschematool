using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace Lewis.Xml
{

	/// <summary>
	/// 
	/// </summary>
	public class CollectionEditor : System.ComponentModel.Design.ArrayEditor
	{		
	
		public CollectionEditor(PropertyDescriptorCollection PropertyDescriptorCollection) : base (typeof(CollectionEditorEntry[]))
		{		
			int i = 0;

			_entries = new CollectionEditorEntry[PropertyDescriptorCollection.Count];

			foreach(PropertyDescriptor propertyDescriptor in PropertyDescriptorCollection)
			{
				_entries[i++] = new CollectionEditorEntry(propertyDescriptor);
			}

			_templateNode = (XmlGridNodeSchemaBinded)((IPropertyDescriptorCommon)PropertyDescriptorCollection[0]).XmlGridNodeSchemaBinded.Clone();

			_parentNode = (XmlGridNodeSchemaBinded)((IPropertyDescriptorCommon)PropertyDescriptorCollection[0]).XmlGridNodeSchemaBinded.ParentNode;

			IOccurencyRestriction occurRestr = (IOccurencyRestriction) _templateNode.GetRestriction(XmlNodeSchemaBindedRestrictionsType.Occurency);

			if (occurRestr != null)
			{
				if (occurRestr.MinOccur != -1)_minOccur = occurRestr.MinOccur;
				
				if (occurRestr.MaxOccur != -1)_maxOccur = occurRestr.MaxOccur;
			}

		}

		private XmlGridNodeSchemaBinded _templateNode;

		private XmlGridNodeSchemaBinded _parentNode;

		private object[] _entries;

		protected override Type CreateCollectionItemType()
		{
			return typeof(CollectionEditorEntry);
		}

		protected override object[] GetItems(object editValue)
		{
			return _entries;
		}
		
		protected override object SetItems(object editValue, object[] value)
		{	
			_entries = value;

			return editValue;
		}

		protected override bool CanSelectMultipleInstances()
		{
			return false;
		}

		protected override object CreateInstance(Type itemType)
		{
			return new CollectionEditorEntry(XmlGridNode.GetPropertyDescriptor((XmlGridNodeSchemaBinded)_parentNode.AppendChild(_templateNode)));
		}

		protected override void DestroyInstance(object instance)
		{
			_parentNode.RemoveChild(((IPropertyDescriptorCommon)((CollectionEditorEntry)instance)._propertyDescriptorCollection[0]).XmlGridNodeSchemaBinded);
		}

		#region collection form events

		protected override System.ComponentModel.Design.CollectionEditor.CollectionForm CreateCollectionForm()
		{
			CollectionEditor.CollectionForm  collectionForm = base.CreateCollectionForm ();

			collectionForm.Load += new EventHandler(collectionForm_Load);

			collectionForm.Controls[1].Click += new EventHandler(Cancel_Click);

			collectionForm.Controls[2].Click += new EventHandler(OK_Click);

			_addButton = (System.Windows.Forms.Button) collectionForm.Controls[6];
			_addButton.Click += new EventHandler(Add_Click);
			_addButton.EnabledChanged +=new EventHandler(_addButton_EnabledChanged);

			_removeButton = (System.Windows.Forms.Button) collectionForm.Controls[5];
			_removeButton.Click += new EventHandler(Remove_Click);
			_removeButton.EnabledChanged +=new EventHandler(_removeButton_EnabledChanged);

			return collectionForm;
		}

		private void collectionForm_Load(object sender, EventArgs e)
		{
			FreezeNodes();

			_itemsCount = _entries.Length;

			ValidateCollectionRange();
		}

		private void collectionForm_Closed(bool Cancel)
		{
			UnfreezeNodes(Cancel);
		}

		private void FreezeNodes()
		{
			_parentNode.FreezeNode();
		}

		private void UnfreezeNodes(bool CancelChangest)
		{
			_parentNode.UnfreezeNode(CancelChangest);
		}

		private void Cancel_Click(object sender, EventArgs e)
		{
			collectionForm_Closed(true);
		}

		private void OK_Click(object sender, EventArgs e)
		{
			collectionForm_Closed(false);

			_parentNode.xmlGridDocumentSchemaBinded.RefreshXmlGrid();
		}

		private void Add_Click(object sender, EventArgs e)
		{
			_itemsCount++;

			ValidateCollectionRange();
		}

		private void Remove_Click(object sender, EventArgs e)
		{
			_itemsCount--;

			ValidateCollectionRange();
		}

		private void _addButton_EnabledChanged(object sender, EventArgs e)
		{
			ValidateCollectionRange();
		}

		private void _removeButton_EnabledChanged(object sender, EventArgs e)
		{
			ValidateCollectionRange();
		}


		#endregion

		#region validate collection range 

		private void ValidateCollectionRange()
		{
			if (_itemsCount <= _minOccur)
			{
				_removeButton.Enabled = false;
			}
			else 
			{
				_removeButton.Enabled = true;
			}

			if (_itemsCount >= _maxOccur)
			{
				_addButton.Enabled = false;
			}
			else 
			{
				_addButton.Enabled = true;
			}

		}

		private int _itemsCount = 0;

		private readonly decimal _minOccur = 0;
		private readonly decimal _maxOccur = decimal.MaxValue;

		private System.Windows.Forms.Button _addButton;
		private System.Windows.Forms.Button _removeButton;

		#endregion

	}
}
