using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;


class Program
{
    static void Main(string[] args)
    {
        var chromeDriverPath = @"C:\"; // Local do chromedriver
        var downloadPath = @"C:\"; 

        var chromeOptions = new ChromeOptions();
        chromeOptions.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe"; // .exe do Chrome na versão compatível com chromedriver
        chromeOptions.AddArgument("--disable-gpu");
        chromeOptions.AddArgument("--start-maximized");
        chromeOptions.AddUserProfilePreference("download.default_directory", downloadPath);

        using (var driver = new ChromeDriver(chromeDriverPath, chromeOptions))
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // Realize o login
            Login(driver, wait);

            // Navegue para a página de download
            driver.Navigate().GoToUrl("http://34.235.5.35:8080/los-dashboard/products/expirationDate.xhtml");
            wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
            ((IJavaScriptExecutor)driver).ExecuteScript("console.clear();");
            IWebElement moreDaysInFilter = driver.FindElement(By.Id("expirationForm:expirationDataTableId:j_idt405"));
            downloadButton.Click();

            // Baixe o arquivo e mova-o para o diretório de destino
            DownloadAndMoveFile(driver, downloadPath);
        }
    }

    static void Login(ChromeDriver driver, WebDriverWait wait)
    {
        driver.Navigate().GoToUrl("http://34.235.5.35:8080/los-dashboard");

        // Aguarde os elementos e insira informações de login
        FillInputAndClick(driver, wait, "#login\\:name", "admin");
        FillInputAndClick(driver, wait, "#login\\:password", "123");
    }

    static void FillInputAndClick(ChromeDriver driver, WebDriverWait wait, string selector, string inputText)
    {
        var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(selector)));
        element.Click();
        element.SendKeys(inputText);
        element.SendKeys(Keys.Enter);
        System.Threading.Thread.Sleep(3000);
    }

    static void DownloadAndMoveFile(ChromeDriver driver, string downloadPath)
    {
        // Clique no botão de download
        IWebElement downloadButton = driver.FindElement(By.Id("expirationForm:expirationDataTableId:j_idt405"));
        downloadButton.Click();
        System.Threading.Thread.Sleep(5000);

        // Mova o arquivo baixado para o diretório de destino
        string sourceFile = Directory.GetFiles(downloadPath, "Vencimentos_*")[0];
        string targetPath = @"C:\";

        if (File.Exists(targetPath))
        {
            File.Delete(targetPath);
        }

        File.Move(sourceFile, targetPath);
    }
}
