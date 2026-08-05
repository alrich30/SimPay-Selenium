using SimPay.SeleniumTests.Infrastructure;
using SimPay.SeleniumTests.Pages;

namespace SimPay.SeleniumTests.Tests;

[TestFixture]
public sealed class LoginTests : SeleniumTestBase
{
    [Test]
    public void Login_WithValidCredentials_ShouldOpenPaymentsPage()
    {
        var loginPage = new LoginPage(Driver, Wait, BaseUrl);

        loginPage.Open();
        loginPage.Login("admin", "SimPay123!");
        loginPage.WaitForPaymentsPage();

        Assert.That(
            Driver.Url,
            Does.Contain("payments.html"));
    }

    [Test]
    public void Login_WithInvalidCredentials_ShouldShowError()
    {
        var loginPage = new LoginPage(Driver, Wait, BaseUrl);

        loginPage.Open();
        loginPage.Login("admin", "incorrect-password");

        string message = loginPage.WaitForErrorMessage();

        Assert.That(
            message,
            Is.EqualTo("Credenciales incorrectas."));
    }

    [Test]
    public void Login_WithEmptyFields_ShouldRemainOnLoginPage()
    {
        var loginPage = new LoginPage(Driver, Wait, BaseUrl);

        loginPage.Open();
        loginPage.SubmitEmptyForm();

        Assert.Multiple(() =>
        {
            Assert.That(loginPage.IsLoginPage(), Is.True);

            Assert.That(
                loginPage.GetUsernameValidationMessage(),
                Is.Not.Empty);
        });
    }
}