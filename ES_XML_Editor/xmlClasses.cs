﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using Microsoft.Win32;


namespace ES_XML_Editor
{
    public class xmlAttrib : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler changedHandler = PropertyChanged;

            if (changedHandler != null)
            {
                changedHandler(this, new PropertyChangedEventArgs(name));
            }
        }

        private String attributeName;
        private String attributeValue;

        #region properties

        public XName Name
        {
            get
            {
                return this.attributeName;
            }
        }

        public String Value
        {
            set
            {
                this.attributeValue = value;
                OnPropertyChanged("Value");
            }
            get
            {
                return this.attributeValue;
            }
        }

        #endregion
        
        #region constructors

        public xmlAttrib()
        {
            attributeName = "";
            attributeValue = "";
        }

        public xmlAttrib(String n, String v)
        {
            attributeName = n;
            attributeValue = v;
        }

        public xmlAttrib(xmlAttrib source)
        {
            attributeName = source.attributeName;
            attributeValue = source.attributeValue;
        }

        public xmlAttrib(XAttribute source)
        {
            attributeName = source.Name.ToString();
            attributeValue = source.Value;
        }

        //public xmlAttrib(xmlAttribute source)
        //{
        //    attributeName = source.Name.ToString();
        //    attributeValue = source.Value;
        //}

        #endregion

        public override string ToString()
        {
            String temp = Name.ToString();
            if (Name.Namespace == XNamespace.Xmlns)
            {
                temp = "xmlns:" + (Name.LocalName) + "=\"" + Value + "\"";
                return temp;
            }

            //if (Name.Namespace != null || Name.Namespace != "")
            //{
            //    temp = "xmlns:" + (Name.LocalName) + "=\"" + Value + "\"";
            //    return temp;
            //}
            temp = Name + "=\"" + Value + "\"";
            return temp;
        }
    }

    public class xmlElem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler changedHandler = PropertyChanged;

            if (changedHandler != null)
            {
                changedHandler(this, new PropertyChangedEventArgs(name));
            }
        }

        #region members

        private String elementName;
        private String elementValue;
        private xmlElem Parent;
        private List<xmlElem> pElements;
        private List<xmlAttrib> pAttributes;

        #endregion

        #region properties

        public xmlAttrib NameAttribute
        {
            get
            {
                return this.Attribute("Name");
            }
        }

        public bool HasAttributes
        {
            get
            {
                if (pAttributes.Count > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public bool HasElements
        {
            get
            {
                if (pElements.Count > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public XName Name
        {
            get
            {
                return this.elementName;
            }
        }

        public String Value
        {
            set
            {
                this.elementValue = value.Trim();
                OnPropertyChanged("Value");
                if (Parent != null)
                {
                    Parent.OnPropertyChanged("Elements");
                    Parent.OnPropertyChanged("onlyChildDataElement");
                    Parent.OnPropertyChanged("onlyChildAttributeElement");
                    Parent.OnPropertyChanged("childRearingElements");
                    Parent.OnPropertyChanged("dummyElements");
                    Parent.OnPropertyChanged("lonelyElements");
                    Parent.OnPropertyChanged("AttributeElements");
                    Parent.OnPropertyChanged("xmlElements");
                }
            }
            get
            {
                return this.elementValue;
            }
        }

        public List<xmlAttrib> Attributes
        {
            set
            {
                if (value != null)
                {
                    pAttributes = value;
                    OnPropertyChanged("Attributes");
                }
            }
            get
            {
                return pAttributes;
            }
        }

        public xmlElem onlyChildDataElement
        {
            set
            {
                OnPropertyChanged("onlyChildDataElement");
            }
            get
            {
                if (Elements.Count == 1)
                {
                    if (Elements[0].HasAttributes == false && Elements[0].HasElements == false)
                    {
                        return Elements[0];
                    }
                }
                return null;
            }
        }

        public xmlElem onlyChildAttributeElement
        {
            set
            {
                OnPropertyChanged("onlyChildAttributeElement");
            }
            get
            {
                if (Elements.Count == 1)
                {
                    if (Elements[0].HasAttributes == true && Elements[0].HasElements == false)
                    {
                        return Elements[0];
                    }
                }
                return null;
            }
        }

        public List<xmlElem> Elements
        {
            set
            {
                if (value != null)
                {
                    foreach (xmlElem item in value)
                    {
                        item.Parent = this;
                    }
                    pElements = value;
                    OnPropertyChanged("xmlElements");
                    OnPropertyChanged("Elements");
                    OnPropertyChanged("onlyChildAttributeElement");
                    OnPropertyChanged("onlyChildDataElement");
                    OnPropertyChanged("childRearingElements");
                    OnPropertyChanged("dummyElements");
                    OnPropertyChanged("lonelyElements");
                    OnPropertyChanged("AttributeElements");
                }
            }
            get
            {
                return pElements;
            }
        }

        public ObservableCollection<xmlAttrib> xmlAttributes
        {
            set
            {
                if (value != null)
                {
                    convertCollection(value, ref pAttributes);
                    OnPropertyChanged("xmlAttributes");
                }
            }
            get
            {
                try
                {
                    ObservableCollection<xmlAttrib> returnable = new ObservableCollection<xmlAttrib>();
                    convertList(pAttributes, ref returnable);
                    return returnable;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ObservableCollection<xmlElem> xmlElements
        {
            set
            {
                if (value != null)
                {
                    convertCollection(value, ref pElements);
                    OnPropertyChanged("xmlElements");
                    OnPropertyChanged("Elements");
                    OnPropertyChanged("onlyChildAttributeElement");
                    OnPropertyChanged("onlyChildDataElement");
                    OnPropertyChanged("childRearingElements");
                    OnPropertyChanged("dummyElements");
                    OnPropertyChanged("lonelyElements");
                    OnPropertyChanged("AttributeElements");
                }
            }
            get
            {
                ObservableCollection<xmlElem> returnable = new ObservableCollection<xmlElem>();
                convertList(pElements, ref returnable);
                return returnable;
            }
        }

        public ObservableCollection<xmlElem> childRearingElements
        {
            set
            {
                OnPropertyChanged("childRearingElements");
            }
            get
            {
                ObservableCollection<xmlElem> returnable = new ObservableCollection<xmlElem>();
                foreach (xmlElem item in pElements)
                {
                    //List<XElement> temp= new List<XElement>(Elements());
                    if (item.pElements.Count > 1 || (item.pElements.Count == 1 && item.pAttributes.Count > 0))
                    {
                        returnable.Add(item);
                    }
                }
                return returnable;
            }
        }

        public ObservableCollection<xmlElem> dummyElements
        {
            set
            {
                OnPropertyChanged("dummyElements");
            }
            get
            {
                ObservableCollection<xmlElem> returnable = new ObservableCollection<xmlElem>();
                foreach (xmlElem item in xmlElements)
                {
                    //List<XElement> temp= new List<XElement>(Elements());
                    if (item.pElements.Count == 1 && item.pAttributes.Count == 0)
                    {
                        returnable.Add(item);
                    }
                }
                return returnable;
            }
        }

        public ObservableCollection<xmlElem> lonelyElements
        {
            set
            {
                OnPropertyChanged("lonelyElements");
            }
            get
            {
                ObservableCollection<xmlElem> returnable = new ObservableCollection<xmlElem>();
                foreach (xmlElem item in pElements)
                {
                    if (item.HasElements == false && item.HasAttributes == false)
                    {
                        returnable.Add(item);
                    }
                }
                return returnable;
            }
        }

        public ObservableCollection<xmlElem> AttributeElements
        {
            set
            {
                OnPropertyChanged("AttributeElements");
            }
            get
            {
                ObservableCollection<xmlElem> returnable = new ObservableCollection<xmlElem>();
                foreach (xmlElem item in pElements)
                {
                    if (item.HasElements == false && item.HasAttributes == true)
                    {
                        returnable.Add(item);
                    }
                }
                return returnable;
            }
        }

        #endregion

        #region constructors

        public xmlElem()
        {
            elementName = "";
            elementValue = "";
            Parent = null;
            pElements = new List<xmlElem>();
            pAttributes = new List<xmlAttrib>();

        }

        public xmlElem(ref xmlElem parent)
        {
            elementName = "";
            elementValue = "";
            Parent = parent;
            pElements = new List<xmlElem>();
            pAttributes = new List<xmlAttrib>();
        }

        public xmlElem(String n, String v)
        {
            elementName = n;
            elementValue = v;
            Parent = null;
            pElements = new List<xmlElem>();
            pAttributes = new List<xmlAttrib>();
        }

        public xmlElem(String n, String v, ref xmlElem parent)
        {
            elementName = n;
            elementValue = v;
            Parent = parent;
            pElements = new List<xmlElem>();
            pAttributes = new List<xmlAttrib>();
        }

        public xmlElem(xmlElem source)
        {
            elementName = source.elementName;
            elementValue = source.elementValue;
            Parent = null;
            pElements = new List<xmlElem>();
            pAttributes = new List<xmlAttrib>();

            //foreach (xmlElem child in source.pElements)
            //{
            //    this.pElements.Add(new xmlElem(child));
                
            //}
            this.AddElements(source.pElements);

            foreach (xmlAttrib item in source.pAttributes)
            {
                this.pAttributes.Add(new xmlAttrib(item));
            }
        }

        public xmlElem(xmlElem source, xmlElem parent)
        {
            elementName = source.elementName;
            elementValue = source.elementValue;
            Parent = parent;
            pElements = new List<xmlElem>();
            pAttributes = new List<xmlAttrib>();

            foreach (xmlElem child in source.pElements)
            {
                this.pElements.Add(new xmlElem(child, this));
            }

            foreach (xmlAttrib item in source.pAttributes)
            {
                this.pAttributes.Add(new xmlAttrib(item));
            }
        }

        //public xmlElem(xmlElement source)
        //{
        //    elementName = source.Name.ToString();
        //    Parent = null;
        //    pElements = new List<xmlElem>();
        //    pAttributes = new List<xmlAttrib>();
        //    if (source.HasElements == false)
        //    {
        //        elementValue = source.Value;
        //    }
        //    else
        //    {
        //        elementValue = "";
        //    }

        //    foreach (XElement child in source.Elements())
        //    {
        //        this.pElements.Add(new xmlElem(child, this));
        //    }

        //    foreach (XAttribute item in source.Attributes())
        //    {
        //        this.pAttributes.Add(new xmlAttrib(item));
        //    }
        //}

        //public xmlElem(xmlElement source, xmlElem parent)
        //{
        //    elementName = source.Name.ToString();
        //    Parent = parent;
        //    pElements = new List<xmlElem>();
        //    pAttributes = new List<xmlAttrib>();

        //    if (source.HasElements == false)
        //    {
        //        elementValue = source.Value;
        //    }
        //    else
        //    {
        //        elementValue = "";
        //    }

        //    foreach (XElement child in source.Elements())
        //    {
        //        this.pElements.Add(new xmlElem(child, this));
        //    }

        //    foreach (XAttribute item in source.Attributes())
        //    {
        //        this.pAttributes.Add(new xmlAttrib(item));
        //    }
        //}

        public xmlElem(XElement source)
        {
            elementName = source.Name.ToString();
            Parent = null;
            pElements = new List<xmlElem>();
            pAttributes = new List<xmlAttrib>();
            if (source.HasElements == false)
            {
                elementValue = source.Value;
            }
            else
            {
                elementValue = "";
            }

            //foreach (XElement child in source.Elements())
            //{
            //    this.pElements.Add(new xmlElem(child, this));
            //}
            this.AddElements(source.Elements());

            foreach (XAttribute item in source.Attributes())
            {
                this.pAttributes.Add(new xmlAttrib(item));
            }
        }

        public xmlElem(XElement source, xmlElem parent)
        {
            elementName = source.Name.ToString();
            Parent = parent;
            pElements = new List<xmlElem>();
            pAttributes = new List<xmlAttrib>();
            if (source.HasElements == false)
            {
                elementValue = source.Value;
            }
            else
            {
                elementValue = "";
            }

            foreach (XElement child in source.Elements())
            {
                this.pElements.Add(new xmlElem(child, this));
            }

            foreach (XAttribute item in source.Attributes())
            {
                this.pAttributes.Add(new xmlAttrib(item));
            }
        }

        #endregion

        #region functions

        // finish this as it might be solution
        protected void convertList(List<xmlElem> source, ref ObservableCollection<xmlElem> destination)
        {
            foreach (xmlElem item in source)
            {
                item.Parent = this;
                destination.Add(item);
                //source.Remove(item);
            }
        }

        protected void convertCollection(ObservableCollection<xmlElem> source, ref List<xmlElem> destination)
        {
            foreach (xmlElem item in source)
            {
                item.Parent = this;
                destination.Add(item);
                //source.Remove(item);
            }
        }

        protected void convertList(List<xmlAttrib> source, ref ObservableCollection<xmlAttrib> destination)
        {
            foreach (xmlAttrib item in source)
            {
                //item.Parent = this;
                destination.Add(item);
                //source.Remove(item);
            }
        }

        protected void convertCollection(ObservableCollection<xmlAttrib> source, ref List<xmlAttrib> destination)
        {
            foreach (xmlAttrib item in source)
            {
                //item.Parent = this;
                destination.Add(item);
                //source.Remove(item);
            }
        }

        public void AddElements(IEnumerable<xmlElem> source)
        {
            List<xmlElem> temp = new List<xmlElem>(source);

            //int i = 0;

            foreach(xmlElem elem in temp)
            {
                elem.Parent = this;
                pElements.Add(elem);
                //pElements[pElements.Count - 1].Parent = this;
                //i++;
            }
            //for (int i = 0; i < temp.Count; i++)
            //{
            //    temp[i].Parent= this;
            //    this.pElements.Add(temp[i]);
            //}

            //this.pElements.AddRange(temp);
        }

        public void AddElements(IEnumerable<XElement> tempSource)
        {
            //List<XElement> source = new List<XElement>(tempSource);
            //List<xmlElem> destination = new List<xmlElem>(source.Count);
            //IEnumerator<XElement> iter = tempSource.GetEnumerator();

            int i = 0;

            foreach (XElement elem in tempSource)
            {
                pElements.Add(new xmlElem(elem));
                pElements[pElements.Count - 1].Parent = this;
                i++;
            }


            //for (int i = 0; i < source.Count; i++)
            //{
            //    iter.MoveNext();
            //    destination.Add(new xmlElem(source[i]));
            //    destination[i].Parent = this;
            //}
            //this.pElements.AddRange(destination);
        }

        public xmlAttrib Attribute(String attrName)
        {

            xmlAttrib childAttr = pAttributes.Find(
                    delegate(xmlAttrib attr)
                    {
                        return (attr.Name == attrName);
                    });

            return childAttr;
        }

        public xmlElem Element(String elemName)
        {
            xmlElem childAttr =  pElements.Find(
                    delegate(xmlElem elem)
                    {
                        return (elem.Name == elemName);
                    });

            return childAttr;
        }

        public void AddElement(xmlElem child)
        {
            child.Parent= this.Parent;
            pElements.Add(new xmlElem(child));
        }

        public void Remove(xmlElem child)
        {
            pElements.Remove(child);
        }

        public override string ToString()
        {
            String temp = "\n<" + elementName;
            if (HasAttributes == true)
            {
                foreach (xmlAttrib attr in pAttributes)
                {
                    temp += " " + attr.ToString();
                }
            }
            temp += ">";

            if (HasElements == true)
            {
                temp += "";
                if (elementValue != "" && elementName != null)
                {
                    temp += elementValue;
                }
                foreach (xmlElem item in pElements)
                {
                    temp += item.ToString() + "";
                }
                temp += "\n</" + elementName + ">";
                return temp;
            }
            else
            {
                if (elementName != null && elementName != "")
                {
                    temp += elementValue;
                }
                temp += "</" + elementName + ">";
                return temp;
            }
        }

        public xmlElem firstChildByAttrValue(String attrName, String attrValue)
        {
            //List<xmlElem> tempList = pElements;

            for (int i = 0; i < pElements.Count; i++)
            {
                xmlAttrib childAttr = pElements[i].pAttributes.Find(
                    delegate(xmlAttrib attr)
                    {
                        return (attr.Name == attrName);
                    });
                try
                {
                    if (childAttr.Value == attrValue)
                    {
                        return pElements[i];
                    }
                }
                catch
                {
                    continue;
                }
            }

            return null;
        }

        public bool replaceChildByAttrValue(String attrName, String attrValue, xmlElem replacingNode)
        {
            for (int i = 0; i < pElements.Count; i++)
            {
                xmlAttrib childAttr = pElements[i].pAttributes.Find(
                    delegate(xmlAttrib attr)
                    {
                        return (attr.Name == attrName);
                    });
                try
                {
                    if (childAttr.Value == attrValue)
                    {
                        this.pElements[i] = new xmlElem(replacingNode);
                        //this.Remove(pElements[i]);
                        //this.Add(replacingNode);
                        OnPropertyChanged("xmlElements");
                        OnPropertyChanged("childRearingElements");
                        OnPropertyChanged("dummyElements");
                        OnPropertyChanged("lonelyElements");
                        OnPropertyChanged("AttributeElements");
                        return true;
                    }
                }
                catch
                {
                    continue;
                }
            }
            return false;
        }

        #endregion
    }

    public class xmlDoc
    {
        private XDeclaration xmlDeclaration;
        private xmlElem rootElem;

        public XDeclaration declaration
        {
            set
            {
                if (value != null)
                {
                    xmlDeclaration = value;
                }
            }
            get
            {
                return xmlDeclaration;
            }
        }

        public xmlElem root
        {
            set
            {
                if (value != null)
                {
                    rootElem = value;
                }
            }
            get
            {
                return rootElem;
            }
        }

        #region constructors

        public xmlDoc()
        {
            xmlDeclaration = new XDeclaration("1.0", "utf-8", "yes");

            rootElem = new xmlElem();
        }

        public xmlDoc(XDeclaration dec)
        {
            xmlDeclaration = new XDeclaration(dec);

            rootElem = new xmlElem();
        }

        public xmlDoc(xmlDoc source)
        {
            xmlDeclaration = new XDeclaration(source.xmlDeclaration);

            rootElem = new xmlElem(source.root);
        }

        public xmlDoc(XDocument source)
        {
            xmlDeclaration = new XDeclaration(source.Declaration);

            rootElem = new xmlElem(source.Root);
        }

        #endregion

        public override string ToString()
        {
            String temp = "";

            if (xmlDeclaration != null || xmlDeclaration.ToString() != "")
            {
                temp = xmlDeclaration + "\n";
            }

            temp += rootElem.ToString();

            return temp;
        }
    }

    #region obsolete

    public class xmlAttribute : XAttribute, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler changedHandler = PropertyChanged;

            if (changedHandler != null)
            {
                changedHandler(this, new PropertyChangedEventArgs(name));
            }
        }

        public XName AttributeName
        {
            get
            {
                return this.Name;
            }
        }

        public String AttributeValue
        {
            set
            {
                this.Value = value;
                OnPropertyChanged("AttributeValue");
            }
            get
            {
                return this.Value;
            }
        }

        public xmlAttribute(XAttribute otherAttr) : base(otherAttr) { }
    }

    public class DataElement : XElement, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler changedHandler = PropertyChanged;

            if (changedHandler != null)
            {
                changedHandler(this, new PropertyChangedEventArgs(name));
            }
        }

        public XName ElementName
        {
            get
            {
                return this.Name;
            }
        }

        public String ElementValue
        {
            set
            {
                this.Value = value;
                OnPropertyChanged("ElementValue");
            }
            get
            {
                return this.Value;
            }
        }

        public DataElement(XElement otherElem) : base(otherElem) { }

        public DataElement(String xmlName) : base(xmlName) { }
    }

    public class WrapperElement : XElement, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler changedHandler = PropertyChanged;

            if (changedHandler != null)
            {
                changedHandler(this, new PropertyChangedEventArgs(name));
            }
        }

        public XName ElementName
        {
            get
            {
                return this.Name;
            }
        }

        public String ElementValue
        {
            set
            {
                this.Value = value;
                OnPropertyChanged("ElementValue");
            }
            get
            {
                return this.Value;
            }
        }

        public DataElement childDataElement
        {
            get
            {
                XElement temp = ElementList()[0];
                if (temp.HasAttributes == false && temp.HasElements == false)
                {
                    return new DataElement(temp);
                }
                return null;
            }
        }

        public AttributesElement AttributeElement
        {
            get
            {
                XElement temp = ElementList()[0];
                if (temp.HasAttributes == true && temp.HasElements == false)
                {
                    return new AttributesElement(temp);
                }
                return null;
            }
        }

        public ObservableCollection<AttributesElement> AttributeElements
        {
            get
            {
                ObservableCollection<AttributesElement> returnable = new ObservableCollection<AttributesElement>();
                foreach (XElement item in Elements())
                {
                    if (item.HasElements == false && item.HasAttributes == true)
                    {
                        returnable.Add(new AttributesElement(item));
                    }
                }
                return returnable;
            }
        }

        public WrapperElement(XElement otherElem) : base(otherElem) { }

        public WrapperElement(String xmlName) : base(xmlName) { }

        private List<XElement> ElementList()
        {
            return new List<XElement>(Elements());
        }
    }

    public class AttributesElement : XElement, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler changedHandler = PropertyChanged;

            if (changedHandler != null)
            {
                changedHandler(this, new PropertyChangedEventArgs(name));
            }
        }

        public XName ElementName
        {
            get
            {
                return this.Name;
            }
        }

        public ObservableCollection<xmlAttribute> xmlAttributes
        {
            get
            {
                ObservableCollection<xmlAttribute> returnable = new ObservableCollection<xmlAttribute>();
                foreach (XAttribute item in Attributes())
                {
                    returnable.Add(new xmlAttribute(item));
                }
                return returnable;
            }
        }

        public AttributesElement(XElement otherElem) : base(otherElem) { }

        public AttributesElement(String xmlName) : base(xmlName) { }
    }

    public class xmlElement : XElement, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler changedHandler = PropertyChanged;

            if (changedHandler != null)
            {
                changedHandler(this, new PropertyChangedEventArgs(name));
            }
        }

        public XName ElementName
        {
            get
            {
                return this.Name;
            }
        }

        public String ElementValue
        {
            set
            {
                this.Value = value;
                OnPropertyChanged("ElementValue");
            }
            get
            {
                return this.Value;
            }
        }

        public ObservableCollection<xmlAttribute> xmlAttributes
        {
            get
            {
                ObservableCollection<xmlAttribute> returnable = new ObservableCollection<xmlAttribute>();
                foreach (XAttribute item in Attributes())
                {
                    returnable.Add(new xmlAttribute(item));
                }
                return returnable;
            }
        }

        public ObservableCollection<xmlElement> xmlElements
        {
            get
            {
                return new ObservableCollection<xmlElement>(ElementList());
            }
        }

        public ObservableCollection<xmlElement> childRearingElements
        {
            get
            {
                ObservableCollection<xmlElement> returnable = new ObservableCollection<xmlElement>();
                foreach (xmlElement item in ElementList())
                {
                    //List<XElement> temp= new List<XElement>(Elements());
                    if (item.ElementList().Count > 1 || (item.ElementList().Count == 1 && item.HasAttributes == true))
                    {
                        returnable.Add(new xmlElement(item));
                    }
                }
                return returnable;
            }
        }

        public ObservableCollection<WrapperElement> dummyElements
        {
            get
            {
                ObservableCollection<WrapperElement> returnable = new ObservableCollection<WrapperElement>();
                foreach (xmlElement item in ElementList())
                {
                    //List<XElement> temp= new List<XElement>(Elements());
                    if (item.ElementList().Count == 1 && item.HasAttributes == false)
                    {
                        returnable.Add(new WrapperElement(item));
                    }
                }
                return returnable;
            }
        }

        public ObservableCollection<DataElement> lonelyElements
        {
            get
            {
                ObservableCollection<DataElement> returnable = new ObservableCollection<DataElement>();
                foreach (XElement item in Elements())
                {
                    if (item.HasElements == false && item.HasAttributes == false)
                    {
                        returnable.Add(new DataElement(item));
                    }
                }
                return returnable;
            }
        }

        public ObservableCollection<AttributesElement> AttributeElements
        {
            get
            {
                ObservableCollection<AttributesElement> returnable = new ObservableCollection<AttributesElement>();
                foreach (XElement item in Elements())
                {
                    if (item.HasElements == false && item.HasAttributes == true)
                    {
                        returnable.Add(new AttributesElement(item));
                    }
                }
                return returnable;
            }
        }

        public xmlElement(XElement otherElem) : base(otherElem) { }

        public xmlElement(String xmlName) : base(xmlName) { }

        public xmlElement firstChildByAttrValue(XName attrName, String attrValue)
        {
            List<xmlElement> tempList = ElementList();

            for (int i = 0; i < tempList.Count; i++)
            {
                XAttribute childAttr = tempList[i].Attribute(attrName);
                try
                {
                    if (childAttr.Value == attrValue)
                    {
                        return tempList[i];
                    }
                }
                catch
                {
                    continue;
                }
            }

            return null;
        }

        public bool replaceChildByAttrValue(XName attrName, String attrValue, xmlElement replacingNode)
        {
            List<xmlElement> tempList = ElementList();

            for (int i = 0; i < tempList.Count; i++)
            {
                XAttribute childAttr = tempList[i].Attribute(attrName);
                try
                {
                    if (childAttr.Value == attrValue)
                    {
                        tempList[i].Remove();
                        this.Add(replacingNode);
                        OnPropertyChanged("xmlElements");
                        OnPropertyChanged("childRearingElements");
                        OnPropertyChanged("dummyElements");
                        OnPropertyChanged("lonelyElements");
                        OnPropertyChanged("AttributeElements");
                        return true;
                    }
                }
                catch
                {
                    continue;
                }
            }
            return false;
        }

        public List<xmlElement> ElementList()
        {
            List<xmlElement> theList = new List<xmlElement>();
            foreach (XElement item in Elements())
            {
                theList.Add(new xmlElement(item));
            }
            return theList;
        }

        private List<xmlElement> ListConverter(List<XElement> source)
        {
            List<xmlElement> theList = new List<xmlElement>();
            foreach (XElement item in source)
            {
                theList.Add(new xmlElement(item));
            }
            return theList;
        }

        public void setAttribute(XName attrName, String attrValue)
        {
            if (this.Attribute(attrName) != null)
            {
                this.SetAttributeValue(attrName, attrValue);
            }
            else if (this.HasElements == true)
            {
                foreach (xmlElement item in xmlElements)
                {
                    item.setAttribute(attrName, attrValue);
                }
            }
        }//endfunction
    }
    #endregion
}