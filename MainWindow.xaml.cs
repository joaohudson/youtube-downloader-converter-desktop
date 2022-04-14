using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Net;
using System;

namespace youtube_download_and_converter_desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TextBlock messageBlock;
        private RadioButton mp3Button, mp4Button;

        public MainWindow()
        {
            InitializeComponent();

            Background = Brushes.Black;
            
            messageBlock = new TextBlock();
            messageBlock.Width = 300;
            messageBlock.Height = 400;
            messageBlock.Foreground = Brushes.DarkTurquoise;
            messageBlock.Margin = new Thickness(0, 30, 0, 0);
            messageBlock.TextAlignment = TextAlignment.Center;
            messageBlock.FontWeight = FontWeights.Bold;

            mp3Button = BuildItemType("mp3");
            mp4Button = BuildItemType("mp4", true);

            Content = BuildMenu();
        }

        private StackPanel BuildMenu(){
            var apiClient = new ApiClient();
            var stackPanel = new StackPanel();

            var title = BuildTitle();
            stackPanel.Children.Add(title);

            var urlBox = BuildUrlBox();
            stackPanel.Children.Add(urlBox);

            var buttonPanel = new StackPanel();
            buttonPanel.HorizontalAlignment = HorizontalAlignment.Center;
            buttonPanel.Orientation = Orientation.Horizontal;

            var downloadButton = BuildDownloadButton(urlBox, apiClient);
            buttonPanel.Children.Add(downloadButton);

            buttonPanel.Children.Add(mp3Button);
            buttonPanel.Children.Add(mp4Button);

            stackPanel.Children.Add(buttonPanel);
            stackPanel.Children.Add(messageBlock);

            return stackPanel;
        }

        private TextBlock BuildTitle(){
            var title = new TextBlock();
            title.Text = "YouTube Downloader And Converter";
            title.Foreground = Brushes.DarkTurquoise;
            title.Width = 400;
            title.FontSize = 20;
            title.FontWeight = FontWeights.Bold;
            title.TextAlignment = TextAlignment.Center;
            title.Margin = new Thickness(0, 80, 0, 0);
            return title;
        }


        private TextBox BuildUrlBox(){
            var urlBox = new TextBox();
            urlBox.Width = 300;
            urlBox.Background = CustomBrushes.DarkGray;
            urlBox.Foreground = Brushes.DarkTurquoise;
            urlBox.Margin = new Thickness(0, 30, 0, 10);
            return urlBox;
        }

        private Button BuildDownloadButton(TextBox urlBox, ApiClient apiClient){
            var type = mp3Button.IsChecked ?? false ? "mp3" : "mp4";
            var downloadButton = new Button();
            downloadButton.Content = "Download";
            downloadButton.Background = CustomBrushes.DarkGray;
            downloadButton.Foreground = Brushes.DarkTurquoise;
            downloadButton.Width = 80;
            downloadButton.Click += (r, e) => DownloadVideo(urlBox, apiClient, downloadButton);
            return downloadButton;
        }

        private RadioButton BuildItemType(string name, bool check = false){
            var item = new RadioButton();
            item.Content = name;
            item.Background = Brushes.DarkTurquoise;
            item.Foreground = Brushes.DarkTurquoise;
            item.BorderThickness = new Thickness(3);
            item.BorderBrush = CustomBrushes.DarkGray;
            item.Margin = new Thickness(10, 10, 0, 0);
            item.GroupName = "DownloadType";
            item.IsChecked = check;
            return item;
        }

        private async void DownloadVideo(TextBox urlBox, ApiClient apiClient, Button downloadButton){
            messageBlock.Text = "Baixando . . .";
            downloadButton.IsEnabled = false;
            string type = mp3Button.IsChecked ?? false ? "mp3" : "mp4";
            try{
                var name = await apiClient.GetVideoName(urlBox.Text);
                var videoData = await apiClient.GetVideo(urlBox.Text, type);
                var videosPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                var path = Path.Combine(videosPath, name + '.' + type);
                using(var file = File.Open(path, FileMode.Create)){
                    videoData.CopyTo(file);
                }
                messageBlock.Text = "Download concluído!";
            }
            catch(WebException e){
                Console.WriteLine(e);
                messageBlock.Text = e.Message;
            }
            catch(Exception e){
                Console.WriteLine(e);
                messageBlock.Text = "Erro ao salvar o video!";
            }
            finally{
                downloadButton.IsEnabled = true;
            }
        }
    }
}
