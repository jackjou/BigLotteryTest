using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BigLotteryTest
{
    enum Command
    {
        CMD_StartLottery,
        CMD_End
    }

    
    


    public partial class Form1 : Form
    {

        String baseUrl = "http://k-king.com.tw/biglottery/";
        APIConnector connector = new APIConnector("http://k-king.com.tw/biglottery/");



        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadtoComboBox("* from biglottery_machine_category", "name", comboBox1);
            LoadtoComboBox("* from biglottery_machine_category", "name", comboBox2);
            LoadtoComboBox("* from biglottery_machine_category", "name", comboBox7);
            LoadtoComboBox("* from biglottery_member", "account", comboBox3);
            LoadtoComboBox("* from biglottery_member", "account", comboBox6);
            LoadtoComboBox("DISTINCT lottery_no FROM biglottery_lottery ", "lottery_no", comboBox4);
            LoadtoComboBox("DISTINCT lottery_no FROM biglottery_lottery ", "lottery_no", comboBox5);
            LoadtoComboBox("DISTINCT lottery_no FROM biglottery_lottery ", "lottery_no", comboBox8);
            LoadtoComboBox("DISTINCT lottery_no FROM biglottery_lottery ", "lottery_no", comboBox9);
            LoadtoComboBox("DISTINCT lottery_no FROM biglottery_lottery ", "lottery_no", comboBox10);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "等待中...";
            if (comboBox1.Text == "") return;
            String result = connector.GetStringWithUrl(baseUrl ,"api_lottery.php?action=startLottery&group_name="+comboBox1.SelectedItem.ToString());

            richTextBox1.Text = result;

            Form1_Load(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = "等待中...";
            if (comboBox2.Text == "") return;
            String result = connector.GetStringWithUrl(baseUrl, "api_lottery.php?action=getTheLatestLottery&group_name="+comboBox2.SelectedItem.ToString());

            richTextBox2.Text = result;

            JObject jObject = connector.StringConvertJObj(result);

            if( jObject["data"]["state"].Value<string>() == "success")
            {
                
                textBox6.Text = textBox4.Text =  textBox5.Text = textBox2.Text = textBox1.Text = jObject["data"]["lottery_no"].Value<string>();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox3.Text = "等待中...";
            if (textBox1.Text == "") return;
            String result = connector.GetStringWithUrl(baseUrl, "api_lottery.php?action=getDrawLotteryInfo&lottery_no=" + textBox1.Text);

            richTextBox3.Text = result;

            JObject jObject = connector.StringConvertJObj(result);
            if( jObject["data"]["state"].Value<string>() == "success" )
            {
                label24.Text = jObject["data"]["total_prize"].Value<string>();
                label25.Text = jObject["data"]["balance_prize"].Value<string>();
            }
            else
            {
                label24.Text = "0";
                label25.Text = "0";
            }
        }

        private void buttonLotteryNum_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                Button btn = (Button)sender;
                if (btn == button54)
                {
                    btn.BackColor = Color.Blue;

                    foreach (Control ctrl in groupBox1.Controls)
                    {
                        Button button = ctrl as Button;
                        String strSplit = button.Text;
                        if (strSplit == "隨選") continue;
                        int btnIndex = Convert.ToInt32(strSplit);
                        if ( btnIndex >= 1 && btnIndex <= 49 )
                        {
                            //button.Enabled = false;
                            button.BackColor = Color.LightGray;
                        }
                    }
                }
                else
                {
                    Button button = sender as Button;
                    String strSplit = button.Text;
                    int btnIndex = Convert.ToInt32(strSplit);
                    if (btnIndex >= 1 && btnIndex <= 49)
                    {
                        if (btn.BackColor == Color.Blue)
                        {
                            btn.BackColor = Color.LightGray;
                        }
                        else
                        {
                            btn.BackColor = Color.Blue;
                            button54.BackColor = Color.LightGray;
                        }
                    }                    
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool blRandomFlag = false;
            int Count = 0;
            String[] strNo = new string[6] { "0", "0", "0", "0", "0", "0" };
            foreach (Control ctrl in groupBox1.Controls)
            {
                Button button = ctrl as Button;
                String strSplit = button.Text;
                if (strSplit == "隨選" )
                {
                    if (button.BackColor == Color.Blue)
                    {
                        blRandomFlag = true;
                    }
                    continue;
                }
                int btnIndex = Convert.ToInt32(strSplit);

                if (btnIndex >= 1 && btnIndex <= 49 && button.BackColor == Color.Blue )
                {
                    strNo[Count] = btnIndex.ToString();
                    Count++;
                }
            }
            if( Count != 6 && blRandomFlag == false)
            {
                MessageBox.Show("不足六個號碼");
                return;
            }

            String strCmd = "api_lottery.php?action=getMemberLottery";

            strCmd += "&lottery_no=" + textBox2.Text;
            strCmd += "&money=" + textBox3.Text;
            strCmd += "&member_account=" + comboBox3.SelectedItem.ToString();
            if (blRandomFlag == false)
            {
                
                for (int i = 0; i < 6; i++)
                {
                    strCmd += "&no" + (i + 1).ToString() + "=" + strNo[i];
                }
            }

            String result = connector.GetStringWithUrl(baseUrl, strCmd);

            richTextBox4.Text = result;

            JObject jObject = connector.StringConvertJObj(result);

            if (jObject["data"]["state"].Value<string>() == "success")
            {
                String[] strGetNo = new string[6] { "0", "0", "0", "0", "0", "0" };
                String strNumAdd = "";
                for (int i = 0; i < 6; i++)
                {
                    strGetNo[i] = jObject["data"]["no" + (i+1).ToString()].Value<string>();
                    strNumAdd += strGetNo[i];

                    if (i < 5)
                    {
                        strNumAdd += ",";
                    }
                }
                checkedListBox2.Items.Add(strNumAdd, false);
                
            }
        }

        private void button55_Click(object sender, EventArgs e)
        {
            richTextBox5.Text = "等待中...";
            String result = connector.GetStringWithUrl(baseUrl, "api_lottery.php?action=getDrawLottery&lottery_no=" + textBox4.Text);

            richTextBox5.Text = result;
        }

        private void button57_Click(object sender, EventArgs e)
        {
            richTextBox7.Text = "等待中...";
            String result = connector.GetStringWithUrl(baseUrl, "pair_award.php?action=pairAward&lottery_no="+textBox6.Text);

            richTextBox7.Text = result;
        }

        private void button56_Click(object sender, EventArgs e)
        {
            richTextBox6.Text = "等待中...";
            String strCmd = "api_lottery.php?action=getMemberLotteryInfo";
            strCmd += "&lottery_no=" + textBox5.Text;

            if (textBox7.Text != "" )
            {
                String[] strSplit = textBox7.Text.Split(',');
                if (strSplit.Length == 6)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        strCmd += "&no" + (i + 1).ToString() + "=" + strSplit[i];
                    }

                    String result = connector.GetStringWithUrl(baseUrl, strCmd);
                    richTextBox6.Text = result;
                }
            }
        }

        private void checkedListBox1_DoubleClick(object sender, EventArgs e)
        {
            if (checkedListBox1.SelectedItem == null) return; 
            textBox7.Text =  checkedListBox1.SelectedItem.ToString();
            textBox5.Text = comboBox5.SelectedItem.ToString();
        }

        private void button58_Click(object sender, EventArgs e)
        {
            richTextBox6.Text = "等待中...";
            String strCmd = "api_lottery.php?action=pairMemberLottery";
            strCmd += "&lottery_no=" + textBox5.Text;

            if (textBox7.Text != "")
            {
                String[] strSplit = textBox7.Text.Split(',');
                if (strSplit.Length == 6)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        strCmd += "&no" + (i + 1).ToString() + "=" + strSplit[i];
                    }

                    String result = connector.GetStringWithUrl(baseUrl, strCmd);
                    richTextBox6.Text = result;
                }
            }
        }

        private void LoadtoComboBox(String strCmd , String schema , ComboBox comboBox)
        {
            JObject jObject = connector.GetJObjectSelectSQLWithUrl("http://k-king.com.tw/biglottery/", strCmd);

            var groupNames = from p in jObject["data"] select (string)p[schema];
            comboBox.Items.Clear();
            foreach (var item in groupNames)
            {
                comboBox.Items.Add(item);
            }
        }

        


        private bool LoadtoCheckedListBox(String strCmd , String schema, CheckedListBox checkListBox)
        {
            JObject jObject = connector.GetJObjectSelectSQLWithUrl("http://k-king.com.tw/biglottery/", strCmd);
            if( jObject == null )   return false;
            var groupNames = from p in jObject["data"]  
                             select p["no1"].Value<string>()+","+ p["no2"].Value<string>()+ "," + p["no3"].Value<string>()+ "," + p["no4"].Value<string>()+ "," + p["no5"].Value<string>()+ "," + p["no6"].Value<string>() ;
            checkListBox.Items.Clear();
            foreach (var item in groupNames)
            {
                checkListBox.Items.Add(item);
            }
            return true;
        }



        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = comboBox4.SelectedItem.ToString();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button59_Click(object sender, EventArgs e)
        {
            String strCmd = "* FROM biglottery_member_lottery ";
            if (comboBox5.SelectedItem != null  && comboBox6.SelectedItem != null)
            {
                strCmd += " where lottery_no='" + comboBox5.SelectedItem.ToString()+"'";
                strCmd += " and member_account='" + comboBox6.SelectedItem.ToString()+"'";
            }
            LoadtoCheckedListBox(strCmd, "", checkedListBox1);

        }

        private void button60_Click(object sender, EventArgs e)
        {
            int prize = 0;
            int hitCnt = 0;
            int turtleCnt = 0;
            int count = checkedListBox1.Items.Count;

            richTextBox6.Text = "等待中...";
            label15.Text = "0";     // prize金額
            label17.Text = "0";     // 中獎獎項數
            label19.Text = count.ToString();     // 樂透張數
            label21.Text = "0";     // 槓龜數

            for ( int index=0;index<count;index++ )
            {
                //if (checkedListBox1.SelectedItem == null) return;
                // member
                textBox5.Text = comboBox5.SelectedItem.ToString();
                // lottery no 
                textBox7.Text = checkedListBox1.Items[index].ToString();

                // 查詢是否中獎
                String strCmd = "api_lottery.php?action=getMemberLotteryInfo";
                strCmd += "&lottery_no=" + textBox5.Text;

                if (textBox7.Text != "")
                {
                    String[] strSplit = textBox7.Text.Split(',');
                    if (strSplit.Length == 6)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            strCmd += "&no" + (i + 1).ToString() + "=" + strSplit[i];
                        }

                        String result = connector.GetStringWithUrl(baseUrl, strCmd);
                        if (result == null) continue; 
                        richTextBox6.AppendText(result+"\n");
                        JObject jObject = connector.StringConvertJObj(result);

                        if (jObject["data"]["state"].Value<string>() == "success")
                        {
                            if( jObject["data"]["prize"].Value<string>() == "0" )
                            {
                                // 沒中
                                turtleCnt++;
                            }
                            else
                            {
                                //如果有中獎
                                prize += int.Parse(jObject["data"]["prize"].Value<string>());
                                hitCnt++;

                                // 兌獎
                                String strCmd2 = "api_lottery.php?action=pairMemberLottery";
                                strCmd2 += "&lottery_no=" + textBox5.Text;

                                if (textBox7.Text != "")
                                {
                                    String[] strSplit2 = textBox7.Text.Split(',');
                                    if (strSplit2.Length == 6)
                                    {
                                        for (int i = 0; i < 6; i++)
                                        {
                                            strCmd2 += "&no" + (i + 1).ToString() + "=" + strSplit2[i];
                                        }

                                        String result2 = connector.GetStringWithUrl(baseUrl, strCmd2);
                                        richTextBox6.AppendText(result2+"\n");
                                        JObject jObject2 = connector.StringConvertJObj(result);

                                        if (jObject2["data"]["state"].Value<string>() == "success")
                                        {
                                            // 兌獎完成
                                        }
                                        else
                                        {
                                            // 銘謝惠顧或是已經兌過獎

                                        }
                                    }
                                }

                            }
                        }

                        
                    }
                }
                label15.Text = prize.ToString();     // prize金額
                label17.Text = hitCnt.ToString();     // 中獎獎項數
                label19.Text = count.ToString();     // 樂透張數
                label21.Text = turtleCnt.ToString();     // 槓龜數
            }
        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
            Form1_Load(sender, e);
            checkedListBox1.Items.Clear();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkedListBox2.Items.Clear();
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox2.Text = comboBox8.SelectedItem.ToString();
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox4.Text = comboBox9.SelectedItem.ToString();
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox6.Text = comboBox10.SelectedItem.ToString();
        }
    }
}
