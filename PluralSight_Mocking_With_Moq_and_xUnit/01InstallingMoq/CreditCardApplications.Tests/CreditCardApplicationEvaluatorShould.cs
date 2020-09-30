using System;
using Moq;
using Xunit;

namespace CreditCardApplications.Tests
{
    public class CreditCardApplicationEvaluatorShould
    {
        [Fact]
        public void AcceptHighIncomeApplications()
        {
            var mockFrequentFlyer = new Mock<IFrequentFlyerNumberValidator>();
            
            
            var sut = new CreditCardApplicationEvaluator(mockFrequentFlyer.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }

        [Fact]
        public void ReferYoungApplications()
        {
            var mockFrequentFlyer = new Mock<IFrequentFlyerNumberValidator>();

            mockFrequentFlyer.Setup(m => m.IsValid(It.IsAny<string>()))
                .Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockFrequentFlyer.Object);

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void TestDeclineLowOutcomeApplications()
        {
            var mockFrequentFlyer = new Mock<IFrequentFlyerNumberValidator>();

            // mockFrequentFlyer.Setup(s => s.IsValid("x"))
            //     .Returns(true);

            // mockFrequentFlyer.Setup(s => s.IsValid(It.IsAny<string>()))
            //     .Returns(true);

            // mockFrequentFlyer.Setup(s => s.IsValid(It.Is<string>(n => n.StartsWith("y"))))
            //     .Returns(true);

            // mockFrequentFlyer.Setup(s => s.IsValid(It.IsInRange("a", "z", Moq.Range.Inclusive)))
            //     .Returns(true);

            // mockFrequentFlyer.Setup(s => s.IsValid(It.IsIn("x", "y", "z")))
            //     .Returns(true);
            
            mockFrequentFlyer.Setup(s => s.IsValid(It.IsRegex("[a-y]")))
                .Returns(true);
            
            var sut = new CreditCardApplicationEvaluator(mockFrequentFlyer.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19999,
                Age = 45,
                FrequentFlyerNumber = "y"
            };

            CreditCardApplicationDecision decision = sut.Evaluate(application);
            
            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }

        [Fact]
        public void ReferInvalidFrequentFlyerApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>()))
                .Returns(false);
            
            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            
            var appliation = new CreditCardApplication();

            CreditCardApplicationDecision decision = sut.Evaluate(appliation);
            
            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void DeclineLowIncomeApplicationsOutDemo()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            bool isValid = true;
            mockValidator.Setup(m => m.IsValid(It.IsAny<string>(), out isValid));
            
            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19999,
                Age = 42
            };

            CreditCardApplicationDecision decision = sut.EvaluateUsingOut(application);
            
            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }

        [Fact]
        public void ReferWhenLicenceKeyExpired()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>()))
                .Returns(true);

            // mockValidator.Setup(k => k.ServiceInformation.Licence.LicenceKey).Returns("EXPIRED");
            mockValidator.Setup(k => k.ServiceInformation.Licence.LicenceKey)
                .Returns(GetLicenceKeyExpiryString);
            // mockValidator.Setup(k => k.LicenceKey).Returns(GetLicenceKeyExpiryString);
            
            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication {Age = 42};

            CreditCardApplicationDecision decision = sut.Evaluate(application);
            
            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        string GetLicenceKeyExpiryString()
        {
            // Note: this won't be called until it is evaluated
            // could be used to simulate the return from an external file
            // or getting the current time
            return "EXPIRED";
        }

        [Fact]
        public void UseDetailedLookupForOlderApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            
            // mock properties don't remember changes made to them. To do this...
            mockValidator.SetupProperty(x => x.ValidatorMode);
            
            // to track changes on all properties. Call before setting up properties
            // mockValidator.SetupAllProperties();

            mockValidator.Setup(x => x.ServiceInformation.Licence.LicenceKey).Returns("OK");
            
            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            
            var application = new CreditCardApplication{ Age = 30 };

            sut.Evaluate(application);
            
            Assert.Equal(ValidatorMode.Detailed, mockValidator.Object.ValidatorMode);
        }

        /// <summary>
        /// Test that a specific method is called.
        /// A custom error message can be returned as part of the Verify method
        /// </summary>
        [Fact]
        public void ValidateFrequentFlyerNumberForLowIncomeApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.Licence.LicenceKey)
                .Returns("OK");
            
            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            
            var application = new CreditCardApplication();

            sut.Evaluate(application);
            
            // verify that IsValid gets called with a value of null as application is created with no argument
            mockValidator.Verify(z => z.IsValid(null));

            var application2 = new CreditCardApplication
            {
                FrequentFlyerNumber = "z"
            };

            sut.Evaluate(application2);
            
            // verifies that IsValid is called with any string argument
            mockValidator.Verify(d => d.IsValid(It.IsAny<string>()));

            var application3 = new CreditCardApplication
            {
                FrequentFlyerNumber = "Q"
            };

            sut.Evaluate(application3);
            
            mockValidator.Verify(f => f.IsValid(null),
                "Frequent Flyer Numbers must be validated with a string");
        }

        /// <summary>
        /// Can also test that a specific method is not called
        /// </summary>
        [Fact]
        public void NotValidateFrequentFlyerNumberForHighIncomeApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.Licence.LicenceKey)
                .Returns("OK");
            
            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 100000
            };

            sut.Evaluate(application);
            
            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Never);
        }
    }
}
