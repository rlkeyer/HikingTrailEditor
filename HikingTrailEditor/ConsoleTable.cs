using System;
using System.Collections.Generic;
using System.Text;

namespace HikingTrailEditor
{
    public class ConsoleTable
    {
        private int _TableWidth = 77;

        public ConsoleTable(int tableWidth)
        {
            _TableWidth = tableWidth;
        }

        public void PrintLine()
        {
            Console.Write(" ");
            Console.WriteLine(new string('-', 165));
        }

        public void PrintRow(params string[] columns)
        {
            //int width = (_TableWidth - columns.Length) / columns.Length;
            string row = "|";

            for (int i = 0; i < columns.Length; i++)
            {
                string column = columns[i];
                int width = 0;

                switch (i)
                {
                    case 0:
                        width = 6;
                        break;
                    case 1:
                        width = 45;
                        break;
                    case 2:
                        width = 25;
                        break;
                    case 3:
                        width = 10;
                        break;
                    case 4:
                        width = 75;
                        break;
                    default:
                        break;
                }

                row += AlignCenter(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        private string AlignCenter(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }

    }
}

