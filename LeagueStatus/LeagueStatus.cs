using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

// move into diff file
// https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/loot.json

namespace LeagueStatus
{
    public partial class LeagueStatusForm : Form
    {

        private LeagueConnection lc;
        private Timer timer = new Timer();
        private bool isInUse = false;

        public LeagueStatusForm()
        {
            InitializeComponent();
        }

        private void LeagueStatusForm_Load(object sender, EventArgs e)
        {
            lc = new LeagueConnection();
            
            timer.Elapsed += OnTimedEvent;
            timer.Interval = 3000;
            timer.Enabled = false;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (lc.IsConnected)
            {
                label2.Text = "connected";
                JsonObject me = await getMe();
                me["statusMessage"] = textBox1.Text;
                //String statusMessage = getStatusMessage(me);
                lc.Put("/lol-chat/v1/me", me.ToString());
            }
            else
            {
                label2.Text = "not connected";
            }
        }
        
        private async Task<dynamic> getMe() { return (JsonObject) await lc.Get("/lol-chat/v1/me"); }
        
        private async void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (lc.IsConnected && !isInUse)
            {
                isInUse = true;
                JsonObject me = await getMe();
                String currentStatus = (String) me["statusMessage"];
                String result;
                if (Char.IsHighSurrogate(currentStatus[0]))
                {
                    result = string.Join("", currentStatus.Skip(2)) + currentStatus[0] + currentStatus[1];
                }
                else
                {
                    result = string.Join("", currentStatus.Skip(1)) + currentStatus[0];
                }

                me["statusMessage"] = result;
                lc.Put("/lol-chat/v1/me", me.ToString());
                isInUse = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer.Interval = (int)numericUpDown1.Value; 
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/MaciejGorczyca/LeagueStatus");
        }
    }
}