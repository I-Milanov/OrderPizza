using System.Text.RegularExpressions;
using OpenCvSharp;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Point = OpenCvSharp.Point;

namespace DominosOrderTest
{
    public static class WebDriverExtensions
    {
        public static ImageWebElement FindElementByImage(this IWebDriver driver, string imagePath, double threshold = 0.8)
        {
            if (!File.Exists(imagePath))
                throw new FileNotFoundException($"Image not found at path: {imagePath}");

            var templateBytes = File.ReadAllBytes(imagePath);
            return driver.FindElementByImage(templateBytes, threshold);
        }

        public static ImageWebElement FindElementByImage64(this IWebDriver driver, string base64Image, double threshold = 0.8)
        {
            if (base64Image.Contains("base64,"))
            {
                base64Image = Regex.Match(base64Image, @"base64,(.*)").Groups[1].Value;
            }

            var templateBytes = Convert.FromBase64String(base64Image);
            return driver.FindElementByImage(templateBytes, threshold);
        }

        public static ImageWebElement FindElementByImage(this IWebDriver driver, byte[] imageBytes, double threshold = 0.8)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            int x = 0, y = 0, width = 0, height = 0;

            wait.Until(d =>
            {
                (x, y, width, height) = FindImageOnScreen(d, imageBytes, threshold);

                return x == -1 ? false : true;
            });

            CheckThatElementIsOnTheScreen(x, y);

            ImageWebElement element = GetElement(driver, x, y, width, height);

            return element;
        }

        private static ImageWebElement GetElement(IWebDriver driver, int x, int y, int width, int height)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            IWebElement element = (IWebElement)js.ExecuteScript("return document.elementFromPoint(arguments[0], arguments[1]);", x + width / 2, y + height / 2);

            if (element == null)
            {
                throw new NoSuchElementException("Cannot find element at calculated position.");
            }

            return new ImageWebElement(driver, element, x, y, width, height);

        }

        private static void CheckThatElementIsOnTheScreen(int x, int y)
        {
            if (x < 0 || y < 0)
            {
                throw new NoSuchElementException("Element matching the image was not found on the page.");
            }
        }

        private static (int x, int y, int width, int height) FindImageOnScreen(IWebDriver driver, byte[] templateBytes, double threshold)
        {
            var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            var screenshotBytes = screenshot.AsByteArray;

            using var templateMat = Mat.FromImageData(templateBytes, ImreadModes.Color);
            using var sourceMat = Mat.FromImageData(screenshotBytes, ImreadModes.Color);
            using var result = new Mat();

            Cv2.MatchTemplate(sourceMat, templateMat, result, TemplateMatchModes.CCoeffNormed);

            Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out Point maxLoc);

            if (maxVal >= threshold)
            {
                return (maxLoc.X, maxLoc.Y, templateMat.Width, templateMat.Height);
            }

            return (-1, -1, 0, 0);
        }
    }
}
