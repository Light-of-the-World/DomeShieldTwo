﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrilliantSkies.Core;
using BrilliantSkies.Core.Help;
using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Builders;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters;
using BrilliantSkies.Ui.Consoles.Interpretters.Simple;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Layouts.DropDowns;
using BrilliantSkies.Ui.Tips;
using UnityEngine;
using AdvShields.Models;
using BrilliantSkies.DataManagement.CopyPasting;

namespace AdvShields.UI
{ 
    public class AdvShieldTab : SuperScreen<AdvShieldProjector>
    {
        private DropDownMenuAlt<enumShieldDomeState> _shieldOnOffDropDown;
        private DropDownMenuAlt<enumPassiveRegenState> _shieldPRDropDown;
        private DropDownMenuAlt<enumShieldClassSelection> _shieldClassSelection;

        public AdvShieldTab(ConsoleWindow window, AdvShieldProjector focus) : base(window, focus)
        {
            Name = new Content("ShieldSettings", new ToolTip("Adjust the core values of the shield", 200f), "shieldz");
            _shieldOnOffDropDown = new DropDownMenuAlt<enumShieldDomeState>(TextAnchor.MiddleCenter);
            _shieldPRDropDown = new DropDownMenuAlt<enumPassiveRegenState>(TextAnchor.MiddleCenter);
            _shieldClassSelection = new DropDownMenuAlt<enumShieldClassSelection>(TextAnchor.MiddleCenter);

            _shieldOnOffDropDown.SetItems(new DropDownMenuAltItem<enumShieldDomeState>[2]
            {
                new DropDownMenuAltItem<enumShieldDomeState>()
                {
                    ObjectForAction = enumShieldDomeState.Off,
                    Name = "Shield type: <color=red>off</color>",
                    ToolTip = "Disable the shield"
                },
                new DropDownMenuAltItem<enumShieldDomeState>()
                {
                    ObjectForAction = enumShieldDomeState.On,
                    Name = "Shield type: <color=cyan>on</color>",
                    ToolTip = "Turns the shield on"
                },
            });
            _shieldPRDropDown.SetItems(new DropDownMenuAltItem<enumPassiveRegenState>[2]
            {
                new DropDownMenuAltItem<enumPassiveRegenState>()
                {
                    ObjectForAction = enumPassiveRegenState.Off,
                    Name = "Passive regen: <color=red>minimum</color>",
                    ToolTip = "Sets passive regen to the absolute minimum (50), removing engine draw and providing a tiny bonus to other stats."
                },
                new DropDownMenuAltItem<enumPassiveRegenState>()
                {
                    ObjectForAction = enumPassiveRegenState.On,
                    Name = "Passive regen: <color=cyan>on</color>",
                    ToolTip = "Turns passive regen ON, incurring the engine draw required to repair the shield while it is active."
                },
            });
            _shieldClassSelection.SetItems(new DropDownMenuAltItem<enumShieldClassSelection>[5]
            {
                new DropDownMenuAltItem<enumShieldClassSelection>()
                {
                    ObjectForAction = enumShieldClassSelection.QH,
                    Name = "Shield Class: <color=yellow>QuickHeal</color>",
                    ToolTip = "Fastest active regen, all other stats slightly lower."
                },
                new DropDownMenuAltItem<enumShieldClassSelection>()
                {
                    ObjectForAction = enumShieldClassSelection.AC,
                    Name = "Shield Class: <color=red>Armored</color>",
                    ToolTip = "Highest armor class, slowest regen."
                },
                new DropDownMenuAltItem<enumShieldClassSelection>()
                {
                    ObjectForAction = enumShieldClassSelection.HE,
                    Name = "Shield Class: <color=purple>Healthy</color>",
                    ToolTip = "1.5x health, all other stats slightly lower."
                },
                new DropDownMenuAltItem<enumShieldClassSelection>()
                {
                    ObjectForAction = enumShieldClassSelection.GEN,
                    Name = "Shield Class: <color=green>Generalist</color>",
                    ToolTip = "Small buffs to all stats; but not 'best in class' for anything."
                },
                new DropDownMenuAltItem<enumShieldClassSelection>()
                {
                    ObjectForAction = enumShieldClassSelection.REG,
                    Name = "Shield Class: <color=cyan>Regenerator</color>",
                    ToolTip = "Fastest passive regen and quick active regen; lowest armor class"
                },
            });
        }

