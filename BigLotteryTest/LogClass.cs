using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


public class LogClass
{
    object lockLog = new object();
    //Write Log 
    public void Log(string txt)
    {
        //今日日期
        DateTime Date = DateTime.Now;
        string TodyTime = Date.ToString("yyyy_MM_dd HH_mm_ss");
        string Tody = Date.ToString("yyyy_MM_dd");

        //檢查此路徑有無資料夾
        if (!Directory.Exists("Log"))
        {
            //新增資料夾
            Directory.CreateDirectory("Log");
        }
        lock (lockLog)
        {
            //把內容寫到目的檔案，若檔案存在則附加在原本內容之後(換行)
            File.AppendAllText("Log\\" + Tody + ".txt", "\r\n" + TodyTime + "_" + txt);
        }
    }
}

