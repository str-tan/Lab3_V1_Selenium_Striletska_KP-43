using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;


namespace Lab3_Tests
{
    public class Variant1Tests
    {
        EdgeDriver driver;
        private const string BaseUrl = "https://the-internet.herokuapp.com/";

        [SetUp]
        public void Setup()
        {
            driver = new EdgeDriver();
            driver.Manage().Window.Maximize();
        }

        // Допоміжний метод для надійного очікування алерту
        private IAlert WaitForAlert(int timeoutSeconds = 5)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(d =>
            {
                try
                {
                    return d.SwitchTo().Alert();
                }
                catch (NoAlertPresentException)
                {
                    return null;
                }
            });
        }

        [Test]
        public void Test_01_AddRemoveElements()
        {
            // Відкриваємо сторінку для додавання та видалення елементів
            driver.Navigate().GoToUrl(BaseUrl + "add_remove_elements/");

            // Знаходимо кнопку додавання елемента та натискаємо її
            var addButton = driver.FindElement(By.XPath("//button[text()='Add Element']"));
            addButton.Click();

            // Отримуємо створену кнопку видалення та переконуємося, що вона відображається
            var deleteButton = driver.FindElement(By.ClassName("added-manually"));
            Assert.That(deleteButton.Displayed, Is.True, "Кнопка Delete не з'явилася!");

            // Видаляємо доданий елемент
            deleteButton.Click();

            // Перевіряємо, що кнопка більше не присутня на сторінці
            var deleteButtons = driver.FindElements(By.ClassName("added-manually"));
            Assert.That(deleteButtons, Is.Empty, "Кнопка Delete не зникла після кліку!");
        }

        [Test]
        public void Test_02_Checkboxes()
        {
            // Відкриваємо сторінку з прапорцями
            driver.Navigate().GoToUrl(BaseUrl + "checkboxes");

            // Отримуємо список усіх чекбоксів
            var checkboxes = driver.FindElements(By.CssSelector("input[type='checkbox']"));

            // Перевіряємо початковий стан першого чекбокса та встановлюємо прапорець
            var checkbox1 = checkboxes[0];
            Assert.That(checkbox1.Selected, Is.False, "Перший чекбокс має бути НЕ вибраний спочатку.");
            checkbox1.Click();
            Assert.That(checkbox1.Selected, Is.True, "Перший чекбокс не вибрався після кліку!");

            // Перевіряємо початковий стан другого чекбокса та знімаємо прапорець
            var checkbox2 = checkboxes[1];
            Assert.That(checkbox2.Selected, Is.True, "Другий чекбокс має бути вибраний спочатку.");
            checkbox2.Click();
            Assert.That(checkbox2.Selected, Is.False, "З другого чекбоксу не знялося виділення після кліку!");
        }

        [Test]
        public void Test_03_Dropdown()
        {
            // Переходимо до сторінки з випадаючим списком
            driver.Navigate().GoToUrl(BaseUrl + "dropdown");

            // Ініціалізуємо об'єкт для роботи з випадаючим списком
            var dropdownElement = driver.FindElement(By.Id("dropdown"));
            var select = new SelectElement(dropdownElement);

            // Вибираємо перший варіант та перевіряємо його
            select.SelectByText("Option 1");
            Assert.That(select.SelectedOption.Text, Is.EqualTo("Option 1"), "Option 1 не обрано!");

            // Вибираємо другий варіант за значенням атрибута
            select.SelectByValue("2");
            Assert.That(select.SelectedOption.Text, Is.EqualTo("Option 2"), "Option 2 не обрано!");
        }

        [Test]
        public void Test_04_Inputs()
        {
            // Відкриваємо сторінку з полем для введення чисел
            driver.Navigate().GoToUrl(BaseUrl + "inputs");
            var inputField = driver.FindElement(By.CssSelector("input[type='number']"));

            // Очищаємо поле та вводимо потрібне число
            inputField.Clear();
            inputField.SendKeys("63");
            Assert.That(inputField.GetAttribute("value"), Is.EqualTo("63"), "Значення в полі не співпадає з введеним!");

            // Збільшуємо значення за допомогою клавіші зі стрілкою вгору
            inputField.SendKeys(Keys.ArrowUp);
            Assert.That(inputField.GetAttribute("value"), Is.EqualTo("64"), "Значення не збільшилось після натискання стрілки вгору!");
        }

        [Test]
        public void Test_05_StatusCodes()
        {
            // Переходимо на сторінку з HTTP-статусами
            driver.Navigate().GoToUrl(BaseUrl + "status_codes");

            // Відкриваємо сторінку зі статусом 404
            driver.FindElement(By.LinkText("404")).Click();

            // Отримуємо текст інформаційного повідомлення та перевіряємо його наявність
            var content = driver.FindElement(By.CssSelector("div.example p")).Text;
            Assert.That(content, Does.Contain("This page returned a 404 status code"), "Неправильний текст після переходу на статус 404!");
        }

