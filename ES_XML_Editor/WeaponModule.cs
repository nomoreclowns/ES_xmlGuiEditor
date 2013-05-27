using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ES_XML_Editor
{

    //NOTE TO SELF: do yourself a favor and  BIND DIRECTLY TO XML!!!!!
    //

    public class zzWeaponModule : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private String pName;

        public String name
        {
            set
            {
                if (value != pName)
                {
                    pName= value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("name");
            }
            get
            {
                return pName;
            }
        }

        private String pCost;

        public String cost
        {
            set
            {
                if (value != pCost)
                {
                    pCost = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("cost");
            }
            get
            {
                return pCost;
            }
        }

        private String pWeight;

        public String weight
        {
            set
            {
                if (value != pWeight)
                {
                    pWeight = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("weight");
            }
            get
            {
                return pWeight;
            }
        }

        private String pMilitaryPower;

        public String militaryPower
        {
            set
            {
                if (value != pMilitaryPower)
                {
                    pMilitaryPower = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("militaryPower");
            }
            get
            {
                return pMilitaryPower;
            }
        }

        private Simulation pSimulationInstance;

        #region pSimulationInstance Setters and Getters

        public String damageMin
        {
            set
            {
                if (value != pSimulationInstance.pDamageMin)
                {
                    pSimulationInstance.pDamageMin = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("damageMin");
            }
            get
            {
                return pSimulationInstance.pDamageMin;
            }
        }

        public String damageMax
        {
            set
            {
                if (value != pSimulationInstance.pDamageMax)
                {
                    pSimulationInstance.pDamageMax = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("damageMax");
            }
            get
            {
                return pSimulationInstance.pDamageMax;
            }
        }

        public String criticMultiplier
        {
            set
            {
                if (value != pSimulationInstance.pCriticMultiplier)
                {
                    pSimulationInstance.pCriticMultiplier = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("criticMultiplier");
            }
            get
            {
                return pSimulationInstance.pCriticMultiplier;
            }
        }

        public String criticChance
        {
            set
            {
                if (value != pSimulationInstance.pCriticChance)
                {
                    pSimulationInstance.pCriticChance = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("criticChance");
            }
            get
            {
                return pSimulationInstance.pCriticChance;
            }
        }

        public String interceptionEvasion
        {
            set
            {
                if (value != pSimulationInstance.pInterceptionEvasion)
                {
                    pSimulationInstance.pInterceptionEvasion = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("interceptionEvasion");
            }
            get
            {
                return pSimulationInstance.pInterceptionEvasion;
            }
        }

        public String numberPerSalve
        {
            set
            {
                if (value != pSimulationInstance.pNumberPerSalve)
                {
                    pSimulationInstance.pNumberPerSalve = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("numberPerSalve");
            }
            get
            {
                return pSimulationInstance.pNumberPerSalve;
            }
        }

        public String accuracy
        {
            set
            {
                if (value != pSimulationInstance.pAccuracy)
                {
                    pSimulationInstance.pAccuracy = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("accuracy");
            }
            get
            {
                return pSimulationInstance.pAccuracy;
            }
        }

        public String turnBeforeReach
        {
            set
            {
                if (value != pSimulationInstance.pTurnBeforeReach)
                {
                    pSimulationInstance.pTurnBeforeReach = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("turnBeforeReach");
            }
            get
            {
                return pSimulationInstance.pTurnBeforeReach;
            }
        }

        public String turnToReload
        {
            set
            {
                if (value != pSimulationInstance.pTurnToReload)
                {
                    pSimulationInstance.pTurnToReload = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("turnToReload");
            }
            get
            {
                return pSimulationInstance.pTurnToReload;
            }
        }

        public String weaponClass
        {
            set
            {
                if (value != pSimulationInstance.pWeaponClass)
                {
                    pSimulationInstance.pWeaponClass = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("weaponClass");
            }
            get
            {
                return pSimulationInstance.pWeaponClass;
            }
        }

        #endregion

        private Reflection pReflectionInstance;

        #region pReflectionInstance Setters and Getters

        public String speed
        {
            set
            {
                if (value != pReflectionInstance.pSpeed)
                {
                    pReflectionInstance.pSpeed = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("speed");
            }
            get
            {
                return pReflectionInstance.pSpeed;
            }
        }

        public String priority
        {
            set
            {
                if (value != pReflectionInstance.pPriority)
                {
                    pReflectionInstance.pPriority = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("priority");
            }
            get
            {
                return pReflectionInstance.pPriority;
            }
        }

        public String boardSideMaxDuration
        {
            set
            {
                if (value != pReflectionInstance.pBoardSideMaxDuration)
                {
                    pReflectionInstance.pBoardSideMaxDuration = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("boardSideMaxDuration");
            }
            get
            {
                return pReflectionInstance.pBoardSideMaxDuration;
            }
        }

        public String boardSideFireDelay
        {
            set
            {
                if (value != pReflectionInstance.pBoardSideFireDelay)
                {
                    pReflectionInstance.pBoardSideFireDelay = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("boardSideFireDelay");
            }
            get
            {
                return pReflectionInstance.pBoardSideFireDelay;
            }
        }

        public String projectilesPrefabs
        {
            set
            {
                if (value != pReflectionInstance.pProjectilesPrefabs.pPath)
                {
                    pReflectionInstance.pProjectilesPrefabs.pPath = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("projectilesPrefabs");
            }
            get
            {
                return pReflectionInstance.pProjectilesPrefabs.pPath;
            }
        }

        #endregion

        private Gui pGuiInstance;

        #region pGuiInstance Setters and Getters

        public String title
        {
            set
            {
                if (value != pGuiInstance.pTitle)
                {
                    pGuiInstance.pTitle = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("title");
            }
            get
            {
                return pGuiInstance.pTitle;
            }
        }

        public String description
        {
            set
            {
                if (value != pGuiInstance.pDescription)
                {
                    pGuiInstance.pDescription = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("description");
            }
            get
            {
                return pGuiInstance.pDescription;
            }
        }

        public String icon
        {
            set
            {
                if (value != pGuiInstance.pIcon)
                {
                    pGuiInstance.pIcon = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("icon");
            }
            get
            {
                return pGuiInstance.pIcon;
            }
        }

        public String charCode
        {
            set
            {
                if (value != pGuiInstance.pCharCode)
                {
                    pGuiInstance.pCharCode = value;
                }

                //call on property changed to update the GUI(hopefully)
                OnPropertyChanged("charCode");
            }
            get
            {
                return pGuiInstance.pCharCode;
            }
        }

        #endregion

        /* ************************************************************************************************************************
         * default constructor
         *************************************************************************************************************************/
        public zzWeaponModule()
        {
            pName = "Unknown";
            pCost = "Unknown";
            pWeight = "Unknown";
            pMilitaryPower = "Unknown";

            pSimulationInstance = new Simulation();

            pReflectionInstance = new Reflection();

            pGuiInstance = new Gui();
        }

        /* ************************************************************************************************************************
         * overloaded constructor
         *************************************************************************************************************************/
        public zzWeaponModule(String suppliedName, String suppliedCost, String suppliedWeight, String suppliedMP)
        {
            pName = suppliedName;
            pCost = suppliedCost;
            pWeight = suppliedWeight;
            pMilitaryPower = suppliedMP;

            pSimulationInstance = new Simulation();

            pReflectionInstance = new Reflection();

            pGuiInstance = new Gui();
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler changedHandler = PropertyChanged;

            if (changedHandler != null)
            {
                changedHandler(this, new PropertyChangedEventArgs(name));
            }
        }

    }

    /* ***********************************************************************************
     * these classes are declared public because they are used only as an objects in the weapon
     * module class, where they are private members, thus negating the need for the 
     * "private" access modifier on thse classes' members.
     * **********************************************************************************/
    public class Simulation
    {
        public String pDamageMin;

        public String pDamageMax;

        public String pCriticMultiplier;

        public String pCriticChance;

        public String pInterceptionEvasion;

        public String pNumberPerSalve;

        public String pAccuracy;

        public String pTurnBeforeReach;

        public String pTurnToReload;

        public String pWeaponClass;

        public Simulation()
        {
            pDamageMin = "Unknown";
            pDamageMax = "Unknown";
            pCriticMultiplier = "Unknown";
            pCriticChance = "Unknown";
            pInterceptionEvasion = "Unknown";
            pNumberPerSalve = "Unknown";
            pAccuracy = "Unknown";
            pTurnBeforeReach = "Unknown";
            pTurnToReload = "Unknown";
            pWeaponClass = "Unknown";
        }
    }

    public class Reflection
    {
        public String pSpeed;

        public String pPriority;

        public String pBoardSideMaxDuration;

        public String pBoardSideFireDelay;

        public ProjectilesPrefabs pProjectilesPrefabs;

        public Reflection()
        {
            pSpeed = "Unknown";
            pPriority = "Unknown";
            pBoardSideMaxDuration = "Unknown";
            pBoardSideFireDelay = "Unknown";
            pProjectilesPrefabs = new ProjectilesPrefabs();
        }

    }

    public class ProjectilesPrefabs
    {
        public String pPath;

        public ProjectilesPrefabs()
        {
            pPath= "Unknown";
        }
    }

    public class Gui
    {
        public String pTitle;

        public String pDescription;

        public String pIcon;

        public String pCharCode;

        public Gui()
        {
            pTitle = "Unknown";
            pDescription = "Unknown";
            pIcon = "Unknown";
            pCharCode = "Unknown";
        }
    }

}
