using System;
using System.IO;

namespace HikingTrailEditor
{
    public class Program
    {

        public static string DataDirectory { get; set; } = AppDomain.CurrentDomain.BaseDirectory;

        static void Main(string[] args)
        {
            TrailMenu.Run();
            
        }
    }
}
