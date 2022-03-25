using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using LiveSplit.SpyroTheDragonMusicPlayer.Hook;
using LiveSplit.SpyroTheDragonBonkCounter.UI.Components;
using System.Diagnostics;

namespace LiveSplit.SpyroTheDragonBonkCounter
{
    public class SpyroTheDragonBonkCounterComponent : IComponent
    {
        public string ComponentName => "Spyro the Dragon Bonk Counter";

        public float HorizontalWidth
        { 
            get
            {
                float horizontalWidth = 0;
                if (Settings.ShowAllTimeBonkCount)
                    horizontalWidth += AllTimeBonkCountComponent.HorizontalWidth;
                if (Settings.ShowRunBonkCount)
                    horizontalWidth += RunBonkCountComponent.HorizontalWidth;
                return horizontalWidth;
            }
        }
        public float MinimumHeight
        { 
            get
            {
                float minimumHeight = 0;
                if (Settings.ShowAllTimeBonkCount)
                    minimumHeight += AllTimeBonkCountComponent.MinimumHeight - AllTimeBonkCountComponent.PaddingTop;
                if (Settings.ShowRunBonkCount)
                    minimumHeight += RunBonkCountComponent.MinimumHeight - RunBonkCountComponent.PaddingTop;
                return minimumHeight + PaddingBottom;
            }
        }
        public float VerticalHeight
        { 
            get
            {
                float verticalHeight = 0;
                if (Settings.ShowAllTimeBonkCount)
                    verticalHeight += AllTimeBonkCountComponent.VerticalHeight - AllTimeBonkCountComponent.PaddingTop;
                if (Settings.ShowRunBonkCount)
                    verticalHeight += RunBonkCountComponent.VerticalHeight - RunBonkCountComponent.PaddingTop;
                return verticalHeight + PaddingBottom;
            }
        }
        public float MinimumWidth
        { 
            get
            {
                float minimumWidth = 0;
                if (Settings.ShowAllTimeBonkCount)
                    minimumWidth += AllTimeBonkCountComponent.MinimumWidth;
                if (Settings.ShowRunBonkCount)
                    minimumWidth += RunBonkCountComponent.MinimumWidth;
                return minimumWidth;
            }
        }

        public float PaddingTop
        {
            get
            {
                float paddingTop = 0;
                if (Settings.ShowAllTimeBonkCount)
                    paddingTop = AllTimeBonkCountComponent.PaddingTop;
                else if (Settings.ShowRunBonkCount)
                    paddingTop = RunBonkCountComponent.PaddingTop;
                return paddingTop;
            }
        }
        public float PaddingBottom
        {
            get
            {
                float paddingBottom = 0;
                if (Settings.ShowRunBonkCount)
                    paddingBottom = RunBonkCountComponent.PaddingBottom;
                else if (Settings.ShowAllTimeBonkCount)
                    paddingBottom = AllTimeBonkCountComponent.PaddingBottom;
                return paddingBottom;
            }
        }
        public float PaddingLeft => 0;
        public float PaddingRight => 0;
        
        public IDictionary<string, Action> ContextMenuControls => null;//throw new NotImplementedException();

        protected LiveSplitState State { get; set; }
        protected SpyroTheDragonGame Spyro { get; set; }
        protected SpyroTheDragonBonkCounterSettings Settings { get; set; }

        protected InfoTextComponent AllTimeBonkCountComponent { get; set; }
        protected InfoTextComponent RunBonkCountComponent { get; set; }

        private const string BONK_LABEL_GENERIC = "Bonks";
        private const string BONK_LABEL_ALL_TIME = "Bonks (All Time)";
        private const string BONK_LABEL_RUN = "Bonks (This Run)";

        
        private int runBonkCount;
        private bool wasBonking;

        public SpyroTheDragonBonkCounterComponent(LiveSplitState state)
        {
            int ramAddressPollingInterval = 5000;
            Spyro = new SpyroTheDragonGame(ramAddressPollingInterval);
            State = state;
            Settings = new SpyroTheDragonBonkCounterSettings();

            // These are the default values, but explicitness is good
            runBonkCount = 0;
            wasBonking = false;

            AllTimeBonkCountComponent = new InfoTextComponent(BONK_LABEL_GENERIC, Settings.AllTimeBonkCount.ToString());
            RunBonkCountComponent = new InfoTextComponent(BONK_LABEL_GENERIC, runBonkCount.ToString());

            State.OnReset += Component_OnReset;
        }

