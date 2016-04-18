using System;
using System.IO;

namespace JojoscarMVCBusinessLogic
{
    public class Logger
    {
        public static bool LogError(string error)
        {
            return LogError(error, null);
        }

        public static bool LogError(Exception exception)
        {
            return LogError("", exception);
        }

        public static bool LogError(string error, Exception exception)
        {
            string strPathName = Environment.CurrentDirectory + @"\Logs.txt";
            if (!File.Exists(strPathName))
            {
                FileStream fs = new FileStream(strPathName,
                        FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs.Close();
            }
            
            // write the error log to that text file
            return WriteErrorLog(strPathName, error, exception);
        }

        private static bool WriteErrorLog(string strPathName, string error, Exception  exception)
        {
            bool bReturn        = false;
            try
            {
                StreamWriter sw = new StreamWriter(strPathName, true);
                sw.WriteLine(DateTime.Now.ToString("YYYY-MM-dd"));
                if (error != "")
                {
                    sw.WriteLine(error);
                }
                if (exception != null)
                {
                    sw.WriteLine("Source        : " +
                            exception.Source.ToString().Trim());
                    sw.WriteLine("Method        : " +
                            exception.TargetSite.Name.ToString());
                    sw.WriteLine("Error        : " +
                        exception.Message.ToString().Trim());
                    sw.WriteLine("Stack Trace    : " +
                        exception.StackTrace.ToString().Trim());
                }
                sw.WriteLine("----------------------------------------------------------------------------");
                sw.Flush();
                sw.Close();
                bReturn    = true;
            }
            catch(Exception)
            {
                bReturn = false;
            }
            return bReturn;
        }


    }
}
