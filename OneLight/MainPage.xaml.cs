using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍

namespace OneLight
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region 私有字段
        static MediaCapture mediacapture = null;
        static bool m_istorchOpened = false;
        static bool m_iscaptureCreated = false;
        #endregion

        #region 属性
        public static bool IsTorchOpened
        {
            get { return m_istorchOpened; }
        }
        public static bool IsCaptureCreated
        {
            get { return m_iscaptureCreated; }
        }
        #endregion
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.CreateCaptureAsync();
            CleanCaptureAsync();
        }
        #region 方法
        private async void Initialmediacapture(DeviceInformation backCapture)//初始化Capture
        {
            mediacapture = new MediaCapture();
            MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings();
            settings.VideoDeviceId = backCapture.Id;
            settings.StreamingCaptureMode = StreamingCaptureMode.Video;
            settings.PhotoCaptureSource = PhotoCaptureSource.Auto;
            await mediacapture.InitializeAsync(settings);
            m_iscaptureCreated = true;
        }
        private async Task<DeviceInformation[]> GetCaptureDeviceAsync()//获取视频设备信息
        {
            var dvs = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            return dvs.ToArray();
        }
        public async void CreateCaptureAsync()//初始化后盖摄像头
        {
            DeviceInformation backCapture = (from d in await GetCaptureDeviceAsync()
                                             where d.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back
                                             select d).FirstOrDefault();
            if(backCapture != null)
            {
                this.Initialmediacapture(backCapture);
            }
        }
        public void CleanCaptureAsync()//清理mediacapture对象
        {
            if (mediacapture != null)
            {
                mediacapture.Dispose();
                mediacapture = null;
                m_iscaptureCreated = false;
            }
        }
        public void Open()//开启闪光灯
        {
            var vdcontrol = mediacapture.VideoDeviceController.TorchControl;
            if (vdcontrol.Supported)
            {
                vdcontrol.Enabled = true;
                m_istorchOpened = true;
            }
        }
        public void CloseTorch()//关闭闪光灯
        {
            var torch = mediacapture.VideoDeviceController.TorchControl;
            torch.Enabled = false;
            m_istorchOpened = false;
        }
        #endregion

        private void One_Click(object sender, RoutedEventArgs e)
        {
            if (!mediacapture.VideoDeviceController.TorchControl.Enabled)
            {
                Open();
                One.Background =new SolidColorBrush(Colors.White);
            }
            else
            {
                CloseTorch();
                CleanCaptureAsync();
                this.CreateCaptureAsync();
                One.Background = new SolidColorBrush(Colors.Black);
            }

        }


    }
}
