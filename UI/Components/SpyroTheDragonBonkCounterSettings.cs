using LiveSplit.UI;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.SpyroTheDragonBonkCounter.UI.Components
{
    public partial class SpyroTheDragonBonkCounterSettings : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ulong _allTimeBonkCount;

        public ulong AllTimeBonkCount 
        { 
            get => _allTimeBonkCount;
            set
            {
                _allTimeBonkCount = value;
                NotifyPropertyChanged();
            }
        }
        public bool ShowAllTimeBonkCount { get; set; }
        public bool ShowRunBonkCount { get; set; }
        public bool LabelAllTimeCounter { get; set; }
        public bool LabelRunCounter { get; set; }
        public bool CountWhenTimerStopped { get; set; }
        public LayoutMode Mode { get; set; }

        public SpyroTheDragonBonkCounterSettings()
        {
            InitializeComponent();
            ShowAllTimeBonkCount = false;
            ShowRunBonkCount = false;
            AllTimeBonkCount = 0; 
        }

        public void DisableBonkCountEditing()
        {
            checkEditAllTimeBonkCount.Checked = false;
            allTimeBonkCountTextBox.Enabled = false;
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;

            AllTimeBonkCount = Convert.ToUInt64(SettingsHelper.ParseString(element["AllTimeBonkCount"]));
            ShowAllTimeBonkCount = SettingsHelper.ParseBool(element["ShowAllTimeBonkCount"]);
            ShowRunBonkCount = SettingsHelper.ParseBool(element["ShowRunBonkCount"]);
            LabelAllTimeCounter = SettingsHelper.ParseBool(element["LabelAllTimeCounter"]);
            LabelRunCounter = SettingsHelper.ParseBool(element["LabelRunCounter"]);
            CountWhenTimerStopped = SettingsHelper.ParseBool(element["CountWhenTimerStopped"]);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            XmlElement settingsNode = document.CreateElement("Settings");

            SettingsHelper.CreateSetting(document, settingsNode, "AllTimeBonkCount", AllTimeBonkCount.ToString());
            SettingsHelper.CreateSetting(document, settingsNode, "ShowAllTimeBonkCount", ShowAllTimeBonkCount);
            SettingsHelper.CreateSetting(document, settingsNode, "ShowRunBonkCount", ShowRunBonkCount);
            SettingsHelper.CreateSetting(document, settingsNode, "LabelAllTimeCounter", LabelAllTimeCounter);
            SettingsHelper.CreateSetting(document, settingsNode, "LabelRunCounter", LabelRunCounter);
            SettingsHelper.CreateSetting(document, settingsNode, "CountWhenTimerStopped", CountWhenTimerStopped);

            return settingsNode;
        }

        private void SpyroTheDragonBonkCounterSettings_Load(object sender, EventArgs e)
        {
            checkAllTimeBonkCount.DataBindings.Clear();
            checkRunBonkCount.DataBindings.Clear();
            checkEditAllTimeBonkCount.DataBindings.Clear();
            allTimeBonkCountTextBox.DataBindings.Clear();
            checkLabelAllTime.DataBindings.Clear();
            checkLabelRun.DataBindings.Clear();
            checkCountWhenTimerStopped.DataBindings.Clear();

            checkAllTimeBonkCount.DataBindings.Add("Checked", this, "ShowAllTimeBonkCount", false, DataSourceUpdateMode.OnPropertyChanged);
            checkRunBonkCount.DataBindings.Add("Checked", this, "ShowRunBonkCount", false, DataSourceUpdateMode.OnPropertyChanged);
            checkEditAllTimeBonkCount.DataBindings.Add("Checked", allTimeBonkCountTextBox, "Enabled", false, DataSourceUpdateMode.OnPropertyChanged);
            checkLabelAllTime.DataBindings.Add("Checked", this, "LabelAllTimeCounter", false, DataSourceUpdateMode.OnPropertyChanged);
            checkLabelRun.DataBindings.Add("Checked", this, "LabelRunCounter", false, DataSourceUpdateMode.OnPropertyChanged);
            checkCountWhenTimerStopped.DataBindings.Add("Checked", this, "CountWhenTimerStopped", false, DataSourceUpdateMode.OnPropertyChanged);

            allTimeBonkCountTextBox.DataBindings.Add("Text", this, "AllTimeBonkCount", false, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
