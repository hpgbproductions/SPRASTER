using System;
using System.IO;
using System.Drawing;

namespace Rastermatic
{
    class Program
    {
        static void Main(string[] args)
        {
            string InputFilepath;
            Bitmap InputBitmap;

            string OutputFilename;
            string OutputFilepath;
            string OutputContent;

            Size BitmapSize;
            Color ColorPrevious = Color.Empty;
            Color ColorCurrent = Color.Empty;

            Console.WriteLine("SP Rastermatic");
            Console.WriteLine("Copyright (C) 2021 hpgbproductions");
            Console.WriteLine("Released under GNU General Public License V3.0");
            Console.WriteLine();
            Console.WriteLine("Enter image file path>");
            InputFilepath = Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine("Opening image...");

            // Load image file
            try
            {
                InputBitmap = new Bitmap(Image.FromFile(InputFilepath));
            }
            catch (Exception ex)
            {
                FailExport(ex, "an error occured when importing the specified file");
                return;
            }

            // Write export contents
            try
            {
                // Write the start of the sub-assembly file
                OutputContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<DesignerParts>
  <DesignerPart name = """;
                OutputContent += Path.GetFileName(InputFilepath);
                OutputContent += @""" category=""Sub Assemblies"" icon=""GroupIconSubAssembly"" description = """">
    <Assembly>
      <Parts>
        <Part id=""2"" partType=""Label-1"" position=""0,0,0"" rotation=""270,0,0"" drag=""0,0,0,0,0,0"" materials=""0,1,2"" scale=""1,1,1"" partCollisionResponse=""Default"" calculateDrag=""false"">
           <Label.State designText = ""&lt;mspace=0.47em&gt;&lt;line-height=0.47em&gt;";

                BitmapSize = InputBitmap.Size;
                Console.WriteLine($"Image resolution: {BitmapSize.Width}x{BitmapSize.Height}");
                Console.WriteLine();

                for (int y = 0; y < BitmapSize.Height; y++)
                {
                    Console.WriteLine($"Writing data for line {y + 1} of {BitmapSize.Height}");
                    for (int x = 0; x < BitmapSize.Width; x++)
                    {
                        // Write data for a pixel
                        ColorCurrent = InputBitmap.GetPixel(x, y);
                        if (ColorCurrent != ColorPrevious)
                        {
                            OutputContent += string.Format("&lt;color=#{0:X2}{1:X2}{2:X2}{3:X2}&gt;",
                                ColorCurrent.R, ColorCurrent.G, ColorCurrent.B, ColorCurrent.A);
                        }
                        OutputContent += "■";

                        ColorPrevious = ColorCurrent;
                    }

                    if (y < BitmapSize.Height - 1)
                    {
                        OutputContent += "&lt;br&gt;";
                    }
                }

                OutputContent += @$"&lt;/mspace&gt;"" fontName=""Default"" fontSize=""1"" horizontalAlignment=""Center"" verticalAlignment=""Middle"" width=""{BitmapSize.Width / 20f}"" height=""{BitmapSize.Height / 20f}"" outlineWidth=""0"" emission=""0"" offset=""0,0.006,0"" rotation=""90,0,0"" gradient=""None"" curvature=""0"" />
        </Part>
      </Parts>
      <Connections/>
    </Assembly>
  </DesignerPart>
</DesignerParts>";
            }
            catch (Exception ex)
            {
                FailExport(ex, "an error occured when writing export contents");
                return;
            }

            // Write the export contents to a file
            try
            {
                OutputFilename = Path.GetFileName(InputFilepath) + ".xml";
                OutputFilepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), OutputFilename);
                StreamWriter writer = File.CreateText(OutputFilepath);
                writer.Write(OutputContent);
                writer.Close();
                writer.Dispose();
            }
            catch (Exception ex)
            {
                FailExport(ex, "an error occured when exporting to file");
                return;
            }

            Console.WriteLine("\nSuccessfully exported data onto desktop: " + OutputFilepath);
            Console.WriteLine("Press ENTER to quit.");
            Console.ReadLine();
        }

        static void FailExport(Exception ex, string message)
        {
            Console.WriteLine("\nExport failed - " + message);
            Console.WriteLine(ex);
            Console.WriteLine("\nPress ENTER to quit.");
            Console.ReadLine();
        }
    }
}
