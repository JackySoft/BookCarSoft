using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Threading;
using System.Globalization;
using System.Management;

namespace BookCarSoft
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string xxzh;
        public string yyrq;
        public string yysd;
        public string yykm;
        public string CNBH;
        public string __VIEWSTATE;
        public string __EVENTVALIDATION;
        //用户勾选的日期列表
        public List<string> dateList = new List<string>();
        //当前可预约车辆的日期列表
        public List<string> avaDateList = new List<string>();
        public List<string> xnsdList = new List<string>();
        public List<string> yyrqList = new List<string>();
        public List<Label> list_1 = null;
        public List<Label> list_2 = null;
        public List<Label> list_3 = null;
        public bool loginSuccFlag = false;
        public DateTime serverTime;
        public string strIP, strSubnet, strGateway, strDNS;
        public bool isQuick = false;
        public string aaaaa = "QzDmLcq7KgPO9VhTLukNWlLGtsIpd9PH2TPQjJnh4Pp4FuDbHME5u9fUK3CS3ZO9b93KvUCXk-add-dxuTpxi1ostYaPkUZ41yzcJcde1xpPepyhvn5DhiFcFnwwQU5-add-QFDFpFnvVCjvypzQ9WFQzCnluEFxw2VZl7H4ejkiEg2fPl8=";
        //07:45--11:30	12:45--16:30	17:00--20:00
        private void btnAboutCar_Click(object sender, EventArgs e)
        {
            ///user/stulogin?username=51511293&password=1028&code=110108015&
            ///aaaaa=PKRtdNFIpILRzoAtb-add-2RLl1sAuMZ2O8IRzsFNSUjObzNNC14Vl/nsrWxpfFDQ6Ip75S82HdyQGZ6FPDja6unl3Xtm1w/A1AwIQonH6AZ9-add-NUlNXGke9wAwbz84F7h-add-2lZnWzyUaCLBgpFBvGfRnQXeWh6fEfD0mB-add-mPsywp7VZA=
            ///&os=ios&version=2.5
            if (this.btnAboutCar.Text.Equals("登陆"))
            {
                string txtUserName = this.txtUserName.Text.ToString();
                string txtPassword = this.txtUserPwd.Text.ToString();
                if (txtUserName.Equals("") || txtUserPwd.Equals(""))
                {
                    MessageBox.Show("请输入用户名或者密码！");
                    return;
                }
                if (!HttpRequestHelper.getUserLimitData(txtUserName))
                {
                    printLog("您当前账号尚未开通权限，请联系管理员进行权限开通！", Color.Red);
                    MessageBox.Show("当前账号未开通权限，请联系管理员进行权限开通才能使用！");
                    return;
                }
                string url = "http://api.xuechebu.com/user/stulogin";
                string param = "username=" + txtUserName + "&password=" + txtPassword + "&code=110108015&aaaaa=" + aaaaa + "&os=ios&version=2.5";
                string html = HttpRequestHelper.HttpGet(url, param, true);
                html = HttpRequestHelper.replaceComma(html);
                MessageInfo codeInfo = FromJsonTo<MessageInfo>(html);
                if (codeInfo.code > 0)
                {
                    printLog(codeInfo.message, Color.Red);
                }
                else 
                {
                    //codeInfo.data.xxzh
                    this.xxzh = txtUserName;
                    printLog("登陆成功.", Color.Green);
                    //获取当前可预约车辆所对应的日期列表
                    this.getAllCarsCount(true);
                    loginSuccFlag = true;
                    
                    this.saveUserInfo(txtUserName, txtPassword);
                    this.btnAboutCar.Text = "退出登陆";
                }
            }
            else 
            {
                loginSuccFlag = false;
                
                if (this.button1.Text.Equals("停止预约")) 
                {
                    this.timer1.Enabled = false;
                    this.button1.Text = "开始预约";
                    printLog("当前已停止约车.", Color.Black);
                }
                //如果抢车专区仍在进行，则需要同时停止
                if (this.btnGrabCar.Text.Equals("停止抢车")) 
                {
                    //关闭所有定时器
                    this.timerGrab.Enabled = false;
                    this.startThread.Enabled = false;
                    this.btnGrabCar.Text = "开始抢车";
                    stopThread();
                    printLog("当前已停止抢车.", Color.Black);
                }
                printLog("当前已退出登陆.", Color.Black);
                this.btnAboutCar.Text = "登陆";
            }
            
        }

        public static T FromJsonTo<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T jsonObject = (T)ser.ReadObject(ms);
            return jsonObject;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                DateTime datetime = System.DateTime.Now;
                //第一列，初始化当前可预约的时间表
                List<Label> lblList = new List<Label>() { lbl_0, lbl_1, lbl_2, lbl_3, lbl_4, lbl_5, lbl_6, lbl_7 };
                //需要动态添加当前的日期到Tag中，便于后续操作
                list_1 = new List<Label>() { lbl_1_1, lbl_1_2, lbl_1_3, lbl_1_4, lbl_1_5, lbl_1_6, lbl_1_7, lbl_1_8 };
                list_2 = new List<Label>() { lbl_2_1, lbl_2_2, lbl_2_3, lbl_2_4, lbl_2_5, lbl_2_6, lbl_2_7, lbl_2_8 };
                list_3 = new List<Label>() { lbl_3_1, lbl_3_2, lbl_3_3, lbl_3_4, lbl_3_5, lbl_3_6, lbl_3_7, lbl_3_8 };
                int curDay = datetime.Day-1;

                DateTime d1 = new DateTime(datetime.Year, datetime.Month, datetime.Day);

                string currentDate = "";
                for (int i = 0; i < lblList.Count; i++)
                {
                    currentDate = d1.Year + "-";
                    if (d1.Month > 9)
                    {
                        currentDate += d1.Month + "-";
                    }
                    else
                    {
                        currentDate += "0" + d1.Month + "-";
                    }
                    if (d1.Day > 9)
                    {
                        currentDate += d1.Day;
                    }
                    else 
                    {
                        currentDate += "0"+d1.Day;
                    }
                    lblList[i].Text = currentDate;
                    //将预约日期和预约时段存入Tag中，便于点击的时候获取
                    list_1[i].Tag = currentDate + "." + list_1[i].Tag;
                    list_2[i].Tag = currentDate + "." + list_2[i].Tag;
                    list_3[i].Tag = currentDate + "." + list_3[i].Tag;
                    d1 = d1.AddDays(1);
                }
                HttpRequestHelper.HttpGet("http://haijia.xuechebu.com:8008/","",false);
                //默认选中时段中的第一个值
                this.cboGrabSD.SelectedIndex = 0;
                //约车设置默认日期
                DateTime dt = DateTime.Now;
                dt = dt.AddDays(7);
                this.dtGrabDate.Value = dt;

                //获取服务器时间
                string dateHtml = HttpRequestHelper.HttpGet("http://haijia.xuechebu.com:8008/API/GetServiceDate", "", true);

                dateHtml = HttpRequestHelper.replaceComma(dateHtml);
                MessageCode timeInfo = FromJsonTo<MessageCode>(dateHtml);
                if (timeInfo.code == 0)
                {
                    serverTime = StampToDateTime(timeInfo.data);
                    lblServerTime.Text = serverTime.ToLongTimeString().ToString();
                    this.timerServer.Enabled = true;
                }

                //SetNetworkAdapter();
                Control.CheckForIllegalCrossThreadCalls = false;
                FileStream aFile = new FileStream("C:\\xuechebu.txt", FileMode.OpenOrCreate);
                StreamReader sr = new StreamReader(aFile);
                if (sr != null)
                {
                    string username = sr.ReadLine();
                    if (username == null)
                    {
                        sr.Close();
                    }
                    else
                    {
                        this.txtUserName.Text = username;
                        string pwd = sr.ReadLine();
                        this.txtUserPwd.Text = pwd;
                    }
                }
            }
            catch
            {
                printLog("软件启动部分信息加载失败，请联系管理员修复！",Color.Red);
                return;
            }
        }

        //设置当前预约的时间段
        private void setAboutCarDate(object sender, EventArgs e)
        {
            if (loginSuccFlag == false) 
            {
                MessageBox.Show("长官：登陆后才能享受预约服务。");
                return;
            }

            Label lbl = (Label)sender;
            string tag = lbl.Tag.ToString();
            string[] tagAra = tag.Split('.');
            
            if (lbl.Text.Equals("未预约")) 
            {
                lbl.Text = "抢车中";
                //yyrqList.Add(tagAra[0]);
                //xnsdList.Add(tagAra[1]);
                dateList.Add(tag);
                lbl.BackColor = Color.OrangeRed;
                printLog("预约信息：" + tagAra[0] + " " + HttpRequestHelper.getXnsdName(tagAra[1]), Color.Black);
            }
            else if (lbl.Text.Equals("抢车中")) 
            {
                lbl.Text = "未预约";
                //yyrqList.Remove(tagAra[0]);
                //xnsdList.Remove(tagAra[1]);
                dateList.Remove(tag);
                lbl.BackColor = Color.PaleGreen;
                printLog("取消预约信息：" + tagAra[0] + " " + HttpRequestHelper.getXnsdName(tagAra[1]), Color.Black);
            }
            
        }

        //查询本周哪天有车可预约
        private void getCarList_Click(object sender, EventArgs e)
        {
            if (loginSuccFlag == false)
            {
                MessageBox.Show("长官:您当前还未登陆，登陆后，小二立马送上约车服务！");
                return;
            }
            this.getAllCarsCount(true);
            if (this.avaDateList.Count == 0) 
            {
                printLog("长官:本周所有的车辆已预约完，您可以点击右边进行检漏。", Color.Blue);
            }
        }

        //开始约车
        private void button1_Click(object sender, EventArgs e)
        {
            if (loginSuccFlag == false) 
            {
                MessageBox.Show("长官，您当前还未登陆，登陆后，小二立马送上约车服务！");
                return;
            }
            //如果用户未进行预约
            if (this.dateList.Count == 0)
            {
                printLog("长官，您是不是在耍我呢，快快在右边预约车辆吧！",Color.OrangeRed);
                return;
            }
            if (this.txtRefreshTime.Text.ToString().Equals(""))
            {
                MessageBox.Show("请输入刷新时间！");
                return;
            }
            /*if (Int32.Parse(this.txtRefreshTime.Text.ToString()) < 5)
            {
                MessageBox.Show("为了您的账号安全，刷新时间不可以小于5秒，刷新3后！");
                return;
            }*/
            try
            {

                this.timer1.Interval = Int32.Parse(this.txtRefreshTime.Text.ToString()) * 1000;
            }
            catch
            {
                this.timer1.Interval = 5000;
                this.txtRefreshTime.Text = "5";
            }

            if (this.button1.Text.Equals("开始预约"))
            {
                printLog("小二已开启预约模式中.",Color.Black);
                this.timer1.Enabled = true;
                this.button1.Text = "停止预约";
                
            }
            else
            {
                this.timer1.Enabled = false;
                this.button1.Text = "开始预约";
                printLog("当前已停止约车.", Color.Black);
            }
        }

        //定时查询预约车辆
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isQuick)
            {
                isQuick = false;
                //Thread.Sleep(3000 * 60);
                button1_Click(sender, e);//暂停捡漏
            }
            else 
            {
                bool isHasCars = false;
                //获取当前可预约车辆所对应的日期列表
                this.getAllCarsCount(false);
                //如果用户的预约列表数量大于0，才可进行预约，否则，停止约车
                if (this.dateList.Count > 0)
                {
                    if (this.avaDateList.Count > 0)
                    {
                        //循环判断，如果用户勾选的日期包含当前日期，并且又可以预约，则进行预约
                        for (int i = 0; i < dateList.Count; i++)
                        {
                            //循环可预约的日期列表，判断是否有用户勾选过的
                            for (int j = 0; j < avaDateList.Count; j++)
                            {
                                //printLog(avaDateList[j] + "  用户勾选的：" + dateList[i]);
                                if (dateList[i].Equals(avaDateList[j]))
                                {
                                    string rq = dateList[i].Split('.')[0];
                                    string sd = dateList[i].Split('.')[1];
                                    //printLog(rq + " " + sd + " 是你所需要的 。 ");
                                    //发现有相同的日期和时段后，进行查询车辆，并循环预订
                                    getAvaliableCars(rq, sd);
                                    isHasCars = true;
                                    break;//一次循环只可能存在一个相同的日期和时段
                                }
                            }
                        }
                        if (isHasCars == false)
                        {
                            printLog("目前系统查询到的车辆与您期望的不匹配，继续检漏中...", Color.Blue);
                        }
                    }
                    else
                    {
                        printLog("当前暂时没有可预约的车辆，小二正努力检漏中...", Color.Black);
                    }
                }
                else
                {
                    this.timer1.Enabled = false;
                    printLog("长官，没有需要预约的车辆了，您哪天有时间练车，快快点击预约吧.", Color.Black);
                    this.button1.Text = "开始预约";
                }
            }
            
        }

        //获取本周可预约的车辆数目
        public void getAllCarsCount(bool isShowLog)
        {
            string url = "http://haijia.xuechebu.com:8008/KM2/ClYyTimeSectionUIQuery2?xxzh=" + xxzh + "&aaaaa=" + aaaaa + "&os=ios&version=2.5";
            
            try
            {
                string html = HttpRequestHelper.HttpGet(url,"",true);
                MessageInfo info = null;
                try
                {
                    html = HttpRequestHelper.replaceComma(html);
                    //html = html.Substring(8, html.IndexOf(",\"code\"") - html.IndexOf("\"data\":")-7);
                    info = FromJsonTo<MessageInfo>(html);
                    if (info.data == null)
                    {
                        if (info.message.Equals("访问太过频繁,请稍后再试！"))
                        {
                            isQuick = true;
                        }
                        printLog(info.message, Color.Red);
                        return;
                    }
                    isQuick = false;
                    //循环加载最新的约车日期
                    List<CarItem> list = info.data.UIDatas;
                    if (list != null)
                    {
                        //清空列表从新添加
                        avaDateList.Clear();
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (list[i].SL > 0)
                            {
                                string rq = list[i].Yyrq.Substring(0, 10).Replace("/", "-");
                                string sd = list[i].Xnsd;

                                //仅仅查询本周哪天有车辆可预约，并不进行记录保存
                                if (isShowLog == true)
                                {
                                    printLog(rq + HttpRequestHelper.getXnsdName(sd) + " 系统查询到了" + list[i].SL + "辆车。", Color.OrangeRed);
                                }

                                //将系统中查询到的有车的日期列表添加到集合中
                                avaDateList.Add(rq + "." + sd);
                                
                            }
                            //说明该时段下，预约成功
                            if (list[i].IsBpked)
                            {
                                int sysDay = System.DateTime.Now.Day;
                                int realDay = Int32.Parse(list[i].Yyrq.Substring(0, 10).Split('/')[2]);
                                string sd = list[i].Xnsd;
                                string item = list[i].Yyrq.Substring(0, 10).Replace('/', '-');
                                
                                DateTime d1 =  Convert.ToDateTime(DateTime.Now.Year+"-"+DateTime.Now.Month+"-"+DateTime.Now.Day);

                                DateTime d2 = Convert.ToDateTime(item);
                                int lineNum = (d2 - d1).Days;
                                if (dateList.Count > 0)
                                {
                                    //当天有一个时段约车成功，都需要将该天的所有时段都取消，防止下次重复提交。
                                    if (dateList.Contains(item + ".711") && sd.Equals("711"))
                                    {
                                        //如果该日期和时段下约车成功了，需要将该项移除，防止下次重复预约
                                        dateList.Remove(item + ".711");
                                        list_1[lineNum].Text = "已预约";
                                        list_1[lineNum].BackColor = System.Drawing.Color.Green;
                                        if (dateList.Contains(item + ".1216")) 
                                        {
                                            dateList.Remove(item + ".1216");
                                            list_2[lineNum].Text = "未预约";
                                            list_2[lineNum].BackColor = System.Drawing.Color.PaleGreen;
                                        }
                                        if (dateList.Contains(item + ".1720"))
                                        {
                                            dateList.Remove(item + ".1720");
                                            list_3[lineNum].Text = "未预约";
                                            list_3[lineNum].BackColor = System.Drawing.Color.PaleGreen;
                                        }
                                        
                                    }

                                    if (dateList.Contains(item + ".1216") && sd.Equals("1216"))
                                    {
                                        //如果该日期和时段下约车成功了，需要将该项移除，防止下次重复预约
                                        dateList.Remove(item + ".1216");
                                        list_2[lineNum].Text = "已预约";
                                        list_2[lineNum].BackColor = System.Drawing.Color.Green;
                                        if (dateList.Contains(item + ".711"))
                                        {
                                            dateList.Remove(item + ".711");
                                            list_1[lineNum].Text = "未预约";
                                            list_1[lineNum].BackColor = System.Drawing.Color.PaleGreen;
                                        }
                                        if (dateList.Contains(item + ".1720"))
                                        {
                                            dateList.Remove(item + ".1720");
                                            list_3[lineNum].Text = "未预约";
                                            list_3[lineNum].BackColor = System.Drawing.Color.PaleGreen;
                                        }

                                    }

                                    if (dateList.Contains(item + ".1720") && sd.Equals("1720"))
                                    {
                                        //如果该日期和时段下约车成功了，需要将该项移除，防止下次重复预约
                                        dateList.Remove(item + ".1720");
                                        list_3[lineNum].Text = "已预约";
                                        list_3[lineNum].BackColor = System.Drawing.Color.Green;
                                        if (dateList.Contains(item + ".711"))
                                        {
                                            dateList.Remove(item + ".711");
                                            list_1[lineNum].Text = "未预约";
                                            list_1[lineNum].BackColor = System.Drawing.Color.PaleGreen;
                                        }
                                        if (dateList.Contains(item + ".1216"))
                                        {
                                            dateList.Remove(item + ".1216");
                                            list_2[lineNum].Text = "未预约";
                                            list_2[lineNum].BackColor = System.Drawing.Color.PaleGreen;
                                        }
                                    }
                                }
                                else 
                                {
                                    if (sd.Equals("711"))
                                    {
                                        list_1[lineNum].Text = "已预约";
                                        list_1[lineNum].BackColor = System.Drawing.Color.Green;
                                    }
                                    else if (sd.Equals("1216"))
                                    {
                                        list_2[lineNum].Text = "已预约";
                                        list_2[lineNum].BackColor = System.Drawing.Color.Green;
                                    }
                                    else if (sd.Equals("1720"))
                                    {
                                        list_3[lineNum].Text = "已预约";
                                        list_3[lineNum].BackColor = System.Drawing.Color.Green;
                                    }
                                }
                            }
                            //此判断，主要用于，当前用户预约成功以后，又去官网取消了，软件界面没有及时把该时段变成未预约，为了提高速度，需重新打开软件。
                            /*else 
                            {
                                int sysDay = System.DateTime.Now.Day;
                                int realDay = Int32.Parse(list[i].Yyrq.Substring(0, 10).Split('/')[2]);
                                string sd = list[i].Xnsd;
                                

                                if (sd.Equals("711"))
                                {
                                    Label lbl = list_1[realDay - sysDay];
                                    if(lbl.Text.Equals("已预约"))
                                    {
                                        lbl.Text = "未预约";
                                        lbl.BackColor = System.Drawing.Color.PaleGreen;
                                    }
                                }
                                else if (sd.Equals("1216"))
                                {
                                    Label lbl = list_2[realDay - sysDay];
                                    if (lbl.Text.Equals("已预约"))
                                    {
                                        lbl.Text = "未预约";
                                        lbl.BackColor = System.Drawing.Color.PaleGreen;
                                    }
                                }
                                else if (sd.Equals("1720"))
                                {
                                    Label lbl = list_3[realDay - sysDay];
                                    if (lbl.Text.Equals("已预约"))
                                    {
                                        lbl.Text = "未预约";
                                        lbl.BackColor = System.Drawing.Color.PaleGreen;
                                    }
                                }

                            }*/
                        }
                    }
                }
                catch (Exception ex)
                {
                    printLog(ex.Message, Color.Red);
                    return;
                }

            }
            catch (WebException ex)
            {
                printLog(ex.Message, Color.Red);
            }
        }

        //获取指定日期下可预约的车辆数目
        public void getAvaliableCars(string yyrq, string xnsd)
        {
            string url = "http://haijia.xuechebu.com:8008/KM2/ClYyCars2?filters[xnsd]=" + xnsd + "&filters[xxzh]=" + xxzh + "&filters[yyrq]=" + yyrq + "&xxzh=" + xxzh + "&aaaaa=" + aaaaa + "&os=ios&version=2.5&pageno=1&pagesize=1000";

            try
            {
                string html = HttpRequestHelper.HttpGet(url, "", true);
                AvaliableCarsInfo info = null;
                html = HttpRequestHelper.replaceComma(html);
                //html = html.Substring(8, html.IndexOf(",\"code\"") - html.IndexOf("\"data\":")-7);
                info = FromJsonTo<AvaliableCarsInfo>(html);
                if (info.data.Total == 0)
                {
                    printLog("暂无可预约的车辆！", Color.Black);
                    return;
                }
                else
                {
                    printLog(yyrq + " " + HttpRequestHelper.getXnsdName(xnsd) + ",系统查询到了" + info.data.Total + " 辆车，小二正在努力抢订中...", Color.OrangeRed);
                    for (int k = 0; k < info.data.Total; k++)
                    {
                        string cph = info.data.Result[k].CNBH;
                        //08240.2015-11-14.711..
                        string result = this.bookCar(cph + "." + yyrq + "." + xnsd + "..");
                        string xnsdName = HttpRequestHelper.getXnsdName(xnsd);
                        if (result.IndexOf("error") > -1)
                        {
                            printLog(yyrq + " " + xnsdName + "车牌号为：" + cph + "的车辆，预约失败，失败信息为：" + result, Color.Red);
                        }
                        else
                        {
                            BookCarResult res = FromJsonTo<BookCarResult>(result);
                            if (res.code == 0)
                            {
                                printLog(yyrq + " " + xnsdName + "车牌号为 " + cph + " 的车辆，预约成功。", Color.Green);
                                break;
                            }
                            else
                            {
                                if (res.message.Contains("超出小时"))
                                {
                                    this.timer1.Enabled = false;
                                    this.button1.Text = "开始预约";
                                    printLog("长官，您当前科目预约时间已经满了，可以预约考试了！", Color.Blue);
                                    break;
                                }
                                else if (res.message.Contains("不能约周六日"))
                                {
                                    this.timer1.Enabled = false;
                                    this.button1.Text = "开始预约";
                                    printLog("很抱歉，您不能预约周六日车辆，可以到驾校更换约车类型...", Color.Red);
                                    break;
                                }
                                else if (res.message.Contains("非预约开放时间"))
                                {
                                    printLog("未到预约时间，暂无法预约，系统已进入紧张准备中...", Color.OrangeRed);
                                    break;
                                }
                                else
                                {
                                    printLog(yyrq + " " + xnsdName + "车牌号为 " + cph + " 的车辆，预约失败，失败信息为：" + res.message, Color.OrangeRed);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                printLog(ex.Message, Color.Red);
                return;
            }
        }

        //车辆预约 
        public string bookCar(string param)
        {
            string getCarListUrl = "http://haijia.xuechebu.com:8008/KM2/ClYyAddByMutil?xxzh=" + xxzh + "&aaaaa=" + aaaaa + "&os=ios&version=2.5&params=" + param + "&isJcsdYyMode=5";
            
            try
            {
                string html = HttpRequestHelper.HttpGet(getCarListUrl,"",true);
                
                return html;
            }
            catch (WebException ex)
            {
                return "error:" + ex.Message;
            }
        }

        //当内容变化时，从新聚焦
        private void logPrint_TextChanged(object sender, EventArgs e)
        {
            this.logPrint.ScrollToCaret();
        }

        //打印日志
        public void printLog(string logStr,Color color)
        {
            string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (color == null)
            {
                this.logPrint.SelectionColor = Color.Black;
            }
            else 
            {
                this.logPrint.SelectionColor = color;
            }
            
            this.logPrint.AppendText(datetime.Split(' ')[1]+" "+logStr + "\r\n");
            this.logPrint.Focus();
        }

        //打印日志
        public void printLog(string logStr)
        {
            string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.logPrint.SelectionColor = Color.Black;
            this.logPrint.AppendText(datetime.Split(' ')[1] + " " + logStr + "\r\n");
            this.logPrint.Focus();
        }

        //将登录信息保存在本地，以便下次使用
        public void saveUserInfo(string username, string userpwd)
        {
            try
            {
                FileStream aFile = new FileStream("C:\\xuechebu.txt", FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(aFile);
                // Write data to file.
                sw.WriteLine(username);
                sw.WriteLine(userpwd);
                sw.Write("##以上信息主要为了保存您当前约车软件的账号，方便下次登录，您也可以自行删除##");
                sw.Flush();
                sw.Close();
            }
            catch
            {
                printLog("账号保存失败！",Color.Red);
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
            this.notifyIcon1.Visible = false;
        }

        public List<string> cphList = new List<string>();
        public List<string> cphList1 = new List<string>();
        public List<string> cphList2 = new List<string>();
        public List<string> cphList3 = new List<string>();
        public int timerIndex = -1;
        public string grabYyrq = "";
        public string grabXnsd = "";
        public Thread t1;
        public Thread t2;
        public Thread t3;
        private void btnGrabCar_Click(object sender, EventArgs e)
        {
            if (loginSuccFlag == false)
            {
                MessageBox.Show("长官，您当前还未登陆，登陆后，小二立马送上约车服务！");
                return;
            }
            grabYyrq = this.dtGrabDate.Value.ToString("yyyy-MM-dd");
            grabXnsd = HttpRequestHelper.getXnsd(this.cboGrabSD.Text.ToString());
            bool isChecked = this.chkCheckAll.Checked;
            cphList.Clear();
            cphList1.Clear();
            cphList2.Clear();
            cphList3.Clear();
            timerIndex = -1;//初始化变量
            DateTime yyDate = Convert.ToDateTime(grabYyrq + " 07:35:00");
            yyDate = yyDate.AddDays(-7);//往前推7天
            if (this.btnGrabCar.Text.Equals("开始抢车"))
            {
                if (isChecked == false)
                {

                    //取出所有车辆的车牌号cnbh
                    this.getGrabCars(grabYyrq, grabXnsd,false);
                    if (cphList.Count > 0)
                    {
                        printLog("系统会在 " + yyDate.ToString() + "，定时开启自动约车，请耐心等待...", Color.Black);
                        //启动定时器，定时约车
                        this.timerGrab.Enabled = true;
                        this.btnGrabCar.Text = "停止抢车";
                    }
                }
                else 
                {
                    this.getGrabCars(grabYyrq, "711",true);
                    this.getGrabCars(grabYyrq, "1216", true);
                    this.getGrabCars(grabYyrq, "1720", true);
                    if (this.cphList1.Count == 0 && cphList2.Count == 0 && cphList3.Count == 0)
                    {
                        //如果时间到了，停止计时器
                        this.startThread.Enabled = false;
                        this.btnGrabCar.Text = "开始抢车";
                        return;
                    }
                    else 
                    {
                        printLog("系统会在 " + yyDate.ToString() + "，定时开启自动约车，请耐心等待...", Color.Black);
                        this.btnGrabCar.Text = "停止抢车";
                        this.startThread.Enabled = true;
                    }
                }
            }
            else 
            {
                printLog("系统已停止定时约车。",Color.Black);
                this.lblCurrentDate.Text = DateTime.Now.ToLongDateString().ToString();
                //关闭所有定时器
                this.timerGrab.Enabled = false;
                this.startThread.Enabled = false;
                this.btnGrabCar.Text = "开始抢车";
                stopThread();
            }
        }

        private void timerGrab_Tick(object sender, EventArgs e)
        {
            long curSecond = serverTime.Ticks; 
            //定时
            DateTime needDate = Convert.ToDateTime(grabYyrq + " 07:35:00");
            needDate = needDate.AddDays(-7);//往前推7天
            long realNeedSecond = needDate.Ticks;
            if (curSecond - realNeedSecond >= 0)
            {
                timerIndex++;

                if (timerIndex == cphList.Count)
                {
                    timerIndex = -1;
                    cphList.Clear();
                    this.getGrabCars(grabYyrq, grabXnsd, false);
                    if (cphList.Count > 0)
                    {
                        printLog("系统再次启动约车,请等待...", Color.Black);
                        //启动定时器，定时约车
                        this.timerGrab.Enabled = true;
                        this.btnGrabCar.Text = "停止抢车";
                    }
                    else 
                    {
                        //启动定时器，定时约车
                        this.timerGrab.Enabled = false;
                        this.btnGrabCar.Text = "开始抢车";
                        printLog("当前系统已停止约车，请开启检漏模式.", Color.Black);
                    }
                    return;
                }
                string cph = cphList[timerIndex];
                string resultHtml = this.bookCar(cph + "." + grabYyrq + "." + grabXnsd + "..");
                string xnsdName = HttpRequestHelper.getXnsdName(grabXnsd);
                if (resultHtml.IndexOf("error") > -1)
                {
                    printLog(yyrq + " " + xnsdName + "车牌号为：" + cph + "的车辆，预约失败，失败信息为：" + resultHtml, Color.Red);
                }
                else
                {
                    BookCarResult res = FromJsonTo<BookCarResult>(resultHtml);
                    if (res.code == 0)
                    {
                        printLog(yyrq + " " + xnsdName + "车牌号为 " + cph + " 的车辆，预约成功。", Color.Green);
                        this.timerGrab.Enabled = false;
                        this.btnGrabCar.Text = "开始抢车";
                    }
                    else
                    {
                        if (res.message.Contains("超出小时"))
                        {
                            this.timerGrab.Enabled = false;
                            this.btnGrabCar.Text = "开始抢车";
                            printLog("长官:您当前科目预约时间已经满了，可以预约考试了！", Color.Blue);
                        }
                        else if (res.message.Contains("非预约开放时间"))
                        {
                            printLog("尚未到预约时间,系统已进入一级战斗中,请等待...", Color.OrangeRed);
                        }
                        else
                        {
                            printLog(yyrq + " " + xnsdName + "车牌号为 " + cph + " 的车辆，预约失败，失败信息为：" + res.message, Color.Red);
                        }
                    }
                }
            }
            
        }

        private void startThread_Tick(object sender, EventArgs e)
        {
            //DateTimeFormatInfo dtFormat = new System.GlobalizationDateTimeFormatInfo();
            //dtFormat.ShortDatePattern = "yyyy/MM/dd";
            long curSecond = DateTime.Now.Ticks;
            //定时
            DateTime needDate = Convert.ToDateTime(grabYyrq+" 07:34:50");
            needDate = needDate.AddDays(-7);//往前推7天
            long realNeedSecond = needDate.Ticks;
            this.lblCurrentDate.Text = DateTime.Now.ToLocalTime().ToString();
            if (curSecond - realNeedSecond >= 0) 
            {
                printLog("系统已为您开启多线程模式约车，请您耐心等待...", Color.Black);
                t1 = new Thread(new ThreadStart(threadYueChe1));
                t1.Name = "Thread_1";
                t1.IsBackground = true;
                t1.Start();
                //t1.Join();
                t2 = new Thread(new ThreadStart(threadYueChe2));
                t2.Name = "Thread_2";
                t2.IsBackground = true;
                t2.Start();
                //t2.Join();
                t3 = new Thread(new ThreadStart(threadYueChe3));
                t3.Name = "Thread_3";
                t3.IsBackground = true;
                t3.Start();
                //如果时间到了，停止计时器
                this.startThread.Enabled = false;
            }
        }

        //获取指定日期下可预约的车辆数目
        public void getGrabCars(string yyrq, string xnsd,bool isThread)
        {
            string url = "http://haijia.xuechebu.com:8008/KM2/ClYyCars2?filters[xnsd]=" + xnsd + "&filters[xxzh]=" + xxzh + "&filters[yyrq]=" + yyrq + "&xxzh=" + xxzh + "&os=ios&version=2.5&pageno=1&pagesize=1000";
            try
            {
                string html = HttpRequestHelper.HttpGet(url, "", true);
                
                AvaliableCarsInfo info = null;
                try
                {
                    html = HttpRequestHelper.replaceComma(html);
                    //html = html.Substring(8, html.IndexOf(",\"code\"") - html.IndexOf("\"data\":")-7);
                    info = FromJsonTo<AvaliableCarsInfo>(html);//{"data":null,"code":110,"message":"身份认证失败,请重新登录"}
                    if (info.data.Total == 0)
                    {
                        printLog(yyrq+HttpRequestHelper.getXnsdName(xnsd)+" 没有可预约车辆，请开启检漏模式！", Color.Black);
                        return;
                    }
                    else
                    {
                        printLog(yyrq + " " + HttpRequestHelper.getXnsdName(xnsd) + " 系统查询到了" + info.data.Total + " 辆车，小二正在努力抢订中...", Color.OrangeRed);
                        for (int k = 0; k < info.data.Total; k++)
                        {
                            string cph = info.data.Result[k].CNBH;
                            if (isThread == false)
                            {
                                cphList.Add(cph);
                            }
                            else 
                            {
                                if (xnsd.Equals("711")) 
                                {
                                    cphList1.Add(cph);
                                }
                                if (xnsd.Equals("1216"))
                                {
                                    cphList2.Add(cph);
                                }
                                if (xnsd.Equals("1720"))
                                {
                                    cphList3.Add(cph);
                                }
                            }
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    printLog(ex.Message, Color.Red);
                    return;
                }

            }
            catch (WebException ex)
            {
                printLog(ex.Message, Color.Red);
            }
        }

        public object locker = new object();
        public void threadYueChe1()
        {

            if (cphList1.Count > 0)
            {
                for (int i = 0; i < cphList1.Count; i++)
                {
                    lock (locker)
                    {
                        string cph = cphList1[i];
                        string resultHtml = this.bookCar(cph + "." + grabYyrq + ".711..");
                        string xnsdName = HttpRequestHelper.getXnsdName(grabXnsd);
                        if (resultHtml.IndexOf("error") > -1)
                        {
                            printLog(yyrq + " " + xnsdName + "车牌号为：" + cph + "的车辆，预约失败，失败信息为：" + resultHtml, Color.Red);
                        }
                        else
                        {
                            BookCarResult res = FromJsonTo<BookCarResult>(resultHtml);
                            if (res.code == 0)
                            {
                                printLog(grabYyrq + " " + xnsdName + "车牌号为 " + cph + " 的车辆，预约成功。", Color.Green);
                                this.btnGrabCar.Text = "开始抢车";
                                this.getAllCarsCount(false);
                                stopThread();
                            }
                            else
                            {
                                if (res.message.Contains("超出小时"))
                                {
                                    this.btnGrabCar.Text = "开始抢车";
                                    printLog("长官:您当前科目预约时间已经满了，可以预约考试了！", Color.Blue);
                                    stopThread();
                                }
                                else if (res.message.Contains("非预约开放时间"))
                                {
                                    printLog("尚未到预约时间,系统已进入一级战斗中,请等待...", Color.OrangeRed);
                                }
                                else if (res.message.Contains("只能预约七天内的车")) 
                                {
                                    printLog("只能预约7天内的车，线程已停止。", Color.OrangeRed);
                                    stopThread();
                                }
                                else
                                {
                                    printLog(yyrq + " " + xnsdName + "车牌号为 " + cph + " 的车辆，预约失败，失败信息为：" + res.message, Color.OrangeRed);
                                }
                            }
                        }
                    }
                    Thread.Sleep(500);
                }
            }
            printLog(Thread.CurrentThread.Name + "约车结束，如果未约到车，请开启检漏模式，或继续抢车！", Color.Black);
            this.btnGrabCar.Text = "开始抢车";
            t1.Abort();
        }

        public void threadYueChe2()
        {

            if (cphList2.Count > 0)
            {
                for (int i = 0; i < cphList2.Count; i++)
                {
                    lock (locker)
                    {
                        string cph = cphList2[i];
                        string resultHtml = this.bookCar(cph + "." + grabYyrq + ".711..");
                        string xnsdName = HttpRequestHelper.getXnsdName(grabXnsd);
                        if (resultHtml.IndexOf("error") > -1)
                        {
                            printLog(yyrq + " " + xnsdName + "车牌号为：" + cph + "的车辆，预约失败，失败信息为：" + resultHtml, Color.Red);
                        }
                        else
                        {
                            BookCarResult res = FromJsonTo<BookCarResult>(resultHtml);
                            if (res.code == 0)
                            {
                                printLog(grabYyrq + " " + xnsdName + "车牌号为 " + cph + " 的车辆，预约成功。", Color.Green);
                                this.btnGrabCar.Text = "开始抢车";
                                this.getAllCarsCount(false);
                                stopThread();
                            }
                            else
                            {
                                if (res.message.Contains("超出小时"))
                                {
                                    this.btnGrabCar.Text = "开始抢车";
                                    printLog("长官:您当前科目预约时间已经满了，可以预约考试了！", Color.Blue);
                                    stopThread();
                                }
                                else if (res.message.Contains("非预约开放时间"))
                                {
                                    printLog("尚未到预约时间,系统已进入一级战斗中,请等待...", Color.OrangeRed);
                                }
                                else if (res.message.Contains("只能预约七天内的车"))
                                {
                                    printLog("只能预约7天内的车，线程已停止。", Color.OrangeRed);
                                    stopThread();
                                }
                                else
                                {
                                    printLog(yyrq + " " + xnsdName + "车牌号为 " + cph + " 的车辆，预约失败，失败信息为：" + res.message, Color.OrangeRed);
                                }
                            }
                        }
                    }
                    Thread.Sleep(500);
                }
            }
            printLog(Thread.CurrentThread.Name + "约车结束，如果未约到车，请开启检漏模式，或继续抢车！", Color.Black);
            this.btnGrabCar.Text = "开始抢车";
            t2.Abort();
        }

        public void threadYueChe3()
        {

            if (cphList3.Count > 0)
            {
                for (int i = 0; i < cphList3.Count; i++)
                {
                    lock (locker)
                    {
                        string cph = cphList3[i];
                        string resultHtml = this.bookCar(cph + "." + grabYyrq + ".711..");
                        string xnsdName = HttpRequestHelper.getXnsdName(grabXnsd);
                        if (resultHtml.IndexOf("error") > -1)
                        {
                            printLog(yyrq + " " + xnsdName + "车牌号为：" + cph + "的车辆，预约失败，失败信息为：" + resultHtml, Color.Red);
                        }
                        else
                        {
                            BookCarResult res = FromJsonTo<BookCarResult>(resultHtml);
                            if (res.code == 0)
                            {
                                printLog(grabYyrq + " " + xnsdName + "车牌号为 " + cph + " 的车辆，预约成功。", Color.Green);
                                this.btnGrabCar.Text = "开始抢车";
                                this.getAllCarsCount(false);
                                stopThread();
                            }
                            else
                            {
                                if (res.message.Contains("超出小时"))
                                {
                                    this.btnGrabCar.Text = "开始抢车";
                                    printLog("长官:您当前科目预约时间已经满了，可以预约考试了！", Color.Blue);
                                    stopThread();
                                }
                                else if (res.message.Contains("非预约开放时间"))
                                {
                                    printLog("尚未到预约时间,系统已进入一级战斗中,请等待...", Color.OrangeRed);
                                }
                                else if (res.message.Contains("只能预约七天内的车"))
                                {
                                    printLog("只能预约7天内的车，线程已停止。", Color.OrangeRed);
                                    stopThread();
                                }
                                else
                                {
                                    printLog(yyrq + " " + xnsdName + "车牌号为 " + cph + " 的车辆，预约失败，失败信息为：" + res.message, Color.OrangeRed);
                                }
                            }
                        }
                    }
                    Thread.Sleep(500);
                }
            }
            printLog(Thread.CurrentThread.Name + "约车结束，如果未约到车，请开启检漏模式，或继续抢车！", Color.Black);
            this.btnGrabCar.Text = "开始抢车";
            t3.Abort();
        }
        public void stopThread()
        {
            if (t1 != null && t1.IsAlive == true) { t1.Abort(); }
            if (t2 != null && t2.IsAlive == true) { t2.Abort(); }
            if (t3 != null && t3.IsAlive == true) { t3.Abort(); }
        }

        // 时间戳转为C#格式时间
        private DateTime StampToDateTime(string timeStamp)
        {
            DateTime dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);

            return dateTimeStart.Add(toNow);
        }

        private void timerServer_Tick(object sender, EventArgs e)
        {
            serverTime = serverTime.AddSeconds(1);
            lblServerTime.Text = serverTime.ToLongTimeString().ToString();
        }

        public void GetIPAndDNS()
        {
            strIP = "0.0.0.0";
            strSubnet = "0.0.0.0";
            strGateway = "0.0.0.0";
            strDNS = "0.0.0.0";
            try
            {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection nics = mc.GetInstances();
                foreach (ManagementObject nic in nics)
                {
                    try
                    {
                        if (Convert.ToBoolean(nic["IPEnabled"]) == true)
                        {

                            if ((nic["IPAddress"] as String[]).Length > 0 && strIP == "0.0.0.0")
                            {
                                strIP = (nic["IPAddress"] as String[])[0];
                            }
                            if ((nic["IPSubnet"] as String[]).Length > 0 && strSubnet == "0.0.0.0")
                            {
                                strSubnet = (nic["IPSubnet"] as String[])[0];
                            }
                            if ((nic["DefaultIPGateway"] as String[]).Length > 0 && strGateway == "0.0.0.0")
                            {
                                strGateway = (nic["DefaultIPGateway"] as String[])[0];
                            }
                            if ((nic["DNSServerSearchOrder"] as String[]).Length > 0 && strDNS == "0.0.0.0")
                            {
                                strDNS = (nic["DNSServerSearchOrder"] as String[])[0];
                            }

                            printLog("本机IP:" + strIP, Color.Orange);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        //修改电脑ip
        private void SetNetworkAdapter(string ip)
        {
            ManagementBaseObject inPar = null;
            ManagementBaseObject outPar = null;
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            string newIp = "";
            ManagementObjectCollection moc = mc.GetInstances();
            if ("".Equals(strIP))
            {
                return;
            }
            else
            {
                newIp = strIP.Split('.')[0] + "." + strIP.Split('.')[1] + "." + strIP.Split('.')[2] + "." + ip;
            }
            foreach (ManagementObject mo in moc)
            {
                if (!(bool)mo["IPEnabled"])
                    continue;

                //设置ip地址和子网掩码
                inPar = mo.GetMethodParameters("EnableStatic");

                inPar["IPAddress"] = new string[] { newIp };// 1.备用 2.IP
                inPar["SubnetMask"] = new string[] { strSubnet };
                outPar = mo.InvokeMethod("EnableStatic", inPar, null);

                //设置网关地址
                inPar = mo.GetMethodParameters("SetGateways");
                inPar["DefaultIPGateway"] = new string[] { strGateway };// 1.网关;2.备用网关
                outPar = mo.InvokeMethod("SetGateways", inPar, null);

                //设置DNS
                inPar = mo.GetMethodParameters("SetDNSServerSearchOrder");
                inPar["DNSServerSearchOrder"] = new string[] { strDNS };// 1.DNS 2.备用DNS
                outPar = mo.InvokeMethod("SetDNSServerSearchOrder", inPar, null);
                break;
            }
            printLog("修改后的IP为：" + newIp + ",如果此IP与其他人冲突，则从新获取即可。");
        }



        private void btnUpdateIp_Click(object sender, EventArgs e)
        {
            Random ran = new Random();
            int RandKey = ran.Next(1, 255);
            if (strIP==null || "".Equals(strIP)) 
            {
                GetIPAndDNS();
            }
            SetNetworkAdapter(RandKey.ToString());

        }

        private void queryIp_Click(object sender, EventArgs e)
        {
            GetIPAndDNS();
        }
        
    }
}
