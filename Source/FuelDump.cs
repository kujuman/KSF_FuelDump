using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KSP;
using UnityEngine;


namespace kuju_utils
{
    public class KSF_FuelDump : PartModule
    {
        [KSPField(guiName = "Arm Fuel Dump", guiActive = true), UI_Toggle()]
        bool isArmed = false;

        [KSPField(guiName = "Fuel Dump Type", guiActive = false),
            UI_Cycle(stateNames = new string[] { "LF+O", "Monoprop", "Liquid Fuel", "Oxidizer" })]
        public int fuelToDrop = 0;

        [KSPField(guiName = "Dump Percent of Resource", guiActive = false),
            UI_FloatRange(minValue = 0f,
                stepIncrement = 1f,
                maxValue = 100f)]
        public float dumpAmount = 100f;

        [KSPField(guiName = "Dump THIS Part", guiActive = false), UI_Toggle()]
        bool dumpThisPart = false;

        [KSPField(guiName = "Dump VESSEL", guiActive = false), UI_Toggle()]
        bool dumpVessel = false;


        void DumpResourceFromPart(string resource, Part part, float percentage)
        {
            if (!part.Resources.Contains(resource)) return;

            if (!part.Resources[resource].flowState) return;

            part.Resources[resource].amount *= ((100 - percentage) / 100f);
        }

        string[] GetResourcesToDrop()
        {
            string[] returnValue;

            switch (fuelToDrop)
            {
                case 0:
                    returnValue = new string[] { "LiquidFuel", "Oxidizer" };
                    break;

                case 1:
                    returnValue = new string[] { "MonoPropellant" };
                    break;

                case 2:
                    returnValue = new string[] { "LiquidFuel" };
                    break;

                case 3:
                    returnValue = new string[] { "Oxidizer" };
                    break;

                default:
                    returnValue = new string[] { };
                    break;
            }

            return returnValue;
        }

        void DumpThisPart(object obj)
        {
            var dropMe = GetResourcesToDrop();
            for (int i = 0; i < dropMe.Length; i++)
            {
                DumpResourceFromPart(dropMe[i], this.part, dumpAmount);
            }
            dumpThisPart = false;
            ArmFuelDump(null);
        }

        void DumpVessel(object obj)
        {
            var dropMe = GetResourcesToDrop();

            for (int i = 0; i < this.vessel.parts.Count; i++)
            {
                for (int j = 0; j < dropMe.Length; j++)
                {
                    DumpResourceFromPart(dropMe[j], this.vessel.parts[i], dumpAmount);
                }
            }
            dumpVessel = false;
            ArmFuelDump(null);
        }

        public override void OnAwake()
        {
            Fields["isArmed"].OnValueModified += ArmFuelDump;

            Fields["dumpThisPart"].OnValueModified += DumpThisPart;
            Fields["dumpVessel"].OnValueModified += DumpVessel;
        }

        void ArmFuelDump(object obj)
        {
            Fields["fuelToDrop"].guiActive = isArmed;
            Fields["dumpAmount"].guiActive = isArmed;
            Fields["dumpThisPart"].guiActive = isArmed;
            Fields["dumpVessel"].guiActive = isArmed;
        }
    }
}
