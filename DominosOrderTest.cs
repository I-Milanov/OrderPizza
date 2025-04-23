using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace DominosOrderTest
{
    [TestFixture]
    public class OrderButtonTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        private string email = Environment.GetEnvironmentVariable("dominos_fake_mail");
        private string password = Environment.GetEnvironmentVariable("dominos_fake_password");

        public ImageWebElement OrderNowButton => driver.FindElementByImage(CreatePath("OrderButton.png"));
        public ImageWebElement NoThanksButton => driver.FindElementByImage(CreatePath("NoThanksButton.png"));
        public ImageWebElement AcceptCookiesButton => driver.FindElementByImage(CreatePath("AcceptCookiesButton.png"));
        public SelectElement ChooseARestorantButton => new SelectElement(wait.Until(ExpectedConditions.ElementExists(By.XPath("//select[@name='store_id']"))));
        public ImageWebElement EmailInput => driver.FindElementByImage(CreatePath("EmailInput.png"));
        public ImageWebElement OrderButton => driver.FindElementByImage(CreatePath("OrderButton.png"));
        public ImageWebElement OrderNowPopUpButton => driver.FindElementByImage(CreatePath("OrderNowPopUpButton.png"));
        public ImageWebElement PasswordInput => driver.FindElementByImage(CreatePath("PasswordInput.png"));
        public ImageWebElement PeperoniPizza => driver.FindElementByImage(CreatePath("PeperoniPizza.png"));
        public ImageWebElement PizzaButton => driver.FindElementByImage(CreatePath("PizzaButton.png"));
        public ImageWebElement TakeForHomeButton => driver.FindElementByImage(CreatePath("TakeForHomeButton.png"));
        public ImageWebElement AddPizzaToCartButton => driver.FindElementByImage(CreatePath("AddPizzaToCartButton.png"));
        public ImageWebElement PernikPlazzaRestorant => driver.FindElementByImage(CreatePath("PernikPlazza.png"));
        public ImageWebElement NoThanksRedButton => driver.FindElementByImage(CreatePath("NoThanksRedButton.png"));
        public IWebElement CartNum => wait.Until(d => d.FindElement(By.CssSelector("div.basket span.cart-num span")));

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            driver.Navigate().GoToUrl("https://www.dominos.bg/");
        }

        [Test]
        public void CartNumberIncrease_When_AddPeperoniPizza()
        {
            NoThanksButton.Click();
            AcceptCookiesButton.Click();
            OrderNowButton.Click();
            EmailInput.SendKeys(email);
            PasswordInput.SendKeys(password + Keys.Enter);
            TakeForHomeButton.Click();
            ChooseARestorantButton.SelectByText("Бургас - Сентрал Парк ( 10′ )");
            //ToDo Should be replaced with WaitForAjax in a real project
            Thread.Sleep(1000);
            OrderNowPopUpButton.Click();
            PizzaButton.Click();
            PeperoniPizza.Click();
            //ToDo Should be replaced with WaitForAjax in a real project
            Thread.Sleep(1000);
            AddPizzaToCartButton.Click();
            NoThanksRedButton.Click();

            Assert.That(CartNum.Text, Is.EqualTo("1"));
        }

        [TearDown]
        public void Cleanup()
        {
            driver?.Dispose();
        }

        private string CreatePath(string image)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", image);
        }
    }
}