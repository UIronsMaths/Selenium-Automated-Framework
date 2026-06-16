using System;
using Allure.Net.Commons;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using System;
using System.Threading;
using System.Threading.Tasks;
using Allure.NUnit;

[TestFixture]
[AllureNUnit]
public class Smoker
{
	[Test]
	public void SimpleAllureTest()
	{
		AllureApi.Step("Just a step");
		Assert.Pass();
	}
}