        public void Dispose()
        {
            Spyro.Dispose();
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            int totalTranslatedX = 0;
            int translateX;
            
            if (Settings.ShowAllTimeBonkCount)
            {
                DrawInternalComponentHorizontally(g, state, height, clipRegion, AllTimeBonkCountComponent);
                translateX = (int)AllTimeBonkCountComponent.HorizontalWidth - (int)AllTimeBonkCountComponent.PaddingRight;
                totalTranslatedX += translateX;
                g.TranslateTransform(translateX, 0);
            }
                
            if (Settings.ShowRunBonkCount)
            {
                
                DrawInternalComponentHorizontally(g, state, height, clipRegion, RunBonkCountComponent);
                translateX = (int)RunBonkCountComponent.HorizontalWidth - (int)RunBonkCountComponent.PaddingRight;
                totalTranslatedX += translateX;
                g.TranslateTransform(translateX, 0);
            }

            g.TranslateTransform(-totalTranslatedX, 0);
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            int totalTranslatedY = 0;
            int translateY;
            
            if (Settings.ShowAllTimeBonkCount)
            {
                DrawInternalComponentVertically(g, state, width, clipRegion, AllTimeBonkCountComponent);
                
                translateY = (int)AllTimeBonkCountComponent.VerticalHeight - (int)AllTimeBonkCountComponent.PaddingBottom;
                totalTranslatedY += translateY;
                g.TranslateTransform(0, translateY);
            }

            if (Settings.ShowRunBonkCount)
            {
                DrawInternalComponentVertically(g, state, width, clipRegion, RunBonkCountComponent);
                
                translateY = (int)RunBonkCountComponent.VerticalHeight - (int)RunBonkCountComponent.PaddingBottom;
                totalTranslatedY += translateY;
                g.TranslateTransform(0, translateY);
            }

            g.TranslateTransform(0, -totalTranslatedY);
        }

        private void DrawInternalComponentVertically(Graphics g, LiveSplitState state, float width, Region clipRegion, InfoTextComponent internalComponent)
        {
            internalComponent.NameLabel.HasShadow = State.LayoutSettings.DropShadows;
            internalComponent.ValueLabel.HasShadow = State.LayoutSettings.DropShadows;

            internalComponent.NameLabel.ForeColor = State.LayoutSettings.TextColor;
            internalComponent.ValueLabel.ForeColor = State.LayoutSettings.TextColor;

            internalComponent.DrawVertical(g, State, width, clipRegion);
        }

        private void DrawInternalComponentHorizontally(Graphics g, LiveSplitState state, float height, Region clipRegion, InfoTextComponent internalComponent)
        {
            internalComponent.NameLabel.HasShadow = State.LayoutSettings.DropShadows;
            internalComponent.ValueLabel.HasShadow = State.LayoutSettings.DropShadows;

            internalComponent.NameLabel.ForeColor = State.LayoutSettings.TextColor;
            internalComponent.ValueLabel.ForeColor = State.LayoutSettings.TextColor;

            internalComponent.DrawHorizontal(g, State, height, clipRegion);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public Control GetSettingsControl(LayoutMode mode)
        {
            Settings.DisableBonkCountEditing();
            Settings.Mode = mode;
            return Settings;
        }

        public void SetSettings(XmlNode settings)
        {
            Settings.SetSettings(settings);
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            bool isBonking = Spyro.IsBonking;
            if (isBonking && !wasBonking)
            {
                if (state.CurrentPhase == TimerPhase.Running || Settings.CountWhenTimerStopped)
                    Settings.AllTimeBonkCount++;
                if (state.CurrentPhase == TimerPhase.Running)
                    runBonkCount++;
            }

            AllTimeBonkCountComponent.InformationName = Settings.LabelAllTimeCounter ? BONK_LABEL_ALL_TIME : BONK_LABEL_GENERIC;
            RunBonkCountComponent.InformationName = Settings.LabelRunCounter ? BONK_LABEL_RUN : BONK_LABEL_GENERIC;

            AllTimeBonkCountComponent.InformationValue = Settings.AllTimeBonkCount.ToString();
            RunBonkCountComponent.InformationValue = runBonkCount.ToString();

            AllTimeBonkCountComponent.Update(invalidator, state, width, height, mode);
            RunBonkCountComponent.Update(invalidator, state, width, height, mode);

            wasBonking = isBonking;
        }

        private void Component_OnReset(object sender, TimerPhase timerPhase)
        {
            runBonkCount = 0;
        }
    }
}
