using System.Collections.ObjectModel;
using System.Drawing;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace DominosOrderTest
{
    public class ImageWebElement : IWebElement
    {
        private readonly IWebDriver _driver;
        private readonly IWebElement _wrappedElement;
        private readonly int _x;
        private readonly int _y;
        private readonly int _width;
        private readonly int _height;

        public ImageWebElement(IWebDriver driver, IWebElement wrappedElement, int x, int y, int width, int height)
        {
            _driver = driver;
            _wrappedElement = wrappedElement;
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public string TagName => _wrappedElement.TagName;
        public string Text => _wrappedElement.Text;
        public bool Enabled => _wrappedElement.Enabled;
        public bool Selected => _wrappedElement.Selected;
        public Point Location => _wrappedElement.Location;
        public Size Size => _wrappedElement.Size;
        public bool Displayed => _wrappedElement.Displayed;
        Point IWebElement.Location => _wrappedElement.Location;
        Size IWebElement.Size => _wrappedElement.Size;

        public void Clear()
        {
            _wrappedElement.Clear();
        }

        public void Click()
        {
            _wrappedElement.Click();
            //ToDo Appropriate wait should be applied instead
            Thread.Sleep(1000);
        }

        public IWebElement FindElement(By by)
        {
            return _wrappedElement.FindElement(by);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return _wrappedElement.FindElements(by);
        }

        public string GetAttribute(string attributeName) => _wrappedElement.GetAttribute(attributeName);

        public string GetCssValue(string propertyName) => _wrappedElement.GetCssValue(propertyName);

        public string GetDomAttribute(string attributeName) => _wrappedElement.GetDomAttribute(attributeName);

        public string GetDomProperty(string propertyName) => _wrappedElement.GetDomProperty(propertyName);

        public void SendKeys(string text)
        {
            ScrollIntoView();
            _wrappedElement.SendKeys(text);
        }

        public void Submit()
        {
            ScrollIntoView();
            _wrappedElement.Submit();
        }

        public void ScrollIntoView()
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript($"window.scrollTo(0, {Math.Max(0, _y - 100)});");
            System.Threading.Thread.Sleep(500);
        }

        public void ClickWithActions()
        {
            ScrollIntoView();
            Actions actions = new Actions(_driver);
            actions.MoveToElement(_wrappedElement).Click().Perform();
        }

        public void ClickWithJavaScript()
        {
            ScrollIntoView();
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("arguments[0].click();", _wrappedElement);
        }

        public void ClickDirectlyAtPosition()
        {
            ScrollIntoView();
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("document.elementFromPoint(arguments[0], arguments[1]).click();",
                _x + _width / 2, _y + _height / 2);
        }

        public Point GetCenterPoint()
        {
            return new Point(_x + _width / 2, _y + _height / 2);
        }

        public override string ToString()
        {
            return $"ImageWebElement at position ({_x}, {_y}) with size ({_width}, {_height})";
        }

        public ISearchContext GetShadowRoot()
        {
            return _wrappedElement.GetShadowRoot();
        }
    }
}
