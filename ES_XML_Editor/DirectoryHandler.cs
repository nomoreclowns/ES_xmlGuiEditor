using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;

namespace ES_XML_Editor
{
    public class ioObject// : INotifyPropertyChanged
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

        public enum Filetype
        {
            File,
            Folder
        }

        protected String pName = "";

        protected Filetype pKind;

        public String name
        {
            get
            {
                return pName;
            }
        }

        public String kind
        {
            get
            {
                return pKind.ToString();
            }
        }

        public ioObject.Filetype fileKind
        {
            get
            {
                return pKind;
            }
        }

        public ioObject(String iName, ioObject.Filetype iKind)
        {
            pName = iName;

            pKind = iKind;
        }
    }

    public class DirectoryHandler
    {
        private controllerShowError contDisplayError;
        private String pDirectoryPath;
        private DirectoryInfo parent;

        private List<ioObject> pContents;

        public String path
        {
            get
            {
                return pDirectoryPath;
            }
        }

        public List<ioObject> contents
        {
            get
            {
                return pContents;
            }
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        public DirectoryHandler(String dirPath, controllerShowError DisplayError)
        {
            contDisplayError = DisplayError;

            pContents = new List<ioObject>();

            pDirectoryPath = dirPath;

            retrieveContents();

            try
            {
                parent = Directory.GetParent(pDirectoryPath);
            }
            catch
            {
                contDisplayError("unable to retrieve parent of folder " + dirPath);
            }


        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void retrieveContents()
        {
            if (Directory.Exists(pDirectoryPath) == true)
            {

                int pathLength = pDirectoryPath.Length;

                try
                {
                    String[] directoryArray = Directory.GetDirectories(pDirectoryPath, "*", SearchOption.TopDirectoryOnly);

                    for (int i = 0; i < directoryArray.Length; i++)
                    {
                        pContents.Add(new ioObject(directoryArray[i].Substring(pathLength), ioObject.Filetype.Folder));
                    }
                }
                catch
                {
                    contDisplayError("Could not retrieve directories.");
                }
                try
                {
                    String[] fileArray = Directory.GetFiles(pDirectoryPath, "*", SearchOption.TopDirectoryOnly);

                    for (int i = 0; i < fileArray.Length; i++)
                    {
                        pContents.Add(new ioObject(fileArray[i].Substring(pathLength), ioObject.Filetype.File));
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                return;
            }
            else
            {
                contDisplayError("Cannot show a directory that does not exist");
            }
        }//end of function
    }
}
