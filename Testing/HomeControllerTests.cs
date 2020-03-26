using System.Collections.Generic;
using AspNetCore_Mentoring_Module1;
using AspNetCore_Mentoring_Module1.Controllers;
using AspNetCore_Mentoring_Module1.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Testing
{
    [TestFixture]
    public class HomeControllerTests
    {
        ILogger<HomeController> _logger;
        Mock<NorthwindContext> _contextMock;
        IOptionsProvider _optionsProvider;
        IProductModelProvider _modelProvider;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<HomeController>>().Object;

            _contextMock = new Mock<NorthwindContext>() { CallBase = true };
            var optionsProviderMock = new Mock<IOptionsProvider>();
            var modelProviderMock = new Mock<IProductModelProvider>();

            optionsProviderMock.Setup(op => op.GetOptions()).Returns(new Mock<IOptions>().Object);

            _optionsProvider = optionsProviderMock.Object;
            _modelProvider = modelProviderMock.Object;
        }
        
        private DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(queryable.Provider));
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));

            return dbSet.Object;
        }

        [Test]
        public async Task CategoriesViewTest()
        {
            var categoriesCount = 2;

            var dbSetCategories = GetQueryableMockDbSet(new List<Categories> { new Categories(), new Categories() });
            var dbSetSuppliers = GetQueryableMockDbSet(new List<Suppliers>());

            _contextMock.SetupGet(c => c.Categories).Returns(dbSetCategories);
            _contextMock.SetupGet(c => c.Suppliers).Returns(dbSetSuppliers);

            var categoryController = new CategoryController( _contextMock.Object);

            var result = await categoryController.Category();

            var model = ((ViewResult)result).Model;

            Assert.IsTrue(result is ViewResult);
            Assert.AreEqual(categoriesCount, ((List<Categories>)model).Count);
        }

        [Ignore ("Ignore while has a problem with query provider async calls...")]
        [Test]
        public async Task ProductsViewTest()
        {
            var dbSetProducts = GetQueryableMockDbSet(new List<Products> { new Products(), new Products(), new Products() });

            _contextMock.SetupGet(c => c.Products).Returns(dbSetProducts);

            var homeController = new HomeController(_logger, _contextMock.Object, _optionsProvider, _modelProvider);

            var expectedProductCount = 4;
            var result = await homeController.Products();
            var model = ((ViewResult)result).Model;

            Assert.IsTrue(result is ViewResult);
            Assert.AreEqual(expectedProductCount, ((List<Products>)model).Count);
        }
    }
}