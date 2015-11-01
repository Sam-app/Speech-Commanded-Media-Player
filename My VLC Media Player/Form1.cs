using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;

namespace My_VLC_Media_Player
{
    public partial class Form1 : Form
    {
        SpeechRecognitionEngine spEngine;
        Choices commands;
       



        public Form1()
        {
            InitializeComponent();

             spEngine = new SpeechRecognitionEngine();
            commands = new Choices();
            

        }

      

        private void btnLoadFiles_Click(object sender, EventArgs e)
        {

            openFile();

        }

        private void btnPlayAll_Click(object sender, EventArgs e)
        {
            playAll();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            string selectedFile = listView1.FocusedItem.SubItems[1].Text;
            axWindowsMediaPlayer1.URL = @selectedFile;
            this.mediaStatus.Text = axWindowsMediaPlayer1.currentMedia.name + " is Playing ...";
        }

        private  void  btnSpeechEnable_Click(object sender, EventArgs e)
        {
            commands.Add(new string[] { "open file","play next","play previous","play all" });
            GrammarBuilder grmrBuilder = new GrammarBuilder(commands);
            grmrBuilder.Append(grmrBuilder);
            Grammar grammer = new Grammar(grmrBuilder);

            try
            {
                spEngine.RequestRecognizerUpdate();
                spEngine.LoadGrammarAsync(grammer);
                spEngine.SetInputToDefaultAudioDevice();
                spEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognisedEvn);
                // Wait LoadGrammer to be completed
                //then start Speech Recognising
                spEngine.LoadGrammarCompleted += new  EventHandler<LoadGrammarCompletedEventArgs>((s, evt) =>
                {
                   
                    spEngine.RecognizeAsync(RecognizeMode.Multiple);

                    ////disable the speech command btn
                    //this.btnSpeechEnable.Enabled = false;
                });

               
            }
            catch (Exception ex)
            {

               MessageBox.Show(ex.Message.ToString());
            }

        }

        #region My handlers

        private void playAll()
        {

            WMPLib.IWMPPlaylist playList = axWindowsMediaPlayer1.playlistCollection.newPlaylist("myPlayList");
            WMPLib.IWMPMedia media;


            try
            {

                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    int ii = 1;

                    // creat media instance from the current file location
                    media = axWindowsMediaPlayer1.newMedia(listView1.Items[i].SubItems[ii].Text);

                    // add the media to playlist
                    playList.appendItem(media);

                    axWindowsMediaPlayer1.currentPlaylist = playList;
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                    axWindowsMediaPlayer1.PlayStateChange += (s ,e)=>
                    {
                        if (axWindowsMediaPlayer1.playState==WMPLib.WMPPlayState.wmppsPlaying)
                        {
                            this.mediaStatus.Text = axWindowsMediaPlayer1.currentMedia.name + " is Playing ...";
                        }
                        
                    };

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Can't Play the Media", "Error");
            }

        }

        private void openFile()
        {
            // open dialoge and select file 
            Stream myStream = null;
            OpenFileDialog opf = new OpenFileDialog();
            //opf.InitialDirectory = @"C:\";
            opf.Filter = "MP3 Audio File (*.mp3)|*.mp3| Windows Media File (*.wma)|*.wma|WAV Audio File  (*.wav)|*.wav|All FILES (*.*)|*.*";

            opf.FilterIndex = 1;
            opf.RestoreDirectory = true;
            opf.Multiselect = true;

            if (opf.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = opf.OpenFile()) != null)
                    {

                       

                        using (myStream)
                        {
                            // clear previous loaded files
                            this.listView1.Items.Clear();

                            string[] fileAndPathNames = opf.FileNames;
                            string[] fileNames = opf.SafeFileNames;
                            for (int i = 0; i < fileNames.Count(); i++)
                            {

                                string[] listViewItems = new string[2];
                                listViewItems[0] = fileNames[i];
                                listViewItems[1] = fileAndPathNames[i];

                                ListViewItem lVItem = new ListViewItem(listViewItems);
                                this.listView1.Items.Add(lVItem);

                                


                            }

                        }

                    }
                }
                catch (Exception)
                {

                    MessageBox.Show("Sorry Can't Open the file", "Error ");
                }
            }
        }

        // Speech Recognised Event
          void SpeechRecognisedEvn(object sender, SpeechRecognizedEventArgs e)
        {
            string speechText = e.Result.Text.ToLower();

            if (e.Result.Text.ToLower().Contains("open") && speechText.Contains("file"))
            {
                openFile();
            }
            else if((speechText.Contains("play")&& speechText.Contains("next"))||speechText.Contains("next"))
                { 
                    playNext();
                 }
            else if ((speechText.Contains("play") && speechText.Contains("previous")) || speechText.Contains("previous"))
            {
                playPrevious();
            }
            else if ((speechText.Contains("play") && speechText.Contains("all")))
            {
                playAll();
            }
            else
            {
                MessageBox.Show(speechText.ToString() + " is not Speech Command");
            }
            


           
        }


        private void playNext()
        {
            axWindowsMediaPlayer1.Ctlcontrols.next();
        }

        private void playPrevious()
        {
            axWindowsMediaPlayer1.Ctlcontrols.previous();
        }


        #endregion

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This App is for College Assessment developed by Samuel", "About");
        }

       
    }


    
}
