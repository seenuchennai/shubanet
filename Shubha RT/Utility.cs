using System;
using System.Collections.Generic;

using System.Text;
using System.IO;

namespace StockD
{
    public static class Utility
    {
        public static string strLog = String.Empty;

        public static void Save(string strPath, string strContent)
        {
            using (Stream stream = File.Create(strPath))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.ASCII))
            {
                writer.Write(strContent);
    
            }
        }

    }
}