        public override void Build()
        {
            ScreenSegmentStandard standardSegment1 = CreateStandardSegment(InsertPosition.OnCursor);
            ScreenSegmentStandard standardSegment2 = CreateStandardSegment(InsertPosition.OnCursor);
            StringDisplay stringDisplay1 = standardSegment1.AddInterpretter(StringDisplay.Quick("<i>Select if the shield is on or off:</i>"));
            standardSegment1.AddInterpretter(new DropDown<AdvShieldData, enumShieldDomeState>(_focus.ShieldData, _shieldOnOffDropDown, (I, b) => I.IsShieldOn == b, (I, b) => I.IsShieldOn.Us = b));
            standardSegment1.AddInterpretter(new Blank(30f));
            StringDisplay stringDisplay2 = standardSegment1.AddInterpretter(StringDisplay.Quick("<i>Select length, width and height of the shield:</i>"));
            standardSegment1.AddInterpretter(Quick.SliderNub(_focus.ShieldData, t => "Length", null));
            standardSegment1.AddInterpretter(Quick.SliderNub(_focus.ShieldData, t => "Width", null));
            standardSegment1.AddInterpretter(Quick.SliderNub(_focus.ShieldData, (t => "Height"), null));
            standardSegment1.AddInterpretter(new Blank(30f));
            StringDisplay stringDisplay3 = standardSegment1.AddInterpretter(StringDisplay.Quick("<i>Select the offset of the shield:</i>"));
            standardSegment1.AddInterpretter(Quick.SliderNub(_focus.ShieldData, t => "LocalPosX", null));
            standardSegment1.AddInterpretter(Quick.SliderNub(_focus.ShieldData, t => "LocalPosY", null));
            standardSegment1.AddInterpretter(Quick.SliderNub(_focus.ShieldData, t => "LocalPosZ", null));
            standardSegment1.AddInterpretter(new Blank(30f));
            StringDisplay stringDisplay4 = standardSegment1.AddInterpretter(StringDisplay.Quick("<i>Select various shield related stats:</i>"));
            standardSegment1.AddInterpretter(new DropDown<AdvShieldData, enumPassiveRegenState>(_focus.ShieldData, _shieldPRDropDown, (I, b) => I.IsPassiveOn == b, (I, b) => I.IsPassiveOn.Us = b));
            standardSegment1.AddInterpretter(new DropDown<AdvShieldData, enumShieldClassSelection>(_focus.ShieldData, _shieldClassSelection, (I, b) => I.ShieldClass == b, (I, b) => I.ShieldClass.Us = b));
            standardSegment1.AddInterpretter(Quick.SliderNub(_focus.ShieldData, t => "ShieldReactivationPercent", null));
            standardSegment1.AddInterpretter(Quick.SliderNub(_focus.ShieldData, t => "ExcessDrive", null));
            standardSegment1.SpaceBelow = 40f;
            standardSegment1.SpaceAbove = 40f;
            stringDisplay1.Justify = new TextAnchor?(TextAnchor.UpperLeft);
            stringDisplay2.Justify = new TextAnchor?(TextAnchor.UpperLeft);
            stringDisplay3.Justify = new TextAnchor?(TextAnchor.UpperLeft);
            stringDisplay4.Justify = new TextAnchor?(TextAnchor.UpperLeft);

            CreateSpace(0);
            standardSegment2.AddInterpretter(SubjectiveDisplay<AdvShieldProjector>.Quick(_focus, M.m<AdvShieldProjector>(I => string.Format("External drive factor is {0} so combined strength is {1}", I.ShieldData.ExternalDriveFactor, Rounding.R2(I.GetExcessDriveAfterFactoring()))))).SetConditionalDisplayFunction(() => _focus.ShieldData.ExternalDriveFactor < 1.0);
            standardSegment2.AddInterpretter(SubjectiveDisplay<AdvShieldProjector>.Quick(_focus, M.m((Func<AdvShieldProjector, string>)(I =>
            {
                if (_focus.GetExcessDriveAfterFactoring() < 1.0)
                    return "Drive less than 1 so the shield is deactivated";

                float powerUse = Rounding.R2(_focus.PowerUse.PowerUsed);
                float num1 = Rounding.R2(_focus.PowerUse.FractionOfPowerRequestedThatWasProvided * 100f);
                float num2 = Rounding.R2(ShieldProjector.GetDisruptionRegenerationRate(powerUse));
                return string.Format("Power use: {0} (working at {1}%). Disruption strength recovery at full power: {2}/s", powerUse, num1, num2);
            }))));
            /*
            standardSegment2.AddInterpretter(SubjectiveDisplay<AdvShieldProjector>.Quick(_focus, M.m((Func<AdvShieldProjector, string>)(I =>
            {
                enumPassiveRegenState type = I.ShieldData.IsPassiveOn;
                string str = "Passive Regen OFF";
                switch (type)
                {
                    case enumPassiveRegenState.Off:
                        str = "Passive Regen OFF";
                        break;
                    case enumPassiveRegenState.On:
                        str = "Passive Regen ON";
                        break;
                }
                return str;
            }))));
            */
            CreateSpace(0);
            ScreenSegmentStandardHorizontal horizontalSegment2 = CreateStandardHorizontalSegment();
            horizontalSegment2.SpaceBelow = 30f;
            horizontalSegment2.AddInterpretter(SubjectiveButton<AdvShieldProjector>.Quick(_focus, "Copy to clipboard", new ToolTip("Copy the shield settings to the clipboard", 200f), I => CopyPaster.Copy(I.ShieldData)));
            horizontalSegment2.AddInterpretter(SubjectiveButton<AdvShieldProjector>.Quick(_focus, "Paste from clipboard", new ToolTip("Paste shield settings from the clipboard", 200f), I => CopyPaster.Paste(I.ShieldData))).FadeOut = M.m((Func<AdvShieldProjector, bool>)(I => !CopyPaster.ReadyToPaste(I.ShieldData)));
        }
    }
}
