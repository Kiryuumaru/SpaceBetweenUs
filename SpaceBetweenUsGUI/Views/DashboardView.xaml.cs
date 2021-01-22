using SpaceBetweenUsGUI.ImgProc;
using System;
using System.Threading;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SpaceBetweenUsGUI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardView : Page
    {
        private readonly FrameRenderer previewRenderer = null;
        private readonly FrameRenderer outputRenderer = null;

        private readonly DispatcherTimer fpsTimer = null;
        private int frameCount = 0;

        public DashboardView()
        {
            InitializeComponent();

            previewRenderer = new FrameRenderer(PreviewImage);
            outputRenderer = new FrameRenderer(OutputImage);

            fpsTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            fpsTimer.Tick += UpdateFPS;

            OpenFile();
        }

        private void UpdateFPS(object sender, object e)
        {
            var frameCount = Interlocked.Exchange(ref this.frameCount, 0);
            FPSMonitor.Text = "FPS: " + frameCount;
        }

        private async void OpenFile()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".mkv");
            openPicker.FileTypeFilter.Add(".avi");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                MediaPlayer mediaPlayer = new MediaPlayer();
                mediaPlayer.AutoPlay = false;
                mediaPlayer.Source = MediaSource.CreateFromStorageFile(file);
                mediaPlayer.MediaFailed += MediaPlayer_MediaFailed;
                mediaPlayer.VideoFrameAvailable += MediaPlayer_VideoFrameAvailable;
                mediaPlayer.IsVideoFrameServerEnabled = true;
                mediaPlayer.Play();
            }
        }

        private void MediaPlayer_MediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {

        }

        private void MediaPlayer_VideoFrameAvailable(MediaPlayer sender, object args)
        {
            var s = new VideoFrame();
            sender.CopyFrameToVideoSurface()
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            fpsTimer.Start();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            fpsTimer.Stop();
        }
    }
}
