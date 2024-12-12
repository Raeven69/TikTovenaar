using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace TikTovenaar
{
    public class FireAnimation
    {
        private readonly ImageBrush _imageBrush;
        private readonly DispatcherTimer _animationTimer;
        private int _currentFrame;

        private string _currentImagePath;
        private double _currentFrameWidth;
        private int _totalFrames;

        private string _startImagePath;
        private double _startImageFrameWidth;
        private int _startImageTotalFrames;

        private string _loopImagePath;
        private double _loopImageFrameWidth;
        private int _loopImageTotalFrames;

        private string _endImagePath;
        private double _endImageFrameWidth;
        private int _endImageTotalFrames;

        private Action _onAnimationComplete;

        public Boolean fireActive = false;

        public FireAnimation(
            ImageBrush imageBrush,
            string startImagePath, double startImageFrameWidth, int startImageTotalFrames,
            string loopImagePath, double loopImageFrameWidth, int loopImageTotalFrames,
            string endImagePath, double endImageFrameWidth, int endImageTotalFrames)
        {
            _imageBrush = imageBrush;
            _imageBrush.Opacity = 0;

            _startImagePath = startImagePath;
            _startImageFrameWidth = startImageFrameWidth;
            _startImageTotalFrames = startImageTotalFrames;

            _loopImagePath = loopImagePath;
            _loopImageFrameWidth = loopImageFrameWidth;
            _loopImageTotalFrames = loopImageTotalFrames;

            _endImagePath = endImagePath;
            _endImageFrameWidth = endImageFrameWidth;
            _endImageTotalFrames = endImageTotalFrames;

            _animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _animationTimer.Tick += AnimateFrame;
        }

        private void AnimateFrame(object sender, EventArgs e)
        {
            if (_currentFrame >= _totalFrames)
            {
                _animationTimer.Stop();
                _onAnimationComplete?.Invoke();
                return;
            }

            var offset = _currentFrame * _currentFrameWidth;
            _imageBrush.Viewbox = new Rect(offset, 0, _currentFrameWidth, 1);
            _currentFrame++;
        }

        public void StartFire()
        {
            _imageBrush.Opacity = 1;
            PlayAnimation(_startImagePath, _startImageFrameWidth, _startImageTotalFrames, () =>
            {
                LoopFire();
            });
            fireActive = true;
        }

        private void LoopFire()
        {
            PlayAnimation(_loopImagePath, _loopImageFrameWidth, _loopImageTotalFrames, null, true);
        }

        public void EndFire()
        {
            PlayAnimation(_endImagePath, _endImageFrameWidth, _endImageTotalFrames, () => _imageBrush.Opacity = 0);
            fireActive = false;
        }

        private void PlayAnimation(string imagePath, double frameWidth, int totalFrames, Action onComplete, bool loop = false)
        {
            _currentImagePath = imagePath;
            _currentFrameWidth = frameWidth;
            _totalFrames = totalFrames;
            _currentFrame = 0;
            _onAnimationComplete = loop ? () => LoopFire() : onComplete;

            _imageBrush.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/{imagePath}", UriKind.Absolute));
            _animationTimer.Start();
        }
    }
}
