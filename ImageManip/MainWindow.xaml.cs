using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Color = System.Windows.Media.Color;

namespace ImageManip
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        
        private async Task<WriteableBitmap> ConvertToGrayScale(BitmapImage bitmapImage)
        {
            try
            {
                WriteableBitmap wb = new WriteableBitmap(bitmapImage);               // create the WritableBitmap using the source
            
                int[] grayPixels = new int[wb.PixelWidth * wb.PixelHeight];
            
                wb.CopyPixels(grayPixels, wb.PixelWidth * 4, 0);
                var size = wb.PixelWidth * wb.PixelHeight;
                // lets use the average algo 
                await Task.Run(() =>
                {
                    for (int x = 0; x < size; x++)
                    {
                
                        // get the pixel
                        int pixel = grayPixels[x];

                        // get the component
                        int red = (pixel & 0x00FF0000) >> 16;
                        int blue = (pixel & 0x0000FF00) >> 8;
                        int green = (pixel & 0x000000FF);

                        // get the average
                        int average = (byte)((red + blue + green) / 3);

                        // assign the gray values keep the alpha
                        unchecked
                        {
                            grayPixels[x] = (int)((pixel & 0xFF000000) | average << 16 | average << 8 | average);
                        }
                    } 
                });


            
                wb.WritePixels(new Int32Rect(0, 0, wb.PixelWidth, wb.PixelHeight), grayPixels, 4 * wb.PixelWidth, 0);
                // copy grayPixels back to Pixels            

                return wb;            
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
        private async void BrowseImage_OnClick(object sender, RoutedEventArgs e)
        {

            var fileOpenDialog = new OpenFileDialog()
            {
                
            };
            if (fileOpenDialog.ShowDialog() ?? false)
            {                
                var bitmapImage = new BitmapImage(new Uri(fileOpenDialog.FileName));
                var grayImage = await ConvertToGrayScale(bitmapImage);
                Image.Source = grayImage;
                
      
            }
            
        }
    }
}