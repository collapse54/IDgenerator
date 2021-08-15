using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using VirtualIDgenerator_MVC.Models;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;                  // for BarcodeWriter
using ZXing.QrCode;           // for QRCode Engine

namespace VirtualIDgenerator_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        //產生虛擬身分證
        #region
        /*    身分證規則說明：
　　　     (1)英文轉成的數字, 個位數乘９再加上十位數 
　   　　  (2)各數字從右到左依次乘１、２、３、４．．．．８ 
　　  　   (3)求出(1),(2)之和 
　　　     (4)求出(3)除10後之餘數,用10減該餘數,結果就是檢查碼,若餘數為0 
　　　　 檢查碼就是0 */
        public string VirtualID(bool sex, int CityIndex)
        {  
            //建立首字字母對應的城市
            string[] firstletter = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            
            //對應的鄉鎮ID
            int[] county_id = { 10, 11, 12, 13, 14, 15, 16, 17, 34, 18, 19, 20, 21, 22, 35, 23, 24, 25, 26, 27, 28, 29, 32, 30, 31, 33 };

           
            string ID = firstletter[CityIndex];

            //鄉鎮ID的十位數和個位數 以進行檢查運算
            int countyID = county_id[CityIndex], Tens=countyID/10, Digits=countyID-Tens*10;

            //建立性別
            int sexID = 2;
            if (sex) { sexID = 1; }
            
            //隨機產生剩餘8位數
            Random r = new Random();

            int randomID = r.Next(0, 10000000);

            //計算 得出符合條件的身分證號
            int checking = Tens + Digits * 9 + sexID * 8;

            for(int i = 7; i >= 1; i--)
            {
                checking += ((randomID / (int)Math.Pow(10, i - 1)) % -10) % 10 * i;
            }

            checking = (10 - (checking % 10)) % 10;

            //計算檢號
            ID += sexID.ToString() + randomID.ToString().PadLeft(7, '0') + checking.ToString();

            return ID;
        }
        #endregion


        //沒用的東西
        #region
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        #endregion

        //功能頁
        public IActionResult VirtualIDGenerator()
        {
            //身分證號條件
            Random r = new Random();

            bool sex = r.Next()<0.5;
            int county = r.Next(0, 25);

            ViewBag.ID = VirtualID(sex, county);
            ViewBag.Time = DateTime.Now.ToString();

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
