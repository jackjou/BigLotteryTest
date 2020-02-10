#define ASYNC

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

public class APIConnector
{
    public String BaseURL { get; set; }

    String sURL;

    public APIConnector(String baseUrl)
    {
        BaseURL = baseUrl;
    }

    // 指令請求, 回復success / error / norespond 
    public String SendRequest(string strCommand)
    {
        /*
        sURL = BaseURL + strCommand; 

        WebRequest wrGETURL;
        wrGETURL = WebRequest.Create(sURL);

        Stream objStream;
        objStream = wrGETURL.GetResponse().GetResponseStream();
        StreamReader objReader = new StreamReader(objStream);




        var data = objReader.ReadToEnd();
        if (data == null) return "norespond";
        */

        string data = GetStringWithUrl(BaseURL, strCommand);

        try
        {
            JObject jObj = JObject.Parse(data);

            var result = (string)jObj.Descendants()
            .OfType<JProperty>()
            .Where(p => p.Name == "state")
            .First().Value;

            return result;
        }
        catch (Exception e)
        {
            LogClass logClass = new LogClass();
            logClass.Log(e.Source + "發生錯誤,原因" + e.Message);

            Newtonsoft.Json.JsonReaderException jsexp = new Newtonsoft.Json.JsonReaderException();
            StreamWriter streamWriter = new StreamWriter("LOG.txt");
            streamWriter.WriteLine(jsexp.ToString());
            streamWriter.WriteLine(data);
            streamWriter.Close();

            return null;
        }
    }

    // 指令請求, 回復success / error / norespond 
    public String SendRequestWithUrl(string Url,string strCommand)
    {
            
        string data = GetStringWithUrl(Url, strCommand);

        try
        {
            JObject jObj = JObject.Parse(data);

            var result = (string)jObj.Descendants()
            .OfType<JProperty>()
            .Where(p => p.Name == "state")
            .First().Value;

            return result;
        }
        catch (Exception e)
        {
            LogClass logClass = new LogClass();
            logClass.Log(e.Source + "發生錯誤,原因" + e.Message);

            Newtonsoft.Json.JsonReaderException jsexp = new Newtonsoft.Json.JsonReaderException();
            StreamWriter streamWriter = new StreamWriter("LOG.txt");
            streamWriter.WriteLine(jsexp.ToString());
            streamWriter.WriteLine(data);
            streamWriter.Close();

            return null;
        }
    }

    public String SendRequestWithUrl(string Url, string strCommand,string strRequest)
    {

        string data = GetStringWithUrl(Url, strCommand);

        try
        {
            JObject jObj = JObject.Parse(data);

            var result = (string)jObj.Descendants()
            .OfType<JProperty>()
            .Where(p => p.Name == strRequest)
            .First().Value;

            return result;
        }
        catch (Exception e)
        {
            LogClass logClass = new LogClass();
            logClass.Log(e.Source + "發生錯誤,原因" + e.Message);

            Newtonsoft.Json.JsonReaderException jsexp = new Newtonsoft.Json.JsonReaderException();
            StreamWriter streamWriter = new StreamWriter("LOG.txt");
            streamWriter.WriteLine(jsexp.ToString());
            streamWriter.WriteLine(data);
            streamWriter.Close();

            return null;
        }
    }

    public string SendInsertSQLwithUrl(string Url,string sql_query)
    {
        return SendRequestWithUrl(Url, "api_lottery.php?action=insertFreeList&query_str=" + sql_query,"id");
    }

    public string SendUpdateSQLwithUrl(string Url, string sql_query)
    {
        return SendRequestWithUrl(Url, "api_lottery.php?action=updateFreeList&query_str=" + sql_query);
    }

    public List<string> GetListSelectSQL(string sql_query,string columnName)
    {
        return GetListSelectSQLWithUrl(BaseURL, sql_query, columnName);
    }

    public List<string> GetListSelectSQLWithUrl(string strUrl,string sql_query,string columnName)
    {
        try
        {
            List<string> list= new List<string>();

            JObject jObject = GetJObjectSelectSQLWithUrl(strUrl, sql_query);
            if (jObject == null) return null;
            int Cnt = jObject.First.First.Count();
            if ( jObject.First.First.Count() > 0 )
            {
                for (int i = 0; i < Cnt; i++)
                {
                    list.Add(jObject["data"][i][columnName].Value<string>());
                        
                }
                return list;
            }
            return null;

        }
        catch 
        {
            return null;
        }

    }

    public JObject GetJObjectSelectSQLWithUrl(string strUrl,string sql_query)
    {
        try
        {
            JObject jObj = GetJObjectWithUrl(strUrl, "api_lottery.php?action=getFreeList&select_str=" + sql_query);

            return jObj;
        }
        catch(Exception e)
        {
            LogClass logClass = new LogClass();
            logClass.Log(e.Source + "發生錯誤,原因" + e.Message+"SQL:"+sql_query);
            return null;
        }
    }

    public JObject GetJObjectWithUrl(string strUrl,string strCommand)
    {
        string data = GetStringWithUrl(strUrl,strCommand);

        try
        {
            JObject jObj = JObject.Parse(data.ToString());
            return jObj;
        }
        catch (Exception e)
        {
            LogClass logClass = new LogClass();
            logClass.Log(e.Source + "發生錯誤,原因" + e.Message);

            Newtonsoft.Json.JsonReaderException jsexp = new Newtonsoft.Json.JsonReaderException();
            StreamWriter streamWriter = new StreamWriter("LOG.txt");
            streamWriter.WriteLine(jsexp.ToString());
            streamWriter.WriteLine(data);
            streamWriter.Close();

            return null;
        }
    }

