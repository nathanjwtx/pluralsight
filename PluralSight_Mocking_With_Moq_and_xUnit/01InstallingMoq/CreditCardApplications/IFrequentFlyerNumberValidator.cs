using System;

namespace CreditCardApplications
{
    public interface ILicenceData
    {
        string LicenceKey { get; }
    }

    public interface IServiceInformation
    {
        ILicenceData Licence { get; }
    }
    
    public interface IFrequentFlyerNumberValidator
    {
        bool IsValid(string frequentFlyerNumber);
        void IsValid(string frequentFlyerNumber, out bool isValid);
        // commented out to demonstrate Property Hierarchies
        // public string LicenceKey { get; }
        
        IServiceInformation ServiceInformation { get; }

        public ValidatorMode ValidatorMode { get; set; }
    }
}