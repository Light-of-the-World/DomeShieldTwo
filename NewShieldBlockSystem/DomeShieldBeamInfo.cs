using BrilliantSkies.Core.Help;
using BrilliantSkies.Core.Logger;
using BrilliantSkies.Core.Widgets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace DomeShieldTwo.newshieldblocksystem
{
    public class DomeShieldBeamInfo
    {
        public float MaxEnergy { get; set; }

        public float EnergyAvailablePerSecond { get; set; }
        public float CavityDischargeRate
        {
            
            get
            {
                return 0.1f;
            }
            
            //This will need rewriten to function without a regulator, as we won't be using it. In fact, it might even be affected by the shield class...
        }
        public float EnergyFractionPerSec
        {
            
            get
            {
                return 0.1f;
            }
            
            //This will need rewritten, as the fraction of energy used to actively regen the shield will be different.
        }
        //Skipped ShotsPerSec, not relevant here.
        public int PowerPerSec
        {
            get
            {
                float mult = GetPowerMultiplier();
                float num = this.TotalCapacitorSize * (20 * mult);
                num += (Spoofers * (300 * mult));
                int num2 = Rounding.FloatToInt(num);
                return num2;
            }
            //We can fine tune this number
        }
        public int NoModCapPowerPerSec
        {
            get
            {
                return this.TotalCapacitorSize * 20;
            }
        }

        public float CapModDifference
        {
            get
            {
                float mult = GetPowerMultiplier();
                int num = this.TotalCapacitorSize * 20;
                float num2 = this.TotalCapacitorSize * (20 * mult);
                return num2 - num;
            }
        }
        public float HealthPerEnergy
        {
            
            get
            {
                return DomeShieldConstants.HealthPerEnergy;
                //Do we want to edit this later? Perhaps make it slightly more or less efficient based on shield size, or other things?
            }
            
        }
        //Skipped DamagePerSec, not relevant here.

        public float GetPowerMultiplier()
        {
            float multiplier = ((1 * GetOverchargerPowerMultiplier()) * GetRectifierPowerMultiplier());
            if (Rectifiers == 0 && Overchargers == 0) multiplier = 1;
            return multiplier;
        }

        private float GetOverchargerPowerMultiplier()
        {
            float baseIncrease = (float)Overchargers * 1.1f;
            float adjustedIncrease1 = ((baseIncrease * 1.83f) - Overchargers) - 1; //tf is this?
            float adjustedIncrease2 = baseIncrease - (Mathf.Pow(Overchargers, 1.01f) * 0.98f);
            adjustedIncrease2 = (Overchargers == 1) ? 0.1f : adjustedIncrease2;
            float OverchargerPenalty = Mathf.Clamp(((Overchargers + 1) / (Rectifiers + 1)) - 0.1f, 0, 1);
            float finalIncrease = 1f + (1f * (adjustedIncrease2 * OverchargerPenalty));
            return finalIncrease;
        }
        private float GetRectifierPowerMultiplier()
        {
            float baseReduction = (Rectifiers * 0.04f);
            float adjustedReduction = baseReduction - (Mathf.Pow(Rectifiers, 1.01f) * 0.005f); //Each new rectifier slowly loses efficiency.
            adjustedReduction = (Rectifiers == 1) ? 0.04f : adjustedReduction; //If only 1 rectifier, we don't want it being affected by scaling
            float RectifierPenalty = Mathf.Clamp(((Rectifiers + 1) / (Overchargers + 1)) - 0.1f, 0.1f, 1f); //Ratio of rectifiers and overcharges, with /0 protection, and a built in 10% penalty that is removed in a favorable ratio.
            float finalReduction = 1f - (1f * (adjustedReduction * RectifierPenalty)); //Actual multiplier returned. Should be between 0-1.
            finalReduction = (finalReduction < 0.2f) ? 0.2f : finalReduction; //TEMPORARY hard cap of 0.2 while we test formulas. This shouldn't be needed by release.
            return finalReduction;
        }
        public float SetParts(DomeShieldFeeler feeler)
        {
            this.Overchargers = feeler.Overchargers;
            this.Rectifiers = feeler.rectifiers;
            this.Hardeners = feeler.hardeners;
            this.TotalCapacitorSize = (int)feeler.TotalCapacitorSize;
            this.TotalBlocks = feeler.ItemsFlownThrough;
            this.Transformers = feeler.transformers;
            this.MaxEnergy = feeler.TotalEnergyInBeam;
            this.TotalEnergyInBeam = (int)MaxEnergy;
            this.Spoofers = feeler.Spoofers;
            float num = this.MaxEnergy;
            return num;
            
            //Yeaaahhh... this one's gonna take some work. FrequencyDoublers and Destabalisers won't be used, but we can easily swap those out for int 'other thing'.
            //Let's see if this plays nicely.
        }

        public void Reset()
        {
            this.Hardeners = 0;
            this.TotalCapacitorSize = 0;
            this.TotalEnergyInBeam = 0;
            this.Transformers = 0;
            this.Rectifiers = 0;
            this.MaxEnergy = 0f;
            this.EnergyAvailablePerSecond = 0f;
            this.TotalBlocks = 0;
            this.Spoofers = 0;
            this.Overchargers = 0;
            //We can add the new modifer blocks (what is replacing FD's and DE's) here.
        }
        //Skipped SetQSwitch, not relavent here. Though, should shield class be referenced here...?

        //Skipped GetReadyToFire, it might not be relevant.
        //Skipped IsPulsed and IsCW. This will always be "CW".

        public DomeShieldBeamInfo()
        {
        }
        //Is adding this important...?
        public int Hardeners;

        public int TotalCapacitorSize;

        public int TotalEnergyInBeam;

        public int Transformers;

        public int Rectifiers;

        public int Overchargers;

        public int TotalBlocks;

        public int Spoofers;

        //public LaserOutputRegulator Regulator;
        //Will we need a regulator?

        [CompilerGenerated]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float k__BackingFieldE;

	    [CompilerGenerated]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float k__BackingFieldME;

	    [CompilerGenerated]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private float k__BackingFieldEPS;

	    public bool HadInitialCharge = false;

        private float _fireTime = -1f;

        private readonly GameTime _lastFireTime = new GameTime();

        public int FeelerVersion;

        private readonly GameTime _lastFireTimeTemp = new GameTime();
    }
}
