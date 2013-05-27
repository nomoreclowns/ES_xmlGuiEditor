using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ES_XML_Editor
{
    interface ControllerInterface
    {
        void openFile(String fileName);

        void showErrorMessage(String messageOfDoom);

        void closeProgram();
    }
}
