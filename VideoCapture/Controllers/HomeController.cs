using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using AForge.Video.DirectShow;
using VideoCapture.Models;
using FilterInfo = AForge.Video.DirectShow.FilterInfo;

namespace VideoCapture.Controllers
{
    public class HomeController : Controller
    {
        private static VideoCaptureDevice videoSource;
        //
        // GET: /Home/

        public ActionResult Index()
        {
            //Получаем список видеокамер.
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            ViewBag.DeviceList = videoDevices.Cast<FilterInfo>()
                .Select(x => new SelectListItem { Text = x.Name,Value = x.MonikerString}).ToList();
            ViewBag.IsRunning = videoSource != null && videoSource.IsRunning;
            return View();
        }

        public ActionResult Start(string deviceName)
        {
                videoSource=new VideoCaptureDevice(deviceName);
                videoSource.NewFrame += VideoSourceNewFrame;
                videoSource.Start();
            return RedirectToAction("Index");
        }

        public ActionResult Stop()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.Stop();
                videoSource = null;
            }

            return RedirectToAction("Index");
        }

        private static byte[] _bufImage=new byte[0];

public ActionResult Video()
{
    if (videoSource == null || !videoSource.IsRunning)
    {
        return null;
    }
    Response.Clear();
    //Устанавливает тип рередаваемых данных и разделитель кадров
    Response.ContentType = "multipart/x-mixed-replace; boundary=--myboundary";
    //Отключаем кеширование
    Response.Expires = 0;
    Response.Cache.SetCacheability(HttpCacheability.NoCache);

    var ae = new ASCIIEncoding();
    //Передаем поток пока клиент не отключится
    while (Response.IsClientConnected)
    {
        try
        {
            //_bufImage - переменная, в которой хранится новый кадр в формате jpeg
            var buf = _bufImage;
            //Формируем заголовок разделителя
            var boundary = ae.GetBytes("\r\n--myboundary\r\nContent-Type: image/jpeg\r\nContent-Length:"
                                        + buf.Length + "\r\n\r\n");
            Response.OutputStream.Write(boundary, 0, boundary.Length);
            Response.OutputStream.Write(buf, 0, buf.Length);
            Response.Flush();
            Thread.Sleep(50);

        }
        catch (Exception)
        {

        }

    }
    Response.End();



    return null;
}

static void VideoSourceNewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
{
    var img = (Image) eventArgs.Frame;
    using (var ms=new MemoryStream())
    {
        img.Save(ms,ImageFormat.Jpeg);
        _bufImage = ms.ToArray();
    }
}

    }
}
