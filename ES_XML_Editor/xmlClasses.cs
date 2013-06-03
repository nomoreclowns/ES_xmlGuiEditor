using System;
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

    public class xmlAttribute : XAttribute
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

    public class DataElement : XElement
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


    public class WrapperElement : XElement
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

    public class AttributesElement : XElement
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

        //public String ElementValue
        //{
        //    set
        //    {
        //        this.Value = value;
        //        OnPropertyChanged("ElementValue");
        //    }
        //    get
        //    {
        //        return this.Value;
        //    }
        //}

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

    public class xmlElement : XElement
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

                //ObservableCollection<xmlElement> returnable = new ObservableCollection<xmlElement>();
                //foreach (XElement item in Elements())
                //{
                //    returnable.Add(new xmlElement(item));
                //}
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

        private List<xmlElement> ElementList()
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
    }

}