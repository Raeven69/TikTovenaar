using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using System;
using System.Windows;

namespace TikTovenaar
{
    public class WizardAnimation
    {
        private readonly ImageBrush _imageBrush;
        private readonly DispatcherTimer _animationTimer;
        private int _currentFrame = 0;
        private double _frameWidth;
        private int _totalFrames;
        private readonly string _idleImagePath;
        private double _idleFrameWidth;
        private int _idleTotalFrames;

        public WizardAnimation(ImageBrush imageBrush, double idleFrameWidth, int idleTotalFrames, string idleImagePath = "Images/wizard_idle.png")
        {
            _imageBrush = imageBrush;
            _idleImagePath = idleImagePath;
            _idleFrameWidth = idleFrameWidth;
            _idleTotalFrames = idleTotalFrames;

            // Set up the animation timer
            _animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _animationTimer.Tick += AnimationTimer_Tick;

            // Start idle animation
            StartIdleAnimation();
        }

        private void StartIdleAnimation()
        {
            StartAnimation(_idleFrameWidth, _idleTotalFrames, _idleImagePath);
        }

        public void StartAnimation(double frameWidth, int totalFrames, string imagePath)
        {
            _frameWidth = frameWidth;
            _totalFrames = totalFrames;

            // Set the start image for the animation
            _imageBrush.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/{imagePath}", UriKind.Absolute));
            _imageBrush.Viewbox = new Rect(0, 0, _frameWidth, 1);

            _currentFrame = 0; // Reset to the first frame
            _animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Update the Viewbox to show the next frame
            double frameStart = _currentFrame * _frameWidth;
            _imageBrush.Viewbox = new Rect(frameStart, 0, _frameWidth, 1);

            // Move to next frame
            _currentFrame = (_currentFrame + 1) % _totalFrames;
        }

        public void UpdateWizard(string imagePath, double frameWidth, int totalFrames, bool resetToIdle = true)
        {
            _animationTimer.Stop(); // Pause the current animation before switching frames
            StartAnimation(frameWidth, totalFrames, imagePath);

            if (resetToIdle)
            {
                // Start a temporary timer to reset to idle after one animation loop
                DispatcherTimer resetTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(100 * totalFrames) // Duration for one full loop
                };
                resetTimer.Tick += (s, e) =>
                {
                    resetTimer.Stop();
                    StartIdleAnimation(); // Restart idle animation after one loop
                };
                resetTimer.Start();
            }
        }
    }
}
