using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ES_XML_Editor
{
    public class FileHandler
    {
        // Shortcut variable for quicker access to the .settings file
        private static ProgramSettings settings = ProgramSettings.Default;

        private String handledFilePath;

        private Stream handledFileShortName;

        private String ext;

        public FileHandler()
        {
            handledFilePath = null;
        }

        public FileHandler(String tempFileName)
        {
            handledFilePath = tempFileName;
        }

        public String fileName
        {
            set
            {
                handledFilePath = value;
            }
            get
            {
                return handledFilePath;
            }
        }

        public List<Object> open()
        {
            ;
            return null;
        }

        public List<Object> open(String tempFileName)
        {
            handledFilePath = tempFileName;
            return open();
        }

        private List<Object> parseXml()
        {
            List<Object> returningList = null;

            try
            {
                //load xml file as a single element representing the root node
                XElement xmlRootNode = XDocument.Load(handledFilePath).Element(settings.xmlRootElementName);

                // An object to enumerate over the XML nodes
                IEnumerable<XElement> xmlElements = (from c in xmlRootNode.Elements() select c);

                switch (handledFilePath)
                {
                    case "WeaponModule.xml":
                        //zztoWeaponModule(out returningList, xmlElements);
                        //return returningList;

                        break;


                }
                return returningList;

            }
            catch
            {
                return null;
            }
            //return null;
        }

        private void toWeaponModule(List<Object> workingList, IEnumerable<XElement> nodes)
        {
            ;
        }


        //obsolete
        private void zztoWeaponModule(out List<Object> workingList, IEnumerable<XElement> nodes)
        {
            workingList = null;

            zzWeaponModule someModule;

            foreach (XElement elementNode in nodes)
            {
                someModule = new zzWeaponModule();
                someModule.name = elementNode.Attribute("Name").Value;
                someModule.cost = elementNode.Attribute("Cost").Value;
                someModule.weight = elementNode.Attribute("Weight").Value;
                someModule.militaryPower = elementNode.Attribute("MilitaryPower").Value;

                XElement childElement = elementNode.Element("Simulation");

                someModule.damageMin = childElement.Attribute("DamageMin").Value;
                someModule.damageMax = childElement.Attribute("DamageMax").Value;
                someModule.criticMultiplier = childElement.Attribute("CriticMultiplier").Value;
                someModule.criticChance = childElement.Attribute("CriticChance").Value;
                someModule.interceptionEvasion = childElement.Attribute("InterceptionEvasion").Value;
                someModule.numberPerSalve = childElement.Attribute("NumberPerSalve").Value;
                someModule.turnBeforeReach = childElement.Attribute("TurnBeforeReach").Value;
                someModule.turnToReload = childElement.Attribute("TurnToReload").Value;
                someModule.weaponClass = childElement.Element("WeaponClass").Value;

                childElement = elementNode.Element("Reflection");

                someModule.speed = childElement.Attribute("Speed").Value;
                someModule.priority = childElement.Attribute("Priority").Value;
                someModule.boardSideMaxDuration = childElement.Attribute("BoardSideMaxDuration").Value;
                someModule.boardSideFireDelay = childElement.Attribute("BoardSideFireDelay").Value;
                someModule.projectilesPrefabs = childElement.Element("Projectiles-Prefabs").Element("Prefab").Attribute("Path").Value;

                childElement = elementNode.Element("Gui");

                someModule.title = childElement.Element("Title").Value;
                someModule.description = childElement.Element("Description").Value;
                someModule.icon = childElement.Element("Icon").Attribute("Small").Value;
                //UNFINISHED CLASS! MISSING  ICON ATTRIBUTE "LARGE"
                someModule.charCode = childElement.Element("CharCode").Value;

            }
        }
    }
}
