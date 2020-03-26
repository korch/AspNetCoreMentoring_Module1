using AspNetCore_Mentoring_Module1;
using AspNetCore_Mentoring_Module1.Controllers;
using AspNetCore_Mentoring_Module1.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Testing
{
    [TestFixture]
    public class HomeControllerTests
    {
        ILogger<HomeController> _logger;
        NorthwindContext _context;
        IOptionsProvider _optionsProvider;
        IProductModelProvider _modelProvider;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<HomeController>>().Object;

            var contextMock = new Mock<NorthwindContext>() { CallBase = true };
            var optionsProviderMock = new Mock<IOptionsProvider>();
            var modelProviderMock = new Mock<IProductModelProvider>();

            optionsProviderMock.Setup(op => op.GetOptions()).Returns(new Mock<IOptions>().Object);

            var dbSetCategories = GetQueryableMockDbSet(new List<Categories>() { new Categories(), new Categories() });
            var dbSetSuppliers = GetQueryableMockDbSet(new List<Suppliers>());

            contextMock.SetupGet(c => c.Categories).Returns(dbSetCategories);
            contextMock.SetupGet(c => c.Suppliers).Returns(dbSetSuppliers);

            _context = contextMock.Object;
            _optionsProvider = optionsProviderMock.Object;
            _modelProvider = modelProviderMock.Object;
        }

        [Test]
        public async Task CategoriesViewTest()
        {
            var homeController = new HomeController(_logger, _context, _optionsProvider, _modelProvider);

            var result = await homeController.Categories();

            var model = ((ViewResult)result).Model;

            Assert.IsTrue(result is ViewResult);
            Assert.AreEqual(2, ((List<Categories>)model).Count);
        }

        private DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));

            return dbSet.Object;
        }
    }
}