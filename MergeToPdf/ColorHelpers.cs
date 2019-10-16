using System;
using System.Collections.Generic;
using System.Text;

namespace MergeToPdf {
    public static class ColorHelpers {

        public static void WriteColor(string str, ConsoleColor fontColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black) {
            Console.ForegroundColor = fontColor;
            Console.BackgroundColor = backColor;
            Console.Write(str);
            Console.ResetColor();
        }

        public static void WriteLineColor(string str, ConsoleColor fontColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black) {
            Console.ForegroundColor = fontColor;
            Console.BackgroundColor = backColor;
            Console.WriteLine(str);
            Console.ResetColor();
        }
    }
}
