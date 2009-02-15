using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;

namespace Lewis.Xml
{

	public enum EditCollectionModes
	{
		CollectionEditor,
		ContextMenu,
		BothEditors,
		NoneEditor
	}

	/// <summary>
	/// The control intended for displaying and editing data of an XML file in property grid. 
	/// Also this control fits data for convenient displaying by particular rules defined in a schema file.
	/// this code came from codeproject at: http://www.codeproject.com/cs/miscctrl/XmlGrid.asp
	/// </summary>
	public class XmlGrid : System.Windows.Forms.PropertyGrid
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ContextMenu itemsContextMenu;
		private XmlGridNode _xmlGridNodeTop;
		private string _xmlFileName;
		private string _xmlSchemaFileName;

		public EditCollectionModes EditCollectionMode = EditCollectionModes.BothEditors;

		public XmlGrid()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();			
		}

		public void LoadXml(string XmlFileName, string XmlSchemaFileName)
		{
			try
			{
				_xmlFileName = XmlFileName;
				_xmlSchemaFileName = XmlSchemaFileName;
				_xmlGridNodeTop = new XmlGridNode(XmlFileName, XmlSchemaFileName, this);		
				this.SelectedObject = _xmlGridNodeTop;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void SaveXml(string xmlFileName)
		{
			((System.Xml.XmlDocument)_xmlGridNodeTop.xmlGridNodeSchemaBinded.xmlGridDocumentSchemaBinded.XmlNode).Save(xmlFileName);
		}

		public XmlDocument	XmlDocument
		{
			get
			{
				return 	_xmlGridNodeTop == null ? null : (XmlDocument)_xmlGridNodeTop.xmlGridNodeSchemaBinded.XmlNode;
			}
		}

		public XmlNode XmlTopNode
		{
			get
			{
                return _xmlGridNodeTop == null ? null : (XmlNode)_xmlGridNodeTop.xmlGridNodeSchemaBinded.XmlNode;
			}
			set 
			{
                if (_xmlGridNodeTop != null)
                {
                    _xmlGridNodeTop.xmlGridNodeSchemaBinded.XmlNode = value;
                }
			}
		}

		public XmlSchema XmlSchema
		{
			get
			{
				return _xmlGridNodeTop == null ? null : _xmlGridNodeTop.xmlGridNodeSchemaBinded.XmlSchema;
			}
		}

		public bool CaptionIsName{get{return _xmlGridNodeTop.xmlGridNodeSchemaBinded.xmlGridDocumentSchemaBinded.captionIsName;}}  

		public bool OmitXmlDeclaration{get{return _xmlGridNodeTop.xmlGridNodeSchemaBinded.xmlGridDocumentSchemaBinded.omitXmlDeclaration;}}  

		public string XmlFileName{get{return _xmlFileName;}}

		public string XmlSchemaFileName{get{return _xmlSchemaFileName;}}

		public XmlGridNodeSchemaBinded SelectedXmlGridNode()
		{
			return ((IPropertyDescriptorCommon)this.SelectedGridItem.PropertyDescriptor).XmlGridNodeSchemaBinded;
		}

		public XmlGridNodeSchemaBinded PasteXmlGridNode(XmlNode xnCopy, string path, GridItem topGridItem, GridItem Destination)
		{
			XmlGridNodeSchemaBinded nodeDestination = null;
			GridItem gi = this.FindGridItem(path, topGridItem, true);
			if (gi != null)
			{
				XmlNodeSchemaBinded nodeCopy = ((IPropertyDescriptorCommon)gi.PropertyDescriptor).XmlGridNodeSchemaBinded.Clone();
				nodeDestination = ((IPropertyDescriptorCommon)Destination.PropertyDescriptor).XmlGridNodeSchemaBinded;
				nodeCopy.XmlNode.InnerXml = xnCopy.InnerXml;
				nodeDestination.AppendChild(nodeCopy);
				nodeDestination.xmlGridDocumentSchemaBinded.RefreshXmlGrid();
			}
			return nodeDestination;
		}

		public void RemoveXmlGridNode(string path, GridItem topGridItem)
		{
			GridItem gi = this.FindGridItem(path, topGridItem, true);
			if (gi != null)
			{
				XmlGridNodeSchemaBinded xmlGridNodeSchemaBinded = ((IPropertyDescriptorCommon)gi.PropertyDescriptor).XmlGridNodeSchemaBinded;
				xmlGridNodeSchemaBinded.ParentNode.RemoveChild(xmlGridNodeSchemaBinded);
				xmlGridNodeSchemaBinded.xmlGridDocumentSchemaBinded.RefreshXmlGrid();	
			}
		}

		// recurse grid items tree
		public GridItem FindGridItem(string path, GridItem gi, bool expandSubItems)
		{
			GridItem giRet = null;
            string[] textValue = new string[] { };
            if (path.Contains("["))
            {
                textValue = path.Split('[');
            }
            if (textValue.Length > 1)
            {
                textValue[1] = textValue[1].Replace("]", "");
            }
            string[] nodes = textValue.Length > 1 ? path.Replace("[" + textValue[1] + "]", "").Split('\\') : path.Split('\\');
            // start at the top
			if ( nodes.Length > 1 && nodes[0] == gi.Label)
			{
				nodes[0] = nodes[0] + @"\";
				path = path.Replace(nodes[0], "");
                for (int ii = 0; ii < gi.GridItems.Count; ii++ )
                {
                    GridItem giChild = gi.GridItems[ii];
                    string value = typeof(string).IsInstanceOfType(giChild.Value) ? (string)giChild.Value : null;
                    //XmlGridNodeSchemaBinded xmlGridNodeSchemaBinded = ((IPropertyDescriptorCommon)giChild.PropertyDescriptor).XmlGridNodeSchemaBinded;
                    if (giChild.Label == nodes[1])
                    {
                        if (giChild.Expandable && expandSubItems)
                        {
                            // expand any sub items for the selected griditem that are not already expanded
                            this.SelectedGridItem = giChild;
                            this.SelectedGridItem.Select();
                            this.SelectedGridItem.Expanded = true;
                        }
                        if (nodes.Length > 2)
                        {
                            giRet = FindGridItem(path, giChild, expandSubItems);
                        }
                        else if (textValue.Length > 1 && value !=null && value.Contains(textValue[1].ToLower()))
                        {//xmlGridNodeSchemaBinded.XmlNode.InnerText.ToLower()
                            giRet = giChild;
                        }
                        else if (textValue.Length < 1)
                        {
                            giRet = giChild;
                        }
                        break;
                    }
                    else if (giChild.Expandable && expandSubItems)
                    {
                        this.SelectedGridItem = giChild;
                        this.SelectedGridItem.Select();
                        this.SelectedGridItem.Expanded = false;
                    }

                    if (giRet != null)
                    {
                        if (giRet.Expandable && expandSubItems)
                        {
                            // expand any sub items for the selected griditem that are not already expanded
                            this.SelectedGridItem = giRet;
                            this.SelectedGridItem.Select();
                            this.SelectedGridItem.Expanded = expandSubItems;
                        }
                        break;
                    }
                }
                if (giRet == null)
                {
                    giRet = FindGridItem(nodes[0] + path, NextGridItem(gi), true);
                }
			}
			return giRet;
		}

        private GridItem NextGridItem(GridItem gi)
        {
            GridItem retGridItem = null;
            int itemCount = gi.Parent.GridItems.Count;
            int ii = 0;
            int start = 0;
            if (gi.Tag != null)
            {
                start = (int)gi.Tag;
            }
            for (ii = start; ii < itemCount; ii++)
            {
                GridItem giChild = gi.Parent.GridItems[ii];
                if (giChild.Label == gi.Label)
                {
                    if (ii + 1 < itemCount)
                    {
                        retGridItem = gi.Parent.GridItems[ii + 1];
                        retGridItem.Tag = ii + 1;
                        break;
                    }
                }
            }
            if (ii >= itemCount)
            {
                retGridItem = gi.Parent.GridItems[itemCount - 1];
            }
            return retGridItem;
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{            
			if( disposing )
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.itemsContextMenu = new System.Windows.Forms.ContextMenu();
			// 
			// itemsContextMenu
			// 
			this.itemsContextMenu.Popup += new System.EventHandler(this.itemsContextMenu_Popup);
			// 
			// XmlGrid
			// 
			this.ContextMenu = this.itemsContextMenu;

		}
		#endregion

		#region Context menu

		private void addElemMenuItem_Click(object sender, EventArgs e)
		{			
			XmlGridNodeSchemaBinded xmlGridNodeSchemaBinded = ContextMenuDataForSGI.XmlGridNodeSchemaBinded;

			xmlGridNodeSchemaBinded.ParentNode.AppendChild(xmlGridNodeSchemaBinded);

			xmlGridNodeSchemaBinded.xmlGridDocumentSchemaBinded.RefreshXmlGrid();	
		}

		private void removeElemMenuItem_Click(object sender, EventArgs e)
		{
			XmlGridNodeSchemaBinded xmlGridNodeSchemaBinded = ContextMenuDataForSGI.XmlGridNodeSchemaBinded;

			xmlGridNodeSchemaBinded.ParentNode.RemoveChild(xmlGridNodeSchemaBinded);

			xmlGridNodeSchemaBinded.xmlGridDocumentSchemaBinded.RefreshXmlGrid();	
		}

		private void itemsContextMenu_Popup(object sender, System.EventArgs e)
		{
			ContextMenu.MenuItems.Clear();

			AddRemoveItemContextData contextMenuData = this.ContextMenuDataForSGI;

			if (contextMenuData == null) return;

			if (EditCollectionMode != EditCollectionModes.BothEditors && EditCollectionMode != EditCollectionModes.ContextMenu)	return;

			switch (contextMenuData.ContextMenuType)
			{
				case GridItemData.ContextMenuTypes.AddItem:
					ContextMenu.MenuItems.Add(new MenuItem("Add " + contextMenuData.XmlGridNodeSchemaBinded.displayedName, new EventHandler(addElemMenuItem_Click)));
					ContextMenu.MenuItems[0].Enabled = contextMenuData.IsContextItemEnabled;
					break;
				case GridItemData.ContextMenuTypes.RemoveItem:
					ContextMenu.MenuItems.Add(new MenuItem("Remove " + contextMenuData.XmlGridNodeSchemaBinded.displayedName, new EventHandler(removeElemMenuItem_Click)));
					ContextMenu.MenuItems[0].Enabled = contextMenuData.IsContextItemEnabled;
					break;
				case GridItemData.ContextMenuTypes.None:
					return;
			}
		}

		/// <summary>
		/// Get data about context menu for selected grid item of PG.
		/// </summary>
		private AddRemoveItemContextData ContextMenuDataForSGI
		{
			get
			{
				if (SelectedGridItem == null) return null;
				if (SelectedGridItem.PropertyDescriptor == null) return null;

				if (SelectedGridItem.PropertyDescriptor is XmlGridNodesCollectionPropertyDescriptor)
				{
					return new AddRemoveItemContextData(((IPropertyDescriptorCommon)((XmlGridNodesCollection)SelectedGridItem.Value).GetProperties()[0]).XmlGridNodeSchemaBinded, GridItemData.ContextMenuTypes.AddItem, SelectedGridItem.GridItems.Count);
				}
				
				//Each PropertyDescriptor in this project beside XmlGridNodesCollectionPropertyDescriptor,
				//implement IPropertyDescriptorCommon interface
				XmlGridNodeSchemaBinded xmlGridNodeSchemaBinded = ((IPropertyDescriptorCommon)SelectedGridItem.PropertyDescriptor).XmlGridNodeSchemaBinded;

				if (((XmlGridNodeSchemaBinded)xmlGridNodeSchemaBinded).bindAsArray)
				{
					return new AddRemoveItemContextData(xmlGridNodeSchemaBinded, GridItemData.ContextMenuTypes.RemoveItem,SelectedGridItem.Parent.GridItems.Count);
				}

				return new AddRemoveItemContextData(xmlGridNodeSchemaBinded, GridItemData.ContextMenuTypes.None,0);
			}
		}

		private class GridItemData
		{
			internal GridItemData(XmlGridNodeSchemaBinded XmlGridNodeSchemaBinded, AddRemoveItemContextData.ContextMenuTypes ContextMenuType)
			{
				this.XmlGridNodeSchemaBinded = XmlGridNodeSchemaBinded;

				this.ContextMenuType = ContextMenuType;
			}

			/// <summary>
			/// Node corresponded to particular grid item of PG.
			/// </summary>
			internal readonly XmlGridNodeSchemaBinded XmlGridNodeSchemaBinded;

			/// <summary>
			/// Context menu type corresponded to particular grid item of PG.
			/// </summary>
			internal readonly ContextMenuTypes ContextMenuType;

			internal enum ContextMenuTypes
			{
				AddItem,
				RemoveItem,
				None
			}
		}

		private class AddRemoveItemContextData : GridItemData
		{
			internal AddRemoveItemContextData(XmlGridNodeSchemaBinded XmlGridNodeSchemaBinded, GridItemData.ContextMenuTypes ContextMenuType, int CurrentItemsCount) : base(XmlGridNodeSchemaBinded, ContextMenuType)
			{
				currentItemsCount = CurrentItemsCount;

				IOccurencyRestriction occurRestr = (IOccurencyRestriction)XmlGridNodeSchemaBinded.GetRestriction(XmlNodeSchemaBindedRestrictionsType.Occurency);

                if (occurRestr != null)
                {
                    minOccur = occurRestr.MinOccur;
                    maxOccur = occurRestr.MaxOccur;
                }
			}

			internal readonly decimal maxOccur;

			internal readonly decimal minOccur;

			internal readonly int currentItemsCount;

			internal bool IsContextItemEnabled
			{
				get
				{
					if (ContextMenuType == ContextMenuTypes.AddItem)
					{
						if (maxOccur <= currentItemsCount)return false;
						else return true;
					}
					else
					{
						if (minOccur >= currentItemsCount)return false;
						else return true;					
					}
					
				}
			}
		}

		#endregion
		
	}
}