    public String GetStringWithUrl(string strUrl,string strCommand)
    {
        LogClass log = new LogClass();

        sURL = strUrl + strCommand;

        log.Log(sURL);
#if ASYNC
        try
        {
            var task = MakeAsyncRequest(sURL, "text/html");

            return task.Result;
        }
        catch (Exception e)
        {
            LogClass logClass = new LogClass();
            logClass.Log("Exception:" + e.Message);
            logClass.Log(" API Connect GetString Error" + sURL);
            return null;
        }
        //Console.WriteLine("Got response of {0}", task.Result);
#else

        //HttpClient httpClient = new HttpClient();

        // Create a 'WebRequest' object with the specified url. 
        WebRequest myWebRequest = WebRequest.Create(sURL);

        // Send the 'WebRequest' and wait for response.
        WebResponse myWebResponse = myWebRequest.GetResponse();
        //myWebRequest.Method = WebRequestMethods.Http.Post;

        // Obtain a 'Stream' object associated with the response object.
        Stream ReceiveStream = myWebResponse.GetResponseStream();

        Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

        // Pipe the stream to a higher level stream reader with the required encoding format. 
        StreamReader objReader = new StreamReader(ReceiveStream, encode);

        /*
        sURL = BaseURL + strCommand;

        WebRequest wrGETURL;
        wrGETURL = WebRequest.Create(sURL);

        Stream objStream;
        objStream = wrGETURL.GetResponse().GetResponseStream();
        StreamReader objReader = new StreamReader(objStream);
        */
        String data = objReader.ReadToEnd();
        try
        {
                
            if (data == null || data.Length == 0 ) return null;


            while (true)
            {
                if (data[0] != '{')
                {
                    data = data.Substring(1);
                }
                else
                {
                    break;
                }
            }

            return data;
            /*
            string data2;
            if (data[0] != '{')
            {
                data2 = data.Substring(1);
            }
            else
            {
                data2 = data;
            }
            return data2;
            */
        }
        catch ( Exception e )
        {
            LogClass logClass = new LogClass();
            logClass.Log("Exception:" + e.Message );
            logClass.Log(" API Connect GetString Error"+ data);
            return null;
        }
#endif
    }

    public JObject GetJObject(string strCommand)
    {
        string data = GetStringWithUrl(BaseURL,strCommand);
            
        try
        {
            JObject jObj = JObject.Parse(data.ToString());
            return jObj;
        }
        catch (Exception e)
        {
            LogClass logClass = new LogClass();
            logClass.Log(e.Source+ "發生錯誤,原因" + e.Message);
                
            Newtonsoft.Json.JsonReaderException jsexp = new Newtonsoft.Json.JsonReaderException();
            StreamWriter streamWriter = new StreamWriter("LOG.txt");
            streamWriter.WriteLine(jsexp.ToString());
            streamWriter.WriteLine(data);
            streamWriter.Close();
                
            return null;
        }
    }

    public JObject StringConvertJObj(string strJson)
    {
        try
        {
            JObject jObj = JObject.Parse(strJson.ToString());
            return jObj;
        }
        catch (Exception e)
        {
            LogClass logClass = new LogClass();
            logClass.Log(e.Source + "發生錯誤,原因" + e.Message);

            Newtonsoft.Json.JsonReaderException jsexp = new Newtonsoft.Json.JsonReaderException();
            StreamWriter streamWriter = new StreamWriter("LOG.txt");
            streamWriter.WriteLine(jsexp.ToString());
            streamWriter.WriteLine(strJson);
            streamWriter.Close();

            return null;
        }
    }


    // Define other methods and classes here
    public static Task<string> MakeAsyncRequest(string url, string contentType)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.ContentType = contentType;
        request.Method = WebRequestMethods.Http.Post;
        request.Timeout = 20000;
        request.Proxy = null;

        Task<WebResponse> task = Task.Factory.FromAsync(
            request.BeginGetResponse,
            asyncResult => request.EndGetResponse(asyncResult),
            (object)null);

        return task.ContinueWith(t => ReadStreamFromResponse(t.Result));
    }

    private static string ReadStreamFromResponse(WebResponse response)
    {
        using (Stream responseStream = response.GetResponseStream())
        // Pipe the stream to a higher level stream reader with the required encoding format. 
        using (StreamReader objReader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8")))
        {
            String data = objReader.ReadToEnd();
            try
            {

                if (data == null || data.Length == 0) return null;


                while (true)
                {
                    if (data[0] != '{')
                    {
                        data = data.Substring(1);
                    }
                    else
                    {
                        break;
                    }
                }

                return data;
                    
            }
            catch (Exception e)
            {
                LogClass logClass = new LogClass();
                logClass.Log("Exception:" + e.Message);
                logClass.Log(" API Connect GetString Error" + data);
                return null;
            }
        }
        /*
        using (StreamReader sr = new StreamReader(responseStream))
        {
            //Need to return this response 
            string strContent = sr.ReadToEnd();
            return strContent;
        }*/
    }

        
}

