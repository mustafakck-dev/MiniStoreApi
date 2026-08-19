using Services.Contracts;

namespace Services;

public class ServiceManager : IServiceManager
{
    public IProductService ProductService { get; }

    public ICategoryService CategoryService { get; }

    public IAuthenticationService AuthenticationService { get; }

    public IOrderService OrderService { get; }

    public ServiceManager(
        IProductService productService,
        ICategoryService categoryService,
        IAuthenticationService authenticationService,
        IOrderService orderService)
    {
        ProductService = productService;
        CategoryService = categoryService;
        AuthenticationService = authenticationService;
        OrderService = orderService;
    }
}