        [Test]
        public void Test_06_DynamicControls()
        {
            // Відкриваємо сторінку з динамічними елементами
            driver.Navigate().GoToUrl(BaseUrl + "dynamic_controls");

            // Натискаємо кнопку видалення чекбокса
            driver.FindElement(By.CssSelector("#checkbox-example button")).Click();

            // Створюємо очікування для завантаження результату операції
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var messageElement = wait.Until(d => d.FindElement(By.Id("message")));

            // Перевіряємо текст та переконуємося, що чекбокс більше не відображається в DOM
            Assert.That(messageElement.Text, Is.EqualTo("It's gone!"), "Повідомлення про видалення не з'явилося!");
            Assert.That(driver.FindElements(By.Id("checkbox")), Is.Empty, "Чекбокс досі залишився на сторінці!");
        }

        [Test]
        public void Test_07_FileUpload()
        {
            // Переходимо до сторінки завантаження файлів
            driver.Navigate().GoToUrl(BaseUrl + "upload");

            // Створюємо тимчасовий файл локально
            string fileName = "test_upload.txt";
            string filePath = Path.GetFullPath(fileName);
            File.WriteAllText(filePath, "Тестовий текст для Лабораторної 3");

            // Вказуємо шлях до файлу та запускаємо процес завантаження
            driver.FindElement(By.Id("file-upload")).SendKeys(filePath);
            driver.FindElement(By.Id("file-submit")).Click();

            // Перевіряємо, чи відображається назва завантаженого файлу
            var uploadedFiles = driver.FindElement(By.Id("uploaded-files")).Text;
            Assert.That(uploadedFiles, Does.Contain(fileName), "Файл не завантажився на сервер!");
        }

        [Test]
        public void Test_08_JavaScriptAlerts()
        {
            // Відкриваємо сторінку з JavaScript-повідомленнями
            driver.Navigate().GoToUrl(BaseUrl + "javascript_alerts");

            // Викликаємо стандартне вікно оповіщення
            driver.FindElement(By.XPath("//button[text()='Click for JS Alert']")).Click();

            // Використовуємо наш надійний метод очікування для перемикання на алерт
            var alert = WaitForAlert();

            // Перевіряємо вміст повідомлення та підтверджуємо його
            Assert.That(alert.Text, Is.EqualTo("I am a JS Alert"), "Текст алерту не співпадає!");
            alert.Accept();

            // Перевіряємо результат виконаної дії на сторінці
            var resultText = driver.FindElement(By.Id("result")).Text;
            Assert.That(resultText, Is.EqualTo("You successfully clicked an alert"), "Неправильний результат після алерту!");
        }

        [Test]
        public void Test_09_MultipleWindows()
        {
            // Переходимо на сторінку для роботи з кількома вікнами
            driver.Navigate().GoToUrl(BaseUrl + "windows");

            // Зберігаємо ідентифікатор початкового вікна
            string originalWindow = driver.CurrentWindowHandle;

            // Відкриваємо нову вкладку через посилання
            driver.FindElement(By.LinkText("Click Here")).Click();

            // Очікуємо відкриття додаткового вікна
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(d => d.WindowHandles.Count > 1);

            // Отримуємо ідентифікатор нової вкладки та переходимо до неї
            string newWindow = driver.WindowHandles.First(handle => handle != originalWindow);
            driver.SwitchTo().Window(newWindow);

            // Перевіряємо заголовок відкритої сторінки
            var newPageText = driver.FindElement(By.TagName("h3")).Text;
            Assert.That(newPageText, Is.EqualTo("New Window"), "Текст у новому вікні не співпадає!");
        }

        [Test]
        public void Test_10_NotificationMessages()
        {
            // Відкриваємо сторінку з повідомленнями
            driver.Navigate().GoToUrl(BaseUrl + "notification_message");

            // Натискаємо посилання для створення нового повідомлення
            driver.FindElement(By.LinkText("Click here")).Click();

            // Отримуємо текст відображеного повідомлення
            var notificationText = driver.FindElement(By.Id("flash")).Text;

            // Переконуємося, що повідомлення містить очікувану інформацію
            Assert.That(notificationText, Is.Not.Empty, "Повідомлення взагалі не з'явилося!");
            Assert.That(notificationText, Does.Contain("Action"), "Текст повідомлення не містить очікуваного слова!");
        }

        [TearDown]
        public void TearDown()
        {
            // Завершуємо роботу браузера та звільняємо ресурси
            driver.Quit();
            driver.Dispose();
        }
    }
}