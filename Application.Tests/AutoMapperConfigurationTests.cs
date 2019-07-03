using Api.Models;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;

namespace Application.Tests
{
    [TestClass]
    public class AutoMapperConfigurationTests
    {
        IMapper _mapper;

        [TestInitialize]
        public void Setup()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ApplicationMapperConfiguration());
                cfg.AddProfile(new ApiMapperConfiguration());
            });
            _mapper = new Mapper(config);
        }

        [TestMethod]
        public void Mapping_Valid()
        {
            _mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
