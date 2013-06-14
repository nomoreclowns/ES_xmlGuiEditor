using System.Windows.Data;
using System.Text;
using System;
using System.Collections.Generic;

namespace ES_XML_Editor
{
    public delegate void controllerManipulateSetting(EditorController.EditorSettings delSettingKey, ref String delSettingValue, bool delSaveSetting = false);

    public delegate void controllerOpenFile(out String delShortFileName, bool delBool = true);

    //public delegate void controllerOpenNewFile(out CollectionView delCollectionView, String delFileName);

    public delegate void controllerOpenNewFile(out CollectionView delCollectionView, int delFileDataSelector);

    public delegate void controllerFolderContents(out CollectionView delCollectionView);

    //public delegate void controllerBind(out CollectionView delCollectionView);

    public delegate void controllerBind(out CollectionView delCollectionView, int delFileDataSelector);

    public delegate void controllerShowError(String delMessageOfDoom);

    //public delegate void controllerSave();

    public delegate void controllerSave(int delFileDataSelector);

    //public delegate void controllerAddItem(xmlElem delData);

    public delegate void controllerAddItem(xmlElem delData, int delFileDataSelector);

    //public delegate void controllerEditItem(xmlElem delData, int[] delSelectedIndeces);

    public delegate void controllerEditItem(xmlElem delData, int[] delSelectedIndeces, int delFileDataSelector);

    //public delegate void controllerGetSelectedData(int[] delIndices, ref xmlElem delItemContainer);

    public delegate void controllerGetSelectedData(int[] delIndices, ref xmlElem delItemContainer, int delFileDataSelector);

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
        private controllerFolderContents pContentsGetter;

        public controllerMiscDelegateContainer(controllerOpenFile iFileOpener, controllerManipulateSetting iSettingHandler, controllerSave iFileSaver, controllerFolderContents iContentsGetter)
        {
            pFileOpener = iFileOpener;
            pFileSaver = iFileSaver;
            pSettingHandler = iSettingHandler;
            pContentsGetter = iContentsGetter;
        }

        public void retrieveDelegates(out controllerOpenFile iFileOpener, out controllerManipulateSetting iSettingHandler, out controllerSave iFileSaver, out controllerFolderContents iContentsGetter)
        {
            iFileOpener = pFileOpener;
            iFileSaver = pFileSaver;
            iSettingHandler = pSettingHandler;
            iContentsGetter = pContentsGetter;
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