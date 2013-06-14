using System.Windows.Data;
using System.Text;
using System;
using System.Collections.Generic;

namespace ES_XML_Editor
{
    public delegate void controllerManipulateSetting(EditorController.EditorSettings delSettingKey, ref String delSettingValue, bool delSaveSetting = false);

    public delegate void controllerOpenFile(bool delBool = false);

    public delegate void controllerOpenFile(String delFileName, ref CollectionView delCollectionView);

    public delegate void controllerOpenFile(int delFileIndex, ref CollectionView delCollectionView);

    public delegate void controllerBind(ref CollectionView delcollectionView);

    public delegate void controllerBind(ref CollectionView delcollectionView, int delFileDataSelector);

    public delegate void controllerShowError(String delMessageOfDoom);

    public delegate void controllerSave();

    public delegate void controllerSave(int delFileDataSelector);

    public delegate void controllerAddItem(xmlElem delData);

    public delegate void controllerAddItem(xmlElem delData, int delFileDataSelector);

    public delegate void controllerEditItem(xmlElem delData, int[] delSelectedIndeces);

    public delegate void controllerEditItem(xmlElem delData, int[] delSelectedIndeces, int delFileDataSelector);

    public delegate void controllerGetSelectedData(int[] delIndices, ref xmlElem delItemContainer);

    public delegate void controllerClose();

    public class controllerDataDelegateContainer
    {
        private controllerGetSelectedData pSelectedDataGetter;
        private controllerBind pBinder;
        private controllerAddItem pItemAdder;
        private controllerEditItem pItemEditor;

        public controllerDataDelegateContainer(controllerBind iBinder, controllerGetSelectedData iSelectedDataGetter, controllerAddItem iItemAdder, controllerEditItem iItemEditor)
        {
            pBinder = iBinder;
            pSelectedDataGetter = iSelectedDataGetter;
            pItemAdder = iItemAdder;
            pItemEditor = iItemEditor;
        }

        public void retrieveDelegates(out controllerBind iBinder, out controllerGetSelectedData iSelectedDataGetter, out controllerAddItem iItemAdder, out controllerEditItem iItemEditor)
        {
            iBinder = pBinder;
            iSelectedDataGetter = pSelectedDataGetter;
            iItemAdder = pItemAdder;
            iItemEditor = pItemEditor;
        }
    }

    public class controllerMiscDelegateContainer
    {
        private controllerSave pFileSaver;
        private controllerOpenFile pFileOpener;
        private controllerManipulateSetting pSettingHandler;

        public controllerMiscDelegateContainer(controllerOpenFile iFileOpener, controllerManipulateSetting iSettingHandler, controllerSave iFileSaver)
        {
            pFileOpener = iFileOpener;
            pFileSaver = iFileSaver;
            pSettingHandler = iSettingHandler;
        }

        public void retrieveDelegates(out controllerOpenFile iFileOpener, out controllerManipulateSetting iSettingHandler, out controllerSave iFileSaver)
        {
            iFileOpener = pFileOpener;
            iFileSaver = pFileSaver;
            iSettingHandler = pSettingHandler;
        }
    }

    public class controllerDelegateContainer
    {
        private controllerShowError pErrorDisplayer;
        private controllerClose pProgramCloser;

        public controllerDelegateContainer(controllerShowError iErrorDisplayer, controllerClose iProgramCloser = null)
        {
            pErrorDisplayer = iErrorDisplayer;
            if (iProgramCloser == null)
            {
                pProgramCloser = new controllerClose(emptyControllerCloseFunc);
                return;
            }
            pProgramCloser = iProgramCloser;
        }

        public void retrieveDelegates(out controllerShowError iErrorDisplayer, out controllerClose iProgramCloser)
        {
            iErrorDisplayer = pErrorDisplayer;
            iProgramCloser = pProgramCloser;
        }

        private void emptyControllerCloseFunc()
        {
        }
    }
